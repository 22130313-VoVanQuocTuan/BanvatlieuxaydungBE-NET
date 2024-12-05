using AcountService.AppException;
using AcountService.dto.request.cart;
using AcountService.entity;
using AcountService.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AcountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> AddProductAsync([FromBody] AddProductToCartRequest request)
        {
            try
            {
                var results = await _cartService.AddProductAsync(request);
                return Ok(new { status = 200, results });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.CustomMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Đã xảy ra lỗi không xác định", errorMessage = ex.Message });
            }
        }

        // Xóa sản phẩm trong giỏ hàng
        [HttpDelete("{CartProductId}")]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> DeleteProductInCart(int CartProductId)
        {
            try
            {
                var results = await _cartService.DeleteProductInCart(CartProductId);
                return Ok(new { status = 200, results });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.CustomMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Đã xảy ra lỗi không xác định", errorMessage = ex.Message });
            }
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        [HttpPut]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> UpdateQuantityInCart([FromBody] UpdateQuantityInCartRequest request)
        {
            try
            {
                var results = await _cartService.UpdateQuantityInCart(request);
                return Ok(new { status = 200, results });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.CustomMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Đã xảy ra lỗi không xác định", errorMessage = ex.Message });
            }
        }

        // Lấy danh sách sản phẩm trong giỏ hàng
        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> GetListProductAsync()
        {
            try
            {
                var user = HttpContext.User;
                var results = await _cartService.GetListProductAsync(user);
                return Ok(new { status = 200, results });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.CustomMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Đã xảy ra lỗi không xác định", errorMessage = ex.Message });
            }
        }

        // Lấy TỔNG TIỀN TRONG GIỎ
        [HttpGet ("{userId}")]
      //  [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> GetCartSummaryAsync(string userId)
        {
            try
            {
                var user = HttpContext.User;
                var results = await _cartService.GetCartSummaryAsync(userId);
                return Ok(new { status = 200, results });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.CustomMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Đã xảy ra lỗi không xác định", errorMessage = ex.Message });
            }
        }
    }
}
