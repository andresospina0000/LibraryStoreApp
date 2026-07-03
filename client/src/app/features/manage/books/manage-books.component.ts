import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { BookService } from '../../../core/services/book.service';
import { AuthorService } from '../../../core/services/author.service';
import { ToastService } from '../../../core/services/toast.service';
import { Author, Book, CreateBook } from '../../../core/models/api.models';

interface BookForm {
  id: string | null;
  name: string;
  description: string;
  price: number;
  discount: number;
  publicationDate: string | null;
  imageUrl: string | null;
  authorIds: string[];
}

function emptyForm(): BookForm {
  return {
    id: null,
    name: '',
    description: '',
    price: 0,
    discount: 0,
    publicationDate: null,
    imageUrl: null,
    authorIds: []
  };
}

@Component({
  selector: 'app-manage-books',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manage-books.component.html'
})
export class ManageBooksComponent implements OnInit {
  private readonly bookService = inject(BookService);
  private readonly authorService = inject(AuthorService);
  private readonly toast = inject(ToastService);

  readonly books = signal<Book[]>([]);
  readonly authors = signal<Author[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly showForm = signal(false);

  form: BookForm = emptyForm();

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    forkJoin({
      books: this.bookService.getAll(1, 100),
      authors: this.authorService.getAll()
    }).subscribe({
      next: ({ books, authors }) => {
        if (books.succeeded && books.data) this.books.set(books.data.items);
        if (authors.succeeded && authors.data) this.authors.set(authors.data);
        this.loading.set(false);
      },
      error: () => {
        this.toast.error('Failed to load data.');
        this.loading.set(false);
      }
    });
  }

  newBook(): void {
    this.form = emptyForm();
    this.showForm.set(true);
  }

  edit(book: Book): void {
    this.form = {
      id: book.id,
      name: book.name,
      description: book.description,
      price: book.price,
      discount: book.discount,
      publicationDate: book.publicationDate ? book.publicationDate.substring(0, 10) : null,
      imageUrl: book.imageUrl,
      authorIds: book.authors.map((a) => a.id)
    };
    this.showForm.set(true);
  }

  cancel(): void {
    this.showForm.set(false);
    this.form = emptyForm();
  }

  toggleAuthor(id: string, checked: boolean): void {
    this.form.authorIds = checked
      ? [...this.form.authorIds, id]
      : this.form.authorIds.filter((a) => a !== id);
  }

  save(): void {
    const payload: CreateBook = {
      name: this.form.name.trim(),
      description: this.form.description.trim(),
      price: Number(this.form.price),
      discount: Number(this.form.discount),
      publicationDate: this.form.publicationDate || null,
      imageUrl: this.form.imageUrl?.trim() || null,
      authorIds: this.form.authorIds
    };

    if (!payload.name || !payload.description || payload.price <= 0 || payload.authorIds.length === 0) {
      this.toast.error('Name, description, a positive price and at least one author are required.');
      return;
    }

    this.saving.set(true);
    const request = this.form.id
      ? this.bookService.update(this.form.id, payload)
      : this.bookService.create(payload);

    request.subscribe({
      next: (res) => {
        this.saving.set(false);
        if (res.succeeded) {
          this.toast.success(this.form.id ? 'Book updated.' : 'Book created.');
          this.cancel();
          this.load();
        } else {
          this.toast.error(res.error ?? 'Could not save the book.');
        }
      },
      error: (err) => {
        this.saving.set(false);
        this.toast.error(err?.error?.error ?? 'Could not save the book.');
      }
    });
  }

  remove(book: Book): void {
    if (!confirm(`Delete "${book.name}"? This cannot be undone.`)) {
      return;
    }
    this.bookService.delete(book.id).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.toast.success('Book deleted.');
          this.load();
        } else {
          this.toast.error(res.error ?? 'Could not delete the book.');
        }
      },
      error: (err) => this.toast.error(err?.error?.error ?? 'Could not delete the book.')
    });
  }

  authorNames(book: Book): string {
    return book.authors.map((a) => `${a.firstName} ${a.lastName}`).join(', ');
  }
}
