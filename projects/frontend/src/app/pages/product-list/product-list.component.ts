import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductsService } from '../../services/products.service';
import { ProductDto } from '../../models/product.model';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [RouterLink, DecimalPipe],
  templateUrl: './product-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductListComponent implements OnInit {
  protected readonly products = inject(ProductsService);

  ngOnInit(): void {
    this.products.loadList();
  }

  async deleteProduct(id: string, name: string): Promise<void> {
    const confirmed = confirm(`Delete "${name}"?`);
    if (!confirmed) return;

    const ok = await this.products.delete(id);
    if (ok) {
      this.products.loadList();
    }
  }
}
