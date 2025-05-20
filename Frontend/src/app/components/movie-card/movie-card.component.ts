import { CommonModule } from '@angular/common';
import { Component, Input} from '@angular/core';
import { Router } from '@angular/router';
import { WatchlistService } from '../../services/watchlist.service';
import { WatchlistAdd } from '../../models/watchlist-add.model';

export interface Movie {
  movieId: number;              
  title: string;
  genre: string | null;
  releaseYear: number | null;
  rating: number | null;
  posterUrl: string | null;
}

@Component({
  selector: 'app-movie-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './movie-card.component.html',
  styleUrls: ['./movie-card.component.css'] 
})
export class MovieCardComponent {
  @Input() movie!: Movie;
  private userId = 2;

  constructor(
    private router: Router,
    private watchlistService: WatchlistService
  ){}

  goToDetails(movieId: number): void {
    this.router.navigate(['/movie-detail', movieId]);
  }
  
  addtoWatchlist(): void {
    if (!this.userId) {
      alert('Please select a user first.');
      return;
    }

    const payload: WatchlistAdd = {
      userId: this.userId,
      movieId: this.movie.movieId,
      status: 'To Watch',                                         // Default status
      rating: null                                                // Initial rating
    };

    this.watchlistService.addtoWatchlist(payload).subscribe({
      next: (response) => {
        alert('Movie added to watchlist!');
      },
      error: (error) => {
        // alert('Failed to add to watchlist: ' + error.message);
      }
    });
  }
}