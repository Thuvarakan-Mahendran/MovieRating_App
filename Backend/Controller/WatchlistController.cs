using Microsoft.AspNetCore.Mvc;
using Backend.Service;
using Backend.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Backend.DTO.Watchlist;

namespace Backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchlistController : ControllerBase
    {
        private readonly WatchlistService _watchlistService;
        private readonly ILogger<WatchlistController> _logger;

        public WatchlistController(WatchlistService watchlistService, ILogger<WatchlistController> logger)
        {
            _watchlistService = watchlistService;
            _logger = logger;
        }

        // POST /api/watchlist/add
        [HttpPost("add")]
        public async Task<IActionResult> AddToWatchlist([FromBody] WatchlistRequestDTO createWatchlistDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Attempting to add movie {MovieId} to watchlist for user {UserId}", createWatchlistDto.MovieId, createWatchlistDto.UserId);

            var result = await _watchlistService.AddToWatchlistAsync(createWatchlistDto);

            if (result == null)
            {
                _logger.LogError("Failed to add movie {MovieId} to watchlist for user {UserId}. Service returned null.", createWatchlistDto.MovieId, createWatchlistDto.UserId);
                return StatusCode(500, "An error occurred while adding the movie to the watchlist.");
            }

            return Ok(new { message = "Operation completed. Movie is in watchlist.", watchlistId = result.WatchlistId });
        }

        // GET /api/watchlist/2
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<WatchlistResponseDTO>>> GetUserWatchlist([FromRoute] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("A valid User ID is required.");
            }

            var watchlistItems = await _watchlistService.GetWatchlistByUserAsync(userId);
            return Ok(watchlistItems);
        }

        // PUT /api/watchlist/rating
        [HttpPut("rating")]
        public async Task<IActionResult> UpdateWatchlistRating([FromBody] UpdateWatchlistRatingDTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Attempting to update rating for movie {MovieId} in watchlist for user {UserId} to {Rating}",
                updateDto.MovieId, updateDto.UserId, updateDto.Rating);

            bool success = await _watchlistService.UpdateWatchlistMovieRatingAsync(updateDto);

            if (success)
            {
                return NoContent();                                                                 // Standard response for a successful PUT that doesn't return content
            }
            else
            {
                _logger.LogWarning("Failed to update rating or item not found for User {UserId}, Movie {MovieId}", updateDto.UserId, updateDto.MovieId);
                return NotFound("Watchlist item not found for the given user and movie, or rating was already set to this value.");
                                                                                                    // o rows affected, general server error, item not found
            }
        }
    }
}