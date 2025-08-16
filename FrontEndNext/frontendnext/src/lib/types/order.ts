export type OrderStatusType =
  | "Pending"
  | "Paid"
  | "Processing"
  | "Shipped"
  | "Delivered"
  | "Cancelled";

export interface OrderDetailResponseDto {
  orderId: number;
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  totalAmount: number;
  createdAt: string;
  updatedAt?: string | null;
}

export interface OrderResponseDto {
  id: number;
  customerId: number;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  shippingAddress: string;
  totalAmount: number;
  status: OrderStatusType;
  notes: string;
  createdAt: string;
  updatedAt?: string | null;
  orderDetails: OrderDetailResponseDto[];
}

export interface OrderDetailCreateDto {
  productId: number;
  quantity: number;
}

export interface OrderCreateDto {
  customerId: number;
  paymentMethod: string;
  shippingMethod: string;
  shippingAddress: string;
  note?: string;
  orderDetails: OrderDetailCreateDto[];
}
