using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Common;
using Alcohol.DTOs.Order;
using Alcohol.Models.Enums;
using Alcohol.DTOs;

namespace Alcohol.Services.Interfaces;

public interface IOrderService
{
    Task<PagedResult<OrderResponseDto>> GetOrdersAsync(OrderFilterDto filter);
    Task<OrderResponseDto> GetOrderByIdAsync(int id);
    Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto orderDto);
    Task<OrderResponseDto> UpdateOrderAsync(int id, OrderUpdateDto orderDto);
    Task<OrderResponseDto> UpdateOrderStatusAsync(int id, OrderStatusType status);
    Task<bool> DeleteOrderAsync(int id);
} 