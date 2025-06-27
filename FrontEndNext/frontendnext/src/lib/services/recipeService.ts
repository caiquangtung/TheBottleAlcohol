import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import { Recipe } from "../types/recipe";

export const recipeApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getRecipes: builder.query<Recipe[], void>({
      query: () => "/recipe",
      transformResponse: transformApiResponse,
      providesTags: ["Recipe"],
    }),
    getRecipeById: builder.query<Recipe, number>({
      query: (id) => `/recipe/${id}`,
      transformResponse: transformApiResponse,
      providesTags: (result, error, id) => [{ type: "Recipe", id }],
    }),
    getFeaturedRecipes: builder.query<Recipe[], void>({
      query: () => "/recipe/featured",
      transformResponse: transformApiResponse,
      providesTags: ["Recipe"],
    }),
    getRecipesByCategory: builder.query<Recipe[], number>({
      query: (categoryId) => `/recipe/category/${categoryId}`,
      transformResponse: transformApiResponse,
      providesTags: ["Recipe"],
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetRecipesQuery,
  useGetRecipeByIdQuery,
  useGetFeaturedRecipesQuery,
  useGetRecipesByCategoryQuery,
} = recipeApi;
