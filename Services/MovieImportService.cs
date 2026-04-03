using Microsoft.EntityFrameworkCore;
using SceneIt.Api.Data;
using SceneIt.Api.Dtos;
using SceneIt.Api.Interfaces;
using SceneIt.Api.Models;

namespace SceneIt.Api.Services
{
  public class MovieImportService : IMovieImportService
  {
    private const int MaxBatchSize = 100;

    private readonly SceneItDbContext _context;
    private readonly IOmdbImportClient _omdbImportClient;
    private readonly IMovieService _movieService;

    public MovieImportService(SceneItDbContext context, IOmdbImportClient omdbImportClient, IMovieService movieService)
    {
      _context = context;
      _omdbImportClient = omdbImportClient;
      _movieService = movieService;
    }

    public async Task<QueueImportResultDto> QueueAsync(IReadOnlyList<ImportQueueItemDto> items, CancellationToken cancellationToken = default)
    {
      var normalizedItems = items
        .Select(item => new
        {
          ImdbId = item.ImdbId.Trim(),
          Title = string.IsNullOrWhiteSpace(item.Title) ? null : item.Title.Trim()
        })
        .Where(item => !string.IsNullOrWhiteSpace(item.ImdbId))
        .GroupBy(item => item.ImdbId, StringComparer.OrdinalIgnoreCase)
        .Select(group => group.First())
        .ToList();

      var imdbIds = normalizedItems.Select(item => item.ImdbId).ToList();
      var existingIds = await _context.ImportQueueItems
        .Where(item => imdbIds.Contains(item.ImdbId))
        .Select(item => item.ImdbId)
        .ToListAsync(cancellationToken);

      var existingIdSet = existingIds.ToHashSet(StringComparer.OrdinalIgnoreCase);
      var newItems = normalizedItems
        .Where(item => !existingIdSet.Contains(item.ImdbId))
        .Select(item => new ImportQueue
        {
          ImdbId = item.ImdbId,
          Title = item.Title,
          Status = ImportQueueStatus.Pending
        })
        .ToList();

      if (newItems.Count > 0)
      {
        _context.ImportQueueItems.AddRange(newItems);
        await _context.SaveChangesAsync(cancellationToken);
      }

      return new QueueImportResultDto
      {
        QueuedCount = newItems.Count,
        SkippedCount = normalizedItems.Count - newItems.Count,
        QueuedItems = newItems.Select(item => item.ToResponseDto()).ToList()
      };
    }

    public async Task<IReadOnlyList<ImportQueueItemResponseDto>> GetQueueAsync(CancellationToken cancellationToken = default)
    {
      var items = await _context.ImportQueueItems
        .OrderBy(item => item.Status == ImportQueueStatus.Pending ? 0 : 1)
        .ThenBy(item => item.ImportQueueId)
        .ToListAsync(cancellationToken);

      return items.Select(item => item.ToResponseDto()).ToList();
    }

    public async Task<IReadOnlyList<ImportRunResultDto>> GetRunsAsync(CancellationToken cancellationToken = default)
    {
      var runs = await _context.ImportRuns
        .OrderByDescending(run => run.StartedAtUtc)
        .ToListAsync(cancellationToken);

      return runs.Select(run => run.ToResponseDto()).ToList();
    }

    public async Task<ImportRunResultDto> RunBatchAsync(int maxCount, CancellationToken cancellationToken = default)
    {
      var effectiveMaxCount = maxCount <= 0 ? MaxBatchSize : Math.Min(maxCount, MaxBatchSize);
      var run = new ImportRun
      {
        StartedAtUtc = DateTime.UtcNow,
        RequestedLimit = effectiveMaxCount,
        Notes = effectiveMaxCount == MaxBatchSize && maxCount > MaxBatchSize
          ? $"Requested limit {maxCount} exceeded the maximum of {MaxBatchSize}."
          : null
      };

      _context.ImportRuns.Add(run);
      await _context.SaveChangesAsync(cancellationToken);

      var pendingItems = await _context.ImportQueueItems
        .Where(item => item.Status == ImportQueueStatus.Pending)
        .OrderBy(item => item.ImportQueueId)
        .Take(effectiveMaxCount)
        .ToListAsync(cancellationToken);

      foreach (var item in pendingItems)
      {
        run.AttemptedCount++;
        item.AttemptCount++;
        item.LastAttemptedAtUtc = DateTime.UtcNow;
        item.ErrorMessage = null;

        try
        {
          var movieRequest = await _omdbImportClient.GetMovieByImdbIdAsync(item.ImdbId, cancellationToken);

          if (movieRequest is null)
          {
            item.Status = ImportQueueStatus.Failed;
            item.ErrorMessage = "OMDb lookup failed or returned incomplete data.";
            run.FailedCount++;
          }
          else
          {
            item.Title ??= movieRequest.Title;

            var result = await _movieService.AddAsync(movieRequest);

            if (result.Created)
            {
              item.Status = ImportQueueStatus.Imported;
              item.ImportedAtUtc = DateTime.UtcNow;
              run.ImportedCount++;
            }
            else
            {
              item.Status = ImportQueueStatus.Duplicate;
              item.ErrorMessage = "Movie already exists in the library.";
              run.DuplicateCount++;
            }
          }
        }
        catch (Exception ex)
        {
          DetachTrackedMovies(item.ImdbId);
          item.Status = ImportQueueStatus.Failed;
          item.ErrorMessage = $"Import failed: {ex.Message}";
          run.FailedCount++;
        }

        await _context.SaveChangesAsync(cancellationToken);
      }

      if (pendingItems.Count == 0)
      {
        run.Notes = string.IsNullOrWhiteSpace(run.Notes)
          ? "No pending import items were available."
          : $"{run.Notes} No pending import items were available.";
      }

      run.CompletedAtUtc = DateTime.UtcNow;
      await _context.SaveChangesAsync(cancellationToken);

      return run.ToResponseDto();
    }

    private void DetachTrackedMovies(string imdbId)
    {
      var normalizedImdbId = imdbId.Trim();

      foreach (var entry in _context.ChangeTracker.Entries<Movie>()
        .Where(entry =>
          entry.State is EntityState.Added or EntityState.Modified &&
          string.Equals(entry.Entity.ImdbId, normalizedImdbId, StringComparison.OrdinalIgnoreCase)))
      {
        entry.State = EntityState.Detached;
      }
    }
  }
}
