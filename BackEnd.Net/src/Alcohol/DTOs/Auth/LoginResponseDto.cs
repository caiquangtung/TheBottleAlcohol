using System;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.Auth;

public class LoginResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public RoleType Role { get; set; }
    public string AccessToken { get; set; }
} 