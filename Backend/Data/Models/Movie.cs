using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Models;

public partial class Movie
{
    [Key]
    public int MovieId { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    [StringLength(100)]
    public string? Genre { get; set; }

    public int? ReleaseYear { get; set; }

    [Column(TypeName = "decimal(3, 1)")]
    public decimal? Rating { get; set; }

    public string? Description { get; set; }

    [StringLength(500)]
    [Url]
    public string? PosterUrl { get; set; }

    [InverseProperty("Movie")]
    public virtual ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
}
