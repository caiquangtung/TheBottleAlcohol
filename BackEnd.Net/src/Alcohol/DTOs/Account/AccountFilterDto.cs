using System;

namespace Alcohol.DTOs.Account;

public class AccountFilterDto
{
    public string SearchTerm { get; set; }
    public bool? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 