using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Notification;
using Alcohol.DTOs;

namespace Alcohol.Services.Interfaces;

public interface INotificationService
{
    Task<PagedResult<NotificationResponseDto>> GetAllNotificationsAsync(NotificationFilterDto filter);
    Task<NotificationResponseDto> GetNotificationByIdAsync(int id);
    Task<IEnumerable<NotificationResponseDto>> GetNotificationsByUserAsync(int userId);
    Task<NotificationResponseDto> CreateNotificationAsync(NotificationCreateDto createDto);
    Task<NotificationResponseDto> UpdateNotificationAsync(int id, NotificationUpdateDto updateDto);
    Task<bool> DeleteNotificationAsync(int id);
    Task<bool> MarkAsReadAsync(int id);
} 