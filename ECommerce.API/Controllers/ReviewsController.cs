using ECommerce.API.DTOs;
using ECommerce.API.Models;
using ECommerce.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductReview>>> GetProductReviews(Guid productId)
        {
            var reviews = await _reviewService.GetProductReviewsAsync(productId);
            return Ok(reviews);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProductReview>> CreateReview([FromBody] CreateReviewDto dto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

                var review = new ProductReview
                {
                    Id = Guid.NewGuid(),
                    ProductId = dto.ProductId,
                    UserId = userId,
                    Rating = dto.Rating,
                    Title = dto.Title,
                    ReviewText = dto.ReviewText
                };

                var createdReview = await _reviewService.CreateReviewAsync(review);
                return Ok(createdReview);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/vote")]
        [Authorize]
        public async Task<ActionResult<ProductReview>> VoteOnReview(Guid id, [FromBody] VoteDto dto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
                var review = await _reviewService.VoteOnReviewAsync(id, userId, dto.IsHelpful);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
