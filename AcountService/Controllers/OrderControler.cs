using AcountService.dto.request.Order;
using AcountService.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderControler : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderControler(OrderService orderService)
        {
            _orderService = orderService;
        }

        //Lấy thông tin hóa đơn
        [HttpPost]
 
        public async Task<IActionResult> GetOrderInfo([FromBody] OrderRequest request)
        {
            try
            {
                // Lấy user từ Claims
                var user = HttpContext.User;
                var results = await _orderService.CreateOrder(request);
                return Ok(new { status = 200, results });

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, ex.Message });
            }
        }
    }
}

