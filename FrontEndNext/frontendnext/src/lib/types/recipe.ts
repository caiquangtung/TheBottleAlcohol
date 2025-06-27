export interface Recipe {
  id: number;
  name: string;
  description: string;
  slug: string;
  imageUrl: string;
  instructions: string;
  difficulty: string;
  preparationTime: number;
  servings: number;
  displayOrder: number;
  isActive: boolean;
  metaTitle?: string;
  metaDescription?: string;
  createdAt: string;
  updatedAt?: string;
  categoryId?: number;
  category?: any; // Có thể tạo type riêng nếu cần
  ingredients?: RecipeIngredient[];
}

export interface RecipeIngredient {
  id: number;
  recipeId: number;
  productId?: number;
  name: string;
  quantity: number;
  unit?: string;
  notes?: string;
  product?: any; // Có thể tạo type riêng nếu cần
}
