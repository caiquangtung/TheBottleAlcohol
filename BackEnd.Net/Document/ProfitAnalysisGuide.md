# Hướng dẫn Phân tích Lợi nhuận - Profit Analysis Guide

## 🎯 **Tổng quan**

Hệ thống Profit Analysis giúp quản lý và tối ưu hóa lợi nhuận từng sản phẩm thông qua:

- **Tính toán margin tự động** dựa trên AverageCost và SellingPrice
- **Đặt target margin** cho từng sản phẩm
- **Đề xuất giá bán** để đạt target margin
- **Phân tích và báo cáo** hiệu suất lợi nhuận

## 💰 **Công thức tính toán**

### **1. Profit Margin (%)**

```
Profit Margin = ((Selling Price - Average Cost) / Selling Price) × 100
```

**Ví dụ:**

- Average Cost: 50,000đ
- Selling Price: 75,000đ
- **Profit Margin = ((75,000 - 50,000) / 75,000) × 100 = 33.33%**

### **2. Recommended Selling Price**

```
Recommended Price = Average Cost / (1 - Target Margin%)
```

**Ví dụ:**

- Average Cost: 50,000đ
- Target Margin: 40%
- **Recommended Price = 50,000 / (1 - 0.4) = 83,333đ**

### **3. Profit Per Unit**

```
Profit Per Unit = Selling Price - Average Cost
```

### **4. Total Potential Profit**

```
Total Potential Profit = Profit Per Unit × Current Stock
```

## 🛠 **API Endpoints**

### **1. Phân tích sản phẩm đơn lẻ**

```http
GET /api/v1/profitanalysis/product/{productId}
```

**Response:**

```json
{
  "productId": 1,
  "productName": "Hennessy VSOP",
  "averageCost": 1200000,
  "sellingPrice": 1800000,
  "profitPerUnit": 600000,
  "profitMarginPercentage": 33.33,
  "targetMarginPercentage": 30.0,
  "recommendedSellingPrice": 1714286,
  "isMarginAchieved": true,
  "currentStock": 50,
  "potentialTotalProfit": 30000000
}
```

### **2. Phân tích tất cả sản phẩm**

```http
GET /api/v1/profitanalysis/all
```

### **3. Báo cáo tổng quan**

```http
GET /api/v1/profitanalysis/summary
```

**Response:**

```json
{
  "totalInventoryValue": 150000000,
  "totalPotentialRevenue": 225000000,
  "totalPotentialProfit": 75000000,
  "averageMarginPercentage": 28.5,
  "productsAboveTargetMargin": 45,
  "productsBelowTargetMargin": 15,
  "topProfitableProducts": [...],
  "lowProfitProducts": [...]
}
```

### **4. Sản phẩm lợi nhuận thấp**

```http
GET /api/v1/profitanalysis/low-profit?threshold=15
```

### **5. Sản phẩm hiệu suất cao**

```http
GET /api/v1/profitanalysis/high-performing
```

### **6. Đặt target margin**

```http
PATCH /api/v1/profitanalysis/product/{productId}/target-margin
Content-Type: application/json

{
  "targetMarginPercentage": 35.0
}
```

### **7. Tính giá bán đề xuất**

```http
GET /api/v1/profitanalysis/product/{productId}/recommended-price?targetMargin=40
```

### **8. Cập nhật giá theo target margin**

```http
PATCH /api/v1/profitanalysis/product/{productId}/update-price-by-margin
Content-Type: application/json

{
  "targetMarginPercentage": 35.0
}
```

## 📊 **Kịch bản sử dụng thực tế**

### **Scenario 1: Đánh giá hiệu suất sản phẩm**

```bash
# 1. Xem báo cáo tổng quan
GET /api/v1/profitanalysis/summary

# 2. Tìm sản phẩm lợi nhuận thấp
GET /api/v1/profitanalysis/low-profit?threshold=20

# 3. Phân tích chi tiết sản phẩm có vấn đề
GET /api/v1/profitanalysis/product/123
```

### **Scenario 2: Tối ưu giá bán**

