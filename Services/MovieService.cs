using Microsoft.EntityFrameworkCore;
using SceneIt.Api.Data;
using SceneIt.Api.Dtos;
using SceneIt.Api.Interfaces;

namespace SceneIt.Api.Services
{
  public class MovieService : IMovieService
  {
    private readonly SceneItDbContext _context;

    public MovieService(SceneItDbContext context)
    {
      _context = context;
    }

    public async Task<IReadOnlyList<MovieResponseDto>> GetAllAsync()
    {
      var movies = await _context.Movies
        .Where(m => !m.IsDeleted)
        .ToListAsync();

      return movies.Select(m => m.ToResponseDto()).ToList();
    }

    public async Task<MovieResponseDto?> GetByIdAsync(int id)
    {
      var movie = await _context.Movies
        .FirstOrDefaultAsync(m => m.MovieId == id && !m.IsDeleted);

      return movie?.ToResponseDto();
    }

    public async Task<CreateMovieResult> AddAsync(CreateMovieRequestDto movie)
    {
      var trimmedImdbId = movie.ImdbId.Trim();
      var existingMovie = await _context.Movies
        .FirstOrDefaultAsync(m => m.ImdbId == trimmedImdbId && !m.IsDeleted);

      if (existingMovie is not null)
      {
        return new CreateMovieResult
        {
          Movie = existingMovie.ToResponseDto(),
          Created = false
        };
      }

      var entity = movie.ToEntity();

      _context.Movies.Add(entity);
      await _context.SaveChangesAsync();

      return new CreateMovieResult
      {
        Movie = entity.ToResponseDto(),
        Created = true
      };
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
