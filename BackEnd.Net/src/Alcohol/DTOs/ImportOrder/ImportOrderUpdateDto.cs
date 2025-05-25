using System;
using System.ComponentModel.DataAnnotations;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.ImportOrder;

public class ImportOrderUpdateDto
{
    [Required(ErrorMessage = "Import date is required")]
    public DateTime ImportDate { get; set; }

    [Required(ErrorMessage = "Total amount is required")]
    public decimal TotalAmount { get; set; }

    [Required(ErrorMessage = "Profit is required")]
    public decimal Profit { get; set; }

    public string Notes { get; set; }
    public ImportOrderStatusType Status { get; set; }
} 