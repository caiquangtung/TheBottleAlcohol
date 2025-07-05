"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import {
  Search,
  Trash2,
  Loader2,
  Bell,
  CheckCircle,
  AlertTriangle,
  XCircle,
  Info,
} from "lucide-react";
import { toast } from "sonner";
import {
  useGetAllNotificationsQuery,
  useDeleteNotificationMutation,
  useUpdateNotificationMutation,
} from "@/lib/services/notificationService";
import { Notification } from "@/lib/types/notification";

export default function NotificationsPage() {
  const [searchTerm, setSearchTerm] = useState("");

  // RTK Query hooks
  const {
    data: notifications = [],
    isLoading,
    error,
  } = useGetAllNotificationsQuery();
  const [deleteNotification, { isLoading: isDeleting }] =
    useDeleteNotificationMutation();


  const handleDelete = async (id: number) => {
    try {
      await deleteNotification(id).unwrap();
      toast.success("Notification deleted successfully");
    } catch (error) {
      toast.error("Failed to delete notification");
      console.error("Error deleting notification:", error);
    }
  };



  const getTypeLabel = (type: number) => {
    switch (type) {
      case 0:
        return <Badge variant="default">Info</Badge>;
      case 1:
        return <Badge variant="default">Success</Badge>;
      case 2:
        return <Badge variant="secondary">Warning</Badge>;
      case 3:
        return <Badge variant="destructive">Error</Badge>;
      default:
        return <Badge variant="outline">Unknown</Badge>;
    }
  };

  const getTypeIcon = (type: number) => {
    switch (type) {
      case 0:
        return <Info className="h-4 w-4 text-blue-500" />;
      case 1:
        return <CheckCircle className="h-4 w-4 text-green-500" />;
      case 2:
        return <AlertTriangle className="h-4 w-4 text-yellow-500" />;
      case 3:
        return <XCircle className="h-4 w-4 text-red-500" />;
      default:
        return <Bell className="h-4 w-4 text-gray-500" />;
    }
  };

  const filteredNotifications = notifications.filter(
    (notification) =>
      notification.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
      notification.message.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="h-8 w-8 animate-spin" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-red-500">Failed to load notifications</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Notifications</h1>
        <div className="text-sm text-gray-500">
          {notifications.filter((n) => !n.isRead).length} unread notifications
        </div>
      </div>

      <div className="flex items-center space-x-2">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search notifications..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-8"
          />
        </div>
      </div>

      <div className="border rounded-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Type</TableHead>
              <TableHead>Title</TableHead>
              <TableHead>Message</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Date</TableHead>
              <TableHead>Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {filteredNotifications.map((notification) => (
              <TableRow
                key={notification.id}
                className={!notification.isRead ? "bg-blue-50" : ""}
              >
                <TableCell>
                  <div className="flex items-center space-x-2">
                    {getTypeIcon(notification.type)}
                    {getTypeLabel(notification.type)}
                  </div>
                </TableCell>
                <TableCell className="font-medium">
                  {notification.title}
                </TableCell>
                <TableCell className="max-w-xs truncate">
                  {notification.message}
                </TableCell>
                <TableCell>
                  <Badge
                    variant={notification.isRead ? "secondary" : "default"}
                  >
                    {notification.isRead ? "Read" : "Unread"}
                  </Badge>
                </TableCell>
                <TableCell className="text-sm text-gray-500">
                  {new Date(notification.createdAt).toLocaleDateString()}
                </TableCell>
                <TableCell>
                  <div className="flex items-center space-x-2">

                    <AlertDialog>
                      <AlertDialogTrigger asChild>
                        <Button variant="outline" size="sm">
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </AlertDialogTrigger>
                      <AlertDialogContent>
                        <AlertDialogHeader>
                          <AlertDialogTitle>
                            Delete Notification
                          </AlertDialogTitle>
                          <AlertDialogDescription>
                            Are you sure you want to delete this notification?
                            This action cannot be undone.
                          </AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                          <AlertDialogCancel>Cancel</AlertDialogCancel>
                          <AlertDialogAction
                            onClick={() => handleDelete(notification.id)}
                            className="bg-red-600 hover:bg-red-700"
                          >
                            Delete
                          </AlertDialogAction>
                        </AlertDialogFooter>
                      </AlertDialogContent>
                    </AlertDialog>
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      {filteredNotifications.length === 0 && (
        <div className="text-center py-8 text-gray-500">
          No notifications found
        </div>
      )}
    </div>
  );
}
