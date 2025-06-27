using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly MyDbContext _context;

    public ReviewRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .Include(r => r.Customer)
            .ToListAsync();
    }

    public async Task<Review> GetByIdAsync(int id)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .Include(r => r.Customer)
            .Where(r => r.ProductId == productId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .Include(r => r.Customer)
            .Where(r => r.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<Review> GetByProductAndCustomerAsync(int productId, int customerId)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.CustomerId == customerId);
    }

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
    }

    public void Update(Review review)
    {
        _context.Reviews.Update(review);
    }

    public void Delete(Review review)
    {
        _context.Reviews.Remove(review);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
} 