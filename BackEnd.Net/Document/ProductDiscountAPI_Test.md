# Product Discount API Testing Guide

## 🧪 API Endpoints Testing

### 1. Get Product with Discount

```http
GET /api/v1/product/{id}/with-discount
```

**Example:**

```bash
curl -X GET "http://localhost:5000/api/v1/product/1/with-discount" \
     -H "Content-Type: application/json"
```

**Expected Response:**

```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Product Name",
    "price": 100.0,
    "originalPrice": 100.0,
    "discountedPrice": 80.0,
    "hasDiscount": true,
    "discountAmount": 20.0,
    "discountPercentage": 20.0,
    "activeDiscounts": [
      {
        "id": 1,
        "code": "SAVE20",
        "discountAmount": 20.0,
        "startDate": "2025-01-01T00:00:00Z",
        "endDate": "2025-12-31T23:59:59Z",
        "isActive": true
      }
    ]
  }
}
```

### 2. Get All Products with Discounts

```http
GET /api/v1/product/with-discounts
```

**Example:**

```bash
curl -X GET "http://localhost:5000/api/v1/product/with-discounts" \
     -H "Content-Type: application/json"
```

### 3. Get Discounted Price Only

```http
GET /api/v1/product/{id}/discount-price
```

**Example:**

```bash
curl -X GET "http://localhost:5000/api/v1/product/1/discount-price" \
     -H "Content-Type: application/json"
```

**Expected Response:**

```json
{
  "success": true,
  "data": 80.0
}
```

### 4. Get Active Discounts for Product

```http
GET /api/v1/product/{id}/active-discounts
```

**Example:**

```bash
curl -X GET "http://localhost:5000/api/v1/product/1/active-discounts" \
     -H "Content-Type: application/json"
```

## 🔧 Testing Scenarios

### Scenario 1: Product with Active Discount

1. Create a discount with valid date range
2. Assign discount to a product
3. Call `/api/v1/product/{id}/with-discount`
4. Verify discount is applied correctly

### Scenario 2: Product without Discount

1. Use a product without any assigned discounts
2. Call `/api/v1/product/{id}/with-discount`
3. Verify `hasDiscount` is false and `discountedPrice` is null

### Scenario 3: Product with Expired Discount

1. Create a discount with past end date
2. Assign to product
3. Call `/api/v1/product/{id}/with-discount`
4. Verify expired discount is not applied

### Scenario 4: Product with Multiple Discounts

1. Create multiple active discounts
2. Assign all to same product
3. Verify best discount (highest amount) is applied

## 🎯 Test Data Setup

### Sample Discount Data

```sql
INSERT INTO Discount (Code, Description, DiscountAmount, MinimumOrderAmount, StartDate, EndDate, IsActive, CreatedAt)
VALUES
('SAVE20', 'Save $20 on selected products', 20.00, 0.00, '2025-01-01', '2025-12-31', 1, NOW()),
('SAVE50', 'Save $50 on premium products', 50.00, 100.00, '2025-01-01', '2025-12-31', 1, NOW()),
('EXPIRED10', 'Expired $10 discount', 10.00, 0.00, '2024-01-01', '2024-12-31', 1, NOW());
```

### Link Discount to Product

```sql
INSERT INTO DiscountProduct (DiscountId, ProductsId)
VALUES
(1, 1),  -- SAVE20 for Product 1
(2, 1),  -- SAVE50 for Product 1 (should pick this one - higher amount)
(3, 2);  -- EXPIRED10 for Product 2 (should not apply)
```

## ✅ Validation Checklist

### Backend Validation

- [ ] ProductDiscountService is registered in DI
- [ ] Product model has Discounts navigation property
- [ ] ProductResponseDto includes discount fields
- [ ] AutoMapper configuration is correct
- [ ] API endpoints return correct status codes
- [ ] Discount calculation logic is accurate
- [ ] Expired discounts are filtered out
- [ ] Best discount is selected when multiple exist

### Frontend Integration

- [ ] TypeScript types are updated
- [ ] API service calls work correctly
- [ ] Product components display discount info
- [ ] Price formatting shows original and discounted prices
- [ ] Discount badges/labels are visible

### Error Handling

- [ ] Invalid product ID returns 404
- [ ] Database connection issues handled gracefully
- [ ] Null reference exceptions prevented
- [ ] Logging works for debugging

## 🚀 Performance Considerations

### Database Queries

- Monitor N+1 query issues when loading products with discounts
- Consider using `.Include()` for eager loading
- Index on Discount.StartDate, EndDate, IsActive for performance

### Caching Strategy

- Cache active discounts for better performance
- Invalidate cache when discounts are updated
- Consider Redis for distributed caching

## 📊 Monitoring & Analytics

### Metrics to Track

- API response times for discount endpoints
- Discount application success rate
- Most used discount codes
- Revenue impact of discounts

### Logging

```csharp
_logger.LogInformation("Applied discount {DiscountCode} to Product {ProductId}: {OriginalPrice} -> {DiscountedPrice}",
    discountCode, productId, originalPrice, discountedPrice);
```

---

**Note**: Make sure to test all endpoints thoroughly before deploying to production. Consider using automated tests for regression testing.
