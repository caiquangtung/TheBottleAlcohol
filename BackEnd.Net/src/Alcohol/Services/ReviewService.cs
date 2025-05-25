using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Review;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

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

    public async Task<IEnumerable<ReviewResponseDto>> GetAllReviewsAsync()
    {
        var reviews = await _reviewRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);
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