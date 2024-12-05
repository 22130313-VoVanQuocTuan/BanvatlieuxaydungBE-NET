namespace BanVatLieuXayDung.dto.request.payment
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; } // Số tiền thanh toán
        public string Method { get; set; } // Phương thức thanh toán (VNPay, Credit Card, etc.)
        public int OrderId { get; set; } // ID đơn hàng liên kết
    }
}
