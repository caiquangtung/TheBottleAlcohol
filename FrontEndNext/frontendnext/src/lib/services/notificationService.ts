import { enhancedApi } from "./api";
import { transformApiResponse } from "../utils/utils";
import {
  Notification,
  NotificationCreateDto,
  NotificationUpdateDto,
} from "../types/notification";

export const notificationApi = enhancedApi.injectEndpoints({
  endpoints: (builder) => ({
    getAllNotifications: builder.query<Notification[], void>({
      query: () => "/notification",
      transformResponse: (response) =>
        transformApiResponse<Notification[]>(response),
      providesTags: ["Notification"],
    }),
    getNotificationById: builder.query<Notification, number>({
      query: (id) => `/notification/${id}`,
      transformResponse: (response) =>
        transformApiResponse<Notification>(response),
      providesTags: ["Notification"],
    }),
    createNotification: builder.mutation<Notification, NotificationCreateDto>({
      query: (notification) => ({
        url: "/notification",
        method: "POST",
        body: notification,
      }),
      transformResponse: (response) =>
        transformApiResponse<Notification>(response),
      invalidatesTags: ["Notification"],
    }),
    updateNotification: builder.mutation<
      Notification,
      { id: number; notification: NotificationUpdateDto }
    >({
      query: ({ id, notification }) => ({
        url: `/notification/${id}`,
        method: "PUT",
        body: notification,
      }),
      transformResponse: (response) =>
        transformApiResponse<Notification>(response),
      invalidatesTags: ["Notification"],
    }),
    deleteNotification: builder.mutation<void, number>({
      query: (id) => ({
        url: `/notification/${id}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Notification"],
    }),
  }),
});

export const {
  useGetAllNotificationsQuery,
  useGetNotificationByIdQuery,
  useCreateNotificationMutation,
  useUpdateNotificationMutation,
  useDeleteNotificationMutation,
} = notificationApi;
