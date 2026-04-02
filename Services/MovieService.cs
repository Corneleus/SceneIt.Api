using SceneIt.Api.Data;
using SceneIt.Api.Models;
using SceneIt.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SceneIt.Api.Services
{
  public class MovieService : IMovieService
  {
    private readonly SceneItDbContext _context;

    public MovieService(SceneItDbContext context)
    {
      _context = context;
    }

    public IEnumerable<Movie> GetAll()
    {
      return _context.Movies.ToList();
    }

    public Movie? GetById(int id)
    {
      return _context.Movies.FirstOrDefault(m => m.MovieId == id);
    }

    public void Add(Movie movie)
    {
      if (!_context.Movies.Any(m => m.ImdbId == movie.ImdbId))
      {
        _context.Movies.Add(movie);
        _context.SaveChanges();
      }
    }

    public async Task<Movie> AddAsync(Movie movie)
    {
        // Example (adjust to your DB logic)
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

  }

}




