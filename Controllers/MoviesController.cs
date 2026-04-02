using Microsoft.AspNetCore.Mvc;
using SceneIt.Api.Interfaces;
using Microsoft.AspNetCore.Http;
using SceneIt.Api.Models;

namespace SceneIt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService movieService;

        public MoviesController(IMovieService movieService)
        {
            this.movieService = movieService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await movieService.GetAllAsync();
            return Ok(movies);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Movie>> GetById(int id)
        {
            var movie = await movieService.GetByIdAsync(id);

            if (movie == null)
                return NotFound();

            return Ok(movie);
        }

        [HttpPost("add")]
        public async Task<ActionResult<Movie>> Add([FromBody] Movie movie)
        {
            var createdMovie = await movieService.AddAsync(movie);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdMovie.MovieId },
                createdMovie
            );
        }

        [HttpPatch("{id:int}/soft-delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var deleted = await movieService.SoftDeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HardDelete(int id)
        {
            var deleted = await movieService.HardDeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}