```bash
# 1. Xem giá đề xuất cho target 35%
GET /api/v1/profitanalysis/product/123/recommended-price?targetMargin=35

# 2. Cập nhật target margin
PATCH /api/v1/profitanalysis/product/123/target-margin
{
  "targetMarginPercentage": 35.0
}

# 3. Cập nhật giá bán theo target
PATCH /api/v1/profitanalysis/product/123/update-price-by-margin
{
  "targetMarginPercentage": 35.0
}
```

### **Scenario 3: Monitoring định kỳ**

```bash
# Hàng tuần: Kiểm tra sản phẩm hiệu suất cao
GET /api/v1/profitanalysis/high-performing

# Hàng tháng: Báo cáo tổng quan
GET /api/v1/profitanalysis/summary

# Khi cần: Điều chỉnh target margin theo thị trường
```

## 🎯 **Best Practices**

### **1. Target Margin Setting**

- **Luxury products**: 40-60%
- **Premium products**: 30-40%
- **Standard products**: 20-30%
- **Promotional products**: 10-20%

### **2. Monitoring Frequency**

- **Daily**: Sản phẩm hot/new
- **Weekly**: Top performers
- **Monthly**: Full portfolio review
- **Quarterly**: Strategy adjustment

### **3. Action Thresholds**

- **< 10% margin**: Immediate action required
- **10-15% margin**: Review pricing/costs
- **15-25% margin**: Monitor closely
- **> 25% margin**: Healthy products

## ⚡ **Dashboard Metrics đề xuất**

### **KPI Cards:**

```
📈 Average Margin: 28.5%
💰 Total Potential Profit: 75M VND
📊 Products Above Target: 45/60
⚠️  Low Profit Products: 5
```

### **Charts:**

1. **Margin Distribution** - Histogram
2. **Top 10 Profitable Products** - Bar chart
3. **Margin Trend** - Line chart over time
4. **Cost vs Price Scatter** - Bubble chart

## 🚨 **Alerts & Notifications**

### **Tự động cảnh báo khi:**

- Margin < 10%
- AverageCost tăng đột biến
- Competitor pricing thay đổi
- Inventory aging (high stock, low turnover)

## 💡 **Advanced Features**

### **1. Dynamic Pricing**

```csharp
// Tự động điều chỉnh giá theo:
// - Demand (high demand = higher margin)
// - Inventory level (overstocked = lower margin)
// - Seasonality (peak season = higher margin)
```

### **2. Competitor Analysis**

```csharp
// So sánh với giá thị trường
// Đề xuất pricing strategy
// Alert khi competitor thay đổi giá
```

### **3. ABC Analysis**

```csharp
// A products: High margin + High volume
// B products: Medium margin/volume
// C products: Low margin/volume
```

## 📈 **ROI của Profit Analysis System**

### **Before (Không có system):**

- ❌ Pricing dựa trên cảm tính
- ❌ Không biết sản phẩm nào profitable
- ❌ Margin không consistent
- ❌ Miss opportunities

### **After (Có system):**

- ✅ Data-driven pricing decisions
- ✅ Identify profitable products
- ✅ Consistent margin management
- ✅ Maximize profit opportunities

### **Expected Impact:**

- 📈 **15-25% increase** in overall margin
- 🎯 **Better product mix** decisions
- ⚡ **Faster pricing** responses
- 💰 **Higher profitability**

## 🔧 **Technical Notes**

### **Performance Considerations:**

- Cache profit calculations for frequently accessed products
- Background jobs for daily margin updates
- Indexes on Product.TargetMarginPercentage

### **Data Quality:**

- Ensure AverageCost is updated from import orders
- Validate margin calculations regularly
- Handle edge cases (zero costs, negative margins)

## 🎉 **Kết luận**

Profit Analysis System là **game changer** cho business:

1. **🎯 Strategic**: Data-driven pricing decisions
2. **💰 Financial**: Maximize profitability
3. **⚡ Operational**: Efficient margin management
4. **📊 Analytical**: Deep business insights

**ROI Timeline:**

- **Month 1**: Setup và training
- **Month 2-3**: Initial optimizations
- **Month 4+**: Full benefits realization

Đây là công cụ **must-have** cho bất kỳ business nào muốn optimize profits! 🚀
