using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.Notification;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Alcohol.DTOs;

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

    public async Task<PagedResult<NotificationResponseDto>> GetAllNotificationsAsync(NotificationFilterDto filter)
    {
        var notifications = await _notificationRepository.GetAllAsync();
        
        // Apply filters
        var filteredNotifications = notifications.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filteredNotifications = filteredNotifications.Where(n => 
                n.Title.Contains(filter.SearchTerm) || 
                n.Content.Contains(filter.SearchTerm));
        }
        
        if (filter.UserId.HasValue)
        {
            filteredNotifications = filteredNotifications.Where(n => n.UserId == filter.UserId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(filter.Type))
        {
            filteredNotifications = filteredNotifications.Where(n => n.Type == filter.Type);
        }
        
        if (filter.IsRead.HasValue)
        {
            filteredNotifications = filteredNotifications.Where(n => n.IsRead == filter.IsRead.Value);
        }
        
        if (filter.StartDate.HasValue)
        {
            filteredNotifications = filteredNotifications.Where(n => n.CreatedAt >= filter.StartDate.Value);
        }
        
        if (filter.EndDate.HasValue)
        {
            filteredNotifications = filteredNotifications.Where(n => n.CreatedAt <= filter.EndDate.Value);
        }
        
        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            filteredNotifications = filter.SortBy.ToLower() switch
            {
                "title" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredNotifications.OrderByDescending(n => n.Title)
                    : filteredNotifications.OrderBy(n => n.Title),
                "createdat" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredNotifications.OrderByDescending(n => n.CreatedAt)
                    : filteredNotifications.OrderBy(n => n.CreatedAt),
                "isread" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredNotifications.OrderByDescending(n => n.IsRead)
                    : filteredNotifications.OrderBy(n => n.IsRead),
                _ => filteredNotifications.OrderBy(n => n.Id)
            };
        }
        else
        {
            filteredNotifications = filteredNotifications.OrderBy(n => n.Id);
        }
        
        var totalRecords = filteredNotifications.Count();
        var pagedNotifications = filteredNotifications
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var notificationDtos = _mapper.Map<List<NotificationResponseDto>>(pagedNotifications);
        return new PagedResult<NotificationResponseDto>(notificationDtos, totalRecords, filter.PageNumber, filter.PageSize);
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
        var notifications = await _notificationRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<NotificationResponseDto>>(notifications);
    }

    public async Task<NotificationResponseDto> CreateNotificationAsync(NotificationCreateDto createDto)
    {
        var notification = _mapper.Map<Notification>(createDto);
        notification.CreatedAt = DateTime.UtcNow;

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
        notification.UpdatedAt = DateTime.UtcNow;

        _notificationRepository.Update(notification);
        await _notificationRepository.SaveChangesAsync();

        return _mapper.Map<NotificationResponseDto>(notification);
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

    public async Task<bool> MarkAsReadAsync(int id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null)
            return false;

        notification.IsRead = true;
        notification.UpdatedAt = DateTime.UtcNow;

        _notificationRepository.Update(notification);
        await _notificationRepository.SaveChangesAsync();
        return true;
    }
} 