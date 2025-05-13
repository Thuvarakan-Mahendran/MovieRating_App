using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTO.Movies
{
    public class MoviesResponseDTO
    {
        public int MovieId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Genre { get; set; }

        public int? ReleaseYear { get; set; }

        public decimal? Rating { get; set; }

        public string? Description { get; set; }

        public string? PosterUrl { get; set; }
    }
}
