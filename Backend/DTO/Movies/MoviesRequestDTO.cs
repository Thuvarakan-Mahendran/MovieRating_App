using System.ComponentModel.DataAnnotations;

namespace Backend.DTO.Movies
{
    public class MoviesRequestDTO
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Genre { get; set; }

        public int? ReleaseYear { get; set; }

        public decimal? Rating { get; set; }

        public string? Description { get; set; }

        [StringLength(500)]
        [Url]
        public string? PosterUrl { get; set; }
    }
}
