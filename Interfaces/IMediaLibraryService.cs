using SceneIt.Api.Dtos;

namespace SceneIt.Api.Interfaces
{
  public interface IMediaLibraryService
  {
    Task<IReadOnlyList<MediaItemResponseDto>> GetAllMediaItemsAsync(string? kind = null, CancellationToken cancellationToken = default);
    Task<MediaItemResponseDto?> GetMediaItemByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CreateMediaItemResult> AddMediaItemAsync(CreateMediaItemRequestDto mediaItem, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteMediaItemAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> HardDeleteMediaItemAsync(int id, CancellationToken cancellationToken = default);
  }
}
