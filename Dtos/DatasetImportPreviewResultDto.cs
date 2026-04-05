namespace SceneIt.Api.Dtos
{
  public class DatasetImportPreviewResultDto
  {
    public int TotalRowsScanned { get; set; }
    public int MatchedCount { get; set; }
    public int AlreadyQueuedCount { get; set; }
    public int AlreadyImportedCount { get; set; }
    public int ReadyToQueueCount { get; set; }
  }
}
