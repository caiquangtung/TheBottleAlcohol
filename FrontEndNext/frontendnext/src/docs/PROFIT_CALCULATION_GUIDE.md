# Manual Profit Calculation Guide

## 📊 Tổng quan

Hệ thống tính toán profit cho từng sản phẩm dựa trên **AverageCost** từ inventory và **SellingPrice** từ product catalog. Đây là hướng dẫn chi tiết về cách tính manual profit.

## 🧮 Công thức tính Profit

### **1. Profit Per Unit (Lợi nhuận trên đơn vị)**
```
ProfitPerUnit = SellingPrice - AverageCost
```

### **2. Profit Margin Percentage (Tỷ lệ lợi nhuận)**
```
ProfitMarginPercentage = (ProfitPerUnit / SellingPrice) × 100
```

### **3. Total Potential Profit (Tổng lợi nhuận tiềm năng)**
```
PotentialTotalProfit = ProfitPerUnit × CurrentStock
```

### **4. Recommended Selling Price (Giá bán đề xuất)**
```
RecommendedPrice = AverageCost / (1 - TargetMargin%)
```

## 📋 Ví dụ tính toán chi tiết

### **Scenario 1: Sản phẩm mới**

#### **Dữ liệu đầu vào:**
```typescript
Product: {
  id: 1,
  name: "Hennessy VSOP",
  price: 2500000,           // Giá bán hiện tại
  targetMarginPercentage: 35.0  // Target margin mong muốn
}

Inventory: {
  productId: 1,
  quantity: 50,
  averageCost: 1800000,     // Từ Import Orders
  totalValue: 90000000      // 50 × 1,800,000
}
```

#### **Tính toán:**
```typescript
// 1. Profit per unit
ProfitPerUnit = 2,500,000 - 1,800,000 = 700,000đ

// 2. Current margin percentage  
CurrentMargin = (700,000 / 2,500,000) × 100 = 28%

// 3. Total potential profit
TotalProfit = 700,000 × 50 = 35,000,000đ

// 4. Recommended price for 35% margin
RecommendedPrice = 1,800,000 / (1 - 0.35) = 2,769,230đ

// 5. Analysis result
IsMarginAchieved = 28% >= 35% = false ❌
```

### **Scenario 2: Multiple Import Orders**

#### **Import History:**
```typescript
ImportOrder1: {
  quantity: 30,
  importPrice: 1700000,
  totalAmount: 51000000
}

ImportOrder2: {
  quantity: 20, 
  importPrice: 1950000,
  totalAmount: 39000000
}

// Weighted Average Cost calculation:
AverageCost = (51,000,000 + 39,000,000) / (30 + 20)
            = 90,000,000 / 50
            = 1,800,000đ
```

#### **Profit với giá bán khác nhau:**
```typescript
// Option A: Giá bán 2,200,000đ
ProfitPerUnit = 2,200,000 - 1,800,000 = 400,000đ
MarginPercentage = (400,000 / 2,200,000) × 100 = 18.18%

// Option B: Giá bán 2,500,000đ  
ProfitPerUnit = 2,500,000 - 1,800,000 = 700,000đ
MarginPercentage = (700,000 / 2,500,000) × 100 = 28%

// Option C: Recommended price for 35% margin
RecommendedPrice = 1,800,000 / (1 - 0.35) = 2,769,230đ
ProfitPerUnit = 2,769,230 - 1,800,000 = 969,230đ
MarginPercentage = 35% ✅
```

## 🎯 Frontend Implementation

### **1. Profit Analysis Types**
```typescript
// src/lib/types/profit.ts
export interface ProfitAnalysis {
  productId: number;
  productName: string;
  averageCost: number;
  sellingPrice: number;
  profitPerUnit: number;
  profitMarginPercentage: number;
  targetMarginPercentage: number;
  recommendedSellingPrice: number;
  isMarginAchieved: boolean;
  currentStock: number;
  potentialTotalProfit: number;
}

export interface ProfitCalculationInput {
  averageCost: number;
  sellingPrice?: number;
  targetMarginPercentage?: number;
  currentStock?: number;
}
```

