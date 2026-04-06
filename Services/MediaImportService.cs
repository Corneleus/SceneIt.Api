using System.IO.Compression;
using Microsoft.EntityFrameworkCore;
using SceneIt.Api.Data;
using SceneIt.Api.Dtos;
using SceneIt.Api.Interfaces;
using SceneIt.Api.Models;

namespace SceneIt.Api.Services
{
    public class MediaImportService : IMediaImportService
    {
        private const int MaxBatchSize = 100;
        private const int DatasetQueueSaveBatchSize = 500;

        private readonly SceneItDbContext _context;
        private readonly IOmdbImportClient _omdbImportClient;
        private readonly IMediaLibraryService _mediaLibraryService;

        public MediaImportService(SceneItDbContext context, IOmdbImportClient omdbImportClient, IMediaLibraryService mediaLibraryService)
        {
            _context = context;
            _omdbImportClient = omdbImportClient;
            _mediaLibraryService = mediaLibraryService;
        }

        public async Task<QueueImportResultDto> QueueAsync(IReadOnlyList<ImportQueueItemDto> items, CancellationToken cancellationToken = default)
        {
            var normalizedItems = items
              .Select(item => (
                  ImdbId: item.ImdbId.Trim(),
                  Title: string.IsNullOrWhiteSpace(item.Title) ? null : item.Title.Trim()
              ))
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
            var newItems = new List<ImportQueue>();

            foreach (var item in normalizedItems)
            {
                if (existingIdSet.Contains(item.ImdbId))
                {
                    continue;
                }

                var mediaItemRequest = await _omdbImportClient.LookupByImdbIdAsync(item.ImdbId, cancellationToken);

                newItems.Add(CreateQueueItem(item, mediaItemRequest));
            }

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

        public Task<DatasetImportPreviewResultDto> PreviewDatasetAsync(DatasetImportPreviewRequestDto request, CancellationToken cancellationToken = default)
        {
            return AnalyzeDatasetAsync<DatasetImportPreviewResultDto>(request, maxToQueue: null, queueMatches: false, cancellationToken);
        }

        public Task<QueueDatasetImportsResultDto> QueueDatasetAsync(QueueDatasetImportsRequestDto request, CancellationToken cancellationToken = default)
        {
            return AnalyzeDatasetAsync<QueueDatasetImportsResultDto>(request, request.MaxToQueue, queueMatches: true, cancellationToken);
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
                    var mediaItemRequest = await _omdbImportClient.LookupByImdbIdAsync(item.ImdbId, cancellationToken);

                    if (mediaItemRequest is null)
                    {
                        item.Status = ImportQueueStatus.Failed;
                        item.ErrorMessage = "OMDb lookup failed or returned incomplete data.";
                        run.FailedCount++;
                    }
                    else
                    {
                        UpdateQueueItemMetadata(item, mediaItemRequest);
                        item.Title ??= mediaItemRequest.Title;

                        var result = await _mediaLibraryService.AddMediaItemAsync(mediaItemRequest);

                        if (result.Created)
                        {
                            item.Status = ImportQueueStatus.Imported;
                            item.ImportedAtUtc = DateTime.UtcNow;
                            run.ImportedCount++;
                        }
                        else
                        {
                            item.Status = ImportQueueStatus.Duplicate;
                            item.ErrorMessage = "Media item already exists in the library.";
                            run.DuplicateCount++;
                        }
                    }
                }
                catch (OmdbException ex)
                {
                    DetachTrackedMediaItems(item.ImdbId);
                    item.Status = ImportQueueStatus.Failed;
                    item.ErrorMessage = $"OMDb lookup failed: {ex.Message}";
                    run.FailedCount++;
                }
                catch (Exception ex)
                {
                    DetachTrackedMediaItems(item.ImdbId);
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

        private void DetachTrackedMediaItems(string imdbId)
        {
            var normalizedImdbId = imdbId.Trim();

            foreach (var entry in _context.ChangeTracker.Entries<MediaItem>()
              .Where(entry =>
                entry.State is EntityState.Added or EntityState.Modified &&
                string.Equals(entry.Entity.ImdbId, normalizedImdbId, StringComparison.OrdinalIgnoreCase)))
            {
                entry.State = EntityState.Detached;
            }
        }

        private static ImportQueue CreateQueueItem((string ImdbId, string? Title) item, CreateMediaItemRequestDto? mediaItemRequest)
        {
            var queueItem = new ImportQueue
            {
                ImdbId = item.ImdbId,
                Title = mediaItemRequest?.Title ?? item.Title,
                Year = mediaItemRequest?.Year,
                Rated = mediaItemRequest?.Rated,
                Runtime = mediaItemRequest?.Runtime,
                Genre = mediaItemRequest?.Genre,
                Director = mediaItemRequest?.Director,
                Writer = mediaItemRequest?.Writer,
                Actors = mediaItemRequest?.Actors,
                Plot = mediaItemRequest?.Plot,
                Language = mediaItemRequest?.Language,
                Country = mediaItemRequest?.Country,
                Awards = mediaItemRequest?.Awards,
                Poster = mediaItemRequest?.Poster,
                Metascore = mediaItemRequest?.Metascore,
                ImdbRating = mediaItemRequest?.ImdbRating,
                ImdbVotes = mediaItemRequest?.ImdbVotes,
                Type = mediaItemRequest?.Type,
                Dvd = mediaItemRequest?.Dvd,
                BoxOffice = mediaItemRequest?.BoxOffice,
                Production = mediaItemRequest?.Production,
                Status = ImportQueueStatus.Pending
            };

            return queueItem;
        }

        private static ImportQueue CreateDatasetQueueItem(ImdbDatasetRow row)
        {
            return new ImportQueue
            {
                ImdbId = row.ImdbId,
                Title = row.Title,
                Year = row.Year,
                Type = row.TitleType,
                Status = ImportQueueStatus.Pending
            };
        }

        private static void UpdateQueueItemMetadata(ImportQueue item, CreateMediaItemRequestDto mediaItemRequest)
        {
            item.Title ??= mediaItemRequest.Title;
            item.Year = mediaItemRequest.Year;
            item.Rated = mediaItemRequest.Rated;
            item.Runtime = mediaItemRequest.Runtime;
            item.Genre = mediaItemRequest.Genre;
            item.Director = mediaItemRequest.Director;
            item.Writer = mediaItemRequest.Writer;
            item.Actors = mediaItemRequest.Actors;
            item.Plot = mediaItemRequest.Plot;
            item.Language = mediaItemRequest.Language;
            item.Country = mediaItemRequest.Country;
            item.Awards = mediaItemRequest.Awards;
            item.Poster = mediaItemRequest.Poster;
            item.Metascore = mediaItemRequest.Metascore;
            item.ImdbRating = mediaItemRequest.ImdbRating;
            item.ImdbVotes = mediaItemRequest.ImdbVotes;
            item.Type = mediaItemRequest.Type;
            item.Dvd = mediaItemRequest.Dvd;
            item.BoxOffice = mediaItemRequest.BoxOffice;
            item.Production = mediaItemRequest.Production;
        }

        private async Task<T> AnalyzeDatasetAsync<T>(
            DatasetImportPreviewRequestDto request,
            int? maxToQueue,
            bool queueMatches,
            CancellationToken cancellationToken)
            where T : class, new()
        {
            var normalizedPath = request.DatasetPath.Trim();

            if (string.IsNullOrWhiteSpace(normalizedPath))
            {
                throw new ArgumentException("A dataset path is required.");
            }

            if (!File.Exists(normalizedPath))
            {
                throw new FileNotFoundException("The dataset file could not be found.", normalizedPath);
            }

            if (request.StartYear.HasValue && request.EndYear.HasValue && request.StartYear > request.EndYear)
            {
                throw new ArgumentException("Start year must be less than or equal to end year.");
            }

            if (maxToQueue.HasValue && maxToQueue.Value <= 0)
            {
                throw new ArgumentException("MaxToQueue must be greater than zero when provided.");
            }

            var filter = DatasetFilter.Create(request);
            var existingQueueIds = (await _context.ImportQueueItems
              .Select(item => item.ImdbId)
              .ToListAsync(cancellationToken))
              .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var existingMediaItemIds = request.SkipAlreadyImported
              ? (await _context.MediaItems
                .Select(mediaItem => mediaItem.ImdbId)
                .ToListAsync(cancellationToken))
                .ToHashSet(StringComparer.OrdinalIgnoreCase)
              : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var totalRowsScanned = 0;
            var matchedCount = 0;
            var alreadyQueuedCount = 0;
            var alreadyImportedCount = 0;
            var readyToQueueCount = 0;
            var queuedCount = 0;
            var pendingAdds = new List<ImportQueue>();

            await foreach (var row in ReadDatasetRowsAsync(normalizedPath, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                totalRowsScanned++;

                if (!filter.Matches(row))
                {
                    continue;
                }

                matchedCount++;

                if (existingQueueIds.Contains(row.ImdbId))
                {
                    alreadyQueuedCount++;
                    continue;
                }

                if (request.SkipAlreadyImported && existingMediaItemIds.Contains(row.ImdbId))
                {
                    alreadyImportedCount++;
                    continue;
                }

                readyToQueueCount++;

                if (!queueMatches)
                {
                    continue;
                }

                if (maxToQueue.HasValue && queuedCount >= maxToQueue.Value)
                {
                    continue;
                }

                pendingAdds.Add(CreateDatasetQueueItem(row));
                existingQueueIds.Add(row.ImdbId);
                queuedCount++;

                if (pendingAdds.Count >= DatasetQueueSaveBatchSize)
                {
                    _context.ImportQueueItems.AddRange(pendingAdds);
                    await _context.SaveChangesAsync(cancellationToken);
                    pendingAdds.Clear();
                }
            }

            if (pendingAdds.Count > 0)
            {
                _context.ImportQueueItems.AddRange(pendingAdds);
                await _context.SaveChangesAsync(cancellationToken);
            }

            if (typeof(T) == typeof(DatasetImportPreviewResultDto))
            {
                return new DatasetImportPreviewResultDto
                {
                    TotalRowsScanned = totalRowsScanned,
                    MatchedCount = matchedCount,
                    AlreadyQueuedCount = alreadyQueuedCount,
                    AlreadyImportedCount = alreadyImportedCount,
                    ReadyToQueueCount = readyToQueueCount
                } as T ?? throw new InvalidOperationException("Unable to create dataset preview result.");
            }

            if (typeof(T) == typeof(QueueDatasetImportsResultDto))
            {
                return new QueueDatasetImportsResultDto
                {
                    TotalRowsScanned = totalRowsScanned,
                    MatchedCount = matchedCount,
                    AlreadyQueuedCount = alreadyQueuedCount,
                    AlreadyImportedCount = alreadyImportedCount,
                    ReadyToQueueCount = readyToQueueCount,
                    QueuedCount = queuedCount
                } as T ?? throw new InvalidOperationException("Unable to create dataset queue result.");
            }

            throw new InvalidOperationException("Unsupported dataset import result type.");
        }

        private static async IAsyncEnumerable<ImdbDatasetRow> ReadDatasetRowsAsync(
            string path,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var fileStream = File.OpenRead(path);
            using Stream datasetStream = path.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)
              ? new GZipStream(fileStream, CompressionMode.Decompress)
              : fileStream;
            using var reader = new StreamReader(datasetStream);

            var headerLine = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(headerLine))
            {
                throw new InvalidDataException("The dataset file is empty.");
            }

            var columnIndexes = DatasetColumnIndexes.Create(headerLine);
            string? line;

            while ((line = await reader.ReadLineAsync(cancellationToken)) is not null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var columns = line.Split('\t');
                if (columns.Length <= columnIndexes.MaxIndex)
                {
                    continue;
                }

                var imdbId = NormalizeDatasetValue(columns[columnIndexes.ImdbId]);
                if (string.IsNullOrWhiteSpace(imdbId))
                {
                    continue;
                }

                yield return new ImdbDatasetRow(
                    imdbId,
                    NormalizeDatasetValue(columns[columnIndexes.TitleType]),
                    NormalizeDatasetValue(columns[columnIndexes.Title]),
                    NormalizeDatasetValue(columns[columnIndexes.IsAdult]),
                    NormalizeDatasetValue(columns[columnIndexes.StartYear]));
            }
        }

        private static string? NormalizeDatasetValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmed = value.Trim();
            return string.Equals(trimmed, "\\N", StringComparison.Ordinal) ? null : trimmed;
        }

