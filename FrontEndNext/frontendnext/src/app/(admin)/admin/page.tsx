"use client";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { enhancedApi } from "@/lib/services/api";

function formatCurrency(value: number | undefined | null) {
  if (typeof value !== "number" || isNaN(value)) return "--";
  return value.toLocaleString("vi-VN", { style: "currency", currency: "VND" });
}

export default function AdminDashboardPage() {
  const { data, isLoading, isError } =
    enhancedApi.useGetDashboardSummaryQuery();

  if (isLoading) {
    return <div>Đang tải dữ liệu...</div>;
  }

  if (isError || !data) {
    return <div>Lỗi khi tải dữ liệu dashboard.</div>;
  }

  const stats = [
    { label: "Orders", value: data.totalOrders ?? "--" },
    { label: "Revenue", value: formatCurrency(data.totalRevenue) },
    { label: "Profit", value: formatCurrency(data.totalProfit) },
    { label: "Avg Order Value", value: formatCurrency(data.averageOrderValue) },
  ];

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
      {stats.map((stat) => (
        <Card key={stat.label}>
          <CardHeader>
            <CardTitle>{stat.label}</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stat.value}</div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
