using SceneIt.Api.Dtos;

namespace SceneIt.Api.Interfaces
{
  public interface IOmdbImportClient
  {
    Task<IReadOnlyList<MovieResponseDto>> SearchMoviesAsync(string query, CancellationToken cancellationToken = default);
    Task<CreateMovieRequestDto?> GetMovieByImdbIdAsync(string imdbId, CancellationToken cancellationToken = default);
  }
}
