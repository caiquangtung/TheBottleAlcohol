using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Notification;

public class NotificationCreateDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    public string Message { get; set; }

    [Required]
    public string Type { get; set; }
} 