export interface Recipe {
  id: number;
  name: string;
  description: string;
  instructions: string;
  difficulty: string;
  prepTime: number;
  cookTime: number;
  servings: number;
  imageUrl: string;
  isFeatured: boolean;
  createdAt: string;
  updatedAt?: string;
  categories?: RecipeCategory[];
  ingredients?: RecipeIngredient[];
}

export interface RecipeCategory {
  id: number;
  name: string;
  description: string;
}

export interface RecipeIngredient {
  id: number;
  recipeId: number;
  productId?: number;
  name: string;
  quantity: number;
  unit?: string;
  notes?: string;
  product?: { id: number; name: string; imageUrl: string }; // Product type for ingredient
}

export interface RecipeCreateDto {
  name: string;
  description: string;
  instructions: string;
  prepTime: number;
  cookTime: number;
  servings: number;
  difficulty: string;
  imageUrl: string;
  isFeatured: boolean;
  categoryIds: number[];
  ingredients: RecipeIngredientCreateDto[];
}

export interface RecipeUpdateDto {
  name: string;
  description: string;
  instructions: string;
  prepTime: number;
  cookTime: number;
  servings: number;
  difficulty: string;
  imageUrl: string;
  isFeatured: boolean;
  categoryIds: number[];
  ingredients: RecipeIngredientUpdateDto[];
}

export interface RecipeIngredientCreateDto {
  name: string;
  quantity: number;
  unit: string;
  notes?: string;
  productId?: number;
}

export interface RecipeIngredientUpdateDto {
  id: number;
  name: string;
  quantity: number;
  unit: string;
  notes?: string;
  productId?: number;
}
