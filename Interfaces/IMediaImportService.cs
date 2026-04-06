using SceneIt.Api.Dtos;

namespace SceneIt.Api.Interfaces
{
  public interface IMediaImportService
  {
    Task<QueueImportResultDto> QueueAsync(IReadOnlyList<ImportQueueItemDto> items, CancellationToken cancellationToken = default);
    Task<DatasetImportPreviewResultDto> PreviewDatasetAsync(DatasetImportPreviewRequestDto request, CancellationToken cancellationToken = default);
    Task<QueueDatasetImportsResultDto> QueueDatasetAsync(QueueDatasetImportsRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ImportQueueItemResponseDto>> GetQueueAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ImportRunResultDto>> GetRunsAsync(CancellationToken cancellationToken = default);
    Task<ImportRunResultDto> RunBatchAsync(int maxCount, CancellationToken cancellationToken = default);
  }
}
