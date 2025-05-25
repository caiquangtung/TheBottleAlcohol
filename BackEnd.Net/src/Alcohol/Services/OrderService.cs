using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.Order;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Alcohol.Common;

namespace Alcohol.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<OrderResponseDto>> GetOrdersAsync(OrderFilterDto filter)
    {
        var orders = await _orderRepository.GetAllAsync();
        var filteredOrders = orders.Where(o => 
            (string.IsNullOrEmpty(filter.SearchTerm) || o.Id.ToString().Contains(filter.SearchTerm)) &&
            (!filter.CustomerId.HasValue || o.AccountId == filter.CustomerId) &&
            (!filter.Status.HasValue || o.Status == filter.Status) &&
            (!filter.StartDate.HasValue || o.OrderDate >= filter.StartDate) &&
            (!filter.EndDate.HasValue || o.OrderDate <= filter.EndDate)
        ).ToList();

        var totalCount = filteredOrders.Count;
        var pagedOrders = filteredOrders
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        var orderDtos = _mapper.Map<List<OrderResponseDto>>(pagedOrders);
        return new PagedResult<OrderResponseDto>
        {
            Items = orderDtos,
            TotalItems = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
            return null;

        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> GetOrderWithDetailsAsync(int id)
    {
        var order = await _orderRepository.GetOrderWithDetailsAsync(id);
        if (order == null)
            return null;

        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetOrdersByCustomerAsync(int customerId)
    {
        var orders = await _orderRepository.GetOrdersByCustomerAsync(customerId);
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetOrdersByStatusAsync(OrderStatusType status)
    {
        var orders = await _orderRepository.GetOrdersByStatusAsync(status.ToString());
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto createDto)
    {
        var order = _mapper.Map<Order>(createDto);
        order.OrderDate = DateTime.UtcNow;
        order.CreatedAt = DateTime.UtcNow;
        order.Status = OrderStatusType.Pending;

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> UpdateOrderAsync(int id, OrderUpdateDto updateDto)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
            return null;

        _mapper.Map(updateDto, order);
        order.UpdatedAt = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync();

        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> UpdateOrderStatusAsync(int id, OrderStatusType status)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
            return null;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync();

        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
            return false;

        _orderRepository.Delete(order);
        await _orderRepository.SaveChangesAsync();
        return true;
    }
}

