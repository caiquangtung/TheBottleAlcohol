# Repositories và Services Documentation

## 1. Quản lý Danh mục (Category)

### CategoryRepository

- `GetAllAsync()`: Lấy tất cả danh mục
- `GetByIdAsync(int id)`: Lấy danh mục theo ID
- `GetRootCategoriesAsync()`: Lấy danh sách danh mục gốc
- `GetSubCategoriesAsync(int parentId)`: Lấy danh sách danh mục con
- `GetCategoryWithChildrenAsync(int id)`: Lấy danh mục kèm các danh mục con
- `GetCategoryWithProductsAsync(int id)`: Lấy danh mục kèm các sản phẩm
- `HasChildrenAsync(int id)`: Kiểm tra có danh mục con không
- `HasProductsAsync(int id)`: Kiểm tra có sản phẩm không

### CategoryService

- `GetAllCategoriesAsync()`: Lấy tất cả danh mục
- `GetCategoryByIdAsync(int id)`: Lấy danh mục theo ID
- `GetRootCategoriesAsync()`: Lấy danh sách danh mục gốc
- `GetSubCategoriesAsync(int parentId)`: Lấy danh sách danh mục con
- `CreateCategoryAsync(CategoryCreateDto dto)`: Tạo danh mục mới
- `UpdateCategoryAsync(int id, CategoryUpdateDto dto)`: Cập nhật danh mục
- `DeleteCategoryAsync(int id)`: Xóa danh mục
- `MoveCategoryAsync(int id, int? newParentId)`: Di chuyển danh mục

## 2. Quản lý Thương hiệu (Brand)

### BrandRepository

- `GetAllAsync()`: Lấy tất cả thương hiệu
- `GetByIdAsync(int id)`: Lấy thương hiệu theo ID
- `GetActiveBrandsAsync()`: Lấy danh sách thương hiệu đang hoạt động
- `GetBrandWithProductsAsync(int id)`: Lấy thương hiệu kèm các sản phẩm
- `HasProductsAsync(int id)`: Kiểm tra có sản phẩm không

### BrandService

- `GetAllBrandsAsync()`: Lấy tất cả thương hiệu
- `GetBrandByIdAsync(int id)`: Lấy thương hiệu theo ID
- `GetActiveBrandsAsync()`: Lấy danh sách thương hiệu đang hoạt động
- `CreateBrandAsync(BrandCreateDto dto)`: Tạo thương hiệu mới
- `UpdateBrandAsync(int id, BrandUpdateDto dto)`: Cập nhật thương hiệu
- `DeleteBrandAsync(int id)`: Xóa thương hiệu
- `ToggleBrandStatusAsync(int id)`: Bật/tắt trạng thái thương hiệu

## 3. Quản lý Nhà cung cấp (Supplier)

### SupplierRepository

- `GetAllAsync()`: Lấy tất cả nhà cung cấp
- `GetByIdAsync(int id)`: Lấy nhà cung cấp theo ID
- `GetActiveSuppliersAsync()`: Lấy danh sách nhà cung cấp đang hoạt động
- `GetSupplierWithImportOrdersAsync(int id)`: Lấy nhà cung cấp kèm các đơn nhập hàng
- `HasImportOrdersAsync(int id)`: Kiểm tra có đơn nhập hàng không

### SupplierService

- `GetAllSuppliersAsync()`: Lấy tất cả nhà cung cấp
- `GetSupplierByIdAsync(int id)`: Lấy nhà cung cấp theo ID
- `GetActiveSuppliersAsync()`: Lấy danh sách nhà cung cấp đang hoạt động
- `CreateSupplierAsync(SupplierCreateDto dto)`: Tạo nhà cung cấp mới
- `UpdateSupplierAsync(int id, SupplierUpdateDto dto)`: Cập nhật nhà cung cấp
- `DeleteSupplierAsync(int id)`: Xóa nhà cung cấp
- `ToggleSupplierStatusAsync(int id)`: Bật/tắt trạng thái nhà cung cấp

## 4. Quản lý Mã giảm giá (Discount)

### DiscountRepository

