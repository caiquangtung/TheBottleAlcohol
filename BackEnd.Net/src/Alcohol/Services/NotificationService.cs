using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Notification;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

namespace Alcohol.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationResponseDto>> GetAllNotificationsAsync()
    {
        var notifications = await _notificationRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<NotificationResponseDto>>(notifications);
    }

    public async Task<NotificationResponseDto> GetNotificationByIdAsync(int id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null)
            return null;

        return _mapper.Map<NotificationResponseDto>(notification);
    }

    public async Task<IEnumerable<NotificationResponseDto>> GetNotificationsByUserAsync(int userId)
    {
        var notifications = await _notificationRepository.GetUserNotificationsAsync(userId);
        return _mapper.Map<IEnumerable<NotificationResponseDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationResponseDto>> GetUnreadNotificationsByUserAsync(int userId)
    {
        var notifications = await _notificationRepository.GetUnreadNotificationsAsync(userId);
        return _mapper.Map<IEnumerable<NotificationResponseDto>>(notifications);
    }

    public async Task<NotificationResponseDto> CreateNotificationAsync(NotificationCreateDto createDto)
    {
        var notification = _mapper.Map<Notification>(createDto);
        notification.CreatedAt = DateTime.UtcNow;
        notification.IsRead = false;

        await _notificationRepository.AddAsync(notification);
        await _notificationRepository.SaveChangesAsync();

        return _mapper.Map<NotificationResponseDto>(notification);
    }

    public async Task<NotificationResponseDto> UpdateNotificationAsync(int id, NotificationUpdateDto updateDto)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null)
            return null;

        _mapper.Map(updateDto, notification);
        _notificationRepository.Update(notification);
        await _notificationRepository.SaveChangesAsync();

        return _mapper.Map<NotificationResponseDto>(notification);
    }

    public async Task<bool> MarkNotificationAsReadAsync(int id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null)
            return false;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        _notificationRepository.Update(notification);
        await _notificationRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAllNotificationsAsReadAsync(int userId)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId);
        return true;
    }

    public async Task<bool> DeleteNotificationAsync(int id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null)
            return false;

        _notificationRepository.Delete(notification);
        await _notificationRepository.SaveChangesAsync();
        return true;
    }
} 