using AcountService.status;
using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class Payment
    {
        [Key]

        public int PaymentId { get; set; } // Khóa chính cho Payment
        public decimal Amount { get; set; } // Số tiền thanh toán
        public string Method { get; set; } // Phương thức thanh toán (ví dụ: Credit Card, PayPal)
        public DateTime PaymentDate { get; set; } // Ngày thanh toán
        public bool IsSuccessful { get; set; } // Trạng thái thanh toán

        // Các trường bổ sung cho VNPay
        public string? VnpTransactionNo { get; set; } // Mã giao dịch VNPay
        public string? VnpSecureHash { get; set; } // Mã bảo mật SHA256 từ VNPay
        public string? VnpTmnCode { get; set; } // Mã định danh cửa hàng/tài khoản VNPay
        public string? VnpReturnUrl { get; set; } // URL trả về sau thanh toán

        public int OrderId { get; set; } // Khóa ngoại đến đơn hàng
        public Order Order { get; set; } // Liên kết đến đơn hàng
    }
}