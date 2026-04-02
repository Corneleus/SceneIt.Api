using System.Collections.Generic;
using SceneIt.Api.Models;

namespace SceneIt.Api.Interfaces
{
  public interface IMovieService
  {
    Task<IReadOnlyList<Movie>> GetAllAsync();
    Task<Movie?> GetByIdAsync(int id);
    Task<Movie> AddAsync(Movie movie);
    Task<bool> SoftDeleteAsync(int id);
    Task<bool> HardDeleteAsync(int id);
  }
}
