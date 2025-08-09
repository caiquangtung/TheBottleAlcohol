import { API_ENDPOINTS } from "./endpoints";
import { enhancedApi } from "./api";
import {
  Wishlist,
  WishlistCreatePayload,
  WishlistDetail,
  WishlistDetailCreatePayload,
} from "../types/wishlist";
import { transformApiResponse } from "../utils/utils";

export const wishlistApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    // Legacy endpoints - keep for backward compatibility if needed
    getWishlistsByCustomer: builder.query<Wishlist[], number>({
      query: (customerId) => API_ENDPOINTS.WISHLIST_BY_CUSTOMER(customerId),
      transformResponse: transformApiResponse,
      providesTags: ["User", "Wishlist"],
    }),

    // New simplified endpoints
    getMyWishlistProducts: builder.query<WishlistDetail[], void>({
      query: () => API_ENDPOINTS.MY_WISHLIST_PRODUCTS,
      transformResponse: transformApiResponse,
      providesTags: ["Wishlist"],
    }),
    addProductToMyWishlist: builder.mutation<WishlistDetail, number>({
      query: (productId) => ({
        url: API_ENDPOINTS.ADD_TO_WISHLIST(productId),
        method: "POST",
      }),
      transformResponse: transformApiResponse,
      invalidatesTags: ["Wishlist"],
    }),
    removeProductFromMyWishlist: builder.mutation<{ message: string }, number>({
      query: (productId) => ({
        url: API_ENDPOINTS.REMOVE_FROM_WISHLIST(productId),
        method: "DELETE",
      }),
      transformResponse: transformApiResponse,
      invalidatesTags: ["Wishlist"],
    }),
  }),
  overrideExisting: false,
});

export const {
  // Legacy hooks
  useGetWishlistsByCustomerQuery,

  // New simplified hooks
  useGetMyWishlistProductsQuery,
  useAddProductToMyWishlistMutation,
  useRemoveProductFromMyWishlistMutation,
} = wishlistApi;
