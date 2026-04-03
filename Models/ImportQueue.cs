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

    public ImportQueueStatus Status { get; set; } = ImportQueueStatus.Pending;
    public int AttemptCount { get; set; }
    public DateTime? LastAttemptedAtUtc { get; set; }
    public DateTime? ImportedAtUtc { get; set; }

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
  }
}
