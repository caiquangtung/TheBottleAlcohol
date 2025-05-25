using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Common;
using Alcohol.Data;
using Alcohol.DTOs.Order;
using Alcohol.DTOs.OrderDetail;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Services;

public class OrderDetailService : IOrderDetailService
{
    private readonly MyDbContext _context;
    private readonly IOrderDetailRepository _orderDetailRepository;
    private readonly IMapper _mapper;

    public OrderDetailService(MyDbContext context, IOrderDetailRepository orderDetailRepository, IMapper mapper)
    {
        _context = context;
        _orderDetailRepository = orderDetailRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderDetailResponseDto>> GetAllOrderDetailsAsync()
    {
        var orderDetails = await _orderDetailRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<OrderDetailResponseDto>>(orderDetails);
    }

    public async Task<OrderDetailResponseDto> GetOrderDetailByIdAsync(int id)
    {
        var orderDetail = await _context.OrderDetails
            .Include(d => d.Product)
            .FirstOrDefaultAsync(d => d.OrderId == id);
            
        if (orderDetail == null)
            return null;

        return _mapper.Map<OrderDetailResponseDto>(orderDetail);
    }

    public async Task<IEnumerable<OrderDetailResponseDto>> GetOrderDetailsByOrderAsync(int orderId)
    {
        var orderDetails = await _orderDetailRepository.GetByOrderIdAsync(orderId);
        return _mapper.Map<IEnumerable<OrderDetailResponseDto>>(orderDetails);
    }

    public async Task<OrderDetailResponseDto> CreateOrderDetailAsync(OrderDetailCreateDto createDto)
    {
        var orderDetail = _mapper.Map<OrderDetail>(createDto);
        orderDetail.CreatedAt = DateTime.UtcNow;

        await _orderDetailRepository.AddAsync(orderDetail);
        await _orderDetailRepository.SaveChangesAsync();

        return _mapper.Map<OrderDetailResponseDto>(orderDetail);
    }

    public async Task<OrderDetailResponseDto> UpdateOrderDetailAsync(int id, OrderDetailUpdateDto updateDto)
    {
        var orderDetail = await _context.OrderDetails
            .Include(d => d.Product)
            .FirstOrDefaultAsync(d => d.OrderId == id);
            
        if (orderDetail == null)
            return null;

        _mapper.Map(updateDto, orderDetail);
        orderDetail.UpdatedAt = DateTime.UtcNow;

        await _orderDetailRepository.UpdateAsync(orderDetail);
        await _orderDetailRepository.SaveChangesAsync();

        return _mapper.Map<OrderDetailResponseDto>(orderDetail);
    }

    public async Task<bool> DeleteOrderDetailAsync(int id)
    {
        var orderDetail = await _context.OrderDetails
            .Include(d => d.Product)
            .FirstOrDefaultAsync(d => d.OrderId == id);
            
        if (orderDetail == null)
            return false;

        await _orderDetailRepository.DeleteAsync(orderDetail.OrderId, orderDetail.ProductId);
        await _orderDetailRepository.SaveChangesAsync();
        return true;
    }

    public async Task<OrderDetailResponseDto> GetOrderDetail(int orderId, int productId)
    {
        var orderDetail = await _context.OrderDetails
            .Include(d => d.Product)
            .FirstOrDefaultAsync(d => d.OrderId == orderId && d.ProductId == productId);

        if (orderDetail == null)
            return null;

        return new OrderDetailResponseDto
        {
            OrderId = orderDetail.OrderId,
            ProductId = orderDetail.ProductId,
            ProductName = orderDetail.Product.Name,
            UnitPrice = orderDetail.UnitPrice,
            Quantity = orderDetail.Quantity,
            TotalAmount = orderDetail.TotalAmount,
            CreatedAt = orderDetail.CreatedAt,
            UpdatedAt = orderDetail.UpdatedAt
        };
    }

    public async Task<OrderDetailResponseDto> CreateOrderDetail(int orderId, OrderDetailCreateDto dto)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null)
            return null;

        var orderDetail = new OrderDetail
        {
            OrderId = orderId,
            ProductId = dto.ProductId,
            UnitPrice = product.Price,
            Quantity = dto.Quantity,
            TotalAmount = product.Price * dto.Quantity,
            CreatedAt = DateTime.UtcNow
        };

        await _orderDetailRepository.AddAsync(orderDetail);
        await _orderDetailRepository.SaveChangesAsync();

        return await GetOrderDetail(orderId, dto.ProductId);
    }

    public async Task<bool> DeleteOrderDetail(int orderId, int productId)
    {
        var orderDetail = await _orderDetailRepository.GetByIdAsync(orderId, productId);
        if (orderDetail == null)
            return false;

        await _orderDetailRepository.DeleteAsync(orderId, productId);
        await _orderDetailRepository.SaveChangesAsync();
        return true;
    }
} 