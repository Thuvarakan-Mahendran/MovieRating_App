import { Component, OnInit } from '@angular/core';
import { Movie, MovieCardComponent } from '../movie-card/movie-card.component';
import { MovieService } from '../../services/movie.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,                                                 // For the use of *ngIf
    MovieCardComponent
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  movies: Movie[] = [];
  paginatedMovies: Movie[][] = [];                                // Stores movies in chunks of 3
  currentPage: number = 0;
  isLoading: boolean = true;                                      // for loading indication
  error: string | null = null;
  heroPosters: string[] = [];
  currentHeroPosterIndex: number = 0;
  currentHeroPosterUrl: string = '';
  posterInterval: any;

  constructor(private movieService : MovieService) { }

  ngOnInit(): void {
    this.loadMovies();
  }

  loadMovies(): void {
    this.isLoading = true;
    this.error = null;
    this.movieService.getMovies().subscribe({
      next: (data) => {
        this.movies = data;
        this.paginatedMovies = this.chunkMovies(data, 3);         // Break into chunks of 3
        this.currentPage = 0;
        this.updateHeroPosters();
        this.startPosterSlideshow();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching movies:', err);
        this.error = 'Failed to load movies. Please try again later.';
        // err.status can be checked for error handling specifically
        this.isLoading = false;
      }
    });
  }

  chunkMovies(movies: Movie[], chunkSize: number): Movie[][] {
    const chunks: Movie[][] = [];
    for (let i = 0; i < movies.length; i += chunkSize) {
      chunks.push(movies.slice(i, i + chunkSize));
    }
    return chunks;
  }

  nextPage(): void {
    if (this.currentPage < this.paginatedMovies.length - 1) {
      this.currentPage++;
      this.updateHeroPosters();
      this.restartPosterSlideshow();
    }
  }

  prevPage(): void {
    if (this.currentPage > 0) {
      this.currentPage--;
      this.updateHeroPosters();
      this.restartPosterSlideshow();
    }
  }

  updateHeroPosters(): void {
    const currentPageMovies = this.paginatedMovies[this.currentPage] || [];
    // this.heroPosters = currentPageMovies.map(movie => movie.posterUrl);
    this.heroPosters = currentPageMovies.map(movie => movie.posterUrl).filter((url): url is string => !!url);
    this.currentHeroPosterIndex = 0;
    this.currentHeroPosterUrl = this.heroPosters[0] || '';
  }

  startPosterSlideshow(): void {
    this.posterInterval = setInterval(() => {
      if (this.heroPosters.length > 0) {
        this.currentHeroPosterIndex = (this.currentHeroPosterIndex + 1) % this.heroPosters.length;
        this.currentHeroPosterUrl = this.heroPosters[this.currentHeroPosterIndex];
      }
    }, 2000);                                                               // 2 seconds
  }

  restartPosterSlideshow(): void {
    clearInterval(this.posterInterval);
    this.startPosterSlideshow();
  }

  ngOnDestroy(): void {
    clearInterval(this.posterInterval);                                     // Clean up when component is destroyed
  }

  getSliderTransform(): string {
    return `translateX(-${this.currentPage * 100}%)`;
  }
}