- `GetAllAsync()`: Lấy tất cả mã giảm giá
- `GetByIdAsync(int id)`: Lấy mã giảm giá theo ID
- `GetByCodeAsync(string code)`: Lấy mã giảm giá theo code
- `GetActiveDiscountsAsync()`: Lấy danh sách mã giảm giá đang hoạt động
- `GetExpiredDiscountsAsync()`: Lấy danh sách mã giảm giá đã hết hạn
- `IsCodeUniqueAsync(string code)`: Kiểm tra code có duy nhất không

### DiscountService

- `GetAllDiscountsAsync()`: Lấy tất cả mã giảm giá
- `GetDiscountByIdAsync(int id)`: Lấy mã giảm giá theo ID
- `GetDiscountByCodeAsync(string code)`: Lấy mã giảm giá theo code
- `GetActiveDiscountsAsync()`: Lấy danh sách mã giảm giá đang hoạt động
- `GetExpiredDiscountsAsync()`: Lấy danh sách mã giảm giá đã hết hạn
- `CreateDiscountAsync(DiscountCreateDto dto)`: Tạo mã giảm giá mới
- `UpdateDiscountAsync(int id, DiscountUpdateDto dto)`: Cập nhật mã giảm giá
- `DeleteDiscountAsync(int id)`: Xóa mã giảm giá
- `ToggleDiscountStatusAsync(int id)`: Bật/tắt trạng thái mã giảm giá
- `ApplyDiscountAsync(string code, decimal totalAmount)`: Áp dụng mã giảm giá

## 5. Quản lý Thông báo (Notification)

### NotificationRepository

- `GetAllAsync()`: Lấy tất cả thông báo
- `GetByIdAsync(int id)`: Lấy thông báo theo ID
- `GetUserNotificationsAsync(int userId)`: Lấy danh sách thông báo của user
- `GetUnreadNotificationsAsync(int userId)`: Lấy danh sách thông báo chưa đọc
- `MarkAsReadAsync(int id)`: Đánh dấu thông báo đã đọc
- `MarkAllAsReadAsync(int userId)`: Đánh dấu tất cả thông báo đã đọc

### NotificationService

- `GetAllNotificationsAsync()`: Lấy tất cả thông báo
- `GetNotificationByIdAsync(int id)`: Lấy thông báo theo ID
- `GetUserNotificationsAsync(int userId)`: Lấy danh sách thông báo của user
- `GetUnreadNotificationsAsync(int userId)`: Lấy danh sách thông báo chưa đọc
- `CreateNotificationAsync(NotificationCreateDto dto)`: Tạo thông báo mới
- `MarkAsReadAsync(int id)`: Đánh dấu thông báo đã đọc
- `MarkAllAsReadAsync(int userId)`: Đánh dấu tất cả thông báo đã đọc
- `DeleteNotificationAsync(int id)`: Xóa thông báo
- `SendNotificationAsync(int userId, string title, string content)`: Gửi thông báo

## 6. Quản lý Đơn hàng (Order)

### OrderRepository

- `GetAllAsync()`: Lấy tất cả đơn hàng
- `GetByIdAsync(int id)`: Lấy đơn hàng theo ID
- `GetOrdersByStatusAsync(OrderStatusType status)`: Lấy đơn hàng theo trạng thái
- `GetUserOrdersAsync(int userId)`: Lấy đơn hàng của user
- `GetOrderWithDetailsAsync(int id)`: Lấy đơn hàng kèm chi tiết
- `UpdateOrderStatusAsync(int id, OrderStatusType status)`: Cập nhật trạng thái đơn hàng

### OrderService

- `GetAllOrdersAsync()`: Lấy tất cả đơn hàng
- `GetOrderByIdAsync(int id)`: Lấy đơn hàng theo ID
- `GetOrdersByStatusAsync(OrderStatusType status)`: Lấy đơn hàng theo trạng thái
- `GetUserOrdersAsync(int userId)`: Lấy đơn hàng của user
- `CreateOrderAsync(OrderCreateDto dto)`: Tạo đơn hàng mới
- `UpdateOrderAsync(int id, OrderUpdateDto dto)`: Cập nhật đơn hàng
- `DeleteOrderAsync(int id)`: Xóa đơn hàng
- `UpdateOrderStatusAsync(int id, OrderStatusType status)`: Cập nhật trạng thái đơn hàng
- `ProcessPaymentAsync(int id, PaymentInfo paymentInfo)`: Xử lý thanh toán

