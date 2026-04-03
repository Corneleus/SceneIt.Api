using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SceneIt.Api.Dtos;
using SceneIt.Api.Interfaces;
using SceneIt.Api.Services;

namespace SceneIt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService movieService;
        private readonly IOmdbImportClient omdbImportClient;

        public MoviesController(IMovieService movieService, IOmdbImportClient omdbImportClient)
        {
            this.movieService = movieService;
            this.omdbImportClient = omdbImportClient;
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IReadOnlyList<MovieResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await movieService.GetAllAsync();
            return Ok(movies);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MovieResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieResponseDto>> GetById(int id)
        {
            var movie = await movieService.GetByIdAsync(id);

            if (movie is null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IReadOnlyList<MovieResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<MovieResponseDto>>> Search([FromQuery] string query, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("A search query is required.");
            }

            try
            {
                var results = await omdbImportClient.SearchMoviesAsync(query, cancellationToken);
                return Ok(results);
            }
            catch (OmdbException ex)
            {
                return Problem(title: "OMDb search failed.", detail: ex.Message, statusCode: ex.StatusCode);
            }
        }

        [HttpGet("lookup/{imdbId}")]
        [ProducesResponseType(typeof(MovieResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieResponseDto>> Lookup(string imdbId, CancellationToken cancellationToken)
        {
            try
            {
                var movie = await omdbImportClient.GetMovieByImdbIdAsync(imdbId, cancellationToken);

                if (movie is null)
                {
                    return NotFound();
                }

                return Ok(movie.ToResponseDto());
            }
            catch (OmdbException ex)
            {
                return Problem(title: "OMDb lookup failed.", detail: ex.Message, statusCode: ex.StatusCode);
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(MovieResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<MovieResponseDto>> Add([FromBody] CreateMovieRequestDto movie)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var createdMovie = await movieService.AddAsync(movie);

            if (!createdMovie.Created)
            {
                var problem = new ProblemDetails
                {
                    Title = "Movie already exists.",
                    Detail = $"A movie with IMDb ID '{createdMovie.Movie.ImdbId}' already exists.",
                    Status = StatusCodes.Status409Conflict
                };

                problem.Extensions["movieId"] = createdMovie.Movie.MovieId;
                problem.Extensions["imdbId"] = createdMovie.Movie.ImdbId;

                return Conflict(problem);
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdMovie.Movie.MovieId },
                createdMovie.Movie
            );
        }

        [HttpPatch("{id:int}/soft-delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var deleted = await movieService.SoftDeleteAsync(id);

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
            var deleted = await movieService.HardDeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
