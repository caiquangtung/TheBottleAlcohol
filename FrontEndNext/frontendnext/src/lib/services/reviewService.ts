import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import { Review, ReviewCreate } from "../types/review";

export const reviewApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getReviewsByProduct: builder.query<Review[], number>({
      query: (productId) => `/review/product/${productId}`,
      transformResponse: transformApiResponse,
      providesTags: ["Product", "Review"],
    }),
    createReview: builder.mutation<Review, ReviewCreate>({
      query: (payload) => ({
        url: "/review",
        method: "POST",
        body: payload,
      }),
      transformResponse: transformApiResponse,
      invalidatesTags: ["Product", "Review"],
    }),
  }),
  overrideExisting: false,
});

export const { useGetReviewsByProductQuery, useCreateReviewMutation } =
  reviewApi;
