import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../core/services/book.service';
import { ToastService } from '../../core/services/toast.service';
import { Book } from '../../core/models/api.models';

@Component({
  selector: 'app-store',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './store.component.html'
})
export class StoreComponent implements OnInit {
  private readonly bookService = inject(BookService);
  private readonly toast = inject(ToastService);

  readonly books = signal<Book[]>([]);
  readonly loading = signal(false);
  readonly page = signal(1);
  readonly totalPages = signal(1);
  readonly totalCount = signal(0);
  readonly pageSize = 8;
  search = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.bookService.getAll(this.page(), this.pageSize, this.search.trim() || undefined).subscribe({
      next: (res) => {
        if (res.succeeded && res.data) {
          this.books.set(res.data.items);
          this.totalPages.set(res.data.totalPages);
          this.totalCount.set(res.data.totalCount);
        }
        this.loading.set(false);
      },
      error: () => {
        this.toast.error('Unable to load books. Is the API running?');
        this.loading.set(false);
      }
    });
  }

  onSearch(): void {
    this.page.set(1);
    this.load();
  }

  goTo(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }
    this.page.set(page);
    this.load();
  }

  authorNames(book: Book): string {
    return book.authors.map((a) => `${a.firstName} ${a.lastName}`).join(', ') || 'Unknown';
  }
}
