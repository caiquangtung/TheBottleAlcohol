"use client";

import { useAppSelector } from "@/lib/store/hooks";
import {
  useGetUserOrdersQuery,
  OrderFilter,
} from "@/lib/services/orderService";
import { OrderStatusBadge } from "@/components/OrderStatusBadge";
import { Button } from "@/components/ui/button";
import { Pagination } from "@/components/ui/pagination";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

export default function OrdersPage() {
  const router = useRouter();
  const user = useAppSelector((s) => s.auth.user);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(5); // Show 5 orders per page

  // Redirect to login if not authenticated
  useEffect(() => {
    if (!user?.id) {
      router.push("/login");
    }
  }, [user, router]);

  const filter: OrderFilter = {
    pageNumber: currentPage,
    pageSize: pageSize,
    sortBy: "CreatedAt",
    sortOrder: "desc",
  };

  const { data, isLoading, isError } = useGetUserOrdersQuery(
    { customerId: user?.id || 0, filter },
    {
      skip: !user?.id,
    }
  );

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    // Scroll to top when page changes
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  if (!user?.id) {
    return null; // Will redirect to login
  }

  if (isLoading) {
    return (
      <div className="container mx-auto p-6">
        <div className="text-center py-12">
          <div className="text-lg">Đang tải danh sách đơn hàng...</div>
        </div>
      </div>
    );
  }

  if (isError || !data) {
    return (
      <div className="container mx-auto p-6">
        <div className="text-center py-12">
          <div className="text-red-500 text-lg mb-4">
            Không thể tải danh sách đơn hàng
          </div>
          <Button onClick={() => window.location.reload()}>Thử lại</Button>
        </div>
      </div>
    );
  }

  const orders = data.items || [];
  const totalPages = Math.ceil((data.totalRecords || 0) / pageSize);

  return (
    <div className="container mx-auto p-6">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold">Đơn hàng của tôi</h1>
        <Link href="/">
          <Button variant="outline">Tiếp tục mua sắm</Button>
        </Link>
      </div>

      {orders.length === 0 ? (
        <div className="text-center py-12">
          <div className="text-6xl mb-4">📦</div>
          <h2 className="text-xl font-medium mb-2">Chưa có đơn hàng nào</h2>
          <p className="text-muted-foreground mb-6">
            Bạn chưa đặt đơn hàng nào. Hãy khám phá các sản phẩm của chúng tôi!
          </p>
          <Link href="/">
            <Button>Mua sắm ngay</Button>
          </Link>
        </div>
      ) : (
        <div className="space-y-4">
          {orders.map((order) => (
            <div
              key={order.id}
              className="border rounded-lg p-6 hover:shadow-md transition-shadow"
            >
              <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4">
                <div className="flex-1">
                  <div className="flex items-center gap-4 mb-2">
                    <h3 className="font-semibold text-lg">
                      Đơn hàng #{order.id}
                    </h3>
                    <OrderStatusBadge status={order.status} />
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm text-muted-foreground">
                    <div>
                      <p>
                        <strong>Ngày đặt:</strong>{" "}
                        {new Date(order.createdAt).toLocaleDateString("vi-VN")}
                      </p>
                      <p>
                        <strong>Tổng tiền:</strong>{" "}
                        <span className="text-lg font-semibold text-primary">
                          {order.totalAmount.toLocaleString("vi-VN")} VNĐ
                        </span>
                      </p>
                    </div>
                    <div>
                      <p>
                        <strong>Địa chỉ giao hàng:</strong>{" "}
                        {order.shippingAddress}
                      </p>
                      <p>
                        <strong>Số lượng sản phẩm:</strong>{" "}
                        {order.orderDetails?.length || 0} sản phẩm
                      </p>
                    </div>
                  </div>

                  {order.notes && (
                    <div className="mt-3 text-sm">
                      <strong>Ghi chú:</strong> {order.notes}
                    </div>
                  )}
                </div>

                <div className="flex flex-col sm:flex-row gap-2 lg:flex-col lg:w-32">
                  <Link href={`/orders/${order.id}`} className="w-full">
                    <Button variant="outline" className="w-full">
                      Xem chi tiết
                    </Button>
                  </Link>

                  {order.status === "Pending" && (
                    <Link href={`/orders/${order.id}`} className="w-full">
                      <Button className="w-full">Thanh toán</Button>
                    </Link>
                  )}
                </div>
              </div>

              {/* Order Details Preview */}
              {order.orderDetails && order.orderDetails.length > 0 && (
                <div className="mt-4 pt-4 border-t">
                  <h4 className="font-medium mb-2">Sản phẩm trong đơn hàng:</h4>
                  <div className="space-y-2">
                    {order.orderDetails.slice(0, 3).map((detail, index) => (
                      <div key={index} className="flex justify-between text-sm">
                        <span>{detail.productName}</span>
                        <span>
                          {detail.quantity} x{" "}
                          {detail.unitPrice.toLocaleString("vi-VN")} VNĐ
                        </span>
                      </div>
                    ))}
                    {order.orderDetails.length > 3 && (
                      <div className="text-sm text-muted-foreground">
                        ... và {order.orderDetails.length - 3} sản phẩm khác
                      </div>
                    )}
                  </div>
                </div>
              )}
            </div>
          ))}

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="mt-8">
              <Pagination
                currentPage={currentPage}
                totalPages={totalPages}
                totalRecords={data.totalRecords || 0}
                pageSize={pageSize}
                onPageChange={handlePageChange}
              />
            </div>
          )}
        </div>
      )}
    </div>
  );
}
