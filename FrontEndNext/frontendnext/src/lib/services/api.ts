import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import type {
  BaseQueryFn,
  FetchArgs,
  FetchBaseQueryError,
} from "@reduxjs/toolkit/query";
import { RootState } from "../store";
import { loginSuccess, logout } from "../features/auth/authSlice";
import { API_ENDPOINTS } from "./endpoints";
import type { DashboardSummaryDto } from "../types/statistics";

const rawBaseQuery = fetchBaseQuery({
  baseUrl: process.env.NEXT_PUBLIC_API_URL || "/api",
  credentials: "include",
  prepareHeaders: (headers, { getState }) => {
    headers.set("Content-Type", "application/json");
    const token = (getState() as RootState).auth.accessToken;
    if (token) {
      headers.set("authorization", `Bearer ${token}`);
    }
    return headers;
  },
});

// Add refresh token handling
const baseQueryWithReauth: BaseQueryFn<
  string | FetchArgs,
  unknown,
  FetchBaseQueryError
> = async (args, api, extraOptions) => {
  let result = await rawBaseQuery(args, api, extraOptions);

  if (result.error?.status === 401) {
    // Try to get a new token using the HttpOnly cookie (backend handles refresh token automatically)
    const refreshResult = await rawBaseQuery(
      {
        url: "/auth/refresh-token",
        method: "POST",
        credentials: "include", // Đảm bảo cookie được gửi
      },
      api,
      extraOptions
    );

    if (refreshResult.data) {
      // Ép kiểu cho refreshResult.data
      const tokenData = refreshResult.data as { data: { accessToken: string } };
      const state = api.getState() as RootState;
      const user =
        state.auth?.user || JSON.parse(localStorage.getItem("user") || "{}");
      api.dispatch(
        loginSuccess({
          user,
          accessToken: tokenData.data.accessToken,
          refreshToken: null,
        })
      );

      // Retry the original query
      result = await rawBaseQuery(args, api, extraOptions);
    } else {
      // If refresh token fails, logout
      api.dispatch(logout());
    }
  }

  return result;
};

export const api = createApi({
  reducerPath: "api",
  baseQuery: baseQueryWithReauth,
  endpoints: () => ({}),
  tagTypes: [
    "Product",
    "Category",
    "User",
    "Profile",
    "Brand",
    "Cart",
    "Wishlist",
    "Recipe",
    "Review",
    "Supplier",
    "Discount",
    "Inventory",
    "Notification",
    "Order",
    "Payment",
    "PaymentMethod",
  ],
});

export const enhancedApi = api.injectEndpoints({
  endpoints: (build) => ({
    getDashboardSummary: build.query<DashboardSummaryDto, void>({
      query: () => ({
        url: API_ENDPOINTS.DASHBOARD_SUMMARY,
        method: "GET",
      }),
    }),
  }),
  overrideExisting: false,
});
