import { Product } from "./product";

export interface CartDetail {
  productId: number;
  product?: Product; // Optional - can be undefined when not fetched
  quantity: number;
  price: number;
}

export interface Cart {
  id: number;
  userId: string;
  items: CartDetail[];
  totalPrice: number;
  rowVersion: string | null; // Base64 encoded byte array, can be null for new carts
}

export interface CartSyncItem {
  productId: number;
  quantity: number;
}

export interface CartSyncPayload {
  items: CartSyncItem[];
  rowVersion: string | null;
}
