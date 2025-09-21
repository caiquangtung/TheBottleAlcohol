# Cập nhật Hệ thống Quản lý Kho - Inventory Management Update

## Tổng quan

Hệ thống quản lý kho đã được cập nhật để khắc phục các vấn đề sau:

1. ✅ Tính toán AverageCost tự động khi nhập hàng
2. ✅ Cập nhật TotalValue = AverageCost × Quantity
3. ✅ Đồng bộ Product.StockQuantity với Inventory.Quantity
4. ✅ Workflow hoàn thành import order và cập nhật kho tự động

## Các Tính năng Mới

### 1. Import Order Workflow

#### Quy trình mới:

```
Pending → Approved → Completed
     ↘         ↗
       Cancelled
```

#### API Endpoints:

- `PATCH /api/v1/importorder/{id}/approve` - Duyệt đơn nhập hàng
- `PATCH /api/v1/importorder/{id}/complete` - Hoàn thành nhập hàng (tự động cập nhật kho)
- `PATCH /api/v1/importorder/{id}/cancel` - Hủy đơn nhập hàng
- `GET /api/v1/importorder/stats` - Thống kê đơn nhập hàng

### 2. Inventory Service Cải tiến

#### Phương thức mới:

- `ImportStockWithCostAsync()` - Nhập hàng với tính toán AverageCost tự động
- `RecalculateAllTotalValuesAsync()` - Tính lại TotalValue cho tất cả inventory
- `GetTotalInventoryValueAsync()` - Lấy tổng giá trị kho
- `GetLowStockItemsAsync()` - Lấy danh sách hàng sắp hết

#### API Endpoints mới:

- `PATCH /api/v1/inventory/{id}/stock` - Cập nhật số lượng tồn kho
- `PATCH /api/v1/inventory/product/{productId}/adjust` - Điều chỉnh tồn kho
- `POST /api/v1/inventory/recalculate-values` - Tính lại tất cả TotalValue
- `GET /api/v1/inventory/total-value` - Lấy tổng giá trị kho
- `GET /api/v1/inventory/low-stock` - Lấy danh sách hàng sắp hết

### 3. Data Migration Service

#### Chức năng:

- Tạo inventory record cho sản phẩm chưa có
- Tính lại AverageCost dựa trên lịch sử nhập hàng
- Đồng bộ Product.StockQuantity với Inventory.Quantity

#### API Endpoints:

- `GET /api/v1/datamigration/report` - Báo cáo trạng thái dữ liệu
- `POST /api/v1/datamigration/recalculate-average-costs` - Tính lại AverageCost
- `POST /api/v1/datamigration/sync-stock-quantities` - Đồng bộ số lượng
- `POST /api/v1/datamigration/create-missing-inventories` - Tạo inventory thiếu
- `POST /api/v1/datamigration/run-all` - Chạy tất cả migration

## Cách sử dụng

### 1. Quy trình nhập hàng mới:

```bash
# 1. Tạo Import Order
POST /api/v1/importorder
{
  "supplierId": 1,
  "managerId": 1,
  "importDate": "2024-01-01",
  "importOrderDetails": [
    {
      "productId": 1,
      "quantity": 100,
      "importPrice": 50000,
      "totalAmount": 5000000
    }
  ]
}

# 2. Duyệt đơn nhập hàng
PATCH /api/v1/importorder/1/approve

# 3. Hoàn thành nhập hàng (tự động cập nhật kho)
PATCH /api/v1/importorder/1/complete
```

### 2. Kiểm tra và migration dữ liệu:

```bash
# 1. Kiểm tra báo cáo
GET /api/v1/datamigration/report

# 2. Chạy migration (nếu cần)
POST /api/v1/datamigration/run-all
```

### 3. Quản lý inventory:

```bash
# Xem tổng giá trị kho
GET /api/v1/inventory/total-value

# Xem hàng sắp hết
GET /api/v1/inventory/low-stock

# Điều chỉnh tồn kho
PATCH /api/v1/inventory/product/1/adjust
{
  "quantity": -5,
  "notes": "Damaged goods"
}
```

## Công thức tính toán

### 1. AverageCost (Giá vốn trung bình):

```
AverageCost = (CurrentTotalValue + ImportValue) / (CurrentQuantity + ImportQuantity)

Trong đó:
- CurrentTotalValue = AverageCost hiện tại × Quantity hiện tại
- ImportValue = ImportPrice × ImportQuantity
```

### 2. TotalValue (Tổng giá trị):

```
TotalValue = AverageCost × Quantity
```

### 3. Profit (Lợi nhuận dự kiến):

```
Profit = (SellPrice - AverageCost) × Quantity
```

## Ví dụ tính toán

### Trường hợp: Nhập hàng nhiều lần với giá khác nhau

**Lần 1:** Nhập 100 chai với giá 50,000đ/chai

- AverageCost = 50,000đ
- TotalValue = 50,000 × 100 = 5,000,000đ

**Lần 2:** Nhập thêm 50 chai với giá 60,000đ/chai

- CurrentTotalValue = 50,000 × 100 = 5,000,000đ
- ImportValue = 60,000 × 50 = 3,000,000đ
- NewAverageCost = (5,000,000 + 3,000,000) / (100 + 50) = 53,333đ
- NewTotalValue = 53,333 × 150 = 8,000,000đ

## Lưu ý quan trọng

1. **Backup dữ liệu** trước khi chạy migration
2. **Chỉ Admin** mới có quyền chạy migration
3. **Kiểm tra báo cáo** trước khi migration
4. **Import Order** phải được duyệt trước khi hoàn thành
5. **AverageCost** chỉ được tính từ các import order đã hoàn thành

## Troubleshooting

### Vấn đề: AverageCost = 0

**Nguyên nhân:** Chưa có import order nào hoàn thành
**Giải pháp:** Hoàn thành ít nhất 1 import order hoặc set AverageCost thủ công

### Vấn đề: Product.StockQuantity ≠ Inventory.Quantity

**Nguyên nhân:** Dữ liệu không đồng bộ
**Giải pháp:** Chạy `POST /api/v1/datamigration/sync-stock-quantities`

### Vấn đề: Sản phẩm không có inventory record

**Nguyên nhân:** Sản phẩm được tạo trước khi có inventory system
**Giải pháp:** Chạy `POST /api/v1/datamigration/create-missing-inventories`
