export interface Product {
  Id: number;
  Name: string;
  Description: string;
  Slug: string;
  Origin: string;
  Volume: number;
  AlcoholContent: number;
  Price: number;
  StockQuantity: number;
  Status: boolean;
  ImageUrl: string;
  MetaTitle: string;
  MetaDescription: string;
  CreatedAt: string;
  UpdatedAt: string | null;
  CategoryId: number;
  CategoryName: string;
  BrandId: number;
  BrandName: string;
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
