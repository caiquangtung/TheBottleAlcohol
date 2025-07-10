using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
    {
        return await _dbSet
            .Include(n => n.User)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId)
    {
        return await _dbSet
            .Include(n => n.User)
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int id)
    {
        var notification = await _dbSet.FindAsync(id);
        if (notification != null)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _dbSet.Update(notification);
        }
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var notifications = await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        _dbSet.UpdateRange(notifications);
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(n => n.User)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
} 