## 7. Quản lý Đơn nhập hàng (ImportOrder)

### ImportOrderRepository

- `GetAllAsync()`: Lấy tất cả đơn nhập hàng
- `GetByIdAsync(int id)`: Lấy đơn nhập hàng theo ID
- `GetImportOrdersByStatusAsync(ImportOrderStatusType status)`: Lấy đơn nhập hàng theo trạng thái
- `GetImportOrderWithDetailsAsync(int id)`: Lấy đơn nhập hàng kèm chi tiết
- `UpdateImportOrderStatusAsync(int id, ImportOrderStatusType status)`: Cập nhật trạng thái đơn nhập hàng

### ImportOrderService

- `GetAllImportOrdersAsync()`: Lấy tất cả đơn nhập hàng
- `GetImportOrderByIdAsync(int id)`: Lấy đơn nhập hàng theo ID
- `GetImportOrdersByStatusAsync(ImportOrderStatusType status)`: Lấy đơn nhập hàng theo trạng thái
- `CreateImportOrderAsync(ImportOrderCreateDto dto)`: Tạo đơn nhập hàng mới
- `UpdateImportOrderAsync(int id, ImportOrderUpdateDto dto)`: Cập nhật đơn nhập hàng
- `DeleteImportOrderAsync(int id)`: Xóa đơn nhập hàng
- `UpdateImportOrderStatusAsync(int id, ImportOrderStatusType status)`: Cập nhật trạng thái đơn nhập hàng
- `ProcessImportOrderAsync(int id)`: Xử lý đơn nhập hàng

## 8. Quản lý Sản phẩm (Product)

### ProductRepository

- `GetAllAsync()`: Lấy tất cả sản phẩm
- `GetByIdAsync(int id)`: Lấy sản phẩm theo ID
- `GetProductsByCategoryAsync(int categoryId)`: Lấy sản phẩm theo danh mục
- `GetProductsByBrandAsync(int brandId)`: Lấy sản phẩm theo thương hiệu
- `GetActiveProductsAsync()`: Lấy danh sách sản phẩm đang bán
- `SearchProductsAsync(string keyword)`: Tìm kiếm sản phẩm
- `UpdateStockAsync(int id, int quantity)`: Cập nhật số lượng tồn kho

### ProductService

- `GetAllProductsAsync()`: Lấy tất cả sản phẩm
- `GetProductByIdAsync(int id)`: Lấy sản phẩm theo ID
- `GetProductsByCategoryAsync(int categoryId)`: Lấy sản phẩm theo danh mục
- `GetProductsByBrandAsync(int brandId)`: Lấy sản phẩm theo thương hiệu
- `GetActiveProductsAsync()`: Lấy danh sách sản phẩm đang bán
- `SearchProductsAsync(string keyword)`: Tìm kiếm sản phẩm
- `CreateProductAsync(ProductCreateDto dto)`: Tạo sản phẩm mới
- `UpdateProductAsync(int id, ProductUpdateDto dto)`: Cập nhật sản phẩm
- `DeleteProductAsync(int id)`: Xóa sản phẩm
- `ToggleProductStatusAsync(int id)`: Bật/tắt trạng thái sản phẩm
- `UpdateStockAsync(int id, int quantity)`: Cập nhật số lượng tồn kho

## 9. Quản lý Tài khoản (Account)

### AccountRepository

- `GetAllAsync()`: Lấy tất cả tài khoản
- `GetByIdAsync(int id)`: Lấy tài khoản theo ID
- `GetByEmailAsync(string email)`: Lấy tài khoản theo email
- `GetByPhoneAsync(string phone)`: Lấy tài khoản theo số điện thoại
- `GetActiveAccountsAsync()`: Lấy danh sách tài khoản đang hoạt động
- `UpdatePasswordAsync(int id, string newPassword)`: Cập nhật mật khẩu

### AccountService

