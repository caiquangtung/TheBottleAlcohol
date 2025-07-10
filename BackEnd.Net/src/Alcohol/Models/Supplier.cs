using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Alcohol.Models.Enums;

namespace Alcohol.Models;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public SupplierStatusType Status { get; set; } = SupplierStatusType.Active;
    public virtual List<ImportOrder> ImportOrders { get; set; } = new List<ImportOrder>();
}
