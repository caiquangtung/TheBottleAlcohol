import { api } from "./api";
import { API_ENDPOINTS } from "./endpoints";

interface LoginCredentials {
  email: string;
  password: string;
}

interface User {
  Id: number;
  FullName: string;
  Email: string;
  Role: string;
}

interface LoginResponse {
  Success: boolean;
  Message: string | null;
  Data: {
    Id: number;
    FullName: string;
    Email: string;
    Role: string;
    AccessToken: string;
    RefreshToken: string;
  };
}

export const authApi = api.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation<LoginResponse, LoginCredentials>({
      query: (credentials) => ({
        url: API_ENDPOINTS.LOGIN,
        method: "POST",
        body: credentials,
      }),
    }),
    logout: builder.mutation<void, void>({
      query: () => ({
        url: API_ENDPOINTS.LOGOUT,
        method: "POST",
      }),
    }),
    getProfile: builder.query<any, void>({
      query: () => "/auth/profile",
      providesTags: ["Profile"],
    }),
  }),
  overrideExisting: false,
});

export const { useLoginMutation, useLogoutMutation, useGetProfileQuery } =
  authApi;