- `GetAllAccountsAsync()`: Lấy tất cả tài khoản
- `GetAccountByIdAsync(int id)`: Lấy tài khoản theo ID
- `GetAccountByEmailAsync(string email)`: Lấy tài khoản theo email
- `GetAccountByPhoneAsync(string phone)`: Lấy tài khoản theo số điện thoại
- `GetActiveAccountsAsync()`: Lấy danh sách tài khoản đang hoạt động
- `RegisterAsync(RegisterDto dto)`: Đăng ký tài khoản mới
- `LoginAsync(LoginDto dto)`: Đăng nhập
- `UpdateAccountAsync(int id, AccountUpdateDto dto)`: Cập nhật thông tin tài khoản
- `ChangePasswordAsync(int id, ChangePasswordDto dto)`: Đổi mật khẩu
- `ToggleAccountStatusAsync(int id)`: Bật/tắt trạng thái tài khoản
- `ForgotPasswordAsync(string email)`: Quên mật khẩu
- `ResetPasswordAsync(string token, string newPassword)`: Đặt lại mật khẩu

## 10. Quản lý Giỏ hàng (Cart)

### CartRepository

- `GetAllAsync()`: Lấy tất cả giỏ hàng
- `GetByIdAsync(int id)`: Lấy giỏ hàng theo ID
- `GetUserCartAsync(int userId)`: Lấy giỏ hàng của user
- `GetCartWithDetailsAsync(int id)`: Lấy giỏ hàng kèm chi tiết
- `ClearCartAsync(int id)`: Xóa tất cả sản phẩm trong giỏ hàng

### CartService

- `GetAllCartsAsync()`: Lấy tất cả giỏ hàng
- `GetCartByIdAsync(int id)`: Lấy giỏ hàng theo ID
- `GetUserCartAsync(int userId)`: Lấy giỏ hàng của user
- `AddToCartAsync(int userId, CartItemDto dto)`: Thêm sản phẩm vào giỏ hàng
- `UpdateCartItemAsync(int userId, int productId, int quantity)`: Cập nhật số lượng sản phẩm
- `RemoveFromCartAsync(int userId, int productId)`: Xóa sản phẩm khỏi giỏ hàng
- `ClearCartAsync(int userId)`: Xóa tất cả sản phẩm trong giỏ hàng
- `GetCartTotalAsync(int userId)`: Tính tổng tiền giỏ hàng

## 11. Quản lý Đánh giá (Review)

### ReviewRepository

- `GetAllAsync()`: Lấy tất cả đánh giá
- `GetByIdAsync(int id)`: Lấy đánh giá theo ID
- `GetProductReviewsAsync(int productId)`: Lấy đánh giá của sản phẩm
- `GetUserReviewsAsync(int userId)`: Lấy đánh giá của user
- `GetAverageRatingAsync(int productId)`: Lấy điểm đánh giá trung bình

### ReviewService

- `GetAllReviewsAsync()`: Lấy tất cả đánh giá
- `GetReviewByIdAsync(int id)`: Lấy đánh giá theo ID
- `GetProductReviewsAsync(int productId)`: Lấy đánh giá của sản phẩm
- `GetUserReviewsAsync(int userId)`: Lấy đánh giá của user
- `CreateReviewAsync(ReviewCreateDto dto)`: Tạo đánh giá mới
- `UpdateReviewAsync(int id, ReviewUpdateDto dto)`: Cập nhật đánh giá
- `DeleteReviewAsync(int id)`: Xóa đánh giá
- `GetAverageRatingAsync(int productId)`: Lấy điểm đánh giá trung bình

## 12. Quản lý Thống kê (Statistics)

### StatisticsRepository

- `GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)`: Lấy doanh số theo khoảng thời gian
- `GetTopSellingProductsAsync(int limit)`: Lấy sản phẩm bán chạy nhất
- `GetRevenueByCategoryAsync(DateTime startDate, DateTime endDate)`: Lấy doanh thu theo danh mục
- `GetInventoryStatusAsync()`: Lấy trạng thái tồn kho

### StatisticsService

- `GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)`: Lấy doanh số theo khoảng thời gian
- `GetTopSellingProductsAsync(int limit)`: Lấy sản phẩm bán chạy nhất
- `GetRevenueByCategoryAsync(DateTime startDate, DateTime endDate)`: Lấy doanh thu theo danh mục
- `GetInventoryStatusAsync()`: Lấy trạng thái tồn kho
- `GetDashboardDataAsync()`: Lấy dữ liệu cho dashboard
- `ExportSalesReportAsync(DateTime startDate, DateTime endDate)`: Xuất báo cáo doanh số
- `ExportInventoryReportAsync()`: Xuất báo cáo tồn kho
