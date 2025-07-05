export interface Notification {
  id: number;
  title: string;
  message: string;
  type: number; // 0: Info, 1: Success, 2: Warning, 3: Error
  isRead: boolean;
  userId: number;
  userName: string;
  createdAt: string;
  updatedAt: string;
}

export interface NotificationCreateDto {
  title: string;
  message: string;
  type: number;
  userId: number;
}

export interface NotificationUpdateDto {
  title: string;
  message: string;
  type: number;
  isRead: boolean;
}
