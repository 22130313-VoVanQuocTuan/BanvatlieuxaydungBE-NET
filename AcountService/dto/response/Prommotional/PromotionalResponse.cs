namespace BanVatLieuXayDung.dto.response.Prommotional
{
    public class PromotionalResponse
    {
        public int SaleId { get; set; } // ID của khuyến mãi (khóa chính)
        public int ProductId { get; set; } // ID sản phẩm liên kết với khuyến mãi
        public decimal DiscountPercentage { get; set; } // Tỷ lệ giảm giá
        public DateTime StartDate { get; set; } // Ngày bắt đầu khuyến mãi
        public DateTime EndDate { get; set; } // Ngày kết thúc khuyến mãi
        public string ProductName { get; set; } // Tên sản phẩm
        public string ImageUrl { get; set; } // URL hình ảnh sản phẩm
        public decimal Price { get; set; }
    }
}
