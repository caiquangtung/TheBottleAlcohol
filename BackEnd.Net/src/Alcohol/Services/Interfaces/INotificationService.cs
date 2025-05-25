using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Notification;

namespace Alcohol.Services.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponseDto>> GetAllNotificationsAsync();
    Task<NotificationResponseDto> GetNotificationByIdAsync(int id);
    Task<IEnumerable<NotificationResponseDto>> GetNotificationsByUserAsync(int userId);
    Task<IEnumerable<NotificationResponseDto>> GetUnreadNotificationsByUserAsync(int userId);
    Task<NotificationResponseDto> CreateNotificationAsync(NotificationCreateDto createDto);
    Task<NotificationResponseDto> UpdateNotificationAsync(int id, NotificationUpdateDto updateDto);
    Task<bool> MarkNotificationAsReadAsync(int id);
    Task<bool> MarkAllNotificationsAsReadAsync(int userId);
    Task<bool> DeleteNotificationAsync(int id);
} 