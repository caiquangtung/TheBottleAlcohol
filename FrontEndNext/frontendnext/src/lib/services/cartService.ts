import { enhancedApi } from "./api";
import { Cart, CartSyncPayload } from "../types/cart";
import { API_ENDPOINTS } from "./endpoints";

// Backend response type
interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
}

// Backend CartResponseDto type
interface CartResponseDto {
  id: number;
  customerId: number;
  customerName: string;
  totalAmount: number;
  createdAt: string;
  updatedAt?: string;
  rowVersion: string | number[];
  cartDetails: Array<{
    id: number;
    cartId: number;
    productId: number;
    productName: string;
    price: number;
    quantity: number;
    createdAt: string;
    updatedAt?: string;
    productImageUrl?: string;
  }>;
}

// Transform backend response to frontend Cart
function transformCartResponse(response: ApiResponse<CartResponseDto>): Cart {
  return {
    id: response.data.id,
    customerId: response.data.customerId,
    customerName: response.data.customerName,
    totalAmount: Number(response.data.totalAmount || 0),
    createdAt: response.data.createdAt,
    updatedAt: response.data.updatedAt,
    rowVersion:
      typeof response.data.rowVersion === "string"
        ? response.data.rowVersion
        : response.data.rowVersion
        ? btoa(
            String.fromCharCode(
              ...Array.from(new Uint8Array(response.data.rowVersion))
            )
          )
        : null,
    cartDetails: (response.data.cartDetails || []).map((detail) => ({
      id: detail.id,
      cartId: detail.cartId,
      productId: detail.productId,
      productName: detail.productName,
      productImageUrl: detail.productImageUrl || "",
      price: detail.price,
      quantity: detail.quantity,
      createdAt: detail.createdAt,
      updatedAt: detail.updatedAt,
    })),
  };
}

export const cartApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getCart: builder.query<Cart, void>({
      query: () => API_ENDPOINTS.CART_CURRENT,
      transformResponse: (response: ApiResponse<CartResponseDto>) =>
        transformCartResponse(response),
      providesTags: ["Cart"],
    }),
    syncCart: builder.mutation<Cart, CartSyncPayload>({
      query: (payload) => ({
        url: API_ENDPOINTS.CART_SYNC,
        method: "POST",
        body: payload,
      }),
      transformResponse: (response: ApiResponse<CartResponseDto>) =>
        transformCartResponse(response),
      invalidatesTags: ["Cart"],
    }),
  }),
});

export const { useGetCartQuery, useSyncCartMutation } = cartApi;
