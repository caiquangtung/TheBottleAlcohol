"use client";

import React, { useState, useMemo } from "react";
import {
  useGetAllImportOrdersQuery,
  useDeleteImportOrderMutation,
  useApproveImportOrderMutation,
  useCompleteImportOrderMutation,
  useCancelImportOrderMutation,
  useGetImportOrderStatsQuery,
} from "@/lib/services/importOrderService";
import {
  ImportOrder,
  ImportOrderFilterDto,
  ImportOrderStatus,
} from "@/lib/types/importOrder";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
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
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { SearchInput } from "@/components/admin/SearchInput";
import {
  ShoppingCart,
  CheckCircle,
  XCircle,
  Clock,
  Plus,
  Edit,
  Trash2,
  Eye,
  TrendingUp,
  DollarSign,
} from "lucide-react";
import { toast } from "sonner";
import { formatCurrency } from "@/lib/utils";

interface ImportOrderListProps {
  onEdit?: (importOrder: ImportOrder) => void;
  onView?: (importOrder: ImportOrder) => void;
  onCreate?: () => void;
}

export function ImportOrderList({
  onEdit,
  onView,
  onCreate,
}: ImportOrderListProps) {
  const [filter, setFilter] = useState<ImportOrderFilterDto>({
    page: 1,
    pageSize: 20,
    sortBy: "createdAt",
    sortOrder: "desc",
  });

  const [searchTerm, setSearchTerm] = useState("");

  const {
    data: importOrderData,
    isLoading,
    error,
  } = useGetAllImportOrdersQuery(filter);

  const { data: stats } = useGetImportOrderStatsQuery();

  const [deleteImportOrder] = useDeleteImportOrderMutation();
  const [approveImportOrder] = useApproveImportOrderMutation();
  const [completeImportOrder] = useCompleteImportOrderMutation();
  const [cancelImportOrder] = useCancelImportOrderMutation();

  // Filter import orders based on search term
  const filteredImportOrders = useMemo(() => {
    if (!importOrderData?.items || !searchTerm) {
      return importOrderData?.items || [];
    }

    return importOrderData.items.filter(
      (item) =>
        item.supplierName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        item.managerName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        item.id.toString().includes(searchTerm)
    );
  }, [importOrderData?.items, searchTerm]);

  const handleDelete = async (id: number) => {
    try {
      await deleteImportOrder(id).unwrap();
      toast.success("Import order deleted successfully");
    } catch (error) {
      toast.error("Failed to delete import order");
    }
  };

  const handleApprove = async (id: number) => {
    try {
      await approveImportOrder({ id }).unwrap();
      toast.success("Import order approved successfully");
    } catch (error) {
      toast.error("Failed to approve import order");
    }
  };

  const handleComplete = async (id: number) => {
    try {
      await completeImportOrder({ id }).unwrap();
      toast.success("Import order completed successfully. Inventory updated!");
    } catch (error) {
      toast.error("Failed to complete import order");
    }
  };

  const handleCancel = async (id: number, reason: string) => {
    try {
      await cancelImportOrder({ id, request: { reason } }).unwrap();
      toast.success("Import order cancelled successfully");
    } catch (error) {
      toast.error("Failed to cancel import order");
    }
  };

  const handleFilterChange = (key: keyof ImportOrderFilterDto, value: any) => {
    setFilter((prev) => ({ ...prev, [key]: value, page: 1 }));
  };

  const getStatusVariant = (status: ImportOrderStatus) => {
    switch (status) {
      case ImportOrderStatus.Pending:
        return "secondary" as const;
      case ImportOrderStatus.Approved:
        return "default" as const;
      case ImportOrderStatus.Completed:
        return "default" as const;
      case ImportOrderStatus.Cancelled:
        return "destructive" as const;
      default:
        return "secondary" as const;
    }
  };

  const getStatusIcon = (status: ImportOrderStatus) => {
    switch (status) {
      case ImportOrderStatus.Pending:
        return Clock;
      case ImportOrderStatus.Approved:
        return CheckCircle;
      case ImportOrderStatus.Completed:
        return TrendingUp;
      case ImportOrderStatus.Cancelled:
        return XCircle;
      default:
        return Clock;
    }
  };

  if (error) {
    return (
      <Card>
        <CardContent className="p-6">
          <div className="text-center text-red-500">
            Error loading import order data
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="space-y-6">
      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Orders</CardTitle>
            <ShoppingCart className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats?.totalOrders || 0}</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Pending</CardTitle>
            <Clock className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-orange-600">
              {stats?.pendingOrders || 0}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Completed</CardTitle>
            <TrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">
              {stats?.completedOrders || 0}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Value</CardTitle>
            <DollarSign className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-blue-600">
              {formatCurrency(stats?.totalValue || 0)}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Filters and Actions */}
      <Card>
        <CardHeader>
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            <CardTitle>Import Orders</CardTitle>
            {onCreate && (
              <Button onClick={onCreate} className="flex items-center gap-2">
                <Plus className="h-4 w-4" />
                Create Import Order
              </Button>
            )}
          </div>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col sm:flex-row gap-4 mb-6">
            <SearchInput
              value={searchTerm}
              onChange={setSearchTerm}
              placeholder="Search by supplier, manager, or ID..."
              className="flex-1"
            />

            <div className="flex gap-2">
              <Select
                value={filter.status || "all"}
                onValueChange={(value) =>
                  handleFilterChange(
                    "status",
                    value === "all" ? undefined : value
                  )
                }
              >
                <SelectTrigger className="w-40">
                  <SelectValue placeholder="All Status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Status</SelectItem>
                  <SelectItem value={ImportOrderStatus.Pending}>
                    Pending
                  </SelectItem>
                  <SelectItem value={ImportOrderStatus.Approved}>
                    Approved
                  </SelectItem>
                  <SelectItem value={ImportOrderStatus.Completed}>
                    Completed
                  </SelectItem>
                  <SelectItem value={ImportOrderStatus.Cancelled}>
                    Cancelled
                  </SelectItem>
                </SelectContent>
              </Select>

              <Select
                value={filter.sortBy}
                onValueChange={(value) => handleFilterChange("sortBy", value)}
              >
                <SelectTrigger className="w-40">
                  <SelectValue placeholder="Sort by" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="createdAt">Date Created</SelectItem>
                  <SelectItem value="importDate">Import Date</SelectItem>
                  <SelectItem value="totalAmount">Total Amount</SelectItem>
                  <SelectItem value="status">Status</SelectItem>
                </SelectContent>
              </Select>

              <Select
                value={filter.sortOrder}
                onValueChange={(value: "asc" | "desc") =>
                  handleFilterChange("sortOrder", value)
                }
              >
                <SelectTrigger className="w-32">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="desc">Newest First</SelectItem>
                  <SelectItem value="asc">Oldest First</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Import Order Table */}
          <div className="border rounded-lg">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>ID</TableHead>
                  <TableHead>Supplier</TableHead>
                  <TableHead>Manager</TableHead>
                  <TableHead>Total Amount</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Import Date</TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {isLoading ? (
                  <TableRow>
                    <TableCell colSpan={7} className="text-center py-8">
                      Loading...
                    </TableCell>
                  </TableRow>
                ) : filteredImportOrders.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={7} className="text-center py-8">
                      No import orders found
                    </TableCell>
                  </TableRow>
                ) : (
                  filteredImportOrders.map((order) => {
                    const StatusIcon = getStatusIcon(order.status);

                    return (
                      <TableRow key={order.id}>
                        <TableCell className="font-mono">#{order.id}</TableCell>
                        <TableCell className="font-medium">
                          {order.supplierName}
                        </TableCell>
                        <TableCell>{order.managerName}</TableCell>
                        <TableCell className="font-medium">
                          {formatCurrency(order.totalAmount)}
                        </TableCell>
                        <TableCell>
                          <Badge
                            variant={getStatusVariant(order.status)}
                            className="flex items-center gap-1 w-fit"
                          >
                            <StatusIcon className="h-3 w-3" />
                            {order.status}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          {new Date(order.importDate).toLocaleDateString()}
                        </TableCell>
                        <TableCell className="text-right">
                          <div className="flex items-center justify-end gap-2">
                            {/* Status Actions */}
                            {order.status === ImportOrderStatus.Pending && (
                              <Button
                                variant="outline"
                                size="sm"
                                onClick={() => handleApprove(order.id)}
                                className="text-green-600 border-green-300 hover:bg-green-50"
                              >
                                Approve
                              </Button>
                            )}
                            {order.status === ImportOrderStatus.Approved && (
                              <Button
                                variant="outline"
                                size="sm"
                                onClick={() => handleComplete(order.id)}
                                className="text-blue-600 border-blue-300 hover:bg-blue-50"
                              >
                                Complete
                              </Button>
                            )}
                            {order.status === ImportOrderStatus.Pending && (
                              <AlertDialog>
                                <AlertDialogTrigger asChild>
                                  <Button
                                    variant="outline"
                                    size="sm"
                                    className="text-red-600 border-red-300 hover:bg-red-50"
                                  >
                                    Cancel
                                  </Button>
                                </AlertDialogTrigger>
                                <AlertDialogContent>
                                  <AlertDialogHeader>
                                    <AlertDialogTitle>
                                      Cancel Import Order
                                    </AlertDialogTitle>
                                    <AlertDialogDescription>
                                      Are you sure you want to cancel import
                                      order #{order.id}? This action cannot be
                                      undone.
                                    </AlertDialogDescription>
                                  </AlertDialogHeader>
                                  <AlertDialogFooter>
                                    <AlertDialogCancel>
                                      Cancel
                                    </AlertDialogCancel>
                                    <AlertDialogAction
                                      onClick={() =>
                                        handleCancel(
                                          order.id,
                                          "Cancelled by admin"
                                        )
                                      }
                                      className="bg-red-600 hover:bg-red-700"
                                    >
                                      Cancel Order
                                    </AlertDialogAction>
                                  </AlertDialogFooter>
                                </AlertDialogContent>
                              </AlertDialog>
                            )}

                            {/* Standard Actions */}
                            {onView && (
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => onView(order)}
                              >
                                <Eye className="h-4 w-4" />
                              </Button>
                            )}
                            {onEdit &&
                              order.status === ImportOrderStatus.Pending && (
                                <Button
                                  variant="ghost"
                                  size="sm"
                                  onClick={() => onEdit(order)}
                                >
                                  <Edit className="h-4 w-4" />
                                </Button>
                              )}
                            {order.status === ImportOrderStatus.Pending && (
                              <AlertDialog>
                                <AlertDialogTrigger asChild>
                                  <Button variant="ghost" size="sm">
                                    <Trash2 className="h-4 w-4" />
                                  </Button>
                                </AlertDialogTrigger>
                                <AlertDialogContent>
                                  <AlertDialogHeader>
                                    <AlertDialogTitle>
                                      Delete Import Order
                                    </AlertDialogTitle>
                                    <AlertDialogDescription>
                                      Are you sure you want to delete import
                                      order #{order.id}? This action cannot be
                                      undone.
                                    </AlertDialogDescription>
                                  </AlertDialogHeader>
                                  <AlertDialogFooter>
                                    <AlertDialogCancel>
                                      Cancel
                                    </AlertDialogCancel>
                                    <AlertDialogAction
                                      onClick={() => handleDelete(order.id)}
                                      className="bg-red-600 hover:bg-red-700"
                                    >
                                      Delete
                                    </AlertDialogAction>
                                  </AlertDialogFooter>
                                </AlertDialogContent>
                              </AlertDialog>
                            )}
                          </div>
                        </TableCell>
                      </TableRow>
                    );
                  })
                )}
              </TableBody>
            </Table>
          </div>

          {/* Pagination */}
          {importOrderData && importOrderData.totalPages > 1 && (
            <div className="flex justify-between items-center mt-4">
              <div className="text-sm text-muted-foreground">
                Showing {((filter.page || 1) - 1) * (filter.pageSize || 20) + 1}{" "}
                to{" "}
                {Math.min(
                  (filter.page || 1) * (filter.pageSize || 20),
                  importOrderData.totalItems
                )}{" "}
                of {importOrderData.totalItems} orders
              </div>
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={(filter.page || 1) <= 1}
                  onClick={() =>
                    handleFilterChange("page", (filter.page || 1) - 1)
                  }
                >
                  Previous
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={(filter.page || 1) >= importOrderData.totalPages}
                  onClick={() =>
                    handleFilterChange("page", (filter.page || 1) + 1)
                  }
                >
                  Next
                </Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}

export default ImportOrderList;
