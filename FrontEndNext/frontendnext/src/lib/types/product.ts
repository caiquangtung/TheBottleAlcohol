export interface Product {
  id: number;
  name: string;
  description: string;
  slug: string;
  origin: string;
  volume: number;
  alcoholContent: number;
  price: number;
  stockQuantity: number;
  status: boolean;
  imageUrl: string;
  metaTitle: string;
  metaDescription: string;
  createdAt: string;
  updatedAt: string | null;
  categoryId: number;
  categoryName: string;
  brandId: number;
  brandName: string;
  age?: number;
  flavor?: string;
}

export interface ProductFilter {
  search?: string;
  categoryId?: number;
  brandId?: number;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}
