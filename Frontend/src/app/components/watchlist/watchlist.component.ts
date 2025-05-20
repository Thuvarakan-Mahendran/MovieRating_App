import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WatchlistService } from '../../services/watchlist.service';
import { WatchlistItem } from '../../models/watchlist-item.model';
import { WatchlistCardComponent } from '../watchlist-card/watchlist-card.component';

@Component({
  selector: 'app-watchlist',
  standalone: true,
  imports: [
    CommonModule,
    WatchlistCardComponent
],
  templateUrl: './watchlist.component.html',
  styleUrl: './watchlist.component.css'
})
export class WatchlistComponent {
  private watchlistService = inject(WatchlistService);

  watchlistItems: WatchlistItem[] = [];
  isLoading: boolean = true;
  error: string | null = null;
  userId: number = 1;                                                   // hardcoded userId

  constructor() {}

  ngOnInit(): void {
    this.loadWatchlist();
  }

  loadWatchlist(): void {
    this.isLoading = true;
    this.error = null;
    this.watchlistService.getUserWatchlist(this.userId).subscribe({
      next: (items: WatchlistItem[]) => {
        this.watchlistItems = items;
        this.isLoading = false;
      },
      error: (err: { message: string; }) => {
        console.error('Error loading watchlist:', err);
        this.error = err.message || 'Failed to load watchlist. Please try again.';
        this.isLoading = false;
      }
    });
  }

  handleStatusChange(event: { itemId: number, newStatus: 'To Watch' | 'Watched' }): void {
    console.log('Parent: Status change event received', event);
    // this.watchlistService.updateWatchlistItemStatus(event.itemId, event.newStatus).subscribe(() => {
    //   this.loadWatchlist(); // Easiest way to reflect changes
    // });
    alert(`Item ${event.itemId} status would be updated to ${event.newStatus}. Implement backend call and refresh.`);
  }

  handleItemRemoved(itemId: number): void {
    console.log('Parent: Item remove event received', itemId);
    // this.watchlistService.removeFromWatchlist(itemId).subscribe(() => {
    //  this.watchlistItems = this.watchlistItems.filter(item => item.id !== itemId);
    //  if (this.watchlistItems.length === 0) { /* handle empty list */ }
    // });
    alert(`Item ${itemId} would be removed. Implement backend call and refresh.`);
  }

  trackById(index: number, item: WatchlistItem): number {
    return item.watchlistId;
  }
}
