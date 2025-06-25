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
    getWishlistsByCustomer: builder.query<Wishlist[], number>({
      query: (customerId) => API_ENDPOINTS.WISHLIST_BY_CUSTOMER(customerId),
      transformResponse: transformApiResponse,
      providesTags: ["User", "Wishlist"],
    }),
    createWishlist: builder.mutation<Wishlist, WishlistCreatePayload>({
      query: (payload) => ({
        url: API_ENDPOINTS.WISHLISTS,
        method: "POST",
        body: payload,
      }),
      transformResponse: transformApiResponse,
      invalidatesTags: ["Wishlist"],
    }),
    deleteWishlist: builder.mutation<{ message: string }, number>({
      query: (id) => ({
        url: API_ENDPOINTS.WISHLIST_DETAIL(id),
        method: "DELETE",
      }),
      transformResponse: transformApiResponse,
      invalidatesTags: ["Wishlist"],
    }),
    getWishlistProducts: builder.query<WishlistDetail[], number>({
      query: (wishlistId) => API_ENDPOINTS.WISHLIST_PRODUCTS(wishlistId),
      transformResponse: transformApiResponse,
      providesTags: ["Wishlist"],
    }),
    addProductToWishlist: builder.mutation<
      WishlistDetail,
      WishlistDetailCreatePayload & { wishlistId: number }
    >({
      query: ({ wishlistId, productId }) => ({
        url: API_ENDPOINTS.WISHLIST_PRODUCTS(wishlistId),
        method: "POST",
        body: { wishlistId, productId },
      }),
      transformResponse: transformApiResponse,
      invalidatesTags: ["Wishlist"],
    }),
    removeProductFromWishlist: builder.mutation<
      { message: string },
      { wishlistId: number; productId: number }
    >({
      query: ({ wishlistId, productId }) => ({
        url: API_ENDPOINTS.WISHLIST_PRODUCT_DETAIL(wishlistId, productId),
        method: "DELETE",
      }),
      transformResponse: transformApiResponse,
      invalidatesTags: ["Wishlist"],
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetWishlistsByCustomerQuery,
  useCreateWishlistMutation,
  useDeleteWishlistMutation,
  useGetWishlistProductsQuery,
  useAddProductToWishlistMutation,
  useRemoveProductFromWishlistMutation,
} = wishlistApi;
