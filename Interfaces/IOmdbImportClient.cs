using SceneIt.Api.Dtos;

namespace SceneIt.Api.Interfaces
{
  public interface IOmdbImportClient
  {
    Task<IReadOnlyList<MediaItemResponseDto>> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<CreateMediaItemRequestDto?> LookupByImdbIdAsync(string imdbId, CancellationToken cancellationToken = default);
  }
}
