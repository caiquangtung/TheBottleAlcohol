export interface CartDetail {
  id: number;
  cartId: number;
  productId: number;
  productName: string;
  productImageUrl: string;
  price: number;
  quantity: number;
  createdAt: string;
  updatedAt?: string | null;
}

export interface Cart {
  id: number;
  customerId: number;
  customerName?: string | null;
  totalAmount: number;
  createdAt: string;
  updatedAt?: string | null;
  rowVersion: string | null;
  cartDetails: CartDetail[];
}

export interface CartSyncItem {
  productId: number;
  quantity: number;
}

export interface CartSyncPayload {
  customerId: number;
  items: CartSyncItem[];
  rowVersion: string | null;
}
