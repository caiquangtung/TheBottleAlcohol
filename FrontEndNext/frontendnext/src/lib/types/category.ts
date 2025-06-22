export interface Category {
  id: number;
  name: string;
  description: string;
  slug: string;
  parentId: number | null;
  parentName: string | null;
  parentSlug: string | null;
  displayOrder: number;
  isActive: boolean;
  imageUrl: string | null;
  metaTitle: string | null;
  metaDescription: string | null;
  productCount: number;
  childrenCount: number;
  createdAt: string;
  updatedAt: string | null;
}

export interface CategoryCreateDto {
  name: string;
  description?: string;
  slug?: string;
  parentId?: number;
  isActive?: boolean;
  displayOrder?: number;
  imageUrl?: string;
  metaTitle?: string;
  metaDescription?: string;
}
