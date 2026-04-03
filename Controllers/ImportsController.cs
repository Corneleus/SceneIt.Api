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
    private readonly IMovieImportService _movieImportService;

    public ImportsController(IMovieImportService movieImportService)
    {
      _movieImportService = movieImportService;
    }

    [HttpPost("queue")]
    [ProducesResponseType(typeof(QueueImportResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<QueueImportResultDto>> Queue([FromBody] QueueImportRequestDto request, CancellationToken cancellationToken)
    {
      if (!ModelState.IsValid)
      {
        return ValidationProblem(ModelState);
      }

      var result = await _movieImportService.QueueAsync(request.Items, cancellationToken);
      return Ok(result);
    }

    [HttpGet("queue")]
    [ProducesResponseType(typeof(IReadOnlyList<ImportQueueItemResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ImportQueueItemResponseDto>>> GetQueue(CancellationToken cancellationToken)
    {
      var items = await _movieImportService.GetQueueAsync(cancellationToken);
      return Ok(items);
    }

    [HttpPost("run")]
    [ProducesResponseType(typeof(ImportRunResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ImportRunResultDto>> Run([FromBody] ImportRunRequestDto? request, CancellationToken cancellationToken)
    {
      var result = await _movieImportService.RunBatchAsync(request?.MaxCount ?? 100, cancellationToken);
      return Ok(result);
    }

    [HttpGet("runs")]
    [ProducesResponseType(typeof(IReadOnlyList<ImportRunResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ImportRunResultDto>>> GetRuns(CancellationToken cancellationToken)
    {
      var runs = await _movieImportService.GetRunsAsync(cancellationToken);
      return Ok(runs);
    }
  }
}
