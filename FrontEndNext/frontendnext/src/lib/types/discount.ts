export interface Discount {
  id: number;
  code: string;
  name: string;
  description: string;
  discountType: number; // 0: Percentage, 1: Fixed Amount
  discountValue: number;
  minimumOrderAmount: number;
  maximumDiscountAmount: number;
  startDate: string;
  endDate: string;
  usageLimit: number;
  usedCount: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface DiscountCreateDto {
  code: string;
  name: string;
  description: string;
  discountType: number;
  discountValue: number;
  minimumOrderAmount: number;
  maximumDiscountAmount: number;
  startDate: string;
  endDate: string;
  usageLimit: number;
  isActive: boolean;
}

export interface DiscountUpdateDto {
  code: string;
  name: string;
  description: string;
  discountType: number;
  discountValue: number;
  minimumOrderAmount: number;
  maximumDiscountAmount: number;
  startDate: string;
  endDate: string;
  usageLimit: number;
  isActive: boolean;
}
