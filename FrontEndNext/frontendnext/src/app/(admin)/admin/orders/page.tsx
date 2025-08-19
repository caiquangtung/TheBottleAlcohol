"use client";

import { useMemo, useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { toast } from "sonner";
import { OrderStatusBadge } from "@/components/OrderStatusBadge";
import { Pagination } from "@/components/ui/pagination";
import {
  useGetAllOrdersQuery,
  useUpdateOrderStatusMutation,
} from "@/lib/services/orderService";
import type { OrderResponseDto } from "@/lib/types";
import type { OrderStatusType } from "@/lib/types/order";

const ALL_STATUSES: OrderStatusType[] = [
  "Pending",
  "Paid",
  "Processing",
  "Shipped",
  "Delivered",
  "Cancelled",
];

function getAllowedNextStatuses(current: OrderStatusType): OrderStatusType[] {
  switch (current) {
    case "Pending":
      return ["Paid", "Cancelled"];
    case "Paid":
      return ["Processing", "Cancelled"];
    case "Processing":
      return ["Shipped", "Cancelled"];
    case "Shipped":
      return ["Delivered"];
    case "Delivered":
    case "Cancelled":
    default:
      return [];
  }
}

export default function AdminOrdersPage() {
  const [search, setSearch] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const { data, isLoading, refetch } = useGetAllOrdersQuery({
    searchTerm: search,
    pageNumber,
    pageSize,
    sortBy: "CreatedAt",
    sortOrder: "desc",
  });
  const [updateOrderStatus, { isLoading: isUpdating }] =
    useUpdateOrderStatusMutation();

  const orders = data?.items || [];

  const handleStatusChange = async (
    orderId: number,
    newStatus: OrderStatusType
  ) => {
    try {
      await updateOrderStatus({ id: orderId, status: newStatus }).unwrap();
      toast.success("Order status updated successfully");
      refetch();
    } catch (error: any) {
      toast.error(
        error?.data?.message || "An error occurred while updating status"
      );
    }
  };

  const filteredOrders = useMemo(() => {
    if (!search) return orders;
    const q = search.toLowerCase();
    return orders.filter((o: OrderResponseDto) =>
      [
        o.id?.toString(),
        o.customerName,
        o.customerEmail,
        o.customerPhone,
        o.shippingAddress,
        o.status,
      ]
        .filter(Boolean)
        .some((field) => field!.toString().toLowerCase().includes(q))
    );
  }, [orders, search]);

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(value);

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Manage Orders</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex items-center gap-3">
            <Input
              placeholder="Search by ID, customer, email, phone, address, status..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
            <Button
              variant="outline"
              onClick={() => refetch()}
              disabled={isLoading}
            >
              Reload
            </Button>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Orders ({data?.totalRecords ?? 0})</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="text-center py-8">Loading...</div>
          ) : filteredOrders.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              No orders found
            </div>
          ) : (
            <div className="border rounded-lg overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>ID</TableHead>
                    <TableHead>Customer</TableHead>
                    <TableHead>Contact</TableHead>
                    <TableHead>Address</TableHead>
                    <TableHead>Created at</TableHead>
                    <TableHead>Total</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Update status</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredOrders.map((order: OrderResponseDto) => (
                    <TableRow key={order.id}>
                      <TableCell className="font-mono">#{order.id}</TableCell>
                      <TableCell>
                        <div className="font-medium">{order.customerName}</div>
                      </TableCell>
                      <TableCell>
                        <div className="text-sm">{order.customerEmail}</div>
                        <div className="text-sm text-muted-foreground">
                          {order.customerPhone}
                        </div>
                      </TableCell>
                      <TableCell className="max-w-[260px] truncate">
                        {order.shippingAddress}
                      </TableCell>
                      <TableCell>
                        {order.createdAt
                          ? new Date(order.createdAt).toLocaleString("vi-VN")
                          : "--"}
                      </TableCell>
                      <TableCell>
                        {formatCurrency(order.totalAmount || 0)}
                      </TableCell>
                      <TableCell>
                        <OrderStatusBadge status={order.status} />
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Select
                            defaultValue={order.status}
                            onValueChange={(value) =>
                              handleStatusChange(
                                order.id,
                                value as OrderStatusType
                              )
                            }
                            disabled={isUpdating}
                          >
                            <SelectTrigger className="w-40">
                              <SelectValue placeholder="Select status" />
                            </SelectTrigger>
                            <SelectContent>
                              {ALL_STATUSES.map((s) => {
                                const allowed = getAllowedNextStatuses(
                                  order.status
                                );
                                const isDisabled = !allowed.includes(s);
                                return (
                                  <SelectItem
                                    key={s}
                                    value={s}
                                    disabled={isDisabled}
                                  >
                                    {s}
                                  </SelectItem>
                                );
                              })}
                            </SelectContent>
                          </Select>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
              {/* Pagination */}
              {(data?.totalPages ?? 1) > 1 && (
                <div className="p-4 flex justify-center">
                  <Pagination
                    currentPage={pageNumber}
                    totalPages={data?.totalPages ?? 1}
                    totalRecords={data?.totalRecords ?? 0}
                    pageSize={pageSize}
                    onPageChange={(p) => setPageNumber(p)}
                  />
                </div>
              )}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