### **2. Manual Calculation Utility**
```typescript
// src/lib/utils/profitCalculator.ts
export class ProfitCalculator {
  static calculateProfitPerUnit(sellingPrice: number, averageCost: number): number {
    return sellingPrice - averageCost;
  }

  static calculateMarginPercentage(sellingPrice: number, averageCost: number): number {
    if (sellingPrice <= 0) return 0;
    const profit = this.calculateProfitPerUnit(sellingPrice, averageCost);
    return (profit / sellingPrice) * 100;
  }

  static calculateRecommendedPrice(averageCost: number, targetMarginPercentage: number): number {
    if (targetMarginPercentage >= 100 || targetMarginPercentage < 0) {
      throw new Error("Target margin must be between 0 and 100");
    }
    return averageCost / (1 - (targetMarginPercentage / 100));
  }

  static calculateTotalProfit(profitPerUnit: number, quantity: number): number {
    return profitPerUnit * quantity;
  }

  static analyzeProduct(input: ProfitCalculationInput): ProfitAnalysis {
    const { averageCost, sellingPrice = 0, targetMarginPercentage = 30, currentStock = 0 } = input;
    
    const profitPerUnit = this.calculateProfitPerUnit(sellingPrice, averageCost);
    const marginPercentage = this.calculateMarginPercentage(sellingPrice, averageCost);
    const recommendedPrice = this.calculateRecommendedPrice(averageCost, targetMarginPercentage);
    const totalProfit = this.calculateTotalProfit(profitPerUnit, currentStock);
    
    return {
      averageCost,
      sellingPrice,
      profitPerUnit,
      profitMarginPercentage: Math.round(marginPercentage * 100) / 100,
      targetMarginPercentage,
      recommendedSellingPrice: Math.round(recommendedPrice),
      isMarginAchieved: marginPercentage >= targetMarginPercentage,
      currentStock,
      potentialTotalProfit: totalProfit
    };
  }
}
```

