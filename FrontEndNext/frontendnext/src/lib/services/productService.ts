import { API_ENDPOINTS } from "./endpoints";
import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";

export interface Product {
  id: number;
  name: string;
  description: string;
  slug: string;
  origin: string;
  volume: number;
  alcoholContent: number;
  price: number;
  stockQuantity: number;
  status: boolean;
  imageUrl: string;
  metaTitle: string;
  metaDescription: string;
  createdAt: string;
  updatedAt?: string;
  categoryId: number;
  categoryName: string;
  brandId: number;
  brandName: string;
  sortOrder?: "asc" | "desc";
}

export interface PagedResult<T> {
  items: T[];
  totalRecords: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  sortOrder?: "asc" | "desc";
}

export interface ProductFilter {
  searchTerm?: string;
  categoryId?: number;
  brandId?: number;
  minPrice?: number;
  maxPrice?: number;
  status?: boolean;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}

export const productApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getProducts: builder.query<PagedResult<Product>, ProductFilter | void>({
      query: (filter) => ({
        url: API_ENDPOINTS.PRODUCTS,
        params: filter || {},
      }),
      transformResponse: transformApiResponse,
      providesTags: ["Product"],
    }),
    getProductById: builder.query<Product, number>({
      query: (id) => API_ENDPOINTS.PRODUCT_DETAIL(id.toString()),
      transformResponse: transformApiResponse,
      providesTags: (result, error, id) => [{ type: "Product", id }],
    }),
  }),
});

export const { useGetProductsQuery, useGetProductByIdQuery } = productApi;
