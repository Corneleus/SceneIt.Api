using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SceneIt.Api.Data;
using SceneIt.Api.Interfaces;
using Microsoft.AspNetCore.Http;

namespace SceneIt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly SceneItDbContext _context;

        private readonly IMovieService movieService;

        public MoviesController(SceneItDbContext context, IMovieService movieService)
        {
            _context = context;
            this.movieService = movieService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _context.Movies.ToListAsync();
            return Ok(movies);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Movie> GetById(int id)
        {
            var movie = movieService.GetById(id);

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



    }


        
}