### **3. Profit Analysis Component**
```typescript
// src/components/inventory/ProfitAnalysisCard.tsx
export function ProfitAnalysisCard({ inventory }: { inventory: Inventory }) {
  const [targetMargin, setTargetMargin] = useState(30);
  const [customPrice, setCustomPrice] = useState(0);

  // Get product info for current selling price
  const { data: product } = useGetProductByIdQuery(inventory.productId);
  
  const currentAnalysis = useMemo(() => {
    if (!product) return null;
    
    return ProfitCalculator.analyzeProduct({
      averageCost: inventory.averageCost,
      sellingPrice: product.price,
      targetMarginPercentage: product.targetMarginPercentage || 30,
      currentStock: inventory.quantity
    });
  }, [inventory, product]);

  const customAnalysis = useMemo(() => {
    if (!customPrice || customPrice <= 0) return null;
    
    return ProfitCalculator.analyzeProduct({
      averageCost: inventory.averageCost,
      sellingPrice: customPrice,
      targetMarginPercentage: targetMargin,
      currentStock: inventory.quantity
    });
  }, [inventory.averageCost, customPrice, targetMargin, inventory.quantity]);

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <TrendingUp className="h-5 w-5" />
          Profit Analysis - {inventory.productName}
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        {/* Current Profit Analysis */}
        {currentAnalysis && (
          <div className="space-y-4">
            <h4 className="font-semibold">Current Pricing Analysis</h4>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div>
                <p className="text-sm text-muted-foreground">Average Cost</p>
                <p className="text-lg font-semibold">{formatCurrency(currentAnalysis.averageCost)}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Selling Price</p>
                <p className="text-lg font-semibold">{formatCurrency(currentAnalysis.sellingPrice)}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Profit/Unit</p>
                <p className={`text-lg font-semibold ${currentAnalysis.profitPerUnit >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                  {formatCurrency(currentAnalysis.profitPerUnit)}
                </p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Margin</p>
                <div className="flex items-center gap-2">
                  <p className={`text-lg font-semibold ${currentAnalysis.isMarginAchieved ? 'text-green-600' : 'text-orange-600'}`}>
                    {currentAnalysis.profitMarginPercentage.toFixed(2)}%
                  </p>
                  {currentAnalysis.isMarginAchieved ? (
                    <CheckCircle className="h-4 w-4 text-green-600" />
                  ) : (
                    <AlertTriangle className="h-4 w-4 text-orange-600" />
                  )}
                </div>
              </div>
            </div>
            
            <div className="grid grid-cols-2 gap-4">
              <div>
                <p className="text-sm text-muted-foreground">Target Margin</p>
                <p className="text-lg font-semibold">{currentAnalysis.targetMarginPercentage}%</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Recommended Price</p>
                <p className="text-lg font-semibold text-blue-600">
                  {formatCurrency(currentAnalysis.recommendedSellingPrice)}
                </p>
              </div>
            </div>

            <div className="bg-muted/50 p-4 rounded-lg">
              <p className="text-sm font-medium mb-2">Total Profit Potential</p>
              <div className="flex justify-between items-center">
                <span className="text-sm">Stock: {currentAnalysis.currentStock} units</span>
                <span className="text-lg font-bold text-green-600">
                  {formatCurrency(currentAnalysis.potentialTotalProfit)}
                </span>
              </div>
            </div>
          </div>
        )}

        {/* Custom Price Calculator */}
        <div className="space-y-4 border-t pt-4">
          <h4 className="font-semibold">Manual Profit Calculator</h4>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label htmlFor="customPrice">Test Selling Price</Label>
              <Input
                id="customPrice"
                type="number"
                min="0"
                step="1000"
                placeholder="Enter test price"
                value={customPrice}
                onChange={(e) => setCustomPrice(parseFloat(e.target.value) || 0)}
              />
            </div>
            <div>
              <Label htmlFor="targetMargin">Target Margin %</Label>
              <Input
                id="targetMargin"
                type="number"
                min="0"
                max="100"
                step="1"
                value={targetMargin}
                onChange={(e) => setTargetMargin(parseFloat(e.target.value) || 30)}
              />
            </div>
          </div>

          {customAnalysis && (
            <div className="bg-blue-50 dark:bg-blue-950/20 p-4 rounded-lg space-y-2">
              <p className="font-medium text-blue-800 dark:text-blue-200">Calculation Results:</p>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="text-muted-foreground">Profit per Unit:</span>
                  <span className={`ml-2 font-semibold ${customAnalysis.profitPerUnit >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                    {formatCurrency(customAnalysis.profitPerUnit)}
                  </span>
                </div>
                <div>
                  <span className="text-muted-foreground">Margin:</span>
                  <span className={`ml-2 font-semibold ${customAnalysis.isMarginAchieved ? 'text-green-600' : 'text-orange-600'}`}>
                    {customAnalysis.profitMarginPercentage.toFixed(2)}%
                  </span>
                </div>
                <div>
                  <span className="text-muted-foreground">Total Profit:</span>
                  <span className="ml-2 font-semibold text-green-600">
                    {formatCurrency(customAnalysis.potentialTotalProfit)}
                  </span>
                </div>
                <div>
                  <span className="text-muted-foreground">Target Achievement:</span>
                  <span className={`ml-2 font-semibold ${customAnalysis.isMarginAchieved ? 'text-green-600' : 'text-red-600'}`}>
                    {customAnalysis.isMarginAchieved ? '✅ Achieved' : '❌ Below Target'}
                  </span>
                </div>
              </div>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
```

## 📊 Ví dụ thực tế

### **Case Study: Hennessy VSOP**

#### **Import History:**
```
Import 1: 20 chai × 1,700,000đ = 34,000,000đ
Import 2: 30 chai × 1,850,000đ = 55,500,000đ

Weighted Average Cost = (34,000,000 + 55,500,000) / (20 + 30)
                      = 89,500,000 / 50  
                      = 1,790,000đ
```

#### **Profit Analysis với giá bán khác nhau:**

| Selling Price | Profit/Unit | Margin % | Total Profit (50 units) | Target 35% ? |
|---------------|-------------|----------|-------------------------|--------------|
| 2,200,000đ    | 410,000đ    | 18.64%   | 20,500,000đ            | ❌ Below     |
| 2,500,000đ    | 710,000đ    | 28.40%   | 35,500,000đ            | ❌ Below     |
| 2,753,846đ    | 963,846đ    | 35.00%   | 48,192,300đ            | ✅ Target    |
| 3,000,000đ    | 1,210,000đ  | 40.33%   | 60,500,000đ            | ✅ Above     |

#### **Price Recommendation:**
```typescript
// Để đạt 35% margin:
RecommendedPrice = 1,790,000 / (1 - 0.35) = 2,753,846đ

// Rounded to: 2,754,000đ
// Actual margin: 35.01% ✅
```

## 🔧 Manual Calculation Steps

### **Step 1: Get AverageCost từ Inventory**
```typescript
// Từ completed Import Orders:
const inventory = await getInventoryByProduct(productId);
const averageCost = inventory.averageCost; // Đã tính weighted average
```

### **Step 2: Determine Selling Price**
```typescript
// Current price từ Product:
const product = await getProductById(productId);
const currentPrice = product.price;

// Hoặc test với custom price:
const testPrice = 2500000; // Manual input
```

### **Step 3: Calculate Profit Metrics**
```typescript
const profitPerUnit = sellingPrice - averageCost;
const marginPercentage = (profitPerUnit / sellingPrice) * 100;
const totalProfit = profitPerUnit * currentStock;
```

### **Step 4: Compare với Target**
```typescript
const targetMargin = product.targetMarginPercentage || 30;
const isMarginAchieved = marginPercentage >= targetMargin;
const recommendedPrice = averageCost / (1 - (targetMargin / 100));
```

## 📈 Advanced Scenarios

### **1. Multiple Product Variants**
```typescript
// Cùng brand, khác volume:
Hennessy VSOP 700ml: AverageCost = 1,800,000đ → Price = 2,754,000đ
Hennessy VSOP 1000ml: AverageCost = 2,400,000đ → Price = 3,692,000đ

// Maintain consistent 35% margin across variants
```

### **2. Seasonal Pricing**
```typescript
// Peak season (Tết):
RegularPrice = 2,754,000đ (35% margin)
PeakPrice = 3,100,000đ (42.3% margin) 
ExtraProfit = 346,000đ per unit

// Off-season promotion:
PromoPrice = 2,400,000đ (25.2% margin)
LostProfit = 354,000đ per unit
```

### **3. Bulk Pricing Strategy**
```typescript
// Volume-based pricing:
1-5 units: 2,754,000đ (35% margin)
6-10 units: 2,616,000đ (31.4% margin) - 5% discount
11+ units: 2,478,000đ (27.7% margin) - 10% discount

// Balance between volume sales và margin maintenance
```

## 🎯 Best Practices

### **1. Regular Margin Review**
- **Monthly**: Review margin performance cho all products
- **Quarterly**: Adjust target margins based on market conditions
- **Yearly**: Comprehensive profit analysis và strategy adjustment

### **2. Price Optimization**
- **Cost-Plus Pricing**: AverageCost + Target Margin
- **Market Competitive**: Compare với competitor pricing
- **Value-Based**: Premium pricing cho high-value products

### **3. Inventory Valuation**
- **FIFO/Weighted Average**: Sử dụng weighted average từ import orders
- **Regular Recalculation**: Chạy recalculate values khi cần
- **Audit Compliance**: Maintain detailed cost tracking

## 🔍 Monitoring & Alerts

### **1. Low Margin Alerts**
```typescript
// Products with margin < target:
const lowMarginProducts = inventory.filter(item => {
  const margin = calculateMarginPercentage(item.sellingPrice, item.averageCost);
  return margin < item.targetMarginPercentage;
});
```

### **2. High-Value Opportunities**
```typescript
// Products with high profit potential:
const highValueProducts = inventory
  .map(item => ({
    ...item,
    totalProfitPotential: calculateTotalProfit(item.profitPerUnit, item.quantity)
  }))
  .sort((a, b) => b.totalProfitPotential - a.totalProfitPotential);
```

### **3. Cost Trend Analysis**
```typescript
// Track AverageCost changes over time:
const costTrends = importOrders
  .filter(order => order.status === 'Completed')
  .map(order => ({
    date: order.completedDate,
    averageCost: order.calculatedAverageCost,
    change: calculateCostChange(previousCost, currentCost)
  }));
```

## 💡 Key Takeaways

### **1. Dependency on Import Orders**
- **AverageCost accuracy** phụ thuộc vào proper Import Order workflow
- **Manual cost input** không được phép để maintain integrity
- **Weighted average** ensures accurate costing với multiple suppliers

### **2. Real-time Profit Tracking**
- **Dynamic calculation** based on current inventory costs
- **Target margin comparison** để optimize pricing
- **Total profit potential** để prioritize high-value products

### **3. Integration với Business Strategy**
- **Pricing decisions** based on accurate cost data
- **Margin optimization** thông qua proper workflow
- **Profit maximization** với data-driven insights

Frontend inventory management cung cấp đầy đủ tools để calculate và optimize profit cho từng sản phẩm một cách chính xác và hiệu quả.
