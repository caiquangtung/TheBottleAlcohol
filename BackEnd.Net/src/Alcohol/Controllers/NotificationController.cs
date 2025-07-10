using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Notification;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllNotifications([FromQuery] NotificationFilterDto filter)
    {
        var result = await _notificationService.GetAllNotificationsAsync(filter);
        return Ok(new ApiResponse<PagedResult<NotificationResponseDto>>(result));
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetNotificationById(int id)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id);
        if (notification == null)
            return NotFound(new ApiResponse<string>("Notification not found"));
        return Ok(new ApiResponse<NotificationResponseDto>(notification));
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetNotificationsByUser(int userId)
    {
        var notifications = await _notificationService.GetNotificationsByUserAsync(userId);
        return Ok(new ApiResponse<NotificationResponseDto[]>(notifications.ToArray()));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateNotification(NotificationCreateDto createDto)
    {
        try
        {
            var notification = await _notificationService.CreateNotificationAsync(createDto);
            return CreatedAtAction(nameof(GetNotificationById), new { id = notification.Id }, new ApiResponse<NotificationResponseDto>(notification));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateNotification(int id, NotificationUpdateDto updateDto)
    {
        var notification = await _notificationService.UpdateNotificationAsync(id, updateDto);
        if (notification == null)
            return NotFound(new ApiResponse<string>("Notification not found"));
        return Ok(new ApiResponse<NotificationResponseDto>(notification));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        var result = await _notificationService.DeleteNotificationAsync(id);
        if (!result)
            return NotFound(new ApiResponse<string>("Notification not found"));
        return Ok(new ApiResponse<string>("Notification deleted successfully"));
    }

    [HttpPut("{id}/mark-read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var result = await _notificationService.MarkAsReadAsync(id);
        if (!result)
            return NotFound(new ApiResponse<string>("Notification not found"));
        return Ok(new ApiResponse<string>("Notification marked as read"));
    }
} 