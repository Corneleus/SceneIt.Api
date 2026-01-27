using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SceneIt.Api.Data;

namespace SceneIt.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class MoviesController : ControllerBase
  {
    private readonly SceneItDbContext _context;

    public MoviesController(SceneItDbContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMovies()
    {
      var movies = await _context.Movies.ToListAsync();
      return Ok(movies);
    }
  }
}
