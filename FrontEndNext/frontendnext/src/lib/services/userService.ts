import { enhancedApi } from "./api";
import { API_ENDPOINTS } from "./endpoints";
import type { User, UserCreate, UserUpdate } from "../types/user";

export const userApi = enhancedApi.injectEndpoints({
  endpoints: (build) => ({
    getUsers: build.query<User[], { search?: string } | void>({
      query: (params) => {
        let url = API_ENDPOINTS.USERS;
        if (params && params.search) {
          const qs = new URLSearchParams({ search: params.search }).toString();
          url += `?${qs}`;
        }
        return {
          url,
          method: "GET",
        };
      },
      providesTags: ["User"],
      transformResponse: (response: any) => response.data?.items ?? [],
    }),
    createUser: build.mutation<User, UserCreate>({
      query: (body) => ({
        url: API_ENDPOINTS.USER_CREATE,
        method: "POST",
        body,
      }),
      invalidatesTags: ["User"],
    }),
    updateUser: build.mutation<User, { id: number; data: UserUpdate }>({
      query: ({ id, data }) => ({
        url: API_ENDPOINTS.USER_UPDATE(id),
        method: "PUT",
        body: data,
      }),
      invalidatesTags: ["User"],
    }),
    deleteUser: build.mutation<{ message: string }, number>({
      query: (id) => ({
        url: API_ENDPOINTS.USER_DELETE(id),
        method: "DELETE",
      }),
      invalidatesTags: ["User"],
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetUsersQuery,
  useCreateUserMutation,
  useUpdateUserMutation,
  useDeleteUserMutation,
} = userApi;
