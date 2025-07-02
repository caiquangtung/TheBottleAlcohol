using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Alcohol.Models.Enums;

namespace Alcohol.Models;

public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string FullName { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [StringLength(500)]
    public string Address { get; set; }

    [Required]
    public GenderType Gender { get; set; }

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; }

    [Required]
    [StringLength(255)]
    public string Email { get; set; }

    [Required]
    public RoleType Role { get; set; }

    [Required]
    [StringLength(255)]
    public string Password { get; set; }

    [Required]
    public bool Status { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [StringLength(255)]
    public string? OAuthProvider { get; set; } // "Google", "Facebook"

    [StringLength(255)]
    public string? OAuthId { get; set; } // ID từ provider

    [StringLength(500)]
    public string? AvatarUrl { get; set; } // URL avatar từ OAuth

    // Navigation properties
    public ICollection<Order> Orders { get; set; }
    public Cart Cart { get; set; }

    public Account()
    {
        Orders = new List<Order>();
    }
}