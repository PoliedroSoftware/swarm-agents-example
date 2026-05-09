import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastService } from './services/toast.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
})
export class App {
  readonly toast = inject(ToastService);
  readonly darkMode = signal(false);
  sidebarOpen = false;

  navItems = [
    { path: '/dashboard', icon: '📊', label: 'Dashboard' },
    { path: '/products', icon: '📦', label: 'Products' },
  ];

  constructor() {
    const saved = localStorage.getItem('theme');
    if (saved === 'dark' || (!saved && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
      this.toggleDark();
    }
  }

  toggleDark() {
    this.darkMode.update(v => !v);
    document.documentElement.classList.toggle('dark', this.darkMode());
    localStorage.setItem('theme', this.darkMode() ? 'dark' : 'light');
  }
}
