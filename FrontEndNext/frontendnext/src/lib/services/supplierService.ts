import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import {
  Supplier,
  SupplierCreateDto,
  SupplierUpdateDto,
} from "../types/supplier";

export const supplierApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getAllSuppliers: builder.query<Supplier[], void>({
      query: () => "/supplier",
      transformResponse: (response) =>
        transformApiResponse<Supplier[]>(response),
      providesTags: ["Supplier"],
    }),
    getSupplierById: builder.query<Supplier, number>({
      query: (id) => `/supplier/${id}`,
      transformResponse: (response) => transformApiResponse<Supplier>(response),
      providesTags: ["Supplier"],
    }),
    createSupplier: builder.mutation<Supplier, SupplierCreateDto>({
      query: (supplier) => ({
        url: "/supplier",
        method: "POST",
        body: supplier,
      }),
      transformResponse: (response) => transformApiResponse<Supplier>(response),
      invalidatesTags: ["Supplier"],
    }),
    updateSupplier: builder.mutation<
      Supplier,
      { id: number; supplier: SupplierUpdateDto }
    >({
      query: ({ id, supplier }) => ({
        url: `/supplier/${id}`,
        method: "PUT",
        body: supplier,
      }),
      transformResponse: (response) => transformApiResponse<Supplier>(response),
      invalidatesTags: ["Supplier"],
    }),
    deleteSupplier: builder.mutation<void, number>({
      query: (id) => ({
        url: `/supplier/${id}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Supplier"],
    }),
  }),
});

export const {
  useGetAllSuppliersQuery,
  useGetSupplierByIdQuery,
  useCreateSupplierMutation,
  useUpdateSupplierMutation,
  useDeleteSupplierMutation,
} = supplierApi;
