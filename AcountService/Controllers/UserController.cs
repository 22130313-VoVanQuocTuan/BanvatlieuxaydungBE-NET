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
                // Gọi phương thức tạo user từ service
                var userResponse = await _userService.CreateUserAsync(userCreateRequest);
                return Ok(new
                {
                    status = 200,
                    emailVerificationCode = "Chưa xác thực",
                    result = userResponse
                }); // Trả về HTTP 200 OK nếu thành công
            }
            catch (Exception ex)
            {
                // Trả về lỗi chung với mã HTTP 500
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Đã xảy ra lỗi trong quá trình xử lý",
                    error = ex.Message
                });
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
                // Trả về lỗi chung với mã HTTP 500
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Đã xảy ra lỗi trong quá trình xử lý",
                    error = ex.Message
                });
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
                // Trả về lỗi chung với mã HTTP 500
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Đã xảy ra lỗi trong quá trình xử lý",
                    error = ex.Message
                });
            }
        }

        // CẬP NHẬT TÀI KHOẢN
        [HttpPut("{id}")]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                // Trả về lỗi validation
                return BadRequest(new
                {
                    status = 400,
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                var result = await _userService.UpdateUserAsync(id, request);
                return Ok(new { status = 200, message = result });
            }
            catch (Exception ex)
            {
                // Trả về lỗi chung với mã HTTP 500
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Đã xảy ra lỗi trong quá trình xử lý",
                    error = ex.Message
                });
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
                // Trả về lỗi chung với mã HTTP 500
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Đã xảy ra lỗi trong quá trình xử lý",
                    error = ex.Message
                });
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
                // Trả về lỗi chung với mã HTTP 500
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Đã xảy ra lỗi trong quá trình xử lý",
                    error = ex.Message
                });
            }
        }
    }
}
