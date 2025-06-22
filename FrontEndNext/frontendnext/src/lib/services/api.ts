import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { RootState } from "../store";
import { loginSuccess, logout } from "../features/auth/authSlice";

export const api = createApi({
  reducerPath: "api",
  baseQuery: fetchBaseQuery({
    baseUrl: process.env.NEXT_PUBLIC_API_URL || "http://localhost:8080/api/v1",
    credentials: "include", // Đảm bảo HttpOnly cookies được gửi
    prepareHeaders: (headers, { getState }) => {
      const token = (getState() as RootState).auth.accessToken;

      if (token) {
        headers.set("authorization", `Bearer ${token}`);
      }

      return headers;
    },
  }),
  endpoints: () => ({}),
  tagTypes: ["Product", "Category", "User", "Profile", "Brand", "Cart"],
});

// Add refresh token handling
const baseQueryWithReauth = async (args: any, api: any, extraOptions: any) => {
  let result = await api.baseQuery(args, api, extraOptions);

  if (result.error?.status === 401) {
    // Try to get a new token using the HttpOnly cookie (backend handles refresh token automatically)
    const refreshResult = await api.baseQuery(
      {
        url: "/auth/refresh-token",
        method: "POST",
        credentials: "include", // Đảm bảo cookie được gửi
      },
      api,
      extraOptions
    );

    if (refreshResult.data) {
      // Store the new access token
      api.dispatch(
        loginSuccess({
          user: JSON.parse(localStorage.getItem("user") || "{}"),
          accessToken: refreshResult.data.data.accessToken,
          refreshToken: null, // Không cần lưu refresh token - backend xử lý qua cookie
        })
      );

      // Retry the original query
      result = await api.baseQuery(args, api, extraOptions);
    } else {
      // If refresh token fails, logout
      api.dispatch(logout());
    }
  }

  return result;
};

export const enhancedApi = api.injectEndpoints({
  endpoints: () => ({}),
  overrideExisting: false,
});
