namespace SceneIt.Api.Dtos
{
  public class ImportQueueItemResponseDto
  {
    public int ImportQueueId { get; set; }
    public required string ImdbId { get; set; }
    public string? Title { get; set; }
    public required string Status { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? LastAttemptedAtUtc { get; set; }
    public DateTime? ImportedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
  }
}
