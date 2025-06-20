import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { RootState } from "../store";
import { loginSuccess, logout } from "../features/auth/authSlice";

export const api = createApi({
  reducerPath: "api",
  baseQuery: fetchBaseQuery({
    baseUrl: process.env.NEXT_PUBLIC_API_URL || "http://localhost:8080/api/v1",
    prepareHeaders: (headers, { getState }) => {
      const token = (getState() as RootState).auth.accessToken;

      if (token) {
        headers.set("authorization", `Bearer ${token}`);
      }

      return headers;
    },
  }),
  endpoints: () => ({}),
  tagTypes: ["Product", "Category", "User", "Profile", "Brand"],
});

// Add refresh token handling
const baseQueryWithReauth = async (args: any, api: any, extraOptions: any) => {
  let result = await api.baseQuery(args, api, extraOptions);

  if (result.error?.status === 401) {
    // Try to get a new token
    const refreshResult = await api.baseQuery(
      {
        url: "/auth/refresh-token",
        method: "POST",
      },
      api,
      extraOptions
    );

    if (refreshResult.data) {
      // Store the new token
      api.dispatch(
        loginSuccess({
          user: JSON.parse(localStorage.getItem("user") || "{}"),
          accessToken: refreshResult.data.Data.AccessToken,
          refreshToken: refreshResult.data.Data.RefreshToken,
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
