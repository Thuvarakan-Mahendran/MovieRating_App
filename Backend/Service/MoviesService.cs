using Backend.Data;
using Backend.DTO.Movies;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Service
{
    public class MoviesService
    {
        private readonly MovieAppDbContext _context;
        private readonly ILogger<MoviesService> _logger;

        public MoviesService(MovieAppDbContext context, ILogger<MoviesService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int?> AddMovieAsync(MoviesRequestDTO createMovieDto)
        {
            try
            {
                return await _context.ExecuteAddMovieSPAsync(createMovieDto);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred in MovieService.AddMovieAsync while adding movie: {Title}", createMovieDto.Title);
                return null;
            }
        }

        public async Task<IEnumerable<MovieSummaryDTO>> GetAllMoviesAsync()
        {
            try
            {
                return await _context.ExecuteGetMoviesSPAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred in MovieService.GetAllMoviesAsync");
                return new List<MovieSummaryDTO>();                                         // Return empty list on error or throw
            }
        }

        public async Task<MoviesResponseDTO?> GetMovieDetailsAsync(int movieId)
        {
            try
            {
                return await _context.ExecuteGetMovieDetailsSPAsync(movieId);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred in MovieService.GetMovieDetailsAsync for movieId: {MovieId}", movieId);
                return null;
            }
        }
    }
}