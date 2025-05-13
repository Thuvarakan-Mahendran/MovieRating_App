using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Models;

[Table("Watchlist")]
public partial class Watchlist
{
    [Key]
    public int WatchlistId { get; set; }

    public int UserId { get; set; }

    public int MovieId { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    public int? Rating { get; set; }

    [ForeignKey("MovieId")]
    [InverseProperty("Watchlists")]
    public virtual Movie Movie { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Watchlists")]
    public virtual User User { get; set; } = null!;
}
