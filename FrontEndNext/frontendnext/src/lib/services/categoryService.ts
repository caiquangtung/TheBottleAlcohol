import { API_ENDPOINTS } from "./endpoints";
import { enhancedApi } from "./api";
import { transformApiResponse, transformToCamelCase } from "../utils/utils";
import { Category, CategoryCreateDto } from "../types/category";

export const categoryApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getAllCategories: builder.query<Category[], void>({
      query: () => API_ENDPOINTS.CATEGORIES,
      transformResponse: transformApiResponse,
      providesTags: ["Category"],
    }),
    getRootCategories: builder.query<Category[], void>({
      query: () => `${API_ENDPOINTS.CATEGORIES}/root`,
      transformResponse: transformApiResponse,
      providesTags: ["Category"],
    }),
    getSubCategories: builder.query<Category[], number>({
      query: (parentId) => `${API_ENDPOINTS.CATEGORIES}/sub/${parentId}`,
      transformResponse: transformApiResponse,
      providesTags: ["Category"],
    }),
    getCategoryById: builder.query<Category, number>({
      query: (id) => `${API_ENDPOINTS.CATEGORIES}/${id}`,
      transformResponse: transformApiResponse,
      providesTags: ["Category"],
    }),
    getCategoryBySlug: builder.query<Category, string>({
      query: (slug) => `${API_ENDPOINTS.CATEGORIES}/slug/${slug}`,
      transformResponse: transformApiResponse,
      providesTags: ["Category"],
    }),
    createCategory: builder.mutation<Category, CategoryCreateDto>({
      query: (category) => ({
        url: API_ENDPOINTS.CATEGORIES,
        method: "POST",
        body: category,
      }),
      transformResponse: transformApiResponse,
      invalidatesTags: ["Category"],
    }),
    updateCategory: builder.mutation<
      Category,
      { id: number; category: Partial<CategoryCreateDto> }
    >({
      query: ({ id, category }) => ({
        url: `${API_ENDPOINTS.CATEGORIES}/${id}`,
        method: "PUT",
        body: category,
      }),
      transformResponse: transformApiResponse,
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
