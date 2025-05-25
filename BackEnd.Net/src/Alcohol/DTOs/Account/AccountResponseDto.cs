using System;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.Account;

public class AccountResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; }
    public GenderType Gender { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public RoleType Role { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 