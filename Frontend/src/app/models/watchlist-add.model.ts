export interface WatchlistAdd {
    userId: number;
    movieId: number;
    status: string;
    rating?: number | null;
}
