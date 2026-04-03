using System.ComponentModel.DataAnnotations;

namespace SceneIt.Api.Models
{
  public class ImportRun
  {
    public int ImportRunId { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public int RequestedLimit { get; set; }
    public int AttemptedCount { get; set; }
    public int ImportedCount { get; set; }
    public int DuplicateCount { get; set; }
    public int FailedCount { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
  }
}
