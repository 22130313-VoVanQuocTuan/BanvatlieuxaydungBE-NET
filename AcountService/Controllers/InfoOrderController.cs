using BanVatLieuXayDung.dto.request;
using BanVatLieuXayDung.service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BanVatLieuXayDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoOrderController : ControllerBase
    {
        public readonly InfoUserShippingService _infoUserShipping;

        public InfoOrderController(InfoUserShippingService infoUserShipping)
        {
            _infoUserShipping = infoUserShipping;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInforOder([FromBody] UpdateInfoShippingRequest request)
        {
            try
            {
                // Kiểm tra tính hợp lệ của model
                if (!ModelState.IsValid)
                {
                    // Nếu model không hợp lệ, trả về thông báo lỗi
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                   .Select(e => e.ErrorMessage)
                                                   .ToList();
                    return BadRequest(errors);  // Trả về lỗi dưới dạng BadRequest
                }
                var response = await _infoUserShipping.UpdateInfoShipping(request); 
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return StatusCode(500, "Đã xảy ra lỗi khi thêm thông tin vận chuyển. Chi tiết: " + ex.Message);
            }
        }
        [HttpGet("userId")]
        public async Task<IActionResult> GetAllInfoShipping(string userId)
        {
            try
            {
                var response = await _infoUserShipping.GetInfoShipping(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
