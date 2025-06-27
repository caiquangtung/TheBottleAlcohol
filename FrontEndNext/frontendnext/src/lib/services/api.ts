import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { RootState } from "../store";
import { loginSuccess, logout } from "../features/auth/authSlice";

const rawBaseQuery = fetchBaseQuery({
  baseUrl: process.env.NEXT_PUBLIC_API_URL || "http://localhost:8080/api/v1",
  credentials: "include", // Đảm bảo HttpOnly cookies được gửi
  prepareHeaders: (headers, { getState }) => {
    const token = (getState() as RootState).auth.accessToken;
    if (token) {
      headers.set("authorization", `Bearer ${token}`);
    }
    return headers;
  },
});

// Add refresh token handling
const baseQueryWithReauth = async (args: any, api: any, extraOptions: any) => {
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
      const state = api.getState();
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
  baseQuery: baseQueryWithReauth, // Sử dụng baseQueryWithReauth cho toàn bộ API
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
  ],
});

export const enhancedApi = api.injectEndpoints({
  endpoints: () => ({}),
  overrideExisting: false,
});
