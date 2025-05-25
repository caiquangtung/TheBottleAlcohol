using System;
using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.ImportOrderDetail;

public class ImportOrderDetailCreateDto
{
    [Required(ErrorMessage = "Import order ID is required")]
    public int ImportOrderId { get; set; }

    [Required(ErrorMessage = "Product ID is required")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Import price is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Import price must be greater than or equal to 0")]
    public int ImportPrice { get; set; }

    [Required(ErrorMessage = "Total amount is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Total amount must be greater than or equal to 0")]
    public int TotalAmount { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public int Status { get; set; }
}