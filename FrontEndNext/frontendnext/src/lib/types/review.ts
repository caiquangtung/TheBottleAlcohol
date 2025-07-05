export interface Review {
  id: number;
  productId: number;
  accountId: number;
  accountName: string;
  rating: number;
  comment: string;
  isApproved?: boolean | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface ReviewCreate {
  productId: number;
  accountId: number;
  rating: number;
  comment: string;
}

export interface ReviewCreateDto {
  productId: number;
  accountId: number;
  rating: number;
  comment: string;
}

export interface ReviewUpdateDto {
  productId: number;
  accountId: number;
  rating: number;
  comment: string;
  isApproved?: boolean;
}
