import { API_ENDPOINTS } from "./endpoints";
import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";

export interface VnPayCreatePaymentRequest {
  orderId: number;
  accountId: number;
  amount: number;
  orderDescription?: string;
  bankCode?: string;
}

export interface VnPayCreatePaymentResponse {
  paymentUrl: string;
  txnRef: string;
  amount: number;
  orderId: number;
  createdDate: string;
}

export interface VnPayPaymentCallback {
  success: boolean;
  responseCode: string;
  transactionRef: string;
  vnPayTransactionNo: string;
  amount: number;
  orderId: number;
  bankCode: string;
  bankTransactionNo?: string;
  cardType?: string;
  paymentDate?: string;
  orderInfo?: string;
  message: string;
}

export const vnpayApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    createVnPayPayment: builder.mutation<
      VnPayCreatePaymentResponse,
      VnPayCreatePaymentRequest
    >({
      query: (body) => ({
        url: API_ENDPOINTS.VNPAY_CREATE_PAYMENT,
        method: "POST",
        body,
      }),
      transformResponse: (response) =>
        transformApiResponse<VnPayCreatePaymentResponse>(response),
      invalidatesTags: ["Payment"],
    }),
    // Accept raw query string to preserve exact encoding/casing from browser URL
    processVnPayReturn: builder.query<VnPayPaymentCallback, string>({
      query: (rawQuery) => API_ENDPOINTS.VNPAY_PAYMENT_RETURN(rawQuery),
      transformResponse: (response) =>
        transformApiResponse<VnPayPaymentCallback>(response),
      providesTags: ["Payment"],
    }),
    getVnPayStatus: builder.query<
      { orderId: number; message: string },
      number | string
    >({
      query: (orderId) => API_ENDPOINTS.VNPAY_PAYMENT_STATUS(orderId),
      transformResponse: (response) =>
        transformApiResponse<{ orderId: number; message: string }>(response),
      providesTags: ["Payment"],
    }),
  }),
});

export const {
  useCreateVnPayPaymentMutation,
  useLazyProcessVnPayReturnQuery,
  useGetVnPayStatusQuery,
} = vnpayApi;
