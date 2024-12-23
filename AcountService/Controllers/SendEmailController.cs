using AcountService.dto.request.email;
using AcountService.entity;
using AcountService.service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ForgotPasswordRequest = BanVatLieuXayDung.dto.request.email.ForgotPasswordRequest;
using ResetPasswordRequest = BanVatLieuXayDung.dto.request.email.ResetPasswordRequest;

namespace AcountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly UserManager<User> _userManager;

        public SendEmailController(EmailService emailService, UserManager<User> userManager)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        //Xác thực email
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfrimRequest request)
        {
            try
            {
                var result = await _emailService.ConfirmEmailAsync(request);
                return Ok(new { status = 200, mailVerificationCode = "đã xác thực", result });

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }
        // API gửi email reset mật khẩu
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {


                // Gửi email đặt lại mật khẩu với token
                await _emailService.SendPasswordResetEmailAsync(request);
                return Ok(new { message = "Email đặt lại mật khẩu đã được gửi tới bạn." });
            }
            catch (Exception ex)
            {
                // Đảm bảo trả về lỗi với trường message
                return BadRequest(new { message = ex.Message });
            }
        }

        // API đặt lại mật khẩu mới
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
        {
            // Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid)
            {
                // Trả về danh sách lỗi
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    status = 400,
                    message = "Dữ liệu không hợp lệ",
                    error = errors
                });
            }
            try
            {
                // Đặt lại mật khẩu với token và mật khẩu mới
                var result = _emailService.ResetPasswordAsync(request);
                return Ok( new { status = 200 ,result});
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    status = 500,
                    message = "Có lỗi xảy ra, vui lòng thử lại",
                    error = e.Message
                });
            }
        }
    }
}