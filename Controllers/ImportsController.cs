using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SceneIt.Api.Dtos;
using SceneIt.Api.Interfaces;

namespace SceneIt.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ImportsController : ControllerBase
  {
    private readonly IMediaImportService _mediaImportService;

    public ImportsController(IMediaImportService mediaImportService)
    {
      _mediaImportService = mediaImportService;
    }

    [HttpPost("queue")]
    [ProducesResponseType(typeof(QueueImportResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<QueueImportResultDto>> Queue([FromBody] QueueImportRequestDto request, CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid)
      {
        return ValidationProblem(ModelState);
      }

      var result = await _mediaImportService.QueueAsync(request.Items, cancellationToken);
      return Ok(result);
    }

    [HttpGet("queue")]
    [ProducesResponseType(typeof(IReadOnlyList<ImportQueueItemResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ImportQueueItemResponseDto>>> GetQueue(CancellationToken cancellationToken)
    {
      var items = await _mediaImportService.GetQueueAsync(cancellationToken);
      return Ok(items);
    }

    [HttpPost("dataset/preview")]
    [ProducesResponseType(typeof(DatasetImportPreviewResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DatasetImportPreviewResultDto>> PreviewDataset([FromBody] DatasetImportPreviewRequestDto request, CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid)
      {
        return ValidationProblem(ModelState);
      }

      try
      {
        var result = await _mediaImportService.PreviewDatasetAsync(request, cancellationToken);
        return Ok(result);
      }
      catch (Exception ex) when (ex is ArgumentException or FileNotFoundException or InvalidDataException)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpPost("dataset/queue")]
    [ProducesResponseType(typeof(QueueDatasetImportsResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<QueueDatasetImportsResultDto>> QueueDataset([FromBody] QueueDatasetImportsRequestDto request, CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid)
      {
        return ValidationProblem(ModelState);
      }

      try
      {
        var result = await _mediaImportService.QueueDatasetAsync(request, cancellationToken);
        return Ok(result);
      }
      catch (Exception ex) when (ex is ArgumentException or FileNotFoundException or InvalidDataException)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpPost("run")]
    [ProducesResponseType(typeof(ImportRunResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ImportRunResultDto>> Run([FromBody] ImportRunRequestDto? request, CancellationToken cancellationToken)
    {
      var result = await _mediaImportService.RunBatchAsync(request?.MaxCount ?? 100, cancellationToken);
      return Ok(result);
    }

    [HttpGet("runs")]
    [ProducesResponseType(typeof(IReadOnlyList<ImportRunResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ImportRunResultDto>>> GetRuns(CancellationToken cancellationToken)
    {
      var runs = await _mediaImportService.GetRunsAsync(cancellationToken);
      return Ok(runs);
    }
  }
}
