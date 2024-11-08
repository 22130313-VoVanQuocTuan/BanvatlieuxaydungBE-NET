using AcountService.entity;
using System.ComponentModel.DataAnnotations;

namespace BanVatLieuXayDung.entity
{
    public class PromotionalProducts
    {
        [Key]
        public int SaleId { get; set; }  // Khóa chính
        public int ProductId { get; set; }  // Khóa ngoại tham chiếu đến bảng sản phẩm
        public decimal DiscountPercentage { get; set; }  // Tỷ lệ giảm giá
        public DateTime StartDate { get; set; }  // Ngày bắt đầu khuyến mãi
        public DateTime EndDate { get; set; }  // Ngày kết thúc khuyến mãi
        public string ConditionDescription { get; set; }  // Mô tả điều kiện khuyến mãi

        // Có thể thêm điều hướng nếu cần thiết
        public virtual Product Product { get; set; }  // Định nghĩa mối quan hệ với thực thể Product
    }
}
