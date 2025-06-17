import { API_ENDPOINTS } from "./endpoints";
import { enhancedApi } from "./api";

export interface Category {
  id: number;
  name: string;
  description: string;
  slug: string;
  parentId: number | null;
  parentName: string | null;
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

// Helper function to convert PascalCase to camelCase
const toCamelCase = (str: string) => {
  return str.charAt(0).toLowerCase() + str.slice(1);
};

// Helper function to transform object keys from PascalCase to camelCase
const transformToCamelCase = (obj: any): any => {
  if (Array.isArray(obj)) {
    return obj.map(transformToCamelCase);
  }
  if (obj !== null && typeof obj === "object") {
    return Object.fromEntries(
      Object.entries(obj).map(([key, value]) => [
        toCamelCase(key),
        transformToCamelCase(value),
      ])
    );
  }
  return obj;
};

export const categoryApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getAllCategories: builder.query<Category[], void>({
      query: () => API_ENDPOINTS.CATEGORIES,
      transformResponse: (response: any) => transformToCamelCase(response),
      providesTags: ["Category"],
    }),
    getRootCategories: builder.query<Category[], void>({
      query: () => `${API_ENDPOINTS.CATEGORIES}/root`,
      transformResponse: (response: any) => transformToCamelCase(response),
      providesTags: ["Category"],
    }),
    getSubCategories: builder.query<Category[], number>({
      query: (parentId) => `${API_ENDPOINTS.CATEGORIES}/sub/${parentId}`,
      transformResponse: (response: any) => transformToCamelCase(response),
      providesTags: ["Category"],
    }),
    getCategoryById: builder.query<Category, number>({
      query: (id) => `${API_ENDPOINTS.CATEGORIES}/${id}`,
      transformResponse: (response: any) => transformToCamelCase(response),
      providesTags: ["Category"],
    }),
    createCategory: builder.mutation<Category, CategoryCreateDto>({
      query: (category) => ({
        url: API_ENDPOINTS.CATEGORIES,
        method: "POST",
        body: category,
      }),
      transformResponse: (response: any) => transformToCamelCase(response),
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
      transformResponse: (response: any) => transformToCamelCase(response),
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
  useCreateCategoryMutation,
  useUpdateCategoryMutation,
  useDeleteCategoryMutation,
} = categoryApi;
