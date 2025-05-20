import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WatchlistItem } from '../../models/watchlist-item.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-watchlist-card',
  standalone: true,
  imports: [CommonModule],                                              // For *ngIf, ngClass, etc.
  templateUrl: './watchlist-card.component.html',
  styleUrls: ['./watchlist-card.component.css']
})
export class WatchlistCardComponent implements OnInit {
  constructor(private router: Router){}
  
  @Input({ required: true }) item!: WatchlistItem;
  @Output() statusChanged = new EventEmitter<{ itemId: number, newStatus: 'To Watch' | 'Watched' }>();
  @Output() removed = new EventEmitter<number>();                       // Emits movieId or watchlistItemId

  // For displaying stars
  stars: boolean[] = [];

  ngOnInit(): void {
    this.stars = Array(5).fill(false).map((_, i) => i < this.item.userRating);
  }

  toggleStatus(): void {
    const newStatus = this.item.status === 'To Watch' ? 'Watched' : 'To Watch';
    // emit an event
    console.log(`Status change requested for item ${this.item.watchlistId} to ${newStatus}`);
    // this.statusChanged.emit({ itemId: this.item.id, newStatus });
    alert(`Placeholder: Item ${this.item.watchlistId} status would change to ${newStatus}. Implement service call.`);
    // To visually update immediately (though this should come after successful API call)
    // this.item.status = newStatus; // This mutates the input, better to get fresh data
  }

  removeItem(): void {
    // In a real app, you'd call a service here to remove from the backend
    console.log(`Remove requested for item ${this.item.watchlistId}`);
    // this.removed.emit(this.item.id);
    alert(`Placeholder: Item ${this.item.watchlistId} would be removed. Implement service call.`);
  }

  goToDetails(movieId: number): void {
    this.router.navigate(['/movie-detail', movieId]);
  }
}