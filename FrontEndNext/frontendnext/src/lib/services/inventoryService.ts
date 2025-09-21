import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import { PagedResult } from "../types";
import {
  Inventory,
  InventoryCreateDto,
  InventoryUpdateDto,
  InventoryFilterDto,
  UpdateStockRequest,
  AdjustStockRequest,
  InventoryTransaction,
  InventoryTransactionCreateDto,
  InventoryTransactionUpdateDto,
  InventoryTransactionFilterDto,
} from "../types/inventory";

export const inventoryApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    // Inventory Management
    getAllInventory: builder.query<
      PagedResult<Inventory>,
      InventoryFilterDto | void
    >({
      query: (filter) => {
        const params = filter || {};
        return {
          url: "/inventory",
          params,
        };
      },
      transformResponse: (response) =>
        transformApiResponse<PagedResult<Inventory>>(response),
      providesTags: ["Inventory"],
    }),
    getInventoryById: builder.query<Inventory, number>({
      query: (id) => `/inventory/${id}`,
      transformResponse: (response) =>
        transformApiResponse<Inventory>(response),
      providesTags: ["Inventory"],
    }),
    getInventoryByProduct: builder.query<Inventory, number>({
      query: (productId) => `/inventory/product/${productId}`,
      transformResponse: (response) =>
        transformApiResponse<Inventory>(response),
      providesTags: ["Inventory"],
    }),
    createInventory: builder.mutation<Inventory, InventoryCreateDto>({
      query: (inventory) => ({
        url: "/inventory",
        method: "POST",
        body: inventory,
      }),
      transformResponse: (response) =>
        transformApiResponse<Inventory>(response),
      invalidatesTags: ["Inventory"],
    }),
    updateInventory: builder.mutation<
      Inventory,
      { id: number; inventory: InventoryUpdateDto }
    >({
      query: ({ id, inventory }) => ({
        url: `/inventory/${id}`,
        method: "PUT",
        body: inventory,
      }),
      transformResponse: (response) =>
        transformApiResponse<Inventory>(response),
      invalidatesTags: ["Inventory"],
    }),
    deleteInventory: builder.mutation<void, number>({
      query: (id) => ({
        url: `/inventory/${id}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Inventory"],
    }),

    // Stock Management
    updateStock: builder.mutation<
      string,
      { id: number; request: UpdateStockRequest }
    >({
      query: ({ id, request }) => ({
        url: `/inventory/${id}/stock`,
        method: "PATCH",
        body: request,
      }),
      transformResponse: (response) => transformApiResponse<string>(response),
      invalidatesTags: ["Inventory"],
    }),
    adjustStock: builder.mutation<
      string,
      { productId: number; request: AdjustStockRequest }
    >({
      query: ({ productId, request }) => ({
        url: `/inventory/product/${productId}/adjust`,
        method: "PATCH",
        body: request,
      }),
      transformResponse: (response) => transformApiResponse<string>(response),
      invalidatesTags: ["Inventory"],
    }),

    // Inventory Analytics
    getTotalInventoryValue: builder.query<number, void>({
      query: () => "/inventory/total-value",
      transformResponse: (response) => transformApiResponse<number>(response),
      providesTags: ["Inventory"],
    }),
    getLowStockItems: builder.query<Inventory[], void>({
      query: () => "/inventory/low-stock",
      transformResponse: (response) =>
        transformApiResponse<Inventory[]>(response),
      providesTags: ["Inventory"],
    }),
    recalculateAllTotalValues: builder.mutation<string, void>({
      query: () => ({
        url: "/inventory/recalculate-values",
        method: "POST",
      }),
      transformResponse: (response) => transformApiResponse<string>(response),
      invalidatesTags: ["Inventory"],
    }),

    // Inventory Transactions
    getAllInventoryTransactions: builder.query<
      PagedResult<InventoryTransaction>,
      InventoryTransactionFilterDto | void
    >({
      query: (filter) => {
        const params = filter || {};
        return {
          url: "/inventorytransaction",
          params,
        };
      },
      transformResponse: (response) =>
        transformApiResponse<PagedResult<InventoryTransaction>>(response),
      providesTags: ["Inventory"],
    }),
    getInventoryTransactionById: builder.query<InventoryTransaction, number>({
      query: (id) => `/inventorytransaction/${id}`,
      transformResponse: (response) =>
        transformApiResponse<InventoryTransaction>(response),
      providesTags: ["Inventory"],
    }),
    getInventoryTransactionsByProduct: builder.query<
      InventoryTransaction[],
      number
    >({
      query: (productId) => `/inventorytransaction/product/${productId}`,
      transformResponse: (response) =>
        transformApiResponse<InventoryTransaction[]>(response),
      providesTags: ["Inventory"],
    }),
    createInventoryTransaction: builder.mutation<
      InventoryTransaction,
      InventoryTransactionCreateDto
    >({
      query: (transaction) => ({
        url: "/inventorytransaction",
        method: "POST",
        body: transaction,
      }),
      transformResponse: (response) =>
        transformApiResponse<InventoryTransaction>(response),
      invalidatesTags: ["Inventory"],
    }),
    updateInventoryTransaction: builder.mutation<
      InventoryTransaction,
      { id: number; transaction: InventoryTransactionUpdateDto }
    >({
      query: ({ id, transaction }) => ({
        url: `/inventorytransaction/${id}`,
        method: "PUT",
        body: transaction,
      }),
      transformResponse: (response) =>
        transformApiResponse<InventoryTransaction>(response),
      invalidatesTags: ["Inventory"],
    }),
    deleteInventoryTransaction: builder.mutation<string, number>({
      query: (id) => ({
        url: `/inventorytransaction/${id}`,
        method: "DELETE",
      }),
      transformResponse: (response) => transformApiResponse<string>(response),
      invalidatesTags: ["Inventory"],
    }),
  }),
});

export const {
  // Inventory Management
  useGetAllInventoryQuery,
  useGetInventoryByIdQuery,
  useGetInventoryByProductQuery,
  useCreateInventoryMutation,
  useUpdateInventoryMutation,
  useDeleteInventoryMutation,

  // Stock Management
  useUpdateStockMutation,
  useAdjustStockMutation,

  // Inventory Analytics
  useGetTotalInventoryValueQuery,
  useGetLowStockItemsQuery,
  useRecalculateAllTotalValuesMutation,

  // Inventory Transactions
  useGetAllInventoryTransactionsQuery,
  useGetInventoryTransactionByIdQuery,
  useGetInventoryTransactionsByProductQuery,
  useCreateInventoryTransactionMutation,
  useUpdateInventoryTransactionMutation,
  useDeleteInventoryTransactionMutation,
} = inventoryApi;
