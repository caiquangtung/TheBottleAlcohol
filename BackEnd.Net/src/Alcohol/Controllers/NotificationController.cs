using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Notification;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using System.Security.Claims;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(new ApiResponse<IEnumerable<NotificationResponseDto>>(notifications));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
                return NotFound(new ApiResponse<string>("Notification not found"));
            return Ok(new ApiResponse<NotificationResponseDto>(notification));
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var notifications = await _notificationService.GetNotificationsByUserAsync(userId);
            return Ok(new ApiResponse<IEnumerable<NotificationResponseDto>>(notifications));
        }

        [HttpGet("user/{userId}/unread")]
        [Authorize]
        public async Task<IActionResult> GetUnreadByUser(int userId)
        {
            var notifications = await _notificationService.GetUnreadNotificationsByUserAsync(userId);
            return Ok(new ApiResponse<IEnumerable<NotificationResponseDto>>(notifications));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(NotificationCreateDto createDto)
        {
            var notification = await _notificationService.CreateNotificationAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = notification.Id }, new ApiResponse<NotificationResponseDto>(notification));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, NotificationUpdateDto updateDto)
        {
            var notification = await _notificationService.UpdateNotificationAsync(id, updateDto);
            if (notification == null)
                return NotFound(new ApiResponse<string>("Notification not found"));
            return Ok(new ApiResponse<NotificationResponseDto>(notification));
        }

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkNotificationAsReadAsync(id);
            if (!result)
                return NotFound(new ApiResponse<string>("Notification not found"));
            return Ok(new ApiResponse<string>("Notification marked as read"));
        }

        [HttpPut("user/{userId}/read-all")]
        [Authorize]
        public async Task<IActionResult> MarkAllAsRead(int userId)
        {
            var result = await _notificationService.MarkAllNotificationsAsReadAsync(userId);
            return Ok(new ApiResponse<string>("All notifications marked as read"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);
            if (!result)
                return NotFound(new ApiResponse<string>("Notification not found"));
            return Ok(new ApiResponse<string>("Notification deleted successfully"));
        }
    }
} 