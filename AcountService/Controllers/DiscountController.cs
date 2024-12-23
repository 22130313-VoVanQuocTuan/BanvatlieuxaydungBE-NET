using AcountService.dto.request.discount;
using AcountService.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        public readonly DiscountService _discountService;

        public DiscountController(DiscountService discountService)

        {
            _discountService = discountService;
        }

        // API tạo mã giảm giá

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]


        //Tạo mã giảm giá
        public async Task<IActionResult> CreateDiscount([FromBody] DiscountResquest resquest)
        {
            try
            {
                var response = await _discountService.CreateDiscount(resquest);
                return Ok(new { status = 200, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 300, ex.Message });
            }
        }


        //Xác nhận mã giảm giá
        [HttpGet("{code}/{UserId}")]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> ConfrimCode(string code, string UserId)
        {
            try
            {
                var response = await _discountService.ConfrimCode(code, UserId);
                return Ok(new { status = 200, response });
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        //Lấy danh sách mã giảm giá
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> getAllDiscountAsync()
        {
            try
            {
                var response = await _discountService.getDiscountAsync();
                return Ok(new { status = 200, response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //Xóa mã giảm giá
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteDiscountAsync(int id)
        {
            try
            {
                var response = await _discountService.DeleteDiscountAsync(id);
                return Ok(new { status = 200, response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
