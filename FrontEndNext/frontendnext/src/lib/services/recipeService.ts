import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import { Recipe, RecipeCreateDto, RecipeUpdateDto } from "../types/recipe";

export const recipeApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getRecipes: builder.query<Recipe[], void>({
      query: () => "/recipe",
      transformResponse: (response) => transformApiResponse<Recipe[]>(response),
      providesTags: ["Recipe"],
    }),
    getRecipeById: builder.query<Recipe, number>({
      query: (id) => `/recipe/${id}`,
      transformResponse: (response) => transformApiResponse<Recipe>(response),
      providesTags: (result, error, id) => [{ type: "Recipe", id }],
    }),
    getFeaturedRecipes: builder.query<Recipe[], void>({
      query: () => "/recipe/featured",
      transformResponse: (response) => transformApiResponse<Recipe[]>(response),
      providesTags: ["Recipe"],
    }),
    getRecipesByCategory: builder.query<Recipe[], number>({
      query: (categoryId) => `/recipe/category/${categoryId}`,
      transformResponse: (response) => transformApiResponse<Recipe[]>(response),
      providesTags: ["Recipe"],
    }),
    createRecipe: builder.mutation<Recipe, RecipeCreateDto>({
      query: (recipe) => ({
        url: "/recipe",
        method: "POST",
        body: recipe,
      }),
      transformResponse: (response) => transformApiResponse<Recipe>(response),
      invalidatesTags: ["Recipe"],
    }),
    updateRecipe: builder.mutation<
      Recipe,
      { id: number; recipe: RecipeUpdateDto }
    >({
      query: ({ id, recipe }) => ({
        url: `/recipe/${id}`,
        method: "PUT",
        body: recipe,
      }),
      transformResponse: (response) => transformApiResponse<Recipe>(response),
      invalidatesTags: ["Recipe"],
    }),
    deleteRecipe: builder.mutation<void, number>({
      query: (id) => ({
        url: `/recipe/${id}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Recipe"],
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetRecipesQuery,
  useGetRecipeByIdQuery,
  useGetFeaturedRecipesQuery,
  useGetRecipesByCategoryQuery,
  useCreateRecipeMutation,
  useUpdateRecipeMutation,
  useDeleteRecipeMutation,
} = recipeApi;
