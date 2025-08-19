import { API_ENDPOINTS } from "./endpoints";
import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import type { OrderResponseDto } from "../types";
import type { PagedResult } from "./productService";
import type { OrderStatusType } from "../types/order";

export interface OrderDetailCreateDto {
  productId: number;
  quantity: number;
}

export interface OrderCreateDto {
  customerId: number;
  paymentMethod: string;
  shippingMethod: string;
  shippingAddress: string;
  note?: string;
  orderDetails: OrderDetailCreateDto[];
}

export interface OrderFilter {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}

export const orderApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getAllOrders: builder.query<
      PagedResult<OrderResponseDto>,
      {
        searchTerm?: string;
        pageNumber?: number;
        pageSize?: number;
        sortBy?: string;
        sortOrder?: "asc" | "desc";
      }
    >({
      query: (args = {}) => ({
        url: API_ENDPOINTS.ORDERS,
        params: {
          searchTerm: args.searchTerm ?? "",
          pageNumber: args.pageNumber ?? 1,
          pageSize: args.pageSize ?? 10,
          sortBy: args.sortBy ?? "CreatedAt",
          sortOrder: args.sortOrder ?? "desc",
        },
      }),
      transformResponse: (response) =>
        transformApiResponse<PagedResult<OrderResponseDto>>(response),
      providesTags: ["Order"],
    }),
    updateOrderStatus: builder.mutation<
      OrderResponseDto,
      { id: number | string; status: OrderStatusType }
    >({
      query: ({ id, status }) => ({
        url: API_ENDPOINTS.ORDER_UPDATE_STATUS(id),
        method: "PUT",
        // Backend expects a JSON string body (e.g., "Paid"), not raw text
        body: JSON.stringify(status),
      }),
      transformResponse: (response) =>
        transformApiResponse<OrderResponseDto>(response),
      invalidatesTags: ["Order"],
    }),
    getOrderById: builder.query<OrderResponseDto, number | string>({
      query: (id) => API_ENDPOINTS.ORDER_DETAIL(id),
      transformResponse: (response) =>
        transformApiResponse<OrderResponseDto>(response),
      providesTags: ["Order"],
    }),
    getUserOrders: builder.query<
      PagedResult<OrderResponseDto>,
      { customerId: number; filter?: OrderFilter }
    >({
      query: ({ customerId, filter = {} }) => ({
        url: `${API_ENDPOINTS.ORDERS}/customer/${customerId}`,
        params: {
          pageNumber: filter.pageNumber || 1,
          pageSize: filter.pageSize || 10,
          searchTerm: filter.searchTerm || "",
          sortBy: filter.sortBy || "CreatedAt",
          sortOrder: filter.sortOrder || "desc",
        },
      }),
      transformResponse: (response) =>
        transformApiResponse<PagedResult<OrderResponseDto>>(response),
      providesTags: ["Order"],
    }),
    createOrder: builder.mutation<OrderResponseDto, OrderCreateDto>({
      query: (body) => ({
        url: API_ENDPOINTS.ORDERS,
        method: "POST",
        body,
      }),
      transformResponse: (response) =>
        transformApiResponse<OrderResponseDto>(response),
      invalidatesTags: ["Order", "Cart"],
    }),
  }),
});

export const {
  useGetOrderByIdQuery,
  useGetUserOrdersQuery,
  useCreateOrderMutation,
  useGetAllOrdersQuery,
  useUpdateOrderStatusMutation,
} = orderApi;
