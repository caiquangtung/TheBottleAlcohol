# Quy trình áp dụng mã giảm giá trực tiếp trên sản phẩm

## 📋 Tổng quan

Tài liệu này mô tả quy trình áp dụng mã giảm giá trực tiếp trên sản phẩm trong hệ thống AlcoholWEB, bao gồm phân tích tình trạng hiện tại, thiết kế hệ thống và quy trình triển khai.

## 🔍 Phân tích hệ thống hiện tại

### ✅ Những gì đã có

#### 1. Database Schema

- **Bảng Discount** hoàn chỉnh:

  ```sql
  CREATE TABLE Discount (
      Id INT PRIMARY KEY AUTO_INCREMENT,
      Code VARCHAR(50) NOT NULL,
      Description TEXT,
      DiscountAmount DECIMAL(18,2) NOT NULL,
      MinimumOrderAmount DECIMAL(18,2) NOT NULL,
      StartDate DATETIME NOT NULL,
      EndDate DATETIME NOT NULL,
      IsActive BOOLEAN DEFAULT TRUE,
      CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
      UpdatedAt DATETIME
  );
  ```

- **Bảng DiscountProduct** (Many-to-Many):
  ```sql
  CREATE TABLE DiscountProduct (
      DiscountId INT NOT NULL,
      ProductsId INT NOT NULL,
      PRIMARY KEY (DiscountId, ProductsId),
      FOREIGN KEY (DiscountId) REFERENCES Discount(Id),
      FOREIGN KEY (ProductsId) REFERENCES Product(Id)
  );
  ```

#### 2. Entity Models

- `Discount.cs`: Model hoàn chỉnh với navigation properties
- `Product.cs`: Model sản phẩm (thiếu navigation property tới Discount)

#### 3. Repository & Service Layer

- `DiscountRepository`: CRUD operations cơ bản
- `DiscountService`: Business logic cho quản lý discount
- `ProductService`: Quản lý sản phẩm (chưa có logic discount)

#### 4. API Endpoints

- `GET /api/v1/discount`: Lấy danh sách discount
- `GET /api/v1/discount/active`: Lấy discount đang hoạt động
- `GET /api/v1/discount/code/{code}`: Lấy discount theo mã

### ❌ Những gì còn thiếu

#### 1. Navigation Properties

- `Product.cs` thiếu:
  ```csharp
  public ICollection<Discount> Discounts { get; set; }
  ```

#### 2. Business Logic

- Không có method tính giá sau discount
- Không có validation logic áp dụng discount
- Không có logic lấy sản phẩm với discount

#### 3. API Endpoints

- Thiếu endpoint lấy sản phẩm có discount
- Thiếu endpoint tính giá có discount
- Thiếu endpoint validate discount cho sản phẩm

#### 4. Frontend Integration

- Không có UI hiển thị discount trên sản phẩm
- Không có logic tính giá discount ở frontend

## 🏗️ Thiết kế hệ thống mới

### 1. Cập nhật Models

#### Product.cs

```csharp
public class Product
{
    // ... existing properties ...

    // Navigation properties
    public ICollection<Discount> Discounts { get; set; }

    public Product()
    {
        // ... existing initialization ...
        Discounts = new HashSet<Discount>();
    }
}
```

### 2. Cập nhật DTOs

#### ProductResponseDto.cs

```csharp
public class ProductResponseDto
{
    // ... existing properties ...

    public decimal OriginalPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public List<DiscountResponseDto> ActiveDiscounts { get; set; }
    public bool HasDiscount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? DiscountPercentage { get; set; }
}
```

### 3. Business Logic Services

#### IProductDiscountService.cs

```csharp
public interface IProductDiscountService
{
    Task<ProductResponseDto> GetProductWithDiscountAsync(int productId);
    Task<List<ProductResponseDto>> GetProductsWithDiscountAsync();
    Task<decimal> CalculateDiscountedPriceAsync(int productId);
    Task<List<DiscountResponseDto>> GetActiveDiscountsForProductAsync(int productId);
    Task<bool> IsDiscountApplicableAsync(int discountId, int productId);
}
```

#### ProductDiscountService.cs

