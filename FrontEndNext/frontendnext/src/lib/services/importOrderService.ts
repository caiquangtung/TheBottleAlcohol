import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import { PagedResult } from "../types";
import {
  ImportOrder,
  ImportOrderCreateDto,
  ImportOrderUpdateDto,
  ImportOrderFilterDto,
  ImportOrderStats,
  ApproveImportOrderRequest,
  CompleteImportOrderRequest,
  CancelImportOrderRequest,
} from "../types/importOrder";

export const importOrderApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    // Import Order Management
    getAllImportOrders: builder.query<
      PagedResult<ImportOrder>,
      ImportOrderFilterDto | void
    >({
      query: (filter) => {
        const params = filter || {};
        return {
          url: "/importorder",
          params,
        };
      },
      transformResponse: (response) =>
        transformApiResponse<PagedResult<ImportOrder>>(response),
      providesTags: ["ImportOrder"],
    }),
    getImportOrderById: builder.query<ImportOrder, number>({
      query: (id) => `/importorder/${id}`,
      transformResponse: (response) =>
        transformApiResponse<ImportOrder>(response),
      providesTags: ["ImportOrder"],
    }),
    createImportOrder: builder.mutation<ImportOrder, ImportOrderCreateDto>({
      query: (importOrder) => ({
        url: "/importorder",
        method: "POST",
        body: importOrder,
      }),
      transformResponse: (response) =>
        transformApiResponse<ImportOrder>(response),
      invalidatesTags: ["ImportOrder", "Inventory"],
    }),
    updateImportOrder: builder.mutation<
      ImportOrder,
      { id: number; importOrder: ImportOrderUpdateDto }
    >({
      query: ({ id, importOrder }) => ({
        url: `/importorder/${id}`,
        method: "PUT",
        body: importOrder,
      }),
      transformResponse: (response) =>
        transformApiResponse<ImportOrder>(response),
      invalidatesTags: ["ImportOrder", "Inventory"],
    }),
    deleteImportOrder: builder.mutation<string, number>({
      query: (id) => ({
        url: `/importorder/${id}`,
        method: "DELETE",
      }),
      transformResponse: (response) => transformApiResponse<string>(response),
      invalidatesTags: ["ImportOrder"],
    }),

    // Import Order Status Management
    approveImportOrder: builder.mutation<
      string,
      { id: number; request?: ApproveImportOrderRequest }
    >({
      query: ({ id, request = {} }) => ({
        url: `/importorder/${id}/approve`,
        method: "PATCH",
        body: request,
      }),
      transformResponse: (response) => transformApiResponse<string>(response),
      invalidatesTags: ["ImportOrder"],
    }),
    completeImportOrder: builder.mutation<
      string,
      { id: number; request?: CompleteImportOrderRequest }
    >({
      query: ({ id, request = {} }) => ({
        url: `/importorder/${id}/complete`,
        method: "PATCH",
        body: request,
      }),
      transformResponse: (response) => transformApiResponse<string>(response),
      invalidatesTags: ["ImportOrder", "Inventory"],
    }),
    cancelImportOrder: builder.mutation<
      string,
      { id: number; request: CancelImportOrderRequest }
    >({
      query: ({ id, request }) => ({
        url: `/importorder/${id}/cancel`,
        method: "PATCH",
        body: request,
      }),
      transformResponse: (response) => transformApiResponse<string>(response),
      invalidatesTags: ["ImportOrder"],
    }),

    // Import Order Analytics
    getImportOrderStats: builder.query<ImportOrderStats, void>({
      query: () => "/importorder/stats",
      transformResponse: (response) =>
        transformApiResponse<ImportOrderStats>(response),
      providesTags: ["ImportOrder"],
    }),
  }),
});

export const {
  // Import Order Management
  useGetAllImportOrdersQuery,
  useGetImportOrderByIdQuery,
  useCreateImportOrderMutation,
  useUpdateImportOrderMutation,
  useDeleteImportOrderMutation,

  // Import Order Status Management
  useApproveImportOrderMutation,
  useCompleteImportOrderMutation,
  useCancelImportOrderMutation,

  // Import Order Analytics
  useGetImportOrderStatsQuery,
} = importOrderApi;
