using AcountService.AppException;
using AcountService.dto.request.product;
using AcountService.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // Thêm sản phẩm
        [HttpPost("product")]
        
        public async Task<IActionResult> AddProductAsync(CreateProductRequest request)
        {
            try
            {
                var result = await _productService.createProductAsync(request);
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

        // Cập nhật sản phẩm
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateProductAsync(int id,  UpdateProductRequest request)
        {
            try
            {
                var result = await _productService.updateProductAsync(id, request);
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

        // Xóa sản phẩm
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProduct(id);
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

        // Lấy danh sách sản phẩm
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]

        public async Task<IActionResult> GetListProductAsync([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var results = await _productService.GetListProduct(page, size);
                return Ok(new { status = 200, results });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { status = ex.ErrorCode, message = ex.CustomMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = "Đã xảy ra lỗi không xác định",  errorMessage = ex.Message });
            }
        }

        // Lấy sản phẩm theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductAsync(int id)
        {
            try
            {
                var result = await _productService.getProductAsync(id);
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

        // Cập nhật tồn kho sản phẩm
        [HttpPut("Stock/{id}")]
 
        public async Task<IActionResult> UpdateStockAsync([FromBody] UpdateStockProductRequest request)
        {
            try
            {
                var response = await _productService.UpdateStock(request);
                return Ok(new { status = 200, response });
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

        // Lấy danh sách sản phẩm theo danh mục
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetListProductInCategoryId(int categoryId, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var results = await _productService.GetListProductInCategoryId(page, size, categoryId);
                return Ok(new { status = 200, results });
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
