export interface ProductAnalyticsDto {
  productId: number;
  productName: string;
  totalQuantity: number;
  totalRevenue: number;
  totalProfit: number;
  currentStock: number;
}

export interface DashboardSummaryDto {
  totalOrders: number;
  totalRevenue: number;
  totalProfit: number;
  averageOrderValue: number;
  topProducts: ProductAnalyticsDto[];
}
