using AcountService.AppException;
using AcountService.service;
using Azure.Core;
using BanVatLieuXayDung.dto.request.Promotional;
using BanVatLieuXayDung.service;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BanVatLieuXayDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionalController : ControllerBase
    {
        private readonly PromotionalService _promotionalService;

        public PromotionalController(PromotionalService promotionalService)
        {
            _promotionalService = promotionalService;
        }

        // Lấy tất cả các sản phẩm khuyến mãi

        [HttpGet]
        public async Task<IActionResult> GetAllPromotional()
        {
            try
            {
                var result = await _promotionalService.GetListPromotions();
                return Ok(new { status = 200, message = result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Có lỗi xảy ra: ", errorMessage = ex.Message });
            }
        }

        //Lấy sản phẩm khuyến mãi theo id
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetPromotional(int productId)
        {
            try
            {
                var result = await _promotionalService.GetPromotional(productId);
                return Ok(new { status = 200, message = result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Có lỗi xảy ra: ", errorMessage = ex.Message });
            }
        }

        //Tạo sản phẩm khuyến mãi
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
         public async Task<IActionResult> CreatePromotional([FromBody] CreatePromotionalRequest request)
        {
            try
            {
                var result = await _promotionalService.CreatePromotional(request);
                return Ok(new { status = 200, message = result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Có lỗi xảy ra: ", errorMessage = ex.Message });
            }
        }



        // Cập nhật khuyến mãi
        [HttpPut("{saleId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdatePromotional(int saleId, [FromBody] UpdatePromotionalRequest request)
        {
            try
            {
                var result = await _promotionalService.UpdatePromotional(saleId,request);
                return Ok(new { status = 200, message = result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Có lỗi xảy ra: ", errorMessage = ex.Message });
            }
        }



        //  Xóa khuyến mãi theo ID
        [HttpDelete("{saleId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeletePromotional(int saleId)
        {
            try
            {
                var result = await _promotionalService.DeletePromotional(saleId);
                return Ok(new { status = 200, message = result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Có lỗi xảy ra: ", errorMessage = ex.Message });
            }
        }
    }
}
