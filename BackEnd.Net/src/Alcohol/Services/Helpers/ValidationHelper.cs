using System;
using System.Threading.Tasks;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Services.Helpers;

public static class ValidationHelper
{
    public static async Task ValidateEntityExistsAsync<T>(int id, IGenericRepository<T> repository) where T : class
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new ArgumentException($"Entity with ID {id} does not exist.");
        }
    }

    public static async Task ValidateUniqueFieldAsync<T>(string field, string value, IGenericRepository<T> repository) where T : class
    {
        var exists = await repository.FindAsync(e => EF.Property<string>(e, field) == value);
        if (exists.Any())
        {
            throw new ArgumentException($"Entity with {field} '{value}' already exists.");
        }
    }

    public static async Task ValidateDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        await Task.Run(() =>
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be before end date.");
            }
        });
    }

    public static async Task ValidateQuantityAsync(int quantity)
    {
        await Task.Run(() =>
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0.");
            }
        });
    }

    public static async Task ValidatePriceAsync(decimal price)
    {
        await Task.Run(() =>
        {
            if (price < 0)
            {
                throw new ArgumentException("Price cannot be negative.");
            }
        });
    }

    public static async Task ValidateEmailAsync(string email)
    {
        await Task.Run(() =>
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be empty.");
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    throw new ArgumentException("Invalid email format.");
                }
            }
            catch
            {
                throw new ArgumentException("Invalid email format.");
            }
        });
    }

    public static async Task ValidatePhoneAsync(string phone)
    {
        await Task.Run(() =>
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Phone number cannot be empty.");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\+?[0-9]{10,15}$"))
            {
                throw new ArgumentException("Invalid phone number format.");
            }
        });
    }

    public static async Task ValidatePasswordAsync(string password)
    {
        await Task.Run(() =>
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty.");
            }

            if (password.Length < 8)
            {
                throw new ArgumentException("Password must be at least 8 characters long.");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]"))
            {
                throw new ArgumentException("Password must contain at least one uppercase letter.");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]"))
            {
                throw new ArgumentException("Password must contain at least one lowercase letter.");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[0-9]"))
            {
                throw new ArgumentException("Password must contain at least one number.");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
            {
                throw new ArgumentException("Password must contain at least one special character.");
            }
        });
    }
} 