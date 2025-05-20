// src/app/services/watchlist.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { WatchlistItem } from '../models/watchlist-item.model';
import { environment } from '../../environments/environment';
import { UpdateWatchlistRating } from '../models/update-watchlist-rating.model';
import { WatchlistAdd } from '../models/watchlist-add.model';

@Injectable({
  providedIn: 'root'
})
export class WatchlistService {
  private apiUrl = environment.apiUrl + '/watchlist/2';
  private watchlistApiUrl = environment.apiUrl + '/watchlist';
  private http = inject(HttpClient);

  constructor() { }

  // Method to get watchlist for a specific user
  getUserWatchlist(userId: number): Observable<WatchlistItem[]> {
    if (userId <= 0) {
      return throwError(() => new Error('Invalid User ID provided to WatchlistService.'));
    }

    const params = new HttpParams().set('userId', userId.toString());

    return this.http.get<WatchlistItem[]>(this.apiUrl).pipe(
      tap(data => console.log('Fetched watchlist:', data)),                                               // For debugging
      catchError(this.handleError)
    );
  }

  // Endpoint: PUT /api/watchlist/rating
  updateUserRating(payload: UpdateWatchlistRating): Observable<any> {
    return this.http.put(`${this.watchlistApiUrl}/rating`, payload).pipe(
      tap(() => console.log(`Rating updated for movieId: ${payload.movieId}, userId: ${payload.userId}`)),
      catchError(this.handleError)
    );
  }

  addtoWatchlist(payload : WatchlistAdd) : Observable<any> {
    return this.http.post(`${this.watchlistApiUrl}/add`, payload);
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      // Client-side or network error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Backend returned an unsuccessful response code
      errorMessage = `Server returned code ${error.status}, error message is: ${error.message}`;
      if (error.error && typeof error.error === 'string') {
        errorMessage += ` Details: ${error.error}`;
      } else if (error.error && error.error.title) {                                                      // For ASP.NET Core problem details
        errorMessage += ` Details: ${error.error.title}`;
      }
    }
    console.error(errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}