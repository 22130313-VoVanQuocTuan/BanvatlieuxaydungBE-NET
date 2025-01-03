using AcountService.dto.request.Order;
using AcountService.service;
using BanVatLieuXayDung.dto.request.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        //Lấy thông tin hóa đơn
        [HttpPost]
        public async Task<IActionResult> GetOrderInfo([FromBody] OrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new { Errors = errors });
                }
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

        //Hủy đơn hàng
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> deleteOrder(int orderId)
        {
            try
            {
              
                // Lấy user từ Claims
             
                var results = await _orderService.deleteOrder(orderId);
                return Ok(new { status = 200, message = results });

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, ex.Message });
            }
        }

        //Xác nhận đơn hàng
        [HttpPost("{orderId}/{userId}")]
        public async Task<IActionResult> confirmOrder(int orderId, string userId)
        {
            try
            {

                // Lấy user từ Claims

                var results = await _orderService.confirmOrderCOD(orderId, userId);
                return Ok(new { status = 200, message = results });

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, ex.Message });
            }
        }


        //Lấy danh sách đơn hàng
        [HttpGet("order-new")]
        public async Task<IActionResult> getOrderNew()
        {
            try
            {
                // Lấy user từ Claims
                var results = await _orderService.getOrderNew();
                return Ok(new { status = 200, message = results });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, ex.Message });
            }
        }
        //Lấy danh sách người dùng mua hàng nhiều nhất top5
        [HttpGet("top-order")]
        public async Task<IActionResult> getListUserOrder()
        {
            try
            {
                // Lấy user từ Claims
                var results = await _orderService.GetTopUserOrder();
                return Ok(new { status = 200, message = results });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, ex.Message });
            }
        }


        //Lấy danh sách đơn hàng
        [HttpGet("order-user/{userId}")]
      
        public async Task<IActionResult> getOrderByUser(string userId)
        {
            try
            {
                // Lấy user từ Claims
                var results = await _orderService.getOrderByUser(userId);
                return Ok(new { status = 200, message = results });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 500, ex.Message });
            }
        }

        //Cập nhật trạng thái hóa đơn

        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateStatusRequest request)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, request.Status);
            if (!result)
            {
                return NotFound(new { message = "Order not found" });
            }

            return Ok(new { message = "Order status updated" });
        }

        //Cập nhật trạng thái thánh toán

        [HttpPut("{orderId}/payment-status")]
        public async Task<IActionResult> UpdatePaymentStatus(int orderId, [FromBody] UpdateStatusRequest request)
        {
            var result = await _orderService.UpdatePaymentStatusAsync(orderId, request.Status);
            if (!result)
            {
                return NotFound(new { message = "Order not found" });
            }

            return Ok(new { message = "Payment status updated" });
        }

        //Cập nhật trạng thái thanh toán khi thanh toán với VNPay

        [HttpPut("{trackingNumber}/VNPay-status")]
        public async Task<IActionResult> UpdatePaymentStatusVNPay(string trackingNumber)
        {
            var result = await _orderService.UpdatePaymentStatusVNPayAsync( trackingNumber);
            if (!result)
            {
                return NotFound(new { message = "Hóa đơn không tồn tại" });
            }

            return Ok(new { message = "Cập nhật thành công" });
        }



    }
}

