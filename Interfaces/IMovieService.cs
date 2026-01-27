using System.Collections.Generic;
using SceneIt.Api.Models;

namespace SceneIt.Api.Interfaces
{
  public interface IMovieService
  {
    IEnumerable<Movie> GetAll();
    Movie? GetById(int id);
  }
}
