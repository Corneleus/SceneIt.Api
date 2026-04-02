using SceneIt.Api.Dtos;

namespace SceneIt.Api.Interfaces
{
  public interface IMovieService
  {
    Task<IReadOnlyList<MovieResponseDto>> GetAllAsync();
    Task<MovieResponseDto?> GetByIdAsync(int id);
    Task<CreateMovieResult> AddAsync(CreateMovieRequestDto movie);
    Task<bool> SoftDeleteAsync(int id);
    Task<bool> HardDeleteAsync(int id);
  }
}
