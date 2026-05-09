import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductsService } from '../../services/products.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [RouterLink, DecimalPipe, FormsModule],
  templateUrl: './product-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductListComponent implements OnInit {
  protected readonly products = inject(ProductsService);
  protected readonly toast = inject(ToastService);
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

  stockLabel(qty: number): string {
    if (qty === 0) return 'Out';
    if (qty <= 20) return 'Low';
    return 'In Stock';
  }

  stockColor(qty: number): string {
    if (qty === 0) return 'red';
    if (qty <= 20) return 'amber';
    return 'green';
  }

  goToPage(page: number) {
    this.currentPage.set(page);
    this.searchQuery.set('');
    this.products.loadList(page, this.pageSize);
  }

  async deleteProduct(id: string, name: string) {
    if (!confirm(`Delete "${name}"? This action cannot be undone.`)) return;
    const ok = await this.products.delete(id);
    if (ok) {
      this.toast.show(`"${name}" deleted`, 'success');
      this.products.loadList(this.currentPage(), this.pageSize);
    }
  }
}
