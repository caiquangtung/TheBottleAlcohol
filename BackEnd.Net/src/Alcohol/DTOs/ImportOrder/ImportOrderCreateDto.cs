using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.ImportOrder;

public class ImportOrderCreateDto
{
    [Required(ErrorMessage = "Supplier ID is required")]
    public int SupplierId { get; set; }

    [Required(ErrorMessage = "Manager ID is required")]
    public int ManagerId { get; set; }

    public string Notes { get; set; }

    [Required(ErrorMessage = "Import date is required")]
    public DateTime ImportDate { get; set; }

    [Required(ErrorMessage = "Import order details are required")]
    public List<ImportOrderDetailCreateDto> ImportOrderDetails { get; set; }
} 