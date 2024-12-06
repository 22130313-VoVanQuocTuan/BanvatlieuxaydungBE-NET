using System.ComponentModel.DataAnnotations;

namespace AcountService.dto.response.product
{
    public class ProductDetailResponse
    {

       
        public int ProductId {  get; set; }
        public string Name { get; set; }


        public string Description { get; set; }


        public double Price { get; set; }


        public string UrlImage { get; set; } // Sử dụng IFormFile để nhận tệp hình ảnh từ người dùng


        public int CategoryId { get; set; }


        public int StockQuantity { get; set; }
        public decimal? DiscountPercentage { get; set; } 
        public  string Status { get; set; }
        public DateTime CreatedAt { get; set; } // Ngày tạo sản phẩm
    }
}