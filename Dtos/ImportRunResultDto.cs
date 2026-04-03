namespace SceneIt.Api.Dtos
{
  public class ImportRunResultDto
  {
    public int ImportRunId { get; init; }
    public DateTime StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public int RequestedLimit { get; init; }
    public int AttemptedCount { get; init; }
    public int ImportedCount { get; init; }
    public int DuplicateCount { get; init; }
    public int FailedCount { get; init; }
    public string? Notes { get; init; }
  }
}
