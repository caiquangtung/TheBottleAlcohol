import { API_ENDPOINTS } from "./endpoints";
import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import {
  Category,
  CategoryCreateDto,
  CategoryUpdateDto,
} from "../types/category";

// Server-side function for SEO
export async function getCategoryById(id: number): Promise<Category | null> {
  try {
    const response = await fetch(
      `${
        process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000"
      }/api/category/${id}`,
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );

    if (!response.ok) {
      return null;
    }

    const data = await response.json();
    return transformApiResponse<Category>(data);
  } catch (error) {
    console.error("Error fetching category:", error);
    return null;
  }
}

export const categoryApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getAllCategories: builder.query<Category[], { SearchTerm?: string } | void>(
      {
        query: (params) => {
          return {
            url: API_ENDPOINTS.CATEGORIES,
            params: params ? { SearchTerm: params.SearchTerm } : undefined,
          };
        },
        transformResponse: (response) => {
          const transformedResponse = transformApiResponse<{
            items: Category[];
          }>(response);
          return transformedResponse.items || [];
        },
        providesTags: ["Category"],
      }
    ),
    getRootCategories: builder.query<Category[], void>({
      query: () => `${API_ENDPOINTS.CATEGORIES}/root`,
      transformResponse: (response) =>
        transformApiResponse<Category[]>(response),
      providesTags: ["Category"],
    }),
    getSubCategories: builder.query<Category[], number>({
      query: (parentId) => `${API_ENDPOINTS.CATEGORIES}/sub/${parentId}`,
      transformResponse: (response) =>
        transformApiResponse<Category[]>(response),
      providesTags: ["Category"],
    }),
    getCategoryById: builder.query<Category, number>({
      query: (id) => `${API_ENDPOINTS.CATEGORIES}/${id}`,
      transformResponse: (response) => transformApiResponse<Category>(response),
      providesTags: ["Category"],
    }),
    getCategoryBySlug: builder.query<Category, string>({
      query: (slug) => `${API_ENDPOINTS.CATEGORIES}/slug/${slug}`,
      transformResponse: (response) => transformApiResponse<Category>(response),
      providesTags: ["Category"],
    }),
    createCategory: builder.mutation<Category, CategoryCreateDto>({
      query: (category) => ({
        url: API_ENDPOINTS.CATEGORIES,
        method: "POST",
        body: category,
      }),
      transformResponse: (response) => transformApiResponse<Category>(response),
      invalidatesTags: ["Category"],
    }),
    updateCategory: builder.mutation<
      Category,
      { id: number; category: CategoryUpdateDto }
    >({
      query: ({ id, category }) => ({
        url: `${API_ENDPOINTS.CATEGORIES}/${id}`,
        method: "PUT",
        body: category,
      }),
      transformResponse: (response) => transformApiResponse<Category>(response),
      invalidatesTags: ["Category"],
    }),
    deleteCategory: builder.mutation<void, number>({
      query: (id) => ({
        url: `${API_ENDPOINTS.CATEGORIES}/${id}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Category"],
    }),
  }),
});

export const {
  useGetAllCategoriesQuery,
  useGetRootCategoriesQuery,
  useGetSubCategoriesQuery,
  useGetCategoryByIdQuery,
  useGetCategoryBySlugQuery,
  useCreateCategoryMutation,
  useUpdateCategoryMutation,
  useDeleteCategoryMutation,
} = categoryApi;
