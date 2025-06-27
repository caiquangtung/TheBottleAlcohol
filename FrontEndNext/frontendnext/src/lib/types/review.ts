export interface Review {
  id: number;
  productId: number;
  accountId: number;
  accountName: string;
  rating: number;
  comment: string;
  createdAt: string;
  updatedAt?: string | null;
}

export interface ReviewCreate {
  productId: number;
  accountId: number;
  rating: number;
  comment: string;
}
