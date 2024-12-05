namespace BanVatLieuXayDung.dto.response.payment
{
    public class PaymentResponse
    {
        public int PaymentId { get; set; } // ID giao dịch
        public decimal Amount { get; set; } // Số tiền thanh toán
        public string Method { get; set; } // Phương thức thanh toán
        public DateTime PaymentDate { get; set; } // Ngày thanh toán
        public bool IsSuccessful { get; set; } // Trạng thái thanh toán
        public string? VnpTransactionNo { get; set; } // Mã giao dịch VNPay
        public string? VnpResponseCode { get; set; } // Mã phản hồi từ VNPay
    }
}
