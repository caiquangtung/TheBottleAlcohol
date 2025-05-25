using System;
using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Account;

public class AccountUpdatePasswordDto
{
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "New password is required")]
    [MinLength(6, ErrorMessage = "New password must be at least 6 characters")]
    public string NewPassword { get; set; }
} 