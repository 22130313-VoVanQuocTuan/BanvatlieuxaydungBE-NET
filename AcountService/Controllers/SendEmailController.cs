using AcountService.dto.request.email;
using AcountService.entity;
using AcountService.service;
using AcountService.status;
using BanVatLieuXayDung.dto.request.email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
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
                return Ok(new { status = 200, mailVerificationCode = Status.VERIFIED, result });

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

                return Ok("Email đặt lại mật khẩu đã được gửi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Trả về lỗi nếu email không tồn tại hoặc có lỗi trong quá trình gửi
            }
        }

        // API đặt lại mật khẩu mới
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
        {

            try
            {
                // Đặt lại mật khẩu với token và mật khẩu mới
                var result = _emailService.ResetPasswordAsync(request);
                return Ok( new { status = 200 ,result});
            }
            catch (Exception e)
            {
                return BadRequest( new { status = 500, e.Message });
            }
        }
    }
}