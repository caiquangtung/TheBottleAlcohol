import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import {
  Discount,
  DiscountCreateDto,
  DiscountUpdateDto,
} from "../types/discount";

export const discountApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getAllDiscounts: builder.query<Discount[], void>({
      query: () => "/discount",
      transformResponse: (response) =>
        transformApiResponse<Discount[]>(response),
      providesTags: ["Discount"],
    }),
    getDiscountById: builder.query<Discount, number>({
      query: (id) => `/discount/${id}`,
      transformResponse: (response) => transformApiResponse<Discount>(response),
      providesTags: ["Discount"],
    }),
    createDiscount: builder.mutation<Discount, DiscountCreateDto>({
      query: (discount) => ({
        url: "/discount",
        method: "POST",
        body: discount,
      }),
      transformResponse: (response) => transformApiResponse<Discount>(response),
      invalidatesTags: ["Discount"],
    }),
    updateDiscount: builder.mutation<
      Discount,
      { id: number; discount: DiscountUpdateDto }
    >({
      query: ({ id, discount }) => ({
        url: `/discount/${id}`,
        method: "PUT",
        body: discount,
      }),
      transformResponse: (response) => transformApiResponse<Discount>(response),
      invalidatesTags: ["Discount"],
    }),
    deleteDiscount: builder.mutation<void, number>({
      query: (id) => ({
        url: `/discount/${id}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Discount"],
    }),
  }),
});

export const {
  useGetAllDiscountsQuery,
  useGetDiscountByIdQuery,
  useCreateDiscountMutation,
  useUpdateDiscountMutation,
  useDeleteDiscountMutation,
} = discountApi;
