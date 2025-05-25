using System;
using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.ImportOrder;

public class ImportOrderDetailUpdateDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public decimal UnitPrice { get; set; }
} 