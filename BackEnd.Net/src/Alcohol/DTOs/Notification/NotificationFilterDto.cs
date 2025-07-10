using System;

namespace Alcohol.DTOs.Notification;

public class NotificationFilterDto : BaseFilterDto
{
    public int? UserId { get; set; }
    public string Type { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
} 