using System.ComponentModel.DataAnnotations;

namespace SceneIt.Api.Models
{
  public class ImportQueue
  {
    public int ImportQueueId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ImdbId { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Title { get; set; }

    [MaxLength(50)]
    public string? Year { get; set; }

    [MaxLength(20)]
    public string? Rated { get; set; }

    [MaxLength(50)]
    public string? Runtime { get; set; }

    [MaxLength(250)]
    public string? Genre { get; set; }

    [MaxLength(150)]
    public string? Director { get; set; }

    [MaxLength(400)]
    public string? Writer { get; set; }

    [MaxLength(500)]
    public string? Actors { get; set; }

    [MaxLength(4000)]
    public string? Plot { get; set; }

    [MaxLength(200)]
    public string? Language { get; set; }

    [MaxLength(200)]
    public string? Country { get; set; }

    [MaxLength(400)]
    public string? Awards { get; set; }

    [MaxLength(500)]
    public string? Poster { get; set; }

    [MaxLength(20)]
    public string? Metascore { get; set; }

    [MaxLength(10)]
    public string? ImdbRating { get; set; }

    [MaxLength(30)]
    public string? ImdbVotes { get; set; }

    [MaxLength(20)]
    public string? Type { get; set; }

    [MaxLength(50)]
    public string? Dvd { get; set; }

    [MaxLength(100)]
    public string? BoxOffice { get; set; }

    [MaxLength(150)]
    public string? Production { get; set; }

    public ImportQueueStatus Status { get; set; } = ImportQueueStatus.Pending;
    public int AttemptCount { get; set; }
    public DateTime? LastAttemptedAtUtc { get; set; }
    public DateTime? ImportedAtUtc { get; set; }

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
  }
}
