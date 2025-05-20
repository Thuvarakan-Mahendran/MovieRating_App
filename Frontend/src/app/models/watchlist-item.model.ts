export interface WatchlistItem {
    watchlistId: number; 
    movieId: number;
    movieTitle: string;
    moviePosterUrl?: string;
    movieReleaseYear: number;
    status: 'To Watch' | 'Watched';
    userRating: number;
}
