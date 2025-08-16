import { OrderStatusType } from "@/lib/types/order";

export interface OrderStatusConfig {
  label: string;
  color: string;
  bgColor: string;
  icon: string;
  description: string;
}

export const orderStatusConfig: Record<OrderStatusType, OrderStatusConfig> = {
  Pending: {
    label: "Chờ xử lý",
    color: "text-yellow-700",
    bgColor: "bg-yellow-100",
    icon: "⏳",
    description: "Đơn hàng đang chờ được xử lý",
  },
  Paid: {
    label: "Đã thanh toán",
    color: "text-green-700",
    bgColor: "bg-green-100",
    icon: "💳",
    description: "Đã thanh toán thành công, chờ xử lý",
  },
  Processing: {
    label: "Đang xử lý",
    color: "text-blue-700",
    bgColor: "bg-blue-100",
    icon: "⚙️",
    description: "Đơn hàng đang được chuẩn bị",
  },
  Shipped: {
    label: "Đã gửi hàng",
    color: "text-purple-700",
    bgColor: "bg-purple-100",
    icon: "🚚",
    description: "Đơn hàng đang được vận chuyển",
  },
  Delivered: {
    label: "Đã giao hàng",
    color: "text-emerald-700",
    bgColor: "bg-emerald-100",
    icon: "✅",
    description: "Đơn hàng đã được giao thành công",
  },
  Cancelled: {
    label: "Đã hủy",
    color: "text-red-700",
    bgColor: "bg-red-100",
    icon: "❌",
    description: "Đơn hàng đã bị hủy",
  },
};

export function getOrderStatusDisplay(
  status: OrderStatusType
): OrderStatusConfig {
  return orderStatusConfig[status] || orderStatusConfig.Pending;
}

export function getOrderStatusBadge(status: OrderStatusType): string {
  const config = getOrderStatusDisplay(status);
  return `${config.icon} ${config.label}`;
}

export function canPayOrder(
  status: OrderStatusType,
  totalAmount: number
): boolean {
  return status === "Pending" && totalAmount > 0;
}

export function canCancelOrder(status: OrderStatusType): boolean {
  return status === "Pending" || status === "Paid";
}

export function isOrderCompleted(status: OrderStatusType): boolean {
  return status === "Delivered" || status === "Cancelled";
}
