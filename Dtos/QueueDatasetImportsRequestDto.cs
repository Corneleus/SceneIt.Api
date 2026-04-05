namespace SceneIt.Api.Dtos
{
  public class QueueDatasetImportsRequestDto : DatasetImportPreviewRequestDto
  {
    public int? MaxToQueue { get; set; }
  }
}
