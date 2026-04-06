using Microsoft.EntityFrameworkCore;
using SceneIt.Api.Data;
using SceneIt.Api.Dtos;
using SceneIt.Api.Interfaces;

namespace SceneIt.Api.Services
{
  public class MediaLibraryService : IMediaLibraryService
  {
    protected readonly SceneItDbContext Context;

    public MediaLibraryService(SceneItDbContext context)
    {
      Context = context;
    }

    public async Task<IReadOnlyList<MediaItemResponseDto>> GetAllMediaItemsAsync(string? kind = null, CancellationToken cancellationToken = default)
    {
      var mediaItemsQuery = ApplyKindFilter(
        Context.MediaItems
          .AsNoTracking()
          .Where(mediaItem => !mediaItem.IsDeleted),
        kind);

      return await mediaItemsQuery
        .Select(mediaItem => mediaItem.ToMediaItemResponseDto())
        .ToListAsync(cancellationToken);
    }

    public async Task<MediaItemResponseDto?> GetMediaItemByIdAsync(int id, CancellationToken cancellationToken = default)
    {
      var mediaItem = await Context.MediaItems
        .AsNoTracking()
        .FirstOrDefaultAsync(entity => entity.MediaItemId == id && !entity.IsDeleted, cancellationToken);

      return mediaItem?.ToMediaItemResponseDto();
    }

    public async Task<CreateMediaItemResult> AddMediaItemAsync(CreateMediaItemRequestDto mediaItem, CancellationToken cancellationToken = default)
    {
      var trimmedImdbId = mediaItem.ImdbId.Trim();
      var existingMediaItem = await Context.MediaItems
        .FirstOrDefaultAsync(entity => entity.ImdbId == trimmedImdbId, cancellationToken);

      if (existingMediaItem is not null)
      {
        if (existingMediaItem.IsDeleted)
        {
          mediaItem.ApplyToEntity(existingMediaItem);
          existingMediaItem.IsDeleted = false;
          existingMediaItem.DeletedAtUtc = null;

          await Context.SaveChangesAsync(cancellationToken);

          return new CreateMediaItemResult
          {
            MediaItem = existingMediaItem.ToMediaItemResponseDto(),
            Created = true
          };
        }

        return new CreateMediaItemResult
        {
          MediaItem = existingMediaItem.ToMediaItemResponseDto(),
          Created = false
        };
      }

      var entity = mediaItem.ToEntity();

      Context.MediaItems.Add(entity);
      await Context.SaveChangesAsync(cancellationToken);

      return new CreateMediaItemResult
      {
        MediaItem = entity.ToMediaItemResponseDto(),
        Created = true
      };
    }

    public async Task<bool> SoftDeleteMediaItemAsync(int id, CancellationToken cancellationToken = default)
    {
      var mediaItem = await Context.MediaItems.FirstOrDefaultAsync(entity => entity.MediaItemId == id, cancellationToken);

      if (mediaItem is null || mediaItem.IsDeleted)
      {
        return false;
      }

      mediaItem.IsDeleted = true;
      mediaItem.DeletedAtUtc = DateTime.UtcNow;

      await Context.SaveChangesAsync(cancellationToken);

      return true;
    }

    public async Task<bool> HardDeleteMediaItemAsync(int id, CancellationToken cancellationToken = default)
    {
      var mediaItem = await Context.MediaItems.FirstOrDefaultAsync(entity => entity.MediaItemId == id, cancellationToken);

      if (mediaItem is null)
      {
        return false;
      }

      Context.MediaItems.Remove(mediaItem);
      await Context.SaveChangesAsync(cancellationToken);

      return true;
    }

    private static IQueryable<Models.MediaItem> ApplyKindFilter(IQueryable<Models.MediaItem> query, string? kind)
    {
      var normalizedTypeTokens = MediaKindMatcher.GetTypeTokens(kind);

      if (normalizedTypeTokens.Count == 0)
      {
        return string.IsNullOrWhiteSpace(kind)
          ? query
          : query.Where(_ => false);
      }

      return query.Where(mediaItem =>
        mediaItem.Type != null &&
        normalizedTypeTokens.Contains(
          mediaItem.Type
            .Trim()
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty)
            .ToLower()));
    }
  }
}
