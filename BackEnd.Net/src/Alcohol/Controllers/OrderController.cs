using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Order;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.Models.Enums;
using Alcohol.DTOs.OrderDetail;
using Alcohol.DTOs;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;
        private readonly IOrderDetailService _orderDetailService;

        public OrderController(IOrderService service, IOrderDetailService orderDetailService)
        {
            _service = service;
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        [Authorize(Roles = "CEO,Manager,Seller")]
        public async Task<IActionResult> GetAll([FromQuery] string search)
        {
            var result = await _service.GetAllOrdersAsync(search);
            return Ok(new ApiResponse<IEnumerable<OrderResponseDto>>(result));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var order = await _service.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound(new ApiResponse<string>("Order not found"));
                return Ok(new ApiResponse<OrderResponseDto>(order));
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpGet("customer/{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var filter = new OrderFilterDto { CustomerId = customerId };
            var result = await _service.GetOrdersAsync(filter);
            return Ok(new ApiResponse<PagedResult<OrderResponseDto>>(result));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(OrderCreateDto orderDto)
        {
            try
            {
                var order = await _service.CreateOrderAsync(orderDto);
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, new ApiResponse<OrderResponseDto>(order));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "CEO,Manager,Seller")]
        public async Task<IActionResult> Update(int id, OrderUpdateDto orderDto)
        {
            try
            {
                var order = await _service.UpdateOrderAsync(id, orderDto);
                if (order == null)
                    return NotFound(new ApiResponse<string>("Order not found"));
                return Ok(new ApiResponse<OrderResponseDto>(order));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "CEO,Manager,Seller")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatusType status)
        {
            try
            {
                var order = await _service.UpdateOrderStatusAsync(id, status);
                if (order == null)
                    return NotFound(new ApiResponse<string>("Order not found"));
                return Ok(new ApiResponse<OrderResponseDto>(order));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteOrderAsync(id);
                if (!result)
                    return NotFound(new ApiResponse<string>("Order not found"));
                return Ok(new ApiResponse<string>("Order deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpGet("{orderId}/details/{productId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetail(int orderId, int productId)
        {
            var orderDetail = await _orderDetailService.GetOrderDetail(orderId, productId);
            if (orderDetail == null)
                return NotFound(new ApiResponse<string>("Order detail not found"));
            return Ok(new ApiResponse<OrderDetailResponseDto>(orderDetail));
        }

        [HttpPost("{orderId}/details")]
        [Authorize]
        public async Task<IActionResult> CreateOrderDetail(int orderId, OrderDetailCreateDto orderDetailDto)
        {
            try
            {
                var orderDetail = await _orderDetailService.CreateOrderDetail(orderId, orderDetailDto);
                if (orderDetail == null)
                    return BadRequest(new ApiResponse<string>("Failed to create order detail"));
                return CreatedAtAction(nameof(GetOrderDetail), 
                    new { orderId = orderDetail.OrderId, productId = orderDetail.ProductId },
                    new ApiResponse<OrderDetailResponseDto>(orderDetail));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpDelete("{orderId}/details/{productId}")]
        [Authorize(Roles = "CEO,Manager,Seller")]
        public async Task<IActionResult> DeleteOrderDetail(int orderId, int productId)
        {
            try
            {
                var result = await _orderDetailService.DeleteOrderDetail(orderId, productId);
                if (!result)
                    return NotFound(new ApiResponse<string>("Order detail not found"));
                return Ok(new ApiResponse<string>("Order detail deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }
    }
}