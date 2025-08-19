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
using Alcohol.DTOs;

namespace Alcohol.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDetailRepository _orderDetailRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public OrderService(
        IOrderRepository orderRepository,
        IOrderDetailRepository orderDetailRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _orderDetailRepository = orderDetailRepository;
        _productRepository = productRepository;
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

        var totalRecords = filteredOrders.Count;
        var pagedOrders = filteredOrders
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        var orderDtos = _mapper.Map<List<OrderResponseDto>>(pagedOrders);
        return new PagedResult<OrderResponseDto>(orderDtos, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(string search = null)
    {
        IEnumerable<Order> orders;
        if (!string.IsNullOrWhiteSpace(search))
            orders = (await _orderRepository.GetAllAsync()).Where(o => o.Id.ToString().Contains(search) || (o.Account != null && o.Account.FullName.Contains(search)));
        else
            orders = await _orderRepository.GetAllAsync();
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

            // Ensure non-nullable columns have safe defaults
            order.ShippingName ??= string.Empty;
            order.ShippingPhone ??= string.Empty;

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            // Persist order details and compute total (deduplicate by product)
            decimal totalAmount = 0m;
            if (createDto.OrderDetails != null && createDto.OrderDetails.Count > 0)
            {
                var grouped = createDto.OrderDetails
                    .GroupBy(d => d.ProductId)
                    .Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                    .ToList();

                foreach (var item in grouped)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null) continue;

                    var unitPrice = product.Price;
                    var lineTotal = unitPrice * item.Quantity;
                    totalAmount += lineTotal;

                    var detail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        UnitPrice = unitPrice,
                        Quantity = item.Quantity,
                        TotalAmount = lineTotal,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _orderDetailRepository.AddAsync(detail);
                }
                await _orderDetailRepository.SaveChangesAsync();
            }

            // Update order total
            order.TotalAmount = totalAmount;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            return await GetOrderWithDetailsAsync(order.Id);
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

        // Enforce valid state transitions to prevent regressions
        var currentStatus = order.Status;
        if (currentStatus == status)
        {
            // No change needed; return current order state
            return _mapper.Map<OrderResponseDto>(order);
        }

        var allowedTransitions = new Dictionary<OrderStatusType, OrderStatusType[]>
        {
            { OrderStatusType.Pending,    new [] { OrderStatusType.Paid, OrderStatusType.Cancelled } },
            { OrderStatusType.Paid,       new [] { OrderStatusType.Processing, OrderStatusType.Cancelled } },
            { OrderStatusType.Processing, new [] { OrderStatusType.Shipped, OrderStatusType.Cancelled } },
            { OrderStatusType.Shipped,    new [] { OrderStatusType.Delivered } },
            { OrderStatusType.Delivered,  Array.Empty<OrderStatusType>() },
            { OrderStatusType.Cancelled,  Array.Empty<OrderStatusType>() },
        };

        if (!allowedTransitions.TryGetValue(currentStatus, out var nextStatuses) || !nextStatuses.Contains(status))
        {
            throw new InvalidOperationException($"Invalid order status transition from {currentStatus} to {status}");
        }

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

