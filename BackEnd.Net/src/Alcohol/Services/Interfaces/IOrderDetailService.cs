using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.OrderDetail;

namespace Alcohol.Services.Interfaces;

public interface IOrderDetailService
{
    Task<IEnumerable<OrderDetailResponseDto>> GetAllOrderDetailsAsync();
    Task<OrderDetailResponseDto> GetOrderDetailByIdAsync(int id);
    Task<IEnumerable<OrderDetailResponseDto>> GetOrderDetailsByOrderAsync(int orderId);
    Task<OrderDetailResponseDto> CreateOrderDetailAsync(OrderDetailCreateDto createDto);
    Task<OrderDetailResponseDto> UpdateOrderDetailAsync(int id, OrderDetailUpdateDto updateDto);
    Task<bool> DeleteOrderDetailAsync(int id);
    Task<OrderDetailResponseDto> GetOrderDetail(int orderId, int productId);
    Task<OrderDetailResponseDto> CreateOrderDetail(int orderId, OrderDetailCreateDto dto);
    Task<bool> DeleteOrderDetail(int orderId, int productId);
} 