using SceneIt.Api.Dtos;

namespace SceneIt.Api.Interfaces
{
  public interface IMediaLibraryService
  {
    Task<IReadOnlyList<MediaItemResponseDto>> GetAllMediaItemsAsync(string? kind = null);
    Task<MediaItemResponseDto?> GetMediaItemByIdAsync(int id);
    Task<CreateMediaItemResult> AddMediaItemAsync(CreateMediaItemRequestDto mediaItem);
    Task<bool> SoftDeleteMediaItemAsync(int id);
    Task<bool> HardDeleteMediaItemAsync(int id);
  }
}
