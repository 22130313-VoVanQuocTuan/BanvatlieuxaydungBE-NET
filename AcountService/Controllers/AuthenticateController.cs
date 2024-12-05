using AcountService.AppException;
using AcountService.dto.request.accountservice;
using AcountService.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly Authenticate _authenticate;

        public AuthenticateController(Authenticate authenticate)
        {
            _authenticate = authenticate;
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] UserLoginRequest userLoginRequest)
        {
            try
            {
                var result = await _authenticate.LoginAsync(userLoginRequest);
                return Ok(new { status = 200, result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Có lỗi xảy ra: " ,errorMessage = ex.Message });
            }
        }

        // REFRESH TOKEN
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenRefreshRequest request)
        {
            try
            {
                var result = await _authenticate.RefreshTokenAsync(request);
                return Ok(new { status = 200, message = result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Có lỗi xảy ra: " , errorMessage = ex.Message });
            }
        }

        // LOGOUT
        [HttpDelete("logout-user")]
        public async Task<IActionResult> LogoutUser([FromBody] LogoutRequest request)
        {
            try
            {
                var result = await _authenticate.LogoutUserAsync(request);
                return Ok(new { status = 200, message = result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Có lỗi xảy ra: " , errorMessage = ex.Message });
            }
        }
    }
}