        private sealed record ImdbDatasetRow(
            string ImdbId,
            string? TitleType,
            string? Title,
            string? IsAdult,
            string? Year);

        private sealed class DatasetFilter
        {
            private readonly HashSet<string> _titleTypes;
            private readonly bool _excludeAdult;
            private readonly int? _startYear;
            private readonly int? _endYear;

            private DatasetFilter(HashSet<string> titleTypes, bool excludeAdult, int? startYear, int? endYear)
            {
                _titleTypes = titleTypes;
                _excludeAdult = excludeAdult;
                _startYear = startYear;
                _endYear = endYear;
            }

            public static DatasetFilter Create(DatasetImportPreviewRequestDto request)
            {
                var titleTypes = request.TitleTypes
                  .Where(type => !string.IsNullOrWhiteSpace(type))
                  .Select(type => type.Trim())
                  .ToHashSet(StringComparer.OrdinalIgnoreCase);

                return new DatasetFilter(titleTypes, request.ExcludeAdult, request.StartYear, request.EndYear);
            }

            public bool Matches(ImdbDatasetRow row)
            {
                if (_titleTypes.Count > 0 && (row.TitleType is null || !_titleTypes.Contains(row.TitleType)))
                {
                    return false;
                }

                if (_excludeAdult && string.Equals(row.IsAdult, "1", StringComparison.Ordinal))
                {
                    return false;
                }

                if (_startYear.HasValue || _endYear.HasValue)
                {
                    if (!int.TryParse(row.Year, out var year))
                    {
                        return false;
                    }

                    if (_startYear.HasValue && year < _startYear.Value)
                    {
                        return false;
                    }

                    if (_endYear.HasValue && year > _endYear.Value)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private sealed class DatasetColumnIndexes
        {
            public int ImdbId { get; }
            public int TitleType { get; }
            public int Title { get; }
            public int IsAdult { get; }
            public int StartYear { get; }
            public int MaxIndex => new[] { ImdbId, TitleType, Title, IsAdult, StartYear }.Max();

            private DatasetColumnIndexes(int imdbId, int titleType, int title, int isAdult, int startYear)
            {
                ImdbId = imdbId;
                TitleType = titleType;
                Title = title;
                IsAdult = isAdult;
                StartYear = startYear;
            }

            public static DatasetColumnIndexes Create(string headerLine)
            {
                var headers = headerLine.Split('\t');

                return new DatasetColumnIndexes(
                    GetRequiredColumnIndex(headers, "tconst"),
                    GetRequiredColumnIndex(headers, "titleType"),
                    GetRequiredColumnIndex(headers, "primaryTitle"),
                    GetRequiredColumnIndex(headers, "isAdult"),
                    GetRequiredColumnIndex(headers, "startYear"));
            }

            private static int GetRequiredColumnIndex(string[] headers, string columnName)
            {
                var index = Array.FindIndex(headers, header => string.Equals(header, columnName, StringComparison.OrdinalIgnoreCase));
                if (index < 0)
                {
                    throw new InvalidDataException($"The dataset file is missing the required '{columnName}' column.");
                }

                return index;
            }
        }
    }
}
