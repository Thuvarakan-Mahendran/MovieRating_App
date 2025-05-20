namespace Backend.DTO.Watchlist
{
    public class WatchlistRequestDTO
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public string Status { get; set; }
        public int? Rating { get; set; }
    }
}
