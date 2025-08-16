"use client";

import { notFound } from "next/navigation";
import { PaymentButton } from "@/components/PaymentButton";
import { OrderStatusBadge } from "@/components/OrderStatusBadge";
import { useGetOrderByIdQuery } from "@/lib/services/orderService";
import { canPayOrder } from "@/lib/utils/orderUtils";
import Link from "next/link";

export default function OrderDetailClient({ id }: { id: number }) {
  const { data: order, isLoading, isError } = useGetOrderByIdQuery(id);

  if (isLoading) return <div className="p-6">Đang tải đơn hàng...</div>;
  if (isError || !order) return notFound();

  const canPay = canPayOrder(order.status, order.totalAmount);

  return (
    <div className="container mx-auto p-6">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-semibold">Đơn hàng #{order.id}</h1>
        <OrderStatusBadge status={order.status} showDescription />
      </div>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="md:col-span-2 space-y-4">
          <div className="rounded-lg border p-4">
            <h2 className="font-medium mb-2">Thông tin khách hàng</h2>
            <div className="space-y-1 text-sm">
              <p>
                <span className="font-medium">Tên:</span>{" "}
                {order.customerName || "-"}
              </p>
              <p>
                <span className="font-medium">Email:</span>{" "}
                {order.customerEmail || "-"}
              </p>
              <p>
                <span className="font-medium">Điện thoại:</span>{" "}
                {order.customerPhone || "-"}
              </p>
              <p>
                <span className="font-medium">Địa chỉ:</span>{" "}
                {order.shippingAddress || "-"}
              </p>
              {(order as any)?.notes && (
                <p>
                  <span className="font-medium">Ghi chú:</span>{" "}
                  {(order as any).notes}
                </p>
              )}
            </div>
          </div>

          <div className="rounded-lg border p-4">
            <h2 className="font-medium mb-2">Sản phẩm</h2>
            <div className="space-y-2">
              {order.orderDetails.map((d) => (
                <div
                  key={`${d.orderId}-${d.productId}`}
                  className="flex justify-between text-sm"
                >
                  <div>
                    <p className="font-medium">{d.productName}</p>
                    <p className="text-muted-foreground">
                      {d.quantity} x {d.unitPrice.toLocaleString("vi-VN")} VNĐ
                    </p>
                  </div>
                  <div className="font-semibold">
                    {d.totalAmount.toLocaleString("vi-VN")} VNĐ
                  </div>
                </div>
              ))}
              {order.orderDetails.length === 0 && (
                <div className="text-sm text-muted-foreground">
                  Không có sản phẩm. Quay lại{" "}
                  <Link href="/checkout" className="underline">
                    Checkout
                  </Link>
                </div>
              )}
            </div>
          </div>
        </div>

        <div className="md:col-span-1 space-y-4">
          <div className="rounded-lg border p-4">
            <h2 className="font-medium mb-4">Thanh toán</h2>
            <div className="flex items-center justify-between">
              <span className="text-muted-foreground">Tổng tiền</span>
              <span className="text-lg font-semibold">
                {order.totalAmount.toLocaleString("vi-VN")} VNĐ
              </span>
            </div>
            <div className="mt-4">
              {canPay ? (
                <PaymentButton
                  orderId={order.id}
                  accountId={order.customerId}
                  amount={order.totalAmount}
                  orderDescription={`Thanh toan don hang #${order.id}`}
                />
              ) : (
                <div className="space-y-2">
                  <div className="text-sm text-muted-foreground">
                    {order.totalAmount <= 0
                      ? "Đơn hàng chưa có tổng tiền hợp lệ."
                      : order.status === "Paid"
                      ? "Đã thanh toán thành công!"
                      : order.status === "Delivered"
                      ? "Đơn hàng đã được giao!"
                      : order.status === "Cancelled"
                      ? "Đơn hàng đã bị hủy."
                      : "Đang xử lý đơn hàng..."}
                  </div>
                  <OrderStatusBadge status={order.status} />
                </div>
              )}
            </div>
            <div className="mt-3 text-xs text-muted-foreground">
              Không thấy sản phẩm? Quay lại{" "}
              <Link href="/checkout" className="underline">
                Checkout
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
