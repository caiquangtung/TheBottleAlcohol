using System;

namespace Alcohol.DTOs.Supplier;

public class SupplierResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
    public DateTime CreatedDate { get; set; }
} 