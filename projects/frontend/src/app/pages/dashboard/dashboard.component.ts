import { ChangeDetectionStrategy, Component, inject, OnInit, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProductsService } from '../../services/products.service';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink, DecimalPipe],
  templateUrl: './dashboard.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardComponent implements OnInit {
  protected readonly products = inject(ProductsService);

  readonly stats = computed(() => {
    const items = this.products.items();
    const total = this.products.page()?.totalCount ?? 0;
    const totalValue = items.reduce((sum, p) => sum + p.priceAmount * p.stockQuantity, 0);
    const lowStock = items.filter(p => p.stockQuantity < 20).length;
    const avgPrice = items.length > 0 ? items.reduce((s, p) => s + p.priceAmount, 0) / items.length : 0;
    return { total, totalValue, lowStock, avgPrice };
  });

  ngOnInit(): void {
    this.products.loadList(1, 5);
  }
}
