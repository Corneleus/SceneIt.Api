using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SceneIt.Api.Dtos;
using SceneIt.Api.Interfaces;
using SceneIt.Api.Services;

namespace SceneIt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaItemsController : ControllerBase
    {
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IOmdbImportClient _omdbImportClient;

        public MediaItemsController(IMediaLibraryService mediaLibraryService, IOmdbImportClient omdbImportClient)
        {
            _mediaLibraryService = mediaLibraryService;
            _omdbImportClient = omdbImportClient;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<MediaItemResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMediaItems([FromQuery] string? kind)
        {
            var mediaItems = await _mediaLibraryService.GetAllMediaItemsAsync(kind);
            return Ok(mediaItems);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MediaItemResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MediaItemResponseDto>> GetById(int id)
        {
            var mediaItem = await _mediaLibraryService.GetMediaItemByIdAsync(id);

            if (mediaItem is null)
            {
                return NotFound();
            }

            return Ok(mediaItem);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IReadOnlyList<MediaItemResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<MediaItemResponseDto>>> Search([FromQuery] string query, [FromQuery] string? kind, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("A search query is required.");
            }

            try
            {
                var results = await _omdbImportClient.SearchAsync(query, cancellationToken);
                return Ok(results
                    .Where(result => MediaKindMatcher.Matches(result.Type, kind))
                    .ToList());
            }
            catch (OmdbException ex)
            {
                return Problem(title: "OMDb search failed.", detail: ex.Message, statusCode: ex.StatusCode);
            }
        }

        [HttpGet("lookup/{imdbId}")]
        [ProducesResponseType(typeof(MediaItemResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MediaItemResponseDto>> Lookup(string imdbId, CancellationToken cancellationToken)
        {
            try
            {
                var mediaItem = await _omdbImportClient.LookupByImdbIdAsync(imdbId, cancellationToken);

                if (mediaItem is null)
                {
                    return NotFound();
                }

                return Ok(mediaItem.ToMediaItemResponseDto());
            }
            catch (OmdbException ex)
            {
                return Problem(title: "OMDb lookup failed.", detail: ex.Message, statusCode: ex.StatusCode);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(MediaItemResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<MediaItemResponseDto>> Add([FromBody] CreateMediaItemRequestDto mediaItem)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var createdMediaItem = await _mediaLibraryService.AddMediaItemAsync(mediaItem);

            if (!createdMediaItem.Created)
            {
                var problem = new ProblemDetails
                {
                    Title = "Media item already exists.",
                    Detail = $"A media item with IMDb ID '{createdMediaItem.MediaItem.ImdbId}' already exists.",
                    Status = StatusCodes.Status409Conflict
                };

                problem.Extensions["mediaItemId"] = createdMediaItem.MediaItem.MediaItemId;
                problem.Extensions["imdbId"] = createdMediaItem.MediaItem.ImdbId;

                return Conflict(problem);
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdMediaItem.MediaItem.MediaItemId },
                createdMediaItem.MediaItem
            );
        }

        [HttpPatch("{id:int}/soft-delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var deleted = await _mediaLibraryService.SoftDeleteMediaItemAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HardDelete(int id)
        {
            var deleted = await _mediaLibraryService.HardDeleteMediaItemAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
