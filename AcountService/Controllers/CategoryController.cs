using AcountService.AppException;
using AcountService.dto.request.Category;
using AcountService.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Thêm danh mục
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AddCategoryAsync([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var result = await _categoryService.AddCategoryAsync(request);
                return Ok(new { status = 200, result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.CustomMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Đã xảy ra lỗi không xác định", errorMessage = ex.Message });
            }
        }

        // Xóa danh mục
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                return Ok(new { status = 200, result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.CustomMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Đã xảy ra lỗi không xác định", errorMessage = ex.Message });
            }
        }

        // Cập nhật danh mục
        [HttpPut("{categoryId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateCategoryAsync(int categoryId, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                var result = await _categoryService.UpdateCategoryAsync(categoryId, request);
                return Ok(new { status = 200, result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.CustomMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Đã xảy ra lỗi không xác định", errorMessage = ex.Message });
            }
        }

        // Lấy danh sách danh mục
        [HttpGet("category")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> getAllListCategoryAsync()
        {
            try
            {
                var result = await _categoryService.GetListCategoryAsync();
                return Ok(new { status = 200, result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.CustomMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Đã xảy ra lỗi không xác định", errorMessage = ex.Message });
            }
        }
    }
}
