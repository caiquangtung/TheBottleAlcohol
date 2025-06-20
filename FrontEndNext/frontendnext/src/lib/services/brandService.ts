import { API_ENDPOINTS } from "./endpoints";
import { enhancedApi } from "./api";
import { transformToCamelCase, transformApiResponse } from "../utils/utils";

export interface Brand {
  id: number;
  name: string;
  description: string;
  logoUrl: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export const brandApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getAllBrands: builder.query<Brand[], void>({
      query: () => API_ENDPOINTS.BRANDS,
      transformResponse: transformApiResponse,
      providesTags: ["Brand"],
    }),
  }),
});

export const { useGetAllBrandsQuery } = brandApi;
