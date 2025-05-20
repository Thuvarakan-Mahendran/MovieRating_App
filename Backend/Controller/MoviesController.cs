using Microsoft.AspNetCore.Mvc;
using Backend.Service;
using Backend.DTO.Movies;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MoviesService _moviesService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(MoviesService movieService, ILogger<MoviesController> logger)
        {
            _moviesService = movieService;
            _logger = logger;
        }

        // POST /api/movies/add
        [HttpPost("add")]
        public async Task<IActionResult> AddMovie([FromBody] MoviesRequestDTO createMovieDto)
        {
            if (!ModelState.IsValid)                                                            //To validate the DTO received
            {
                return BadRequest(ModelState);
            }

            var newMovieId = await _moviesService.AddMovieAsync(createMovieDto);

            if (newMovieId == null)
            {
                _logger.LogWarning("Failed to add movie: {Title}", createMovieDto.Title);
                return StatusCode(500, "An error occurred while adding the movie.");
            }

            var createdMovie = await _moviesService.GetMovieDetailsAsync(newMovieId.Value);
            if (createdMovie == null)
            {
                _logger.LogWarning("Movie added with ID {MovieId} but could not be retrieved.", newMovieId.Value);
                return StatusCode(201, new { movieId = newMovieId.Value, message = "Movie created but could not be fully retrieved." });
            }

            return CreatedAtAction(nameof(GetMovieById), new { movieId = createdMovie.MovieId }, createdMovie);
        }

        // GET /api/movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieSummaryDTO>>> GetAllMovies()
        {
            var movies = await _moviesService.GetAllMoviesAsync();
            return Ok(movies);
        }

        // GET /api/movies/{movieId}
        [HttpGet("{movieId}")]
        public async Task<ActionResult<MoviesResponseDTO>> GetMovieById([FromRoute]int movieId)
        {
            if (movieId <= 0)
            {
                return BadRequest("Movie ID must be a positive integer.");
            }

            var movie = await _moviesService.GetMovieDetailsAsync(movieId);

            if (movie == null)
            {
                return NotFound($"Movie with ID {movieId} not found.");
            }

            return Ok(movie);
        }
    }
}