using Microsoft.EntityFrameworkCore;
using SceneIt.Api.Models;

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
    public DbSet<ImportQueue> ImportQueueItems => Set<ImportQueue>();
    public DbSet<ImportRun> ImportRuns => Set<ImportRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Movie>()
        .HasIndex(movie => movie.ImdbId)
        .IsUnique();

      modelBuilder.Entity<Movie>()
        .Property(movie => movie.IsDeleted)
        .HasDefaultValue(false);

      modelBuilder.Entity<Movie>()
        .Property(movie => movie.Released)
        .HasColumnType("date");

      modelBuilder.Entity<ImportQueue>()
        .HasIndex(queueItem => queueItem.ImdbId)
        .IsUnique();

      modelBuilder.Entity<ImportQueue>()
        .Property(queueItem => queueItem.Status)
        .HasConversion<string>()
        .HasMaxLength(20);
    }
  }
}
