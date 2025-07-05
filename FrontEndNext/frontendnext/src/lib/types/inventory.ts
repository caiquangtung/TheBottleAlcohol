export interface Inventory {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  reorderLevel: number;
  location: string;
  notes: string;
  lastUpdated: string;
  createdAt: string;
  updatedAt: string;
}

export interface InventoryCreateDto {
  productId: number;
  quantity: number;
  reorderLevel: number;
  location: string;
  notes: string;
}

export interface InventoryUpdateDto {
  productId: number;
  quantity: number;
  reorderLevel: number;
  location: string;
  notes: string;
}
