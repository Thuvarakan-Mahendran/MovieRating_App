import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MovieAdd } from '../../models/movie-add.model';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MovieService } from '../../services/movie.service';

@Component({
  selector: 'app-movie-add',
  standalone: true,
  imports: [ 
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './movie-add.component.html',
  styleUrl: './movie-add.component.css'
})
export class MovieAddComponent {
  private movieService = inject(MovieService);
  movieForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.movieForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(255)]],
      genre: ['', [Validators.maxLength(100)]],
      releaseYear: [null, [Validators.min(1888)]],
      rating: [null, [Validators.min(0), Validators.max(10)]],
      description: [''],
      posterUrl: ['', [Validators.maxLength(500), Validators.pattern('https?://.+')]],
    });
  }

  onSubmit() {
    if (this.movieForm.valid) {
      const newMovie: MovieAdd = this.movieForm.value;
      this.movieService.addMovie(newMovie).subscribe({
        next: (response) => {
          alert('Movie added successfully');
          this.movieForm.reset();
        }
      });
    } else {
      this.movieForm.markAllAsTouched();
    }
  }
}
