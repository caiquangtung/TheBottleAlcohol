# Use Cases Documentation

## 1. Quản lý Tài khoản

### 1.1. Đăng ký tài khoản

- **Actor**: Khách hàng
- **Mô tả**: Khách hàng đăng ký tài khoản mới
- **Luồng chính**:
  1. Khách hàng nhập thông tin cá nhân
  2. Hệ thống kiểm tra tính hợp lệ của thông tin
  3. Hệ thống tạo tài khoản mới
  4. Gửi email xác nhận
- **Điều kiện tiên quyết**: Chưa có tài khoản
- **Kết quả**: Tài khoản mới được tạo

### 1.2. Đăng nhập

- **Actor**: Khách hàng, Admin, Staff
- **Mô tả**: Người dùng đăng nhập vào hệ thống
- **Luồng chính**:
  1. Nhập email và mật khẩu
  2. Hệ thống xác thực thông tin
  3. Cấp quyền truy cập theo vai trò
- **Điều kiện tiên quyết**: Đã có tài khoản
- **Kết quả**: Đăng nhập thành công

### 1.3. Quản lý thông tin cá nhân

- **Actor**: Khách hàng
- **Mô tả**: Cập nhật thông tin cá nhân
- **Luồng chính**:
  1. Xem thông tin hiện tại
  2. Cập nhật thông tin mới
  3. Lưu thay đổi
- **Kết quả**: Thông tin được cập nhật

## 2. Quản lý Sản phẩm

### 2.1. Xem danh sách sản phẩm

- **Actor**: Khách hàng
- **Mô tả**: Xem danh sách sản phẩm với bộ lọc
- **Luồng chính**:
  1. Chọn danh mục
  2. Áp dụng bộ lọc (giá, độ tuổi, hương vị)
  3. Xem kết quả
- **Kết quả**: Hiển thị danh sách sản phẩm phù hợp

### 2.2. Tìm kiếm sản phẩm

- **Actor**: Khách hàng
- **Mô tả**: Tìm kiếm sản phẩm theo từ khóa
- **Luồng chính**:
  1. Nhập từ khóa tìm kiếm
  2. Hệ thống tìm kiếm theo tên, mô tả
  3. Hiển thị kết quả
- **Kết quả**: Danh sách sản phẩm phù hợp

### 2.3. Xem chi tiết sản phẩm

- **Actor**: Khách hàng
- **Mô tả**: Xem thông tin chi tiết sản phẩm
- **Luồng chính**:
  1. Chọn sản phẩm
  2. Xem thông tin chi tiết
  3. Xem đánh giá và bình luận
- **Kết quả**: Hiển thị thông tin chi tiết sản phẩm

## 3. Quản lý Giỏ hàng

### 3.1. Thêm vào giỏ hàng

- **Actor**: Khách hàng
- **Mô tả**: Thêm sản phẩm vào giỏ hàng
- **Luồng chính**:
  1. Chọn sản phẩm
  2. Chọn số lượng
  3. Thêm vào giỏ hàng
- **Kết quả**: Sản phẩm được thêm vào giỏ hàng

### 3.2. Cập nhật giỏ hàng

- **Actor**: Khách hàng
- **Mô tả**: Cập nhật số lượng sản phẩm trong giỏ hàng
- **Luồng chính**:
  1. Xem giỏ hàng
  2. Thay đổi số lượng
  3. Cập nhật
- **Kết quả**: Giỏ hàng được cập nhật

### 3.3. Xóa khỏi giỏ hàng

- **Actor**: Khách hàng
- **Mô tả**: Xóa sản phẩm khỏi giỏ hàng
- **Luồng chính**:
  1. Chọn sản phẩm trong giỏ hàng
  2. Xóa sản phẩm
- **Kết quả**: Sản phẩm bị xóa khỏi giỏ hàng

## 4. Quản lý Đơn hàng

### 4.1. Tạo đơn hàng

- **Actor**: Khách hàng
- **Mô tả**: Tạo đơn hàng mới từ giỏ hàng
- **Luồng chính**:
  1. Kiểm tra giỏ hàng
  2. Nhập thông tin giao hàng
  3. Chọn phương thức thanh toán
  4. Xác nhận đơn hàng
