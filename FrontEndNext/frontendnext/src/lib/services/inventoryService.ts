import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import {
  Inventory,
  InventoryCreateDto,
  InventoryUpdateDto,
} from "../types/inventory";

export const inventoryApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getAllInventory: builder.query<Inventory[], void>({
      query: () => "/inventory",
      transformResponse: (response) => {
        const transformedResponse = transformApiResponse<{
          items: Inventory[];
        }>(response);
        return transformedResponse.items || [];
      },
      providesTags: ["Inventory"],
    }),
    getInventoryById: builder.query<Inventory, number>({
      query: (id) => `/inventory/${id}`,
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
  }),
});

export const {
  useGetAllInventoryQuery,
  useGetInventoryByIdQuery,
  useCreateInventoryMutation,
  useUpdateInventoryMutation,
  useDeleteInventoryMutation,
} = inventoryApi;
