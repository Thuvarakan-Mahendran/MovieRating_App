export interface MovieDetails {
    movieId: number;
    title: string;
    genre: string | null;      
    releaseYear: number | null; 
    rating: number | null;    
    description: string | null;
    posterUrl: string | null;  
}
