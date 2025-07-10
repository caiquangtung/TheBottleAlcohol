using System;

namespace Alcohol.DTOs.Account;

public class AccountFilterDto : BaseFilterDto
{
    public bool? Status { get; set; }
} 