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

    public async Task<IReadOnlyList<Movie>> GetAllAsync()
    {
      return await _context.Movies
        .Where(m => !m.IsDeleted)
        .ToListAsync();
    }

    public async Task<Movie?> GetByIdAsync(int id)
    {
      return await _context.Movies
        .FirstOrDefaultAsync(m => m.MovieId == id && !m.IsDeleted);
    }

    public async Task<Movie> AddAsync(Movie movie)
    {
      var existingMovie = await _context.Movies
        .FirstOrDefaultAsync(m => m.ImdbId == movie.ImdbId);

      if (existingMovie is not null)
      {
        return existingMovie;
      }

      _context.Movies.Add(movie);
      await _context.SaveChangesAsync();

      return movie;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
      var movie = await _context.Movies.FirstOrDefaultAsync(m => m.MovieId == id);

      if (movie is null || movie.IsDeleted)
      {
        return false;
      }

      movie.IsDeleted = true;
      movie.DeletedAtUtc = DateTime.UtcNow;

      await _context.SaveChangesAsync();

      return true;
    }

    public async Task<bool> HardDeleteAsync(int id)
    {
      var movie = await _context.Movies.FirstOrDefaultAsync(m => m.MovieId == id);

      if (movie is null)
      {
        return false;
      }

      _context.Movies.Remove(movie);
      await _context.SaveChangesAsync();

      return true;
    }
  }
}




