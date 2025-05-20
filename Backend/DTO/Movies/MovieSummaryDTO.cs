namespace Backend.DTO.Movies
{
    public class MovieSummaryDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Genre { get; set; }
        public int? ReleaseYear { get; set; }
        public decimal? Rating { get; set; } 
        public string? PosterUrl { get; set; }
    }
}
