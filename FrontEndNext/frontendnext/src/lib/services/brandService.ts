import { API_ENDPOINTS } from "./endpoints";
import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import { Brand } from "../types/brand";

export const brandApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getAllBrands: builder.query<Brand[], void>({
      query: () => API_ENDPOINTS.BRANDS,
      transformResponse: (response) => transformApiResponse<Brand[]>(response),
      providesTags: ["Brand"],
    }),
  }),
});

export const { useGetAllBrandsQuery } = brandApi;
