import { enhancedApi } from "./api";
import { API_ENDPOINTS } from "./endpoints";
import {
  LoginCredentials,
  RegisterCredentials,
  User,
  LoginResponse,
  RegisterResponse,
  ProfileResponse,
} from "@/types/auth";

export const authApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation<LoginResponse, LoginCredentials>({
      query: (credentials) => ({
        url: API_ENDPOINTS.LOGIN,
        method: "POST",
        body: credentials,
      }),
    }),
    register: builder.mutation<RegisterResponse, RegisterCredentials>({
      query: (credentials) => ({
        url: API_ENDPOINTS.REGISTER,
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
    getProfile: builder.query<ProfileResponse, void>({
      query: () => "/auth/profile",
      providesTags: ["Profile"],
    }),
  }),
  overrideExisting: false,
});

export const {
  useLoginMutation,
  useRegisterMutation,
  useLogoutMutation,
  useGetProfileQuery,
} = authApi;
