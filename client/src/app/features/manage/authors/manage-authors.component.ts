import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthorService } from '../../../core/services/author.service';
import { ToastService } from '../../../core/services/toast.service';
import { Author, CreateAuthor } from '../../../core/models/api.models';

interface AuthorForm {
  id: string | null;
  firstName: string;
  lastName: string;
  biography: string | null;
}

function emptyForm(): AuthorForm {
  return { id: null, firstName: '', lastName: '', biography: null };
}

@Component({
  selector: 'app-manage-authors',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manage-authors.component.html'
})
export class ManageAuthorsComponent implements OnInit {
  private readonly authorService = inject(AuthorService);
  private readonly toast = inject(ToastService);

  readonly authors = signal<Author[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly showForm = signal(false);

  form: AuthorForm = emptyForm();

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.authorService.getAll().subscribe({
      next: (res) => {
        if (res.succeeded && res.data) this.authors.set(res.data);
        this.loading.set(false);
      },
      error: () => {
        this.toast.error('Failed to load authors.');
        this.loading.set(false);
      }
    });
  }

  newAuthor(): void {
    this.form = emptyForm();
    this.showForm.set(true);
  }

  edit(author: Author): void {
    this.form = {
      id: author.id,
      firstName: author.firstName,
      lastName: author.lastName,
      biography: author.biography
    };
    this.showForm.set(true);
  }

  cancel(): void {
    this.showForm.set(false);
    this.form = emptyForm();
  }

  save(): void {
    const payload: CreateAuthor = {
      firstName: this.form.firstName.trim(),
      lastName: this.form.lastName.trim(),
      biography: this.form.biography?.trim() || null
    };

    if (!payload.firstName || !payload.lastName) {
      this.toast.error('First and last name are required.');
      return;
    }

    this.saving.set(true);
    const request = this.form.id
      ? this.authorService.update(this.form.id, payload)
      : this.authorService.create(payload);

    request.subscribe({
      next: (res) => {
        this.saving.set(false);
        if (res.succeeded) {
          this.toast.success(this.form.id ? 'Author updated.' : 'Author created.');
          this.cancel();
          this.load();
        } else {
          this.toast.error(res.error ?? 'Could not save the author.');
        }
      },
      error: (err) => {
        this.saving.set(false);
        this.toast.error(err?.error?.error ?? 'Could not save the author.');
      }
    });
  }

  remove(author: Author): void {
    if (!confirm(`Delete ${author.firstName} ${author.lastName}?`)) {
      return;
    }
    this.authorService.delete(author.id).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.toast.success('Author deleted.');
          this.load();
        } else {
          this.toast.error(res.error ?? 'Could not delete the author.');
        }
      },
      error: (err) => this.toast.error(err?.error?.error ?? 'Could not delete the author.')
    });
  }
}