- **Kết quả**: Đơn hàng mới được tạo

### 4.2. Theo dõi đơn hàng

- **Actor**: Khách hàng
- **Mô tả**: Xem trạng thái đơn hàng
- **Luồng chính**:
  1. Xem danh sách đơn hàng
  2. Chọn đơn hàng cần theo dõi
  3. Xem trạng thái và lịch sử
- **Kết quả**: Hiển thị thông tin đơn hàng

### 4.3. Hủy đơn hàng

- **Actor**: Khách hàng
- **Mô tả**: Hủy đơn hàng chưa xử lý
- **Luồng chính**:
  1. Chọn đơn hàng
  2. Xác nhận hủy
- **Kết quả**: Đơn hàng bị hủy

## 5. Quản lý Công thức

### 5.1. Xem công thức

- **Actor**: Khách hàng
- **Mô tả**: Xem danh sách công thức pha chế
- **Luồng chính**:
  1. Chọn danh mục công thức
  2. Xem danh sách
  3. Xem chi tiết công thức
- **Kết quả**: Hiển thị thông tin công thức

### 5.2. Tìm kiếm công thức

- **Actor**: Khách hàng
- **Mô tả**: Tìm kiếm công thức theo từ khóa
- **Luồng chính**:
  1. Nhập từ khóa
  2. Xem kết quả tìm kiếm
- **Kết quả**: Hiển thị công thức phù hợp

## 6. Quản lý Đánh giá

### 6.1. Đánh giá sản phẩm

- **Actor**: Khách hàng
- **Mô tả**: Đánh giá sản phẩm đã mua
- **Luồng chính**:
  1. Chọn sản phẩm
  2. Nhập đánh giá và bình luận
  3. Gửi đánh giá
- **Kết quả**: Đánh giá được lưu

### 6.2. Xem đánh giá

- **Actor**: Khách hàng
- **Mô tả**: Xem đánh giá của sản phẩm
- **Luồng chính**:
  1. Chọn sản phẩm
  2. Xem danh sách đánh giá
- **Kết quả**: Hiển thị đánh giá

## 7. Quản lý Kho

### 7.1. Nhập kho

- **Actor**: Admin, Staff
- **Mô tả**: Nhập sản phẩm mới vào kho
- **Luồng chính**:
  1. Tạo đơn nhập hàng
  2. Nhập thông tin sản phẩm
  3. Xác nhận nhập kho
- **Kết quả**: Sản phẩm được nhập vào kho

### 7.2. Kiểm tra tồn kho

- **Actor**: Admin, Staff
- **Mô tả**: Kiểm tra số lượng tồn kho
- **Luồng chính**:
  1. Xem danh sách sản phẩm
  2. Kiểm tra số lượng
  3. Cập nhật nếu cần
- **Kết quả**: Hiển thị thông tin tồn kho

## 8. Quản lý Khuyến mãi

### 8.1. Áp dụng mã giảm giá

- **Actor**: Khách hàng
- **Mô tả**: Sử dụng mã giảm giá khi đặt hàng
- **Luồng chính**:
  1. Nhập mã giảm giá
  2. Hệ thống kiểm tra tính hợp lệ
  3. Áp dụng giảm giá
- **Kết quả**: Giá đơn hàng được giảm

### 8.2. Quản lý mã giảm giá

- **Actor**: Admin
- **Mô tả**: Tạo và quản lý mã giảm giá
- **Luồng chính**:
  1. Tạo mã mới
  2. Thiết lập điều kiện
  3. Kích hoạt mã
- **Kết quả**: Mã giảm giá được tạo

## 9. Quản lý Thông báo

### 9.1. Gửi thông báo

- **Actor**: Admin, Staff
- **Mô tả**: Gửi thông báo cho khách hàng
- **Luồng chính**:
  1. Tạo thông báo
  2. Chọn đối tượng nhận
  3. Gửi thông báo
- **Kết quả**: Thông báo được gửi

### 9.2. Xem thông báo

- **Actor**: Khách hàng
- **Mô tả**: Xem danh sách thông báo
- **Luồng chính**:
  1. Truy cập mục thông báo
  2. Xem danh sách
  3. Đánh dấu đã đọc
- **Kết quả**: Hiển thị thông báo
