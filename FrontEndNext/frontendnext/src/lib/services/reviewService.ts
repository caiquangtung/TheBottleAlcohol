import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import {
  Review,
  ReviewCreate,
  ReviewCreateDto,
  ReviewUpdateDto,
} from "../types/review";

export const reviewApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getReviewsByProduct: builder.query<Review[], number>({
      query: (productId) => `/review/product/${productId}`,
      transformResponse: transformApiResponse,
      providesTags: ["Product", "Review"],
    }),
    getAllReviews: builder.query<Review[], void>({
      query: () => "/review",
      transformResponse: (response) => transformApiResponse<Review[]>(response),
      providesTags: ["Review"],
    }),
    getReviewById: builder.query<Review, number>({
      query: (id) => `/review/${id}`,
      transformResponse: (response) => transformApiResponse<Review>(response),
      providesTags: ["Review"],
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
    createReviewAdmin: builder.mutation<Review, ReviewCreateDto>({
      query: (payload) => ({
        url: "/review",
        method: "POST",
        body: payload,
      }),
      transformResponse: (response) => transformApiResponse<Review>(response),
      invalidatesTags: ["Review"],
    }),
    updateReview: builder.mutation<
      Review,
      { id: number; review: ReviewUpdateDto }
    >({
      query: ({ id, review }) => ({
        url: `/review/${id}`,
        method: "PUT",
        body: review,
      }),
      transformResponse: (response) => transformApiResponse<Review>(response),
      invalidatesTags: ["Review"],
    }),
    deleteReview: builder.mutation<void, number>({
      query: (id) => ({
        url: `/review/${id}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Review"],
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetReviewsByProductQuery,
  useCreateReviewMutation,
  useGetAllReviewsQuery,
  useGetReviewByIdQuery,
  useCreateReviewAdminMutation,
  useUpdateReviewMutation,
  useDeleteReviewMutation,
} = reviewApi;
