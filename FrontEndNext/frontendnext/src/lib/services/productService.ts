import { API_ENDPOINTS } from "./endpoints";
import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import { Product } from "../types/product";
import { Discount } from "../types/discount";
import type {
  ProductFilter,
  ProductCreate,
  ProductUpdate,
} from "../types/product";

export type { ProductFilter, ProductCreate, ProductUpdate };

export interface PagedResult<T> {
  items: T[];
  totalRecords: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  sortOrder?: "asc" | "desc";
}

// Server-side function for SEO
export async function getProductById(id: number): Promise<Product | null> {
  try {
    const response = await fetch(
      `${
        process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000"
      }/api/product/${id}`,
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
    return transformApiResponse(data);
  } catch (error) {
    console.error("Error fetching product:", error);
    return null;
  }
}

export const productApi = enhancedApi.injectEndpoints({
  endpoints: (build) => ({
    getProducts: build.query<PagedResult<Product>, ProductFilter>({
      query: (filter) => ({
        url: API_ENDPOINTS.PRODUCTS,
        method: "GET",
        params: filter,
      }),
      transformResponse: (response) =>
        transformApiResponse<PagedResult<Product>>(response),
      providesTags: ["Product"],
    }),
    getProductById: build.query<Product, number>({
      query: (id) => ({
        url: API_ENDPOINTS.PRODUCT_DETAIL(id.toString()),
        method: "GET",
      }),
      transformResponse: (response) => transformApiResponse<Product>(response),
      providesTags: ["Product"],
    }),
    getProductsByBrand: build.query<Product[], number>({
      query: (brandId) => ({
        url: API_ENDPOINTS.PRODUCTS_BY_BRAND(brandId),
        method: "GET",
      }),
      transformResponse: (response) =>
        transformApiResponse<Product[]>(response),
      providesTags: ["Product"],
    }),
    getProductsByIds: build.query<Product[], number[]>({
      query: (ids) => ({
        url: API_ENDPOINTS.PRODUCTS_BY_IDS,
        method: "POST",
        body: ids,
      }),
      transformResponse: (response) =>
        transformApiResponse<Product[]>(response),
      providesTags: ["Product"],
    }),
    // Admin CRUD mutations
    createProduct: build.mutation<Product, ProductCreate>({
      query: (body) => ({
        url: API_ENDPOINTS.PRODUCT_CREATE,
        method: "POST",
        body,
      }),
      transformResponse: (response) => transformApiResponse<Product>(response),
      invalidatesTags: ["Product"],
    }),
    updateProduct: build.mutation<Product, { id: number; data: ProductUpdate }>(
      {
        query: ({ id, data }) => ({
          url: API_ENDPOINTS.PRODUCT_UPDATE(id),
          method: "PUT",
          body: data,
        }),
        transformResponse: (response) =>
          transformApiResponse<Product>(response),
        invalidatesTags: ["Product"],
      }
    ),
    deleteProduct: build.mutation<{ message: string }, number>({
      query: (id) => ({
        url: API_ENDPOINTS.PRODUCT_DELETE(id),
        method: "DELETE",
      }),
      invalidatesTags: ["Product"],
    }),
    // Discount-related endpoints
    getProductWithDiscount: build.query<Product, number>({
      query: (id) => ({
        url: `${API_ENDPOINTS.PRODUCTS}/${id}/with-discount`,
        method: "GET",
      }),
      transformResponse: (response) => transformApiResponse<Product>(response),
      providesTags: ["Product"],
    }),
    getProductsWithDiscounts: build.query<Product[], void>({
      query: () => ({
        url: `${API_ENDPOINTS.PRODUCTS}/with-discounts`,
        method: "GET",
      }),
      transformResponse: (response) =>
        transformApiResponse<Product[]>(response),
      providesTags: ["Product"],
    }),
    getDiscountedPrice: build.query<number, number>({
      query: (id) => ({
        url: `${API_ENDPOINTS.PRODUCTS}/${id}/discount-price`,
        method: "GET",
      }),
      transformResponse: (response) => transformApiResponse<number>(response),
    }),
    getActiveDiscountsForProduct: build.query<Discount[], number>({
      query: (id) => ({
        url: `${API_ENDPOINTS.PRODUCTS}/${id}/active-discounts`,
        method: "GET",
      }),
      transformResponse: (response) =>
        transformApiResponse<Discount[]>(response),
      providesTags: ["Discount"],
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetProductsQuery,
  useGetProductByIdQuery,
  useGetProductsByBrandQuery,
  useGetProductsByIdsQuery,
  useCreateProductMutation,
  useUpdateProductMutation,
  useDeleteProductMutation,
  // Discount-related hooks
  useGetProductWithDiscountQuery,
  useGetProductsWithDiscountsQuery,
  useGetDiscountedPriceQuery,
  useGetActiveDiscountsForProductQuery,
} = productApi;
