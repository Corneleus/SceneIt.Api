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

    public async Task<IReadOnlyList<MediaItemResponseDto>> GetAllMediaItemsAsync(string? kind = null)
    {
      var mediaItems = await Context.MediaItems
        .Where(mediaItem => !mediaItem.IsDeleted)
        .ToListAsync();

      return mediaItems
        .Where(mediaItem => MediaKindMatcher.Matches(mediaItem.Type, kind))
        .Select(mediaItem => mediaItem.ToMediaItemResponseDto())
        .ToList();
    }

    public async Task<MediaItemResponseDto?> GetMediaItemByIdAsync(int id)
    {
      var mediaItem = await Context.MediaItems
        .FirstOrDefaultAsync(entity => entity.MediaItemId == id && !entity.IsDeleted);

      return mediaItem?.ToMediaItemResponseDto();
    }

    public async Task<CreateMediaItemResult> AddMediaItemAsync(CreateMediaItemRequestDto mediaItem)
    {
      var trimmedImdbId = mediaItem.ImdbId.Trim();
      var existingMediaItem = await Context.MediaItems
        .FirstOrDefaultAsync(entity => entity.ImdbId == trimmedImdbId);

      if (existingMediaItem is not null)
      {
        if (existingMediaItem.IsDeleted)
        {
          mediaItem.ApplyToEntity(existingMediaItem);
          existingMediaItem.IsDeleted = false;
          existingMediaItem.DeletedAtUtc = null;

          await Context.SaveChangesAsync();

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
      await Context.SaveChangesAsync();

      return new CreateMediaItemResult
      {
        MediaItem = entity.ToMediaItemResponseDto(),
        Created = true
      };
    }

    public async Task<bool> SoftDeleteMediaItemAsync(int id)
    {
      var mediaItem = await Context.MediaItems.FirstOrDefaultAsync(entity => entity.MediaItemId == id);

      if (mediaItem is null || mediaItem.IsDeleted)
      {
        return false;
      }

      mediaItem.IsDeleted = true;
      mediaItem.DeletedAtUtc = DateTime.UtcNow;

      await Context.SaveChangesAsync();

      return true;
    }

    public async Task<bool> HardDeleteMediaItemAsync(int id)
    {
      var mediaItem = await Context.MediaItems.FirstOrDefaultAsync(entity => entity.MediaItemId == id);

      if (mediaItem is null)
      {
        return false;
      }

      Context.MediaItems.Remove(mediaItem);
      await Context.SaveChangesAsync();

      return true;
    }
  }
}
