using AcountService.entity;
using AcountService.Repository;
using BanVatLieuXayDung.dto.request.payment;
using BanVatLieuXayDung.dto.response.payment;
using BanVatLieuXayDung.Library;
using BanVatLieuXayDung.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace BanVatLieuXayDung.Controllers
{
    [Area("VNPayAPI")]
    [Route("api/vnpay")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly string _url;
        private readonly string _returnUrl;
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly PaymentService _vnPayService;
        private readonly DataContext _context;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IConfiguration configuration, PaymentService vnPayService, DataContext context, ILogger<PaymentController> logger)
        {
            _url = configuration["VNPay:PaymentUrl"];
            _returnUrl = configuration["VNPay:ReturnUrl"];
            _tmnCode = configuration["VNPay:TmnCode"];
            _hashSecret = configuration["VNPay:HashSecret"];
            _vnPayService = vnPayService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("payment")]
        public IActionResult Payment([FromQuery] string amount, [FromQuery] string infor, [FromQuery] string orderinfor)
        {
            try
            {
                if (string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(infor) || string.IsNullOrEmpty(orderinfor))
                {
                    return BadRequest(new { message = "Missing required parameters" });
                }

                string paymentUrl = _vnPayService.GeneratePaymentUrl(amount, infor, orderinfor);
                return Ok(new { paymentUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating payment URL");
                return BadRequest(new { message = "Error generating payment URL", error = ex.Message });
            }
        }

        [HttpPost("paymentconfirm")]
        public IActionResult PaymentConfirm([FromQuery] string vnp_TxnRef, [FromQuery] string vnp_OrderInfo,
                                    [FromQuery] string vnp_TransactionNo, [FromQuery] string vnp_ResponseCode,
                                    [FromQuery] string vnp_SecureHash)
        {
            try
            {
                string vnp_HashSecret = "02TSIRUQB162HG88ANX5M30QUEW1LX1B"; // Bạn cần lấy key này từ cấu hình VNPay của bạn

                // Xây dựng chuỗi dữ liệu cần ký
                string dataToSign = $"vnp_TxnRef={vnp_TxnRef}&vnp_OrderInfo={vnp_OrderInfo}&vnp_TransactionNo={vnp_TransactionNo}&vnp_ResponseCode={vnp_ResponseCode}";

                // In ra chuỗi cần ký để debug
                Console.WriteLine($"Data to Sign: {dataToSign}");

                // Tính toán chữ ký
                string myChecksum = VnPayLibrary.HmacSHA512(vnp_HashSecret, dataToSign);

                // In ra chữ ký tính toán để so sánh
                Console.WriteLine($"My Checksum: {myChecksum}");
                Console.WriteLine($"Received Secure Hash: {vnp_SecureHash}");

                // So sánh chữ ký
                if (myChecksum.Equals(vnp_SecureHash, StringComparison.OrdinalIgnoreCase))
                {
                    // Chữ ký hợp lệ, thực hiện các xử lý thanh toán
                    var order = _context.Orders.FirstOrDefault(o => o.TrackingNumber == vnp_TxnRef);
                    if (order != null)
                    {
                        order.Status = "Paid";
                        _context.SaveChanges();
                    }
                    return Ok(new { message = "Payment successful" });
                }
                else
                {
                    return Unauthorized(new { message = "Invalid signature" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
