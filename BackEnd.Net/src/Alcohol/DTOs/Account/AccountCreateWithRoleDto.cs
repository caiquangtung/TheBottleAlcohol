using System.ComponentModel.DataAnnotations;
using Alcohol.Models;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.Account;

public class AccountCreateWithRoleDto
{
    [Required]
    public string FullName { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public GenderType Gender { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    public RoleType Role { get; set; }
} 