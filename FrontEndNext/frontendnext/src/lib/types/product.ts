import { Discount } from "./discount";

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
  salesCount: number;
  targetMarginPercentage: number;

  // Discount properties
  originalPrice: number;
  discountedPrice?: number;
  activeDiscounts: Discount[];
  hasDiscount: boolean;
  discountAmount?: number;
  discountPercentage?: number;
}

export interface ProductCreate {
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
  categoryId: number;
  brandId: number;
  age?: number;
  flavor?: string;
  targetMarginPercentage?: number;
}

export interface ProductUpdate {
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
  categoryId: number;
  brandId: number;
  age?: number;
  flavor?: string;
  targetMarginPercentage?: number;
}

export interface ProductFilter {
  SearchTerm?: string;
  categoryId?: number;
  brandId?: number;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
  pageNumber?: number;
  pageSize?: number;
}
