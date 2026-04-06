using System.ComponentModel.DataAnnotations;

namespace SceneIt.Api.Models
{
  public class MediaItem
  {
    [Key]
    public int MediaItemId { get; set; }

    [Required]
    [MaxLength(255)]
    public required string Title { get; set; }

    [MaxLength(20)]
    public string? Year { get; set; }

    [MaxLength(255)]
    public string? Rated { get; set; }

    public DateTime? Released { get; set; }

    [MaxLength(255)]
    public string? Runtime { get; set; }

    [MaxLength(255)]
    public string? Genre { get; set; }

    [MaxLength(255)]
    public string? Director { get; set; }

    [MaxLength(500)]
    public string? Writer { get; set; }

    [MaxLength(500)]
    public string? Actors { get; set; }

    [MaxLength(1000)]
    public string? Plot { get; set; }

    [MaxLength(500)]
    public string? Language { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(255)]
    public string? Awards { get; set; }

    [MaxLength(1000)]
    public string? Poster { get; set; }

    [MaxLength(20)]
    public string? Metascore { get; set; }

    [MaxLength(20)]
    public string? ImdbRating { get; set; }

    [MaxLength(50)]
    public string? ImdbVotes { get; set; }

    [Required]
    [MaxLength(50)]
    public string ImdbId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Type { get; set; }

    [MaxLength(255)]
    public string? Dvd { get; set; }

    [MaxLength(255)]
    public string? BoxOffice { get; set; }

    [MaxLength(255)]
    public string? Production { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }

    public ICollection<UserMediaItem> UserMediaItems { get; set; } = new List<UserMediaItem>();
  }
}
