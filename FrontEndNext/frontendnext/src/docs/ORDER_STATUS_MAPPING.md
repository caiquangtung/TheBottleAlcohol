# Order Status Mapping Documentation

## Backend to Frontend Mapping

### Backend Enum (C#)

```csharp
public enum OrderStatusType
{
    Pending,    // 0 - Chờ xử lý
    Paid,       // 1 - Đã thanh toán
    Processing, // 2 - Đang xử lý
    Shipped,    // 3 - Đã gửi hàng
    Delivered,  // 4 - Đã giao hàng
    Cancelled   // 5 - Đã hủy
}
```

### Frontend Type (TypeScript)

```typescript
export type OrderStatusType =
  | "Pending" // Chờ xử lý
  | "Paid" // Đã thanh toán
  | "Processing" // Đang xử lý
  | "Shipped" // Đã gửi hàng
  | "Delivered" // Đã giao hàng
  | "Cancelled"; // Đã hủy
```

## Status Flow & Logic

### Order Lifecycle

```
Pending → Paid → Processing → Shipped → Delivered
    ↓
 Cancelled (có thể hủy ở Pending hoặc Paid)
```

### Payment Logic

- **Can Pay**: Status = "Pending" AND TotalAmount > 0
- **Cannot Pay**: Status ≠ "Pending" OR TotalAmount ≤ 0

### Status Display Configuration

| Status     | Label         | Color   | Icon | Description                         |
| ---------- | ------------- | ------- | ---- | ----------------------------------- |
| Pending    | Chờ xử lý     | Yellow  | ⏳   | Đơn hàng đang chờ được xử lý        |
| Paid       | Đã thanh toán | Green   | 💳   | Đã thanh toán thành công, chờ xử lý |
| Processing | Đang xử lý    | Blue    | ⚙️   | Đơn hàng đang được chuẩn bị         |
| Shipped    | Đã gửi hàng   | Purple  | 🚚   | Đơn hàng đang được vận chuyển       |
| Delivered  | Đã giao hàng  | Emerald | ✅   | Đơn hàng đã được giao thành công    |
| Cancelled  | Đã hủy        | Red     | ❌   | Đơn hàng đã bị hủy                  |

## Usage Examples

### Using Status Utilities

```typescript
import { canPayOrder, getOrderStatusDisplay } from "@/lib/utils/orderUtils";

// Check if order can be paid
const canPay = canPayOrder(order.status, order.totalAmount);

// Get status display config
const statusConfig = getOrderStatusDisplay(order.status);
```

### Using Status Badge Component

```tsx
import { OrderStatusBadge } from "@/components/OrderStatusBadge";

// Basic usage
<OrderStatusBadge status={order.status} />

// With description
<OrderStatusBadge status={order.status} showDescription />

// Without icon
<OrderStatusBadge status={order.status} showIcon={false} />
```

## Backend Integration

### VNPAY Payment Success Flow

When payment is successful:

1. Payment.Status = Completed
2. **Order.Status = Paid** (NEW!)
3. Cart is cleared
4. Customer can no longer pay this order

### API Response Format

```json
{
  "id": 123,
  "status": "Paid", // String representation
  "totalAmount": 500000
  // ... other fields
}
```

## Notes

- Status values are case-sensitive strings in API responses
- Frontend must handle all 6 status values
- New "Paid" status was added for better payment flow tracking
- Status progression should follow the defined lifecycle
