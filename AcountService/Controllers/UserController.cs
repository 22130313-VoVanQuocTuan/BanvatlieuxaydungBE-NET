using AcountService.dto.request.accountservice;
using AcountService.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // TẠO TÀI KHOẢN
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest userCreateRequest)
        {
            try
            {
                // Gọi phương thức tạo user từ service
                var userResponse = await _userService.CreateUserAsync(userCreateRequest);
                return Ok(new
                {
                    status = 200,
                    emailVerificationCode = "Chứ xác thực",
                    result = userResponse
                }); // Trả về HTTP 200 OK nếu thành công
            }
            catch (Exception ex)
            {
                // Trả về lỗi chung với mã HTTP 400
                return BadRequest(new { message = ex.Message });
            }
        }

        // LẤY DANH SÁCH TẤT CẢ USER
        [HttpGet("users")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await _userService.getAllUserAsync();
                return Ok(new { status = 200,  result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // LẤY THÔNG TIN TÀI KHOẢN ĐANG ĐĂNG NHẬP
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var result = await _userService.getUserAsync(id);
                return Ok(new { status = 200, message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }

        // CẬP NHẬT TÀI KHOẢN
        [HttpPut("{id}")]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateRequest request)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(id, request);
                return Ok(new { status = 200, message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }

        // XÓA TÀI KHOẢN
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                return Ok(new { status = 200, message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }

        // Số người dùng
        [HttpGet("number")]
     
        public async Task<IActionResult> getNumberUser()
        {
            try
            {
                var result = await _userService.GetTotalUserRateRevenueAsync();
                return Ok(new { status = 200, message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, message = ex.Message });
            }
        }
    }
}
