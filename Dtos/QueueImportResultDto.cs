namespace SceneIt.Api.Dtos
{
  public class QueueImportResultDto
  {
    public int QueuedCount { get; init; }
    public int SkippedCount { get; init; }
    public IReadOnlyList<ImportQueueItemResponseDto> QueuedItems { get; init; } = [];
  }
}
