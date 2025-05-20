using Backend.Data;
using Backend.DTO.Watchlist;
using Backend.DTO;   
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Service
{
    public class WatchlistService
    {
        private readonly MovieAppDbContext _context;
        private readonly ILogger<WatchlistService> _logger;

        public WatchlistService(MovieAppDbContext context, ILogger<WatchlistService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AddToWatchlistResultDTO?> AddToWatchlistAsync(WatchlistRequestDTO createWatchlistDto)
        {
            try
            {
                return await _context.ExecuteAddToWatchlistSPAsync(createWatchlistDto.UserId, createWatchlistDto.MovieId, createWatchlistDto.Status, createWatchlistDto.Rating);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in WatchlistService.AddToWatchlistAsync for User {UserId}, Movie {MovieId}", createWatchlistDto.UserId, createWatchlistDto.MovieId);
                return null;
            }
        }

        public async Task<IEnumerable<WatchlistResponseDTO>> GetWatchlistByUserAsync(int userId)
        {
            try
            {
                return await _context.ExecuteGetWatchlistByUserSPAsync(userId);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in WatchlistService.GetWatchlistByUserAsync for User {UserId}", userId);
                return new List<WatchlistResponseDTO>();                                                                // Return empty or throw
            }
        }

        public async Task<bool> UpdateWatchlistMovieRatingAsync(UpdateWatchlistRatingDTO updateDto)
        {
            try
            {
                int rowsAffected = await _context.ExecuteUpdateWatchlistRatingSPAsync(updateDto.UserId, updateDto.MovieId, updateDto.Rating);   //number of rows affected will be returned
                return rowsAffected > 0;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in WatchlistService.UpdateWatchlistMovieRatingAsync for User {UserId}, Movie {MovieId}", updateDto.UserId, updateDto.MovieId);
                return false;
            }
        }
    }
}