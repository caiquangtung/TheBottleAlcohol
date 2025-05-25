using System;

namespace Alcohol.Models;

public class CartDetail
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Cart Cart { get; set; }
    public virtual Product Product { get; set; }
} 