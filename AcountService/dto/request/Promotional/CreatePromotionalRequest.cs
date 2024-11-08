namespace BanVatLieuXayDung.dto.request.Promotional
{
    public class CreatePromotionalRequest
    {
        public int ProductId { get; set; } // ID sản phẩm liên kết với khuyến mãi
        public decimal DiscountPercentage { get; set; } // Tỷ lệ giảm giá
        public DateTime StartDate { get; set; } // Ngày bắt đầu khuyến mãi
        public DateTime EndDate { get; set; } // Ngày kết thúc khuyến mãi
    }
}
