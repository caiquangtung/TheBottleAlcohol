using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.Review;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Alcohol.DTOs;

namespace Alcohol.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ReviewResponseDto>> GetAllReviewsAsync(ReviewFilterDto filter)
    {
        var reviews = await _reviewRepository.GetAllAsync();
        
        // Apply filters
        var filteredReviews = reviews.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filteredReviews = filteredReviews.Where(r => 
                r.Comment.Contains(filter.SearchTerm) || 
                (r.Product != null && r.Product.Name.Contains(filter.SearchTerm)) ||
                (r.Customer != null && r.Customer.FullName.Contains(filter.SearchTerm)));
        }
        
        if (filter.ProductId.HasValue)
        {
            filteredReviews = filteredReviews.Where(r => r.ProductId == filter.ProductId.Value);
        }
        
        if (filter.CustomerId.HasValue)
        {
            filteredReviews = filteredReviews.Where(r => r.CustomerId == filter.CustomerId.Value);
        }
        
        if (filter.MinRating.HasValue)
        {
            filteredReviews = filteredReviews.Where(r => r.Rating >= filter.MinRating.Value);
        }
        
        if (filter.MaxRating.HasValue)
        {
            filteredReviews = filteredReviews.Where(r => r.Rating <= filter.MaxRating.Value);
        }
        
        if (filter.StartDate.HasValue)
        {
            filteredReviews = filteredReviews.Where(r => r.CreatedAt >= filter.StartDate.Value);
        }
        
        if (filter.EndDate.HasValue)
        {
            filteredReviews = filteredReviews.Where(r => r.CreatedAt <= filter.EndDate.Value);
        }
        
        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            filteredReviews = filter.SortBy.ToLower() switch
            {
                "rating" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredReviews.OrderByDescending(r => r.Rating)
                    : filteredReviews.OrderBy(r => r.Rating),
                "createdat" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredReviews.OrderByDescending(r => r.CreatedAt)
                    : filteredReviews.OrderBy(r => r.CreatedAt),
                _ => filteredReviews.OrderBy(r => r.Id)
            };
        }
        else
        {
            filteredReviews = filteredReviews.OrderBy(r => r.Id);
        }
        
        var totalRecords = filteredReviews.Count();
        var pagedReviews = filteredReviews
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var reviewDtos = _mapper.Map<List<ReviewResponseDto>>(pagedReviews);
        return new PagedResult<ReviewResponseDto>(reviewDtos, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<ReviewResponseDto> GetReviewByIdAsync(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review == null)
            return null;

        return _mapper.Map<ReviewResponseDto>(review);
    }

    public async Task<IEnumerable<ReviewResponseDto>> GetReviewsByProductAsync(int productId)
    {
        var reviews = await _reviewRepository.GetByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);
    }

    public async Task<IEnumerable<ReviewResponseDto>> GetReviewsByCustomerAsync(int customerId)
    {
        var reviews = await _reviewRepository.GetByCustomerIdAsync(customerId);
        return _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);
    }

    public async Task<ReviewResponseDto> CreateReviewAsync(ReviewCreateDto createDto)
    {
        var review = _mapper.Map<Review>(createDto);
        review.CreatedAt = DateTime.UtcNow;

        await _reviewRepository.AddAsync(review);
        await _reviewRepository.SaveChangesAsync();

        return _mapper.Map<ReviewResponseDto>(review);
    }

    public async Task<ReviewResponseDto> UpdateReviewAsync(int id, ReviewUpdateDto updateDto)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review == null)
            return null;

        _mapper.Map(updateDto, review);
        review.UpdatedAt = DateTime.UtcNow;

        _reviewRepository.Update(review);
        await _reviewRepository.SaveChangesAsync();

        return _mapper.Map<ReviewResponseDto>(review);
    }

    public async Task<bool> DeleteReviewAsync(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review == null)
            return false;

        _reviewRepository.Delete(review);
        await _reviewRepository.SaveChangesAsync();
        return true;
    }
} 