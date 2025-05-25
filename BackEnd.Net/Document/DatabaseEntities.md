# Database Entities Documentation

## Enum Types

### OrderStatusType

- Pending: Chờ xử lý
- Processing: Đang xử lý
- Shipped: Đã gửi hàng
- Delivered: Đã giao hàng
- Cancelled: Đã hủy

### PaymentMethodType

- Cash: Tiền mặt
- CreditCard: Thẻ tín dụng
- BankTransfer: Chuyển khoản
- EWallet: Ví điện tử

### PaymentStatusType

- Pending: Chờ thanh toán
- Completed: Đã thanh toán
- Failed: Thanh toán thất bại
- Refunded: Đã hoàn tiền

### ShippingMethodType

- Standard: Giao hàng tiêu chuẩn
- Express: Giao hàng nhanh
- Pickup: Nhận tại cửa hàng

### ShippingStatusType

- Pending: Chờ xử lý
- Processing: Đang xử lý
- InTransit: Đang vận chuyển
- Delivered: Đã giao hàng
- Failed: Giao hàng thất bại

### GenderType

- Male: Nam
- Female: Nữ
- Other: Khác

### RoleType

- Admin: Quản trị viên
- Staff: Nhân viên
- Customer: Khách hàng

### InventoryTransactionType

- Import: Nhập kho
- Export: Xuất kho
- Adjustment: Điều chỉnh

### InventoryTransactionStatusType

- Pending: Chờ xử lý
- Completed: Hoàn thành
- Failed: Thất bại

### ImportOrderStatusType

- Pending: Chờ xử lý
- Processing: Đang xử lý
- Completed: Hoàn thành
- Cancelled: Đã hủy

### ReferenceType

- Order: Đơn hàng
- Import: Nhập kho
- Adjustment: Điều chỉnh

## Account

- **Description**: Thông tin tài khoản người dùng
- **Properties**:
  - `Id` (int, PK): Mã tài khoản
  - `FullName` (string, 255): Họ và tên
  - `DateOfBirth` (DateTime): Ngày sinh
  - `Address` (string, 500): Địa chỉ
  - `Gender` (GenderType): Giới tính
  - `PhoneNumber` (string, 20): Số điện thoại
  - `Email` (string, 255): Email
  - `Role` (RoleType): Vai trò
  - `Password` (string, 255): Mật khẩu
  - `Status` (bool): Trạng thái
  - `CreatedAt` (DateTime): Ngày tạo
  - `UpdatedAt` (DateTime?): Ngày cập nhật
- **Relationships**:
  - Has many `Orders`
  - Has one `Cart`
  - Has one `Wishlist`
  - Has many `Reviews`
  - Has many `Notifications`
  - Has many `Payments`

## Product

- **Description**: Thông tin sản phẩm
- **Properties**:
  - `Id` (int, PK): Mã sản phẩm
  - `Name` (string, 100): Tên sản phẩm
  - `Description` (string, 500): Mô tả
  - `Slug` (string, 100): URL thân thiện
  - `Origin` (string, 100): Xuất xứ
  - `Volume` (decimal): Dung tích
  - `AlcoholContent` (decimal): Nồng độ cồn
  - `Price` (decimal): Giá
  - `StockQuantity` (int): Số lượng tồn kho
  - `Status` (bool): Trạng thái
  - `ImageUrl` (string, 500): URL hình ảnh
  - `Age` (int?): Độ tuổi (cho whisky)
  - `Flavor` (string, 100): Hương vị (cho rum)
  - `SalesCount` (int): Số lượng đã bán
  - `MetaTitle` (string, 200): Tiêu đề SEO
  - `MetaDescription` (string, 500): Mô tả SEO
  - `CreatedAt` (DateTime): Ngày tạo
  - `UpdatedAt` (DateTime?): Ngày cập nhật
