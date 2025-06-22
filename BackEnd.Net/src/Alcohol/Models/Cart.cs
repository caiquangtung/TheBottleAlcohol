using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Alcohol.Models;

public class Cart
{
    public Cart()
    {
        CartDetails = new List<CartDetail>();
    }

    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }

    public virtual Account Customer { get; set; }
    public virtual ICollection<CartDetail> CartDetails { get; set; }
} 