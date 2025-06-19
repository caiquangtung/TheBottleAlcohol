import { api } from "./api";
import { Product, ProductFilter } from "../types/product";

export const productApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getAllProducts: builder.query<Product[], ProductFilter | undefined>({
      query: (filter) => ({
        url: "/Product",
        method: "GET",
        ...(filter && { params: filter }),
      }),
      transformResponse: (response: any) => {
        console.log("API Response:", response);
        if (!response?.Success) {
        //   console.error("API Error:", response?.Message || "Unknown error");
          return [];
        }
        return response.Data || [];
      },
      providesTags: ["Product"],
    }),

    getProductById: builder.query<Product, number>({
      query: (id) => ({
        url: `/Product/${id}`,
        method: "GET",
      }),
      transformResponse: (response: any) => {
        console.log("API Response:", response);
        if (!response?.Success) {
        //   console.error("API Error:", response?.Message || "Unknown error");
          return null;
        }
        return response.Data;
      },
      providesTags: (result, error, id) => [{ type: "Product", id }],
    }),

    getFeaturedProducts: builder.query<Product[], void>({
      query: () => ({
        url: "/Product",
        method: "GET",
        params: {
          sortBy: "createdAt",
          sortOrder: "desc",
          limit: 6,
        },
      }),
      transformResponse: (response: any) => {
        console.log("API Response:", response);
        if (!response?.Success) {
          console.error("API Error:", response?.Message || "Unknown error");
          return [];
        }
        return response.Data || [];
      },
      providesTags: ["Product"],
    }),

    createProduct: builder.mutation<Product, Omit<Product, "id">>({
      query: (product) => ({
        url: "/Product",
        method: "POST",
        body: product,
      }),
      transformResponse: (response: any) => {
        console.log("API Response:", response);
        if (!response?.Success) {
          console.error("API Error:", response?.Message || "Unknown error");
          throw new Error(response?.Message || "Failed to create product");
        }
        return response.Data;
      },
      invalidatesTags: ["Product"],
    }),

    updateProduct: builder.mutation<
      Product,
      { id: number; product: Partial<Product> }
    >({
      query: ({ id, product }) => ({
        url: `/Product/${id}`,
        method: "PUT",
        body: product,
      }),
      transformResponse: (response: any) => {
        console.log("API Response:", response);
        if (!response?.Success) {
          console.error("API Error:", response?.Message || "Unknown error");
          throw new Error(response?.Message || "Failed to update product");
        }
        return response.Data;
      },
      invalidatesTags: (result, error, { id }) => [{ type: "Product", id }],
    }),

    deleteProduct: builder.mutation<void, number>({
      query: (id) => ({
        url: `/Product/${id}`,
        method: "DELETE",
      }),
      transformResponse: (response: any) => {
        console.log("API Response:", response);
        if (!response?.Success) {
          console.error("API Error:", response?.Message || "Unknown error");
          throw new Error(response?.Message || "Failed to delete product");
        }
        return response.Data;
      },
      invalidatesTags: ["Product"],
    }),
  }),
});

export const {
  useGetAllProductsQuery,
  useGetProductByIdQuery,
  useGetFeaturedProductsQuery,
  useCreateProductMutation,
  useUpdateProductMutation,
  useDeleteProductMutation,
} = productApi;
