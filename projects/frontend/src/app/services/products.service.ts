import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { CreateProductRequest, PagedResult, ProductDto, UpdateProductRequest } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/products';

  // ─── state ───
  private readonly _page = signal<PagedResult<ProductDto> | null>(null);
  private readonly _selected = signal<ProductDto | null>(null);
  private readonly _loading = signal(false);
  private readonly _error = signal<string | null>(null);

  readonly page = this._page.asReadonly();
  readonly selected = this._selected.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly items = computed(() => this._page()?.items ?? []);

  // ─── actions ───

  async loadList(pageNumber = 1, pageSize = 20): Promise<void> {
    this._loading.set(true);
    this._error.set(null);
    try {
      const result = await firstValueFrom(
        this.http.get<PagedResult<ProductDto>>(`${this.baseUrl}`, {
          params: { pageNumber, pageSize },
        }),
      );
      this._page.set(result);
    } catch (e) {
      this._error.set(this.message(e));
    } finally {
      this._loading.set(false);
    }
  }

  async loadById(id: string): Promise<void> {
    this._loading.set(true);
    this._error.set(null);
    try {
      const result = await firstValueFrom(
        this.http.get<ProductDto>(`${this.baseUrl}/${id}`),
      );
      this._selected.set(result);
    } catch (e) {
      this._error.set(this.message(e));
    } finally {
      this._loading.set(false);
    }
  }

  async create(req: CreateProductRequest): Promise<ProductDto | null> {
    this._loading.set(true);
    this._error.set(null);
    try {
      const id = await firstValueFrom(
        this.http.post<string>(this.baseUrl, req),
      );
      await this.loadById(id);
      return this._selected();
    } catch (e) {
      this._error.set(this.message(e));
      return null;
    } finally {
      this._loading.set(false);
    }
  }

  async update(id: string, req: UpdateProductRequest): Promise<boolean> {
    this._loading.set(true);
    this._error.set(null);
    try {
      await firstValueFrom(this.http.put(`${this.baseUrl}/${id}`, req));
      return true;
    } catch (e) {
      this._error.set(this.message(e));
      return false;
    } finally {
      this._loading.set(false);
    }
  }

  async delete(id: string): Promise<boolean> {
    this._loading.set(true);
    this._error.set(null);
    try {
      await firstValueFrom(this.http.delete(`${this.baseUrl}/${id}`));
      return true;
    } catch (e) {
      this._error.set(this.message(e));
      return false;
    } finally {
      this._loading.set(false);
    }
  }

  // ─── helpers ───

  clearError(): void {
    this._error.set(null);
  }

  private message(e: unknown): string {
    if (e instanceof Error) return e.message;
    return String(e);
  }
}
