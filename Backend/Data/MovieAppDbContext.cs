using System;
using System.Collections.Generic;
using System.Linq;                          // Required for .FirstOrDefault() and .ToListAsync()
using System.Threading.Tasks;               // Required for async methods
using Backend.Data.Models;   
using Backend.DTO.Movies;
using Backend.DTO.Watchlist;
using Microsoft.Data.SqlClient;             // Required for SqlParameter
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public partial class MovieAppDbContext : DbContext
{
    public MovieAppDbContext()
    {
    }

    public MovieAppDbContext(DbContextOptions<MovieAppDbContext> options)
        : base(options)
    {
    }

    // DbSets for your Entities
    public virtual DbSet<Movie> Movies { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Watchlist> Watchlists { get; set; } = null!;                       // For the purpose of overriding in extended Dbcontext

    //DbSets for Stored Procedure Result DTOs
    //matches the DTOs defined based on SP output
    public DbSet<MovieSummaryDTO> SpMovieResultDtos { get; set; } = null!;                  // For sp_GetMovies
    public DbSet<MoviesResponseDTO> SpMovieDetailsDtos { get; set; } = null!;               // sp_GetMovieDetails
    public DbSet<AddMovieResultDTO> SpAddMovieResultDtos { get; set; } = null!;             // For sp_AddMovie
    public DbSet<WatchlistResponseDTO> SpUserWatchlistResultDtos { get; set; } = null!;     // For sp_GetWatchlistByUser
    public DbSet<AddToWatchlistResultDTO> SpAddToWatchlistResultDtos { get; set; } = null!; // For sp_AddToWatchlist

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Entity Configurations
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Genre).HasMaxLength(100);
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 1)");
            entity.Property(e => e.PosterUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.Email, "UK_Email_Users").IsUnique();
            entity.HasIndex(e => e.UserName, "UK_UserName_Users").IsUnique();
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<Watchlist>(entity =>
        {
            entity.HasKey(e => e.WatchlistId);
            entity.ToTable("Watchlist");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);

            entity.HasOne(d => d.Movie).WithMany(p => p.Watchlists)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Watchlist_Movies");

            entity.HasOne(d => d.User).WithMany(p => p.Watchlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Watchlist_Users");
        });

        // Configure DTOs as Keyless Entity Types for SP results
        modelBuilder.Entity<MoviesResponseDTO>(e => {           // Used by SpMovieResultDtos
            e.HasNoKey();
            e.ToView(null);
        });

        modelBuilder.Entity<MovieSummaryDTO>(e => {             // Used by SpMovieResultDtos
            e.HasNoKey();
            e.ToView(null);
        });

        modelBuilder.Entity<AddMovieResultDTO>(e => {           // Used by SpAddMovieResultDtos
            e.HasNoKey();
            e.ToView(null);
        });

        modelBuilder.Entity<WatchlistResponseDTO>(e => {        // Used by SpUserWatchlistResultDtos
            e.HasNoKey();
            e.ToView(null);
        });

        modelBuilder.Entity<AddToWatchlistResultDTO>(e => {     // Used by SpAddToWatchlistResultDtos
            e.HasNoKey();
            e.ToView(null);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    // 1. sp_AddMovie
    public async Task<int?> ExecuteAddMovieSPAsync(MoviesRequestDTO movieDto)                                   // Task for async operation
    {
        var titleParam = new SqlParameter("@Title", movieDto.Title);
        var genreParam = new SqlParameter("@Genre", (object)movieDto.Genre ?? DBNull.Value);
        var releaseYearParam = new SqlParameter("@ReleaseYear", (object)movieDto.ReleaseYear ?? DBNull.Value);
        // sp_AddMovie takes @Rating, not @Description first based on provided SP.
        // Ensure parameter order and names match your SP definition *exactly*.
        var ratingParam = new SqlParameter("@Rating", (object)movieDto.Rating ?? DBNull.Value);
        var descriptionParam = new SqlParameter("@Description", (object)movieDto.Description ?? DBNull.Value);
        var posterUrlParam = new SqlParameter("@PosterUrl", (object)movieDto.PosterUrl ?? DBNull.Value);


        // Using SpAddMovieResultDtos for the result, defined in Db set above
        var result = await SpAddMovieResultDtos
            .FromSqlRaw("EXEC dbo.sp_AddMovie @Title, @Genre, @ReleaseYear, @Rating, @Description, @PosterUrl", // param order matches SP, needed
                titleParam, genreParam, releaseYearParam, ratingParam, descriptionParam, posterUrlParam)
            .ToListAsync();

        return result.FirstOrDefault()?.MovieId;                                                                // Accessing MovieId from AddMovieResultDTO
    }

    // 2. sp_GetMovies
    public async Task<List<MovieSummaryDTO>> ExecuteGetMoviesSPAsync()
    {
        // sp_GetMovies returns columns compatible with MovieSummaryDTO (Description will be null)
        return await SpMovieResultDtos
            .FromSqlRaw("EXEC dbo.sp_GetMovies")
            .ToListAsync();
    }

    // 3. sp_AddToWatchlist
    public async Task<AddToWatchlistResultDTO?> ExecuteAddToWatchlistSPAsync(int userId, int movieId, string status, int? rating)
    {
        var userIdParam = new SqlParameter("@UserId", userId);
        var movieIdParam = new SqlParameter("@MovieId", movieId);
        var statusParam = new SqlParameter("@Status", status);
        var ratingParam = new SqlParameter("@Rating", (object)rating ?? DBNull.Value);

        // Using SpAddToWatchlistResultDtos for the result
        var result = await SpAddToWatchlistResultDtos
            .FromSqlRaw("EXEC dbo.sp_AddToWatchlist @UserId, @MovieId, @Status, @Rating",
                userIdParam, movieIdParam, statusParam, ratingParam)
            .ToListAsync();                                                                                     // Use ToListAsync as FromSqlRaw returns IQueryable

        return result.FirstOrDefault();                                                                         // Returns AddToWatchlistResultDTO first item in list or null
    }

    // 4. sp_GetWatchlistByUser
    public async Task<List<WatchlistResponseDTO>> ExecuteGetWatchlistByUserSPAsync(int userId)
    {
        var userIdParam = new SqlParameter("@UserId", userId);
        // Using SpUserWatchlistResultDtos for the result
        return await SpUserWatchlistResultDtos
            .FromSqlRaw("EXEC dbo.sp_GetWatchlistByUser @UserId", userIdParam)                                  // execute the raw sql and maps it to the given DTO
            .ToListAsync();                                                                                     // executet the query built and make list of it
    }

    // 5. sp_UpdateRating
    public async Task<int> ExecuteUpdateWatchlistRatingSPAsync(int userId, int movieId, int rating)
    {
        var userIdParam = new SqlParameter("@UserId", userId);
        var movieIdParam = new SqlParameter("@MovieId", movieId);
        var ratingParam = new SqlParameter("@Rating", rating);

        return await Database.ExecuteSqlRawAsync(
            "EXEC dbo.sp_UpdateRating @UserId, @MovieId, @Rating",
            userIdParam, movieIdParam, ratingParam);
    }

    // 6. sp_GetMovieDetails
    public async Task<MoviesResponseDTO?> ExecuteGetMovieDetailsSPAsync(int movieId)
    {
        var movieIdParam = new SqlParameter("@MovieId", movieId);
        // sp_GetMovieDetails returns columns compatible with MovieDto (Description will be populated)
        var result = await SpMovieDetailsDtos
            .FromSqlRaw("EXEC dbo.sp_GetMovieDetails @MovieId", movieIdParam)
            .ToListAsync();

        return result.FirstOrDefault();
    }
}