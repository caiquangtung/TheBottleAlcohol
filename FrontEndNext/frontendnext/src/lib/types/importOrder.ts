// Import Order Types
export enum ImportOrderStatus {
  Pending = "Pending",
  Approved = "Approved",
  Completed = "Completed",
  Cancelled = "Cancelled",
}

export interface ImportOrder {
  id: number;
  supplierId: number;
  supplierName: string;
  managerId: number;
  managerName: string;
  totalAmount: number;
  status: ImportOrderStatus;
  notes?: string;
  importDate: string;
  createdAt: string;
  updatedAt?: string;
  importOrderDetails: ImportOrderDetail[];
}

export interface ImportOrderDetail {
  id: number;
  importOrderId: number;
  productId: number;
  productName: string;
  quantity: number;
  importPrice: number;
  totalAmount: number;
  status: ImportOrderStatus;
  createdAt: string;
  updatedAt?: string;
}

export interface ImportOrderCreateDto {
  supplierId: number;
  managerId: number;
  notes?: string;
  importDate: string;
  importOrderDetails: ImportOrderDetailCreateDto[];
}

export interface ImportOrderDetailCreateDto {
  productId: number;
  quantity: number;
  importPrice: number;
  totalAmount: number;
}

export interface ImportOrderUpdateDto {
  importDate: string;
  totalAmount: number;
  profit: number;
  notes?: string;
  status: ImportOrderStatus;
}

export interface ImportOrderFilterDto {
  supplierId?: number;
  status?: ImportOrderStatus;
  startDate?: string;
  endDate?: string;
  minTotal?: number;
  maxTotal?: number;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}

export interface ImportOrderStats {
  totalOrders: number;
  pendingOrders: number;
  approvedOrders: number;
  completedOrders: number;
  cancelledOrders: number;
  totalValue: number;
}

// Action DTOs
export interface ApproveImportOrderRequest {
  notes?: string;
}

export interface CompleteImportOrderRequest {
  notes?: string;
}

export interface CancelImportOrderRequest {
  reason: string;
}
