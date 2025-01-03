using AcountService.Repository;
using BanVatLieuXayDung.dto.request.payment;
using BanVatLieuXayDung.entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.Common;

[ApiController]
[Route("api/[controller]")]
public class VNPayController : ControllerBase
{
    private readonly VNPaySettings _vnPaySettings;
    private readonly DataContext _context;

    public VNPayController(IOptions<VNPaySettings> vnPaySettings)
    {
        _vnPaySettings = vnPaySettings.Value;
    }

    [HttpPost("payment")]
    [Authorize(Policy = "UserOnly")]
    public IActionResult CreatePayment([FromBody] PaymentRequest request)
    {
        if (request.Amount <= 0)
        {
            return BadRequest(new { message = "Số tiền không nhỏ hơn 0" });
        }

        // Kiểm tra thoogn tin
        if (string.IsNullOrEmpty(request.Orderinfor))
        {
            return BadRequest(new { message = "Thiếu thông tin đơn hàng." });
        }



        var vnpay = new VnPayLibrary();
        vnpay.AddRequestData("vnp_Version", "2.0.0");
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", _vnPaySettings.TmnCode);
        vnpay.AddRequestData("vnp_Amount", (request.Amount * 100).ToString());
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString());
        vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());
        vnpay.AddRequestData("vnp_OrderInfo", request.Orderinfor);
        vnpay.AddRequestData("vnp_OrderType", "your_category_code_here");
        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_ReturnUrl", _vnPaySettings.ReturnUrl);
        vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

        string paymentUrl = vnpay.CreateRequestUrl(_vnPaySettings.PaymentUrl, _vnPaySettings.HashSecret);
        return Ok(new { paymentUrl });
    }
}

