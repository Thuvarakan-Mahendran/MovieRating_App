namespace Backend.DTO.Watchlist
{
    public class WatchlistResponseDTO
    {
        public int WatchlistId { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string? MoviePosterUrl { get; set; }
        public int? MovieReleaseYear { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? UserRating { get; set; }
    }
}
