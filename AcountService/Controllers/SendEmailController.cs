using AcountService.dto.request.email;
using AcountService.entity;
using AcountService.service;
using AcountService.status;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailController : ControllerBase
    {
        private  readonly EmailService _emailService;

        public SendEmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        //Xác thực email
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfrimRequest request)
        {
            try
            {
                var result = await _emailService.ConfirmEmailAsync(request);
                return Ok(new {status =200, mailVerificationCode = Status.VERIFIED, result });

            }
            catch (Exception ex)
            {
                return BadRequest(new {status =500, message = ex.Message });
            }
        }


    }
}
