export interface Wishlist {
  id: number;
  accountId: number;
  name: string;
  createdAt: string;
  updatedAt?: string | null;
}

export interface WishlistCreatePayload {
  accountId: number;
  name: string;
}

export interface WishlistDetail {
  id: number;
  wishlistId: number;
  productId: number;
  createdAt: string;
  updatedAt?: string | null;
}

export interface WishlistDetailCreatePayload {
  wishlistId: number;
  productId: number;
}