```csharp
public class ProductDiscountService : IProductDiscountService
{
    public async Task<decimal> CalculateDiscountedPriceAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        var activeDiscounts = await GetActiveDiscountsForProductAsync(productId);

        if (!activeDiscounts.Any())
            return product.Price;

        // Apply best discount (highest discount amount)
        var bestDiscount = activeDiscounts.OrderByDescending(d => d.DiscountAmount).First();
        return Math.Max(0, product.Price - bestDiscount.DiscountAmount);
    }
}
```

### 4. API Endpoints mới

#### ProductController.cs

```csharp
[HttpGet("{id}/with-discount")]
public async Task<IActionResult> GetProductWithDiscount(int id)
{
    var product = await _productDiscountService.GetProductWithDiscountAsync(id);
    if (product == null)
        return NotFound();
    return Ok(new ApiResponse<ProductResponseDto>(product));
}

[HttpGet("with-discounts")]
public async Task<IActionResult> GetProductsWithDiscounts()
{
    var products = await _productDiscountService.GetProductsWithDiscountAsync();
    return Ok(new ApiResponse<List<ProductResponseDto>>(products));
}
```

## 📋 Quy trình triển khai

### Phase 1: Cập nhật Backend Core

#### Bước 1: Cập nhật Models

1. Thêm navigation property vào `Product.cs`
2. Cập nhật `MyDbContext.cs` để cấu hình relationship
3. Tạo migration nếu cần

#### Bước 2: Tạo Services

1. Tạo `IProductDiscountService` interface
2. Implement `ProductDiscountService`
3. Register service trong `Program.cs`

#### Bước 3: Cập nhật DTOs

1. Thêm discount properties vào `ProductResponseDto`
2. Cập nhật AutoMapper profiles

#### Bước 4: Cập nhật Controllers

1. Thêm endpoints mới vào `ProductController`
2. Cập nhật existing endpoints để include discount info

### Phase 2: Frontend Integration

#### Bước 1: Cập nhật Types

```typescript
// types/product.ts
export interface Product {
  // ... existing properties ...
  originalPrice: number;
  discountedPrice?: number;
  activeDiscounts: Discount[];
  hasDiscount: boolean;
  discountAmount?: number;
  discountPercentage?: number;
}
```

#### Bước 2: Cập nhật Services

```typescript
// services/productService.ts
export const productApi = createApi({
  // ... existing configuration ...
  endpoints: (builder) => ({
    // ... existing endpoints ...
    getProductWithDiscount: builder.query<Product, number>({
      query: (id) => `/product/${id}/with-discount`,
    }),
    getProductsWithDiscounts: builder.query<Product[], void>({
      query: () => "/product/with-discounts",
    }),
  }),
});
```

#### Bước 3: Cập nhật UI Components

```tsx
// components/ProductCard.tsx
const ProductCard = ({ product }: { product: Product }) => {
  return (
    <div className="product-card">
      <div className="price-section">
        {product.hasDiscount ? (
          <>
            <span className="original-price">${product.originalPrice}</span>
            <span className="discounted-price">${product.discountedPrice}</span>
            <span className="discount-badge">-${product.discountAmount}</span>
          </>
        ) : (
          <span className="price">${product.originalPrice}</span>
        )}
      </div>
    </div>
  );
};
```

### Phase 3: Advanced Features

#### Bước 1: Discount Rules Engine

1. Tạo `DiscountRulesEngine` để handle complex discount logic
2. Support multiple discount types (percentage, fixed amount, buy-x-get-y)
3. Implement discount priority system

#### Bước 2: Discount Analytics

1. Track discount usage statistics
2. A/B testing for discount effectiveness
3. ROI analysis for discount campaigns

#### Bước 3: Admin Management

1. UI để assign/unassign discounts cho products
2. Bulk discount operations
3. Discount performance dashboard

## 🔧 Implementation Details

### Database Configuration

#### MyDbContext.cs Update

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // ... existing configurations ...

    // Configure Product-Discount Many-to-Many relationship
    modelBuilder.Entity<Product>()
        .HasMany(p => p.Discounts)
        .WithMany(d => d.Products)
        .UsingEntity<Dictionary<string, object>>(
            "DiscountProduct",
            j => j.HasOne<Discount>().WithMany().HasForeignKey("DiscountId"),
            j => j.HasOne<Product>().WithMany().HasForeignKey("ProductsId"));
}
```

### Service Implementation Example

#### ProductDiscountService.cs

```csharp
public class ProductDiscountService : IProductDiscountService
{
    private readonly IProductRepository _productRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;

