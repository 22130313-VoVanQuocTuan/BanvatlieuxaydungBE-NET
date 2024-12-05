using AcountService.entity;
using AcountService.Repository;
using BanVatLieuXayDung.dto.request.payment;
using BanVatLieuXayDung.Library;
using System.Net;
using System.Web;

namespace BanVatLieuXayDung.service
{
    public class PaymentService
    {
        private readonly string _url;
        private readonly string _returnUrl;
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentService(IConfiguration configuration, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _url = configuration["VNPay:PaymentUrl"];
            _returnUrl = configuration["VNPay:ReturnUrl"];
            _tmnCode = configuration["VNPay:TmnCode"];
            _hashSecret = configuration["VNPay:HashSecret"];
            _httpContextAccessor = httpContextAccessor;
        }

        public string GeneratePaymentUrl(string amount, string infor, string orderinfor)
        {
            string hostName = Dns.GetHostName();
            string clientIPAddress = GetClientIPAddress();  // Có thể cần cải tiến cách lấy IP

            VnPayLibrary pay = new VnPayLibrary();

            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", _tmnCode);
            pay.AddRequestData("vnp_Amount", (int.Parse(amount) * 100).ToString());  // Đảm bảo đúng đơn vị tiền tệ
            pay.AddRequestData("vnp_BankCode", "");
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", clientIPAddress);
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", infor);
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", _returnUrl);
            pay.AddRequestData("vnp_TxnRef", orderinfor);

            // Generate and append the Secure Hash
            string secureHash = pay.CreateRequestUrl(_url, _hashSecret);
            return secureHash;
        }

        private string GetClientIPAddress()
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            return ipAddress ?? "0.0.0.0";  // Nếu không lấy được IP thì trả về mặc định
        }

    }

    }
