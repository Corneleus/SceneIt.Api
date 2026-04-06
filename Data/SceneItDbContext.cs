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

    public DbSet<MediaItem> MediaItems => Set<MediaItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserMediaItem> UserMediaItems => Set<UserMediaItem>();
    public DbSet<ImportQueue> ImportQueueItems => Set<ImportQueue>();
    public DbSet<ImportRun> ImportRuns => Set<ImportRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<MediaItem>()
        .HasIndex(mediaItem => mediaItem.ImdbId)
        .IsUnique();

      modelBuilder.Entity<MediaItem>()
        .Property(mediaItem => mediaItem.IsDeleted)
        .HasDefaultValue(false);

      modelBuilder.Entity<MediaItem>()
        .Property(mediaItem => mediaItem.Released)
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
