using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Review;

namespace Alcohol.Services.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<ReviewResponseDto>> GetAllReviewsAsync();
    Task<ReviewResponseDto> GetReviewByIdAsync(int id);
    Task<IEnumerable<ReviewResponseDto>> GetReviewsByProductAsync(int productId);
    Task<IEnumerable<ReviewResponseDto>> GetReviewsByCustomerAsync(int customerId);
    Task<ReviewResponseDto> CreateReviewAsync(ReviewCreateDto createDto);
    Task<ReviewResponseDto> UpdateReviewAsync(int id, ReviewUpdateDto updateDto);
    Task<bool> DeleteReviewAsync(int id);
} 