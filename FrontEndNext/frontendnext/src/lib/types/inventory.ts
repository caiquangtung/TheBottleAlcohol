export interface Inventory {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  reorderLevel?: number;
  averageCost: number;
  totalValue: number;
  notes?: string;
  lastUpdated: string;
  createdAt: string;
  updatedAt?: string;
}

export interface InventoryCreateDto {
  productId: number;
  quantity: number;
  notes?: string;
}

export interface InventoryUpdateDto {
  productId: number;
  quantity: number;
  notes?: string;
}

export interface InventoryFilterDto {
  productId?: number;
  minQuantity?: number;
  maxQuantity?: number;
  minReorderLevel?: number;
  maxReorderLevel?: number;
  isLowStock?: boolean;
  startDate?: string;
  endDate?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}

export interface UpdateStockRequest {
  quantity: number;
}

export enum StockAdjustmentReason {
  Damaged = "Damaged",
  Expired = "Expired",
  Theft = "Theft",
  CountCorrection = "CountCorrection",
  QualityIssue = "QualityIssue",
  ReturnToSupplier = "ReturnToSupplier",
  Other = "Other",
}

export interface AdjustStockRequest {
  quantity: number;
  reason: StockAdjustmentReason;
  notes?: string;
}

// Inventory Transaction Types
export enum InventoryTransactionType {
  Import = "Import",
  Export = "Export",
  Adjustment = "Adjustment",
  Transfer = "Transfer",
  Return = "Return",
}

export enum InventoryTransactionStatus {
  Pending = "Pending",
  Completed = "Completed",
  Cancelled = "Cancelled",
}

export enum ReferenceType {
  ImportOrder = "ImportOrder",
  Order = "Order",
  Manual = "Manual",
}

export interface InventoryTransaction {
  id: number;
  transactionNumber: string;
  productId: number;
  productName?: string;
  transactionType: InventoryTransactionType;
  quantity: number;
  referenceType: ReferenceType;
  referenceId: number;
  status: InventoryTransactionStatus;
  notes?: string;
  transactionDate: string;
  createdAt: string;
  updatedAt?: string;
}

export interface InventoryTransactionCreateDto {
  productId: number;
  transactionType: InventoryTransactionType;
  quantity: number;
  referenceType: ReferenceType;
  referenceId: number;
  notes?: string;
}

export interface InventoryTransactionUpdateDto {
  transactionType: InventoryTransactionType;
  quantity: number;
  referenceType: ReferenceType;
  referenceId: number;
  status: InventoryTransactionStatus;
  notes?: string;
}

export interface InventoryTransactionFilterDto {
  productId?: number;
  transactionType?: InventoryTransactionType;
  status?: InventoryTransactionStatus;
  referenceType?: ReferenceType;
  startDate?: string;
  endDate?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}
