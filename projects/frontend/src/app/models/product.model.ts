// ⚠️ AUTO-GENERATED from contracts/api.openapi.yaml
// Do not edit by hand — your changes will be lost on the next sync.
// Extend behavior in src/app/services/ instead.

export interface ProductDto {
  id: string;
  sku: string;
  name: string;
  description: string | null;
  priceAmount: number;
  priceCurrency: string;
  stockQuantity: number;
  createdAt: string;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface CreateProductRequest {
  sku: string;
  name: string;
  description: string | null;
  priceAmount: number;
  priceCurrency: string;
  stockQuantity: number;
}

export interface UpdateProductRequest {
  name: string;
  description: string | null;
  priceAmount: number;
  priceCurrency: string;
  stockQuantity: number;
}

export interface ProblemDetails {
  type: string;
  title: string;
  status: number;
  detail: string;
  traceId: string;
  errors?: Record<string, string[]>;
}
