import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductsService } from '../../services/products.service';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [RouterLink, DecimalPipe, FormsModule],
  templateUrl: './product-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductListComponent implements OnInit {
  protected readonly products = inject(ProductsService);
  protected readonly searchQuery = signal('');
  protected readonly currentPage = signal(1);
  protected readonly pageSize = 12;

  ngOnInit(): void {
    this.products.loadList(this.currentPage(), this.pageSize);
  }

  get filteredItems() {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.products.items();
    return this.products.items().filter(p =>
      p.name.toLowerCase().includes(query) ||
      p.sku.toLowerCase().includes(query) ||
      (p.description ?? '').toLowerCase().includes(query)
    );
  }

  goToPage(page: number) {
    this.currentPage.set(page);
    this.products.loadList(page, this.pageSize);
  }

  async deleteProduct(id: string, name: string) {
    if (!confirm(`Delete "${name}"? This action cannot be undone.`)) return;
    const ok = await this.products.delete(id);
    if (ok) this.products.loadList(this.currentPage(), this.pageSize);
  }
}
