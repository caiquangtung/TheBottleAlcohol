using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Review;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetAllReviews()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewResponseDto>> GetReviewById(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null)
            return NotFound();

        return Ok(review);
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetReviewsByProduct(int productId)
    {
        var reviews = await _reviewService.GetReviewsByProductAsync(productId);
        return Ok(reviews);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetReviewsByCustomer(int customerId)
    {
        var reviews = await _reviewService.GetReviewsByCustomerAsync(customerId);
        return Ok(reviews);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReviewResponseDto>> CreateReview(ReviewCreateDto createDto)
    {
        try
        {
            var review = await _reviewService.CreateReviewAsync(createDto);
            return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
        }
        catch (Exception ex)
        {
            // Trả về lỗi 400 với message rõ ràng
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ReviewResponseDto>> UpdateReview(int id, ReviewUpdateDto updateDto)
    {
        var review = await _reviewService.UpdateReviewAsync(id, updateDto);
        if (review == null)
            return NotFound();

        return Ok(review);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteReview(int id)
    {
        var result = await _reviewService.DeleteReviewAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
} 