- **Relationships**:
  - Belongs to `Category`
  - Belongs to `Brand`
  - Has many `OrderDetails`
  - Has many `ImportOrderDetails`
  - Has one `Inventory`
  - Has many `CartDetails`
  - Has many `WishlistDetails`
  - Has many `Reviews`
  - Has many `RecipeIngredients`

## Category

- **Description**: Danh mục sản phẩm
- **Properties**:
  - `Id` (int, PK): Mã danh mục
  - `Name` (string, 100): Tên danh mục
  - `Description` (string, 500): Mô tả
  - `Slug` (string, 100): URL thân thiện
  - `DisplayOrder` (int): Thứ tự hiển thị
  - `IsActive` (bool): Trạng thái hoạt động
  - `ImageUrl` (string, 500): URL hình ảnh
  - `CreatedAt` (DateTime): Ngày tạo
  - `ParentId` (int?): Mã danh mục cha
- **Relationships**:
  - Has many `Products`
  - Has one `Parent` (self-referencing)
  - Has many `Children` (self-referencing)

## Order

- **Description**: Đơn hàng
- **Properties**:
  - `Id` (int, PK): Mã đơn hàng
  - `AccountId` (int): Mã khách hàng
  - `OrderDate` (DateTime): Ngày đặt hàng
  - `TotalAmount` (decimal): Tổng tiền
  - `PaymentMethod` (PaymentMethodType): Phương thức thanh toán
  - `ShippingMethod` (ShippingMethodType): Phương thức vận chuyển
  - `ShippingAddress` (string): Địa chỉ giao hàng
  - `ShippingPhone` (string): Số điện thoại giao hàng
  - `ShippingName` (string): Tên người nhận
  - `Note` (string): Ghi chú
  - `CreatedAt` (DateTime): Ngày tạo
  - `UpdatedAt` (DateTime?): Ngày cập nhật
- **Relationships**:
  - Belongs to `Account`
  - Has many `OrderDetails`
  - Has many `OrderStatuses`
  - Has one `Payment`
  - Has one `Shipping`

## OrderDetail

- **Description**: Chi tiết đơn hàng
- **Properties**:
  - `OrderId` (int): Mã đơn hàng
  - `ProductId` (int): Mã sản phẩm
  - `UnitPrice` (decimal): Đơn giá
  - `Quantity` (int): Số lượng
  - `TotalAmount` (decimal): Thành tiền
- **Relationships**:
  - Belongs to `Order`
  - Belongs to `Product`

## Cart

- **Description**: Giỏ hàng
- **Properties**:
  - `Id` (int, PK): Mã giỏ hàng
  - `CustomerId` (int): Mã khách hàng
  - `CreatedAt` (DateTime): Ngày tạo
  - `UpdatedAt` (DateTime?): Ngày cập nhật
- **Relationships**:
  - Belongs to `Account` (Customer)
  - Has many `CartDetails`

## CartDetail

- **Description**: Chi tiết giỏ hàng
- **Properties**:
  - `Id` (int, PK): Mã chi tiết
  - `CartId` (int): Mã giỏ hàng
  - `ProductId` (int): Mã sản phẩm
  - `Quantity` (int): Số lượng
- **Relationships**:
  - Belongs to `Cart`
  - Belongs to `Product`

## Brand

- **Description**: Thương hiệu
- **Properties**:
  - `Id` (int, PK): Mã thương hiệu
  - `Name` (string, 100): Tên thương hiệu
  - `Description` (string, 500): Mô tả
  - `Slug` (string, 100): URL thân thiện
  - `LogoUrl` (string, 500): URL logo
  - `Website` (string, 200): Website
  - `DisplayOrder` (int): Thứ tự hiển thị
  - `IsActive` (bool): Trạng thái hoạt động
  - `MetaTitle` (string, 200): Tiêu đề SEO
  - `MetaDescription` (string, 500): Mô tả SEO
  - `CreatedAt` (DateTime): Ngày tạo
- **Relationships**:
  - Has many `Products`

## Recipe

- **Description**: Công thức pha chế
- **Properties**:
  - `Id` (int, PK): Mã công thức
  - `Name` (string, 100): Tên công thức
  - `Description` (string, 500): Mô tả
  - `Slug` (string, 100): URL thân thiện
  - `ImageUrl` (string, 500): URL hình ảnh
  - `Instructions` (string): Hướng dẫn
  - `Difficulty` (string, 50): Độ khó
  - `PreparationTime` (int): Thời gian chuẩn bị
  - `Servings` (int): Số phần
  - `DisplayOrder` (int): Thứ tự hiển thị
  - `IsActive` (bool): Trạng thái hoạt động
  - `MetaTitle` (string, 200): Tiêu đề SEO
  - `MetaDescription` (string, 500): Mô tả SEO
  - `CreatedAt` (DateTime): Ngày tạo
  - `UpdatedAt` (DateTime?): Ngày cập nhật
  - `CategoryId` (int?): Mã danh mục
- **Relationships**:
  - Belongs to `Category`
  - Has many `Ingredients`

## RecipeIngredient

- **Description**: Nguyên liệu công thức
- **Properties**:
  - `Id` (int, PK): Mã nguyên liệu
  - `RecipeId` (int): Mã công thức
  - `ProductId` (int?): Mã sản phẩm
  - `Name` (string, 100): Tên nguyên liệu
  - `Quantity` (decimal): Số lượng
  - `Unit` (string, 50): Đơn vị
  - `Notes` (string): Ghi chú
- **Relationships**:
  - Belongs to `Recipe`
  - Belongs to `Product` (optional)

## RecipeCategory

- **Description**: Danh mục công thức
- **Properties**:
  - `RecipeId` (int): Mã công thức
  - `CategoryId` (int): Mã danh mục
- **Relationships**:
  - Belongs to `Recipe`
  - Belongs to `Category`

## Inventory

- **Description**: Kho hàng
- **Properties**:
  - `Id` (int, PK): Mã kho
  - `ProductId` (int): Mã sản phẩm
  - `Quantity` (int): Số lượng
- **Relationships**:
  - Belongs to `Product`

## InventoryTransaction

- **Description**: Giao dịch kho
- **Properties**:
  - `Id` (int, PK): Mã giao dịch
  - `ProductId` (int): Mã sản phẩm
  - `Quantity` (int): Số lượng
  - `Type` (InventoryTransactionType): Loại giao dịch
  - `ReferenceType` (ReferenceType): Loại tham chiếu
  - `ReferenceId` (int): Mã tham chiếu
  - `Status` (InventoryTransactionStatusType): Trạng thái
  - `CreatedAt` (DateTime): Ngày tạo
- **Relationships**:
  - Belongs to `Product`

## OrderStatus

- **Description**: Trạng thái đơn hàng
- **Properties**:
  - `Id` (int, PK): Mã trạng thái
  - `OrderId` (int): Mã đơn hàng
  - `Status` (OrderStatusType): Trạng thái
  - `UpdateDate` (DateTime): Ngày cập nhật
- **Relationships**:
  - Belongs to `Order`

## Payment

- **Description**: Thanh toán
- **Properties**:
  - `Id` (int, PK): Mã thanh toán
  - `OrderId` (int): Mã đơn hàng
  - `AccountId` (int): Mã tài khoản
  - `Amount` (decimal): Số tiền
  - `Status` (PaymentStatusType): Trạng thái
  - `CreatedAt` (DateTime): Ngày tạo
  - `UpdatedAt` (DateTime?): Ngày cập nhật
- **Relationships**:
  - Belongs to `Order`
  - Belongs to `Account`

## Discount

- **Description**: Mã giảm giá
- **Properties**:
  - `Id` (int, PK): Mã giảm giá
  - `Code` (string, 50): Mã code
  - `DiscountAmount` (decimal): Số tiền giảm
  - `StartDate` (DateTime): Ngày bắt đầu
  - `EndDate` (DateTime): Ngày kết thúc
  - `IsActive` (bool): Trạng thái hoạt động
- **Relationships**:
  - Many-to-many with `Products`

## Notification

- **Description**: Thông báo
- **Properties**:
  - `Id` (int, PK): Mã thông báo
  - `UserId` (int): Mã người dùng
  - `Title` (string, 255): Tiêu đề
  - `Content` (string): Nội dung
  - `IsRead` (bool): Đã đọc
  - `CreatedAt` (DateTime): Ngày tạo
  - `ReadAt` (DateTime?): Ngày đọc
- **Relationships**:
  - Belongs to `Account` (User)

## Shipping

- **Description**: Vận chuyển
- **Properties**:
  - `Id` (int, PK): Mã vận chuyển
  - `OrderId` (int): Mã đơn hàng
  - `ShippingMethod` (ShippingMethodType): Phương thức vận chuyển
  - `Status` (ShippingStatusType): Trạng thái
  - `TrackingNumber` (string): Mã theo dõi
  - `EstimatedDeliveryDate` (DateTime): Ngày giao hàng dự kiến
  - `ActualDeliveryDate` (DateTime?): Ngày giao hàng thực tế
- **Relationships**:
  - Belongs to `Order`

## Wishlist

- **Description**: Danh sách yêu thích
- **Properties**:
  - `Id` (int, PK): Mã danh sách
  - `CustomerId` (int): Mã khách hàng
  - `CreatedAt` (DateTime): Ngày tạo
- **Relationships**:
  - Belongs to `Account` (Customer)
  - Has many `WishlistDetails`

## WishlistDetail

- **Description**: Chi tiết danh sách yêu thích
- **Properties**:
  - `Id` (int, PK): Mã chi tiết
  - `WishlistId` (int): Mã danh sách
  - `ProductId` (int): Mã sản phẩm
- **Relationships**:
  - Belongs to `Wishlist`
  - Belongs to `Product`

## Review

- **Description**: Đánh giá sản phẩm
- **Properties**:
  - `Id` (int, PK): Mã đánh giá
  - `ProductId` (int): Mã sản phẩm
  - `CustomerId` (int): Mã khách hàng
  - `Rating` (int): Đánh giá (1-5)
  - `Comment` (string): Bình luận
  - `CreatedAt` (DateTime): Ngày tạo
- **Relationships**:
  - Belongs to `Product`
  - Belongs to `Account` (Customer)

## ImportOrder

- **Description**: Đơn nhập hàng
- **Properties**:
  - `Id` (int, PK): Mã đơn nhập
  - `ImportDate` (DateTime): Ngày nhập
  - `TotalAmount` (decimal): Tổng tiền
  - `Profit` (decimal): Lợi nhuận
  - `SupplierId` (int): Mã nhà cung cấp
  - `ManagerId` (int): Mã người quản lý
- **Relationships**:
  - Belongs to `Supplier`
  - Belongs to `Account` (Manager)
  - Has many `ImportOrderDetails`

## ImportOrderDetail

- **Description**: Chi tiết đơn nhập hàng
- **Properties**:
  - `ImportOrderId` (int): Mã đơn nhập
  - `ProductId` (int): Mã sản phẩm
  - `Quantity` (int): Số lượng
  - `ImportPrice` (decimal): Giá nhập
  - `TotalAmount` (decimal): Thành tiền
  - `Status` (ImportOrderStatusType): Trạng thái
- **Relationships**:
  - Belongs to `ImportOrder`
  - Belongs to `Product`

## Supplier

- **Description**: Nhà cung cấp
- **Properties**:
  - `Id` (int, PK): Mã nhà cung cấp
  - `Name` (string): Tên nhà cung cấp
  - `PhoneNumber` (string, 20): Số điện thoại
  - `Email` (string): Email
- **Relationships**:
  - Has many `ImportOrders`
