using System.ComponentModel.DataAnnotations;

namespace Backend.DTO.Watchlist
{
    public class UpdateWatchlistRatingDTO
    {
        public int UserId { get; set; }

        public int MovieId { get; set; }

        public int Rating { get; set; }
    }
}
