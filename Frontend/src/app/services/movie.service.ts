import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs';
import { Movie } from '../components/movie-card/movie-card.component';
import { MovieDetails } from '../models/movie-details.model';
import { environment } from '../../environments/environment';
import { MovieAdd } from '../models/movie-add.model';

@Injectable({
  providedIn: 'root'                                                              // Service available application-wide
})
export class MovieService {
  private apiUrl = environment.apiUrl + '/movies';                                // e.g., http://localhost:5000/api/movies
  // private apiUrl = environment.apiUrl + '/watchlist';

  constructor(private http: HttpClient) { }

  getMovies(): Observable<Movie[]> {
    return this.http.get<Movie[]>(this.apiUrl);
  }

  getMovieById(movieId: number): Observable<MovieDetails> {
    return this.http.get<MovieDetails>(`${this.apiUrl}/${movieId}`);
  }

  addMovie(movieData: MovieAdd): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/add`, movieData);
  }
  //handleError

}