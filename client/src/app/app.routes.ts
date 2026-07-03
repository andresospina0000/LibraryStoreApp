import { Routes } from '@angular/router';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'store' },
  {
    path: 'store',
    loadComponent: () => import('./features/store/store.component').then((m) => m.StoreComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./features/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'manage/books',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./features/manage/books/manage-books.component').then((m) => m.ManageBooksComponent)
  },
  {
    path: 'manage/authors',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./features/manage/authors/manage-authors.component').then(
        (m) => m.ManageAuthorsComponent
      )
  },
  {
    path: 'manage/myself',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./features/manage/myself/myself.component').then((m) => m.MyselfComponent)
  },
  { path: '**', redirectTo: 'store' }
];