    public async Task<ProductResponseDto> GetProductWithDiscountAsync(int productId)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(productId);
        if (product == null) return null;

        var activeDiscounts = await GetActiveDiscountsForProductAsync(productId);
        var productDto = _mapper.Map<ProductResponseDto>(product);

        productDto.OriginalPrice = product.Price;
        productDto.ActiveDiscounts = activeDiscounts;
        productDto.HasDiscount = activeDiscounts.Any();

        if (productDto.HasDiscount)
        {
            var bestDiscount = activeDiscounts.OrderByDescending(d => d.DiscountAmount).First();
            productDto.DiscountedPrice = Math.Max(0, product.Price - bestDiscount.DiscountAmount);
            productDto.DiscountAmount = bestDiscount.DiscountAmount;
            productDto.DiscountPercentage = Math.Round((bestDiscount.DiscountAmount / product.Price) * 100, 2);
        }

        return productDto;
    }

    public async Task<List<DiscountResponseDto>> GetActiveDiscountsForProductAsync(int productId)
    {
        var now = DateTime.UtcNow;
        var discounts = await _discountRepository.GetActiveDiscountsAsync();

        var applicableDiscounts = discounts
            .Where(d => d.Products.Any(p => p.Id == productId))
            .ToList();

        return _mapper.Map<List<DiscountResponseDto>>(applicableDiscounts);
    }

    public async Task<bool> IsDiscountApplicableAsync(int discountId, int productId)
    {
        var discount = await _discountRepository.GetByIdAsync(discountId);
        if (discount == null || !discount.IsActive) return false;

        var now = DateTime.UtcNow;
        if (now < discount.StartDate || now > discount.EndDate) return false;

        return discount.Products.Any(p => p.Id == productId);
    }
}
```

## 📊 Testing Strategy

### Unit Tests

```csharp
[Test]
public async Task CalculateDiscountedPrice_WithActiveDiscount_ReturnsCorrectPrice()
{
    // Arrange
    var productId = 1;
    var originalPrice = 100m;
    var discountAmount = 20m;

    // Act
    var discountedPrice = await _productDiscountService.CalculateDiscountedPriceAsync(productId);

    // Assert
    Assert.AreEqual(80m, discountedPrice);
}
```

### Integration Tests

```csharp
[Test]
public async Task GetProductWithDiscount_ValidProduct_ReturnsProductWithDiscountInfo()
{
    // Test API endpoint returns correct discount information
}
```

## 🚀 Deployment Checklist

### Backend

- [ ] Cập nhật Models và DTOs
- [ ] Tạo và test Services
- [ ] Thêm API endpoints
- [ ] Viết unit tests
- [ ] Cập nhật documentation
- [ ] Migration database (nếu cần)

### Frontend

- [ ] Cập nhật TypeScript types
- [ ] Tạo API service calls
- [ ] Cập nhật UI components
- [ ] Test responsive design
- [ ] Cross-browser testing

### DevOps

- [ ] Deploy backend changes
- [ ] Deploy frontend changes
- [ ] Monitor performance
- [ ] Verify discount calculations
- [ ] User acceptance testing

## 🎯 Success Metrics

### Technical Metrics

- API response time < 200ms
- Zero calculation errors
- 99.9% uptime for discount features

### Business Metrics

- Discount usage rate
- Conversion rate improvement
- Average order value with discounts
- Customer satisfaction scores

## 📚 References

### Related Documents

- [Database Entities](./DatabaseEntities.md)
- [Repositories and Services](./RepositoriesAndServices.md)
- [Use Cases](./UseCases.md)

### API Documentation

- Discount API: `/api/v1/discount`
- Product API: `/api/v1/product`

### Database Schema

- Discount table structure
- DiscountProduct junction table
- Product table relationships

---

**Tác giả**: Development Team  
**Ngày tạo**: September 2025  
**Phiên bản**: 1.0  
**Trạng thái**: Draft
