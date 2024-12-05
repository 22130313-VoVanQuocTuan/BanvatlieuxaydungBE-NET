using BanVatLieuXayDung.dto.request.rating;
using BanVatLieuXayDung.service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BanVatLieuXayDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] ReviewRequest request)
        {
            try
            {
                var response = await _reviewService.CreateRate(request);
                return Ok(new { status = 200, response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                var response = await _reviewService.DeleteReview(reviewId);
                return Ok(new { status = 200, response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllReview()
        {
            try
            {
                var response = await _reviewService.GetAllReview();
                return Ok(new { status = 200, response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
