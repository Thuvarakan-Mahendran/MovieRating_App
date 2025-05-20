import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule, Location } from '@angular/common';
import { Subscription, forkJoin, of } from 'rxjs'; // forkJoin for parallel calls, of for default values
import { switchMap, catchError, tap } from 'rxjs/operators';

import { MovieService } from '../../services/movie.service';
import { WatchlistService } from '../../services/watchlist.service'; // Import WatchlistService
import { MovieDetails } from '../../models/movie-details.model';
import { UpdateWatchlistRating } from '../../models/update-watchlist-rating.model';
import { WatchlistItem } from '../../models/watchlist-item.model';
// import { AuthService } from '../../services/auth.service'; // For currentUserId

@Component({
  selector: 'app-movie-detail',
  imports: [
    CommonModule
  ],
  templateUrl: './movie-detail.component.html',
  styleUrls: ['./movie-detail.component.css']
})
export class MovieDetailComponent implements OnInit, OnDestroy {
  movie: MovieDetails | null = null;
  heroBackgroundUrl: string = 'assets/images/batman-hero.jpg';
  isLoading: boolean = true;
  error: string | null = null;

  userRating: number = 0; // User's current rating (1-5, 0 if not rated)
  hoveredRating: number = 0;
  maxRating: number = 5;
  currentMovieId: number = 0;

  private routeSub: Subscription | undefined;
  private dataLoadSub: Subscription | undefined;
  private ratingUpdateSub: Subscription | undefined;

  // ** IMPORTANT: Placeholder for current user ID.
  // In a real app, you'd get this from an AuthService after login.
  // This should be treated as a global variable or obtained from a shared service.
  private currentUserId: number = 2; // Example User ID (replace with actual global/service value)

  constructor(
    private route: ActivatedRoute,
    private movieService: MovieService,
    private watchlistService: WatchlistService, // Inject WatchlistService
    private location: Location,
    private router: Router,
    private cdRef: ChangeDetectorRef
    // private authService: AuthService // Inject your AuthService
  ) {}

  ngOnInit(): void {
    // Example of getting from a global variable (not best practice, service is better)
    // if (window.hasOwnProperty('currentGlobalUserId')) {
    //   this.currentUserId = (window as any).currentGlobalUserId;
    // }
    // Or from authService:
    // this.currentUserId = this.authService.getCurrentUserId();

    if (!this.currentUserId) {
        this.error = "User not identified. Cannot load movie ratings.";
        this.isLoading = false;
        // Potentially redirect to login or show an appropriate message
        return;
    }

    this.routeSub = this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.currentMovieId = +id;
        this.loadMovieData(this.currentMovieId);
      } else {
        this.error = "Movie ID not found in URL.";
        this.isLoading = false;
        this.router.navigate(['/dashboard']); // Or some error page
      }
    });
  }

  loadMovieData(movieId: number): void {
    this.isLoading = true;
    this.error = null;
    if (this.dataLoadSub) {
      this.dataLoadSub.unsubscribe();
    }

    // Fetch movie details and user's watchlist in parallel
    this.dataLoadSub = forkJoin({
      movieDetails: this.movieService.getMovieById(movieId).pipe(
        catchError(err => {
          this.error = `Failed to load movie details: ${err.message || 'Unknown error'}`;
          return of(null); // Return null on error to allow forkJoin to complete
        })
      ),
      userWatchlist: this.watchlistService.getUserWatchlist(this.currentUserId).pipe(
        catchError(err => {
          console.warn(`Failed to load user watchlist (this is not critical for movie details page if user hasn't rated): ${err.message || 'Unknown error'}`);
          return of([]); // Return empty array on error
        })
      )
    }).subscribe({
      next: (results) => {
        if (results.movieDetails) {
          this.movie = results.movieDetails;
          // Set background from poster if available
          // this.heroBackgroundUrl = this.movie.posterUrl || 'assets/images/batman-hero.jpg';
        } else {
          // Error was already set by the movieDetails pipe
          if (!this.error) this.error = "Movie details could not be loaded.";
        }

        // Find the current movie in the user's watchlist to get their rating
        const watchlistItem = results.userWatchlist.find(item => item.movieId === movieId);
        this.userRating = watchlistItem?.userRating || 0;

        this.isLoading = false;
        this.cdRef.detectChanges();
      },
      error: (forkJoinError) => {
        // This error block for forkJoin itself is usually not hit if individual streams handle errors with of(null) or of([])
        this.error = `An unexpected error occurred while loading data: ${forkJoinError.message || 'Unknown error'}`;
        this.isLoading = false;
        this.cdRef.detectChanges();
      }
    });
  }

  // reload just the movie details (e.g., after rating update if avg rating changes)
  reloadMovieDetails(movieId: number): void {
    this.isLoading = true; // Can be a smaller loading indicator for just avg rating
    this.movieService.getMovieById(movieId).subscribe({
        next: (data) => {
            this.movie = data; // Update movie object with new average rating
            this.isLoading = false;
            this.cdRef.detectChanges();
        },
        error: (err) => {
            console.error("Error reloading movie details:", err);
            this.isLoading = false; // Ensure loading is turned off
            // Optionally show a small error message for this part
        }
    });
  }


  getStarArray(count: number = this.maxRating): number[] {
    return Array(count).fill(0).map((x, i) => i + 1);
  }

  getAverageStarClass(starPosition: number): string {
    if (!this.movie || this.movie.rating === null || this.movie.rating === undefined) {
      return 'far fa-star text-muted';
    }
    const roundedAverageRating = Math.round(this.movie.rating);
    return starPosition <= roundedAverageRating ? 'fas fa-star text-warning' : 'far fa-star text-muted';
  }

  getUserStarClass(starPosition: number): string {
    const ratingToShow = this.hoveredRating > 0 ? this.hoveredRating : this.userRating;
    return starPosition <= ratingToShow ? 'fas fa-star user-star active' : 'far fa-star user-star';
  }

  onStarHover(rating: number): void {
    this.hoveredRating = rating;
  }

  onStarLeave(): void {
    this.hoveredRating = 0;
  }

  setUserRating(newRating: number): void {
    if (this.isLoading || !this.movie || !this.currentUserId) return;

    const previousUserRating = this.userRating; // Store for potential rollback
    this.userRating = newRating; // Optimistic UI update
    this.hoveredRating = 0;

    const payload: UpdateWatchlistRating = {
      userId: this.currentUserId,
      movieId: this.movie.movieId,
      rating: newRating
    };

    this.isLoading = true; // Indicate processing (could be a specific spinner for rating)
    if (this.ratingUpdateSub) {
      this.ratingUpdateSub.unsubscribe();
    }
    this.ratingUpdateSub = this.watchlistService.updateUserRating(payload).subscribe({
      next: () => {
        console.log('User rating updated successfully on backend!');
        // After successful rating update, reload movie details to get the potentially updated average rating.
        this.reloadMovieDetails(this.currentMovieId);
        // isLoading will be set to false by reloadMovieDetails
      },
      error: (err) => {
        // this.refreshPage();
        this.error = `Failed to update rating: ${err.message || 'Unknown error'}`;
        this.userRating = previousUserRating; // Rollback optimistic update
        this.isLoading = false;
        this.cdRef.detectChanges();
        // this.refreshPage();
      }
    });
  }

  onRateMeClick(): void {
    if (this.userRating > 0) {
      this.setUserRating(this.userRating);
      // this.refreshPage();
    } else {
      alert("Please select a star rating first by clicking on the stars.");
    }
  }

  goBack(): void {
    this.location.back();
  }

  refreshPage(): void { 
      this.router.navigate([this.router.url]);
  }

  ngOnDestroy(): void {
    this.routeSub?.unsubscribe();
    this.dataLoadSub?.unsubscribe();
    this.ratingUpdateSub?.unsubscribe();
  }
}