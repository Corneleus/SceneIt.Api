using Microsoft.EntityFrameworkCore;
using SceneIt.Api.Models;
using System.Collections.Generic;

namespace SceneIt.Api.Data
{
  public class SceneItDbContext : DbContext
  {
    public SceneItDbContext(DbContextOptions<SceneItDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserMovie> UserMovies => Set<UserMovie>();
  }
}
