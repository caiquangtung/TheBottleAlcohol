import { API_ENDPOINTS } from "./endpoints";
import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import { Brand, BrandCreateDto, BrandUpdateDto } from "../types/brand";

export const brandApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getAllBrands: builder.query<Brand[], void>({
      query: () => API_ENDPOINTS.BRANDS,
      transformResponse: (response) => transformApiResponse<Brand[]>(response),
      providesTags: ["Brand"],
    }),
    getBrandById: builder.query<Brand, number>({
      query: (id) => `${API_ENDPOINTS.BRANDS}/${id}`,
      transformResponse: (response) => transformApiResponse<Brand>(response),
      providesTags: ["Brand"],
    }),
    createBrand: builder.mutation<Brand, BrandCreateDto>({
      query: (brand) => ({
        url: API_ENDPOINTS.BRANDS,
        method: "POST",
        body: brand,
      }),
      transformResponse: (response) => transformApiResponse<Brand>(response),
      invalidatesTags: ["Brand"],
    }),
    updateBrand: builder.mutation<Brand, { id: number; brand: BrandUpdateDto }>({
      query: ({ id, brand }) => ({
        url: `${API_ENDPOINTS.BRANDS}/${id}`,
        method: "PUT",
        body: brand,
      }),
      transformResponse: (response) => transformApiResponse<Brand>(response),
      invalidatesTags: ["Brand"],
    }),
    deleteBrand: builder.mutation<void, number>({
      query: (id) => ({
        url: `${API_ENDPOINTS.BRANDS}/${id}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Brand"],
    }),
  }),
});

export const { 
  useGetAllBrandsQuery, 
  useGetBrandByIdQuery,
  useCreateBrandMutation,
  useUpdateBrandMutation,
  useDeleteBrandMutation,
} = brandApi;
