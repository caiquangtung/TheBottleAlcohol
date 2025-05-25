using System;
using System.Collections.Generic;
using Alcohol.DTOs.CartDetail;

namespace Alcohol.DTOs.Cart;

public class CartResponseDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<CartDetailResponseDto> CartDetails { get; set; }
} 