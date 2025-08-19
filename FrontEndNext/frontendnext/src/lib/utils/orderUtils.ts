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
    label: "Pending",
    color: "text-yellow-700",
    bgColor: "bg-yellow-100",
    icon: "⏳",
    description: "The order is awaiting processing",
  },
  Paid: {
    label: "Paid",
    color: "text-green-700",
    bgColor: "bg-green-100",
    icon: "💳",
    description: "Payment successful, waiting for processing",
  },
  Processing: {
    label: "Processing",
    color: "text-blue-700",
    bgColor: "bg-blue-100",
    icon: "⚙️",
    description: "The order is being prepared",
  },
  Shipped: {
    label: "Shipped",
    color: "text-purple-700",
    bgColor: "bg-purple-100",
    icon: "🚚",
    description: "The order is in transit",
  },
  Delivered: {
    label: "Delivered",
    color: "text-emerald-700",
    bgColor: "bg-emerald-100",
    icon: "✅",
    description: "The order has been delivered successfully",
  },
  Cancelled: {
    label: "Cancelled",
    color: "text-red-700",
    bgColor: "bg-red-100",
    icon: "❌",
    description: "The order has been cancelled",
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
