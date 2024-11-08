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
        [Authorize(Policy ="AdminOnly")]

        public async Task<IActionResult> CreateDiscount([FromBody] DiscountResquest resquest)
        {
            try
            {
                var response = await _discountService.CreateDiscount(resquest);
                return Ok(new { status = 200, response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{code}")]
        [Authorize]
         public async Task<IActionResult> ConfrimCode(string code)
        {
            try
            {
                var response = await _discountService.ConfrimCode(code);
                return Ok(new { status = 200, response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
