// Export all types

export * from "./brand";
export * from "./cart";
export * from "./category";
export * from "./discount";
export * from "./inventory";
export * from "./notification";
export * from "./product";
export * from "./recipe";
export * from "./review";
export * from "./statistics";
export * from "./supplier";
export * from "./user";
export * from "./wishlist";
export * from "./order";
export * from "./importOrder";

// Common types
export interface PagedResult<T> {
  items: T[];
  totalRecords: number;
  totalItems: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  sortOrder?: "asc" | "desc";
}
