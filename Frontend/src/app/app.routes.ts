import { Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { WatchlistComponent } from './components/watchlist/watchlist.component';
import { MovieDetailComponent } from './components/movie-detail/movie-detail.component';
import { MovieAddComponent } from './components/movie-add/movie-add.component';

export const routes: Routes = [
    { path: 'dashboard', component: DashboardComponent },
    { path: 'watchlist', component: WatchlistComponent },
    { path: 'movie-detail/:id', component: MovieDetailComponent },
    { path: 'addMovie', component: MovieAddComponent }
    // { path: '', redirectTo: '/dashboard', pathMatch: 'full' }, // Default route
    // { path: '**', redirectTo: '/dashboard' } // Wildcard route (optional, for 404 handling)
];
