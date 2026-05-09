import { ChangeDetectionStrategy, Component, effect, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ProductsService } from '../../services/products.service';
import { CreateProductRequest, UpdateProductRequest } from '../../models/product.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './product-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  protected readonly products = inject(ProductsService);

  readonly isEditing = signal(false);
  readonly submitted = signal(false);
  private productId = signal<string | null>(null);

  form: FormGroup = this.fb.group({
    sku: ['', [Validators.required, Validators.maxLength(32), Validators.pattern('^[A-Z0-9-]+$')]],
    name: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', Validators.maxLength(2000)],
    priceAmount: [0, [Validators.required, Validators.min(0)]],
    priceCurrency: ['USD', [Validators.required, Validators.minLength(3), Validators.maxLength(3)]],
    stockQuantity: [0, [Validators.required, Validators.min(0)]],
  });

  constructor() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditing.set(true);
      this.productId.set(id);
      this.products.loadById(id);
    }

    effect(() => {
      const product = this.products.selected();
      if (product && this.isEditing()) {
        this.form.patchValue({
          sku: product.sku,
          name: product.name,
          description: product.description ?? '',
          priceAmount: product.priceAmount,
          priceCurrency: product.priceCurrency,
          stockQuantity: product.stockQuantity,
        });
        this.form.get('sku')?.disable();
      }
    });
  }

  async submit(): Promise<void> {
    this.submitted.set(true);
    if (this.form.invalid) return;

    const raw = this.form.getRawValue();
    let ok: boolean;

    if (this.isEditing()) {
      const req: UpdateProductRequest = {
        name: raw.name,
        description: raw.description || null,
        priceAmount: raw.priceAmount,
        priceCurrency: raw.priceCurrency,
        stockQuantity: raw.stockQuantity,
      };
      ok = await this.products.update(this.productId()!, req);
    } else {
      const req: CreateProductRequest = {
        sku: raw.sku,
        name: raw.name,
        description: raw.description || null,
        priceAmount: raw.priceAmount,
        priceCurrency: raw.priceCurrency,
        stockQuantity: raw.stockQuantity,
      };
      const created = await this.products.create(req);
      ok = created !== null;
    }

    if (ok) {
      this.router.navigate(['/products']);
    }
  }

  ctrl(name: string) { return this.form.get(name); }

  error(name: string): string | null {
    const c = this.ctrl(name);
    if (!c || (!c.touched && !this.submitted()) || !c.errors) return null;
    if (c.errors['required']) return 'This field is required';
    if (c.errors['maxlength']) return `Maximum ${c.errors['maxlength'].requiredLength} characters`;
    if (c.errors['min']) return `Minimum value is ${c.errors['min'].min}`;
    if (c.errors['minlength']) return `Minimum ${c.errors['minlength'].requiredLength} characters`;
    if (c.errors['pattern']) return 'Only uppercase letters, digits and hyphens';
    return 'Invalid';
  }
}
