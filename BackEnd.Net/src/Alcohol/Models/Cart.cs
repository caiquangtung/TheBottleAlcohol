using System;
using System.Collections.Generic;

namespace Alcohol.Models;

public class Cart
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Account Customer { get; set; }
    public virtual ICollection<CartDetail> CartDetails { get; set; }
} 