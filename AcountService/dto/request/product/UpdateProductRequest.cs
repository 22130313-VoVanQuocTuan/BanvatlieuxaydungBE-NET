using System.ComponentModel.DataAnnotations;

namespace AcountService.dto.request.product
{
    public class UpdateProductRequest
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Mô tả sản phẩm không được để trống.")]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        public decimal Price { get; set; }

      
        public IFormFile? UrlImage { get; set;} // Sử dụng IFormFile để nhận tệp hình ảnh từ người dùng

        [Required(ErrorMessage = "Danh mục sản phẩm không được để trống.")]
        public int CategoryId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải không âm.")]
        public int StockQuantity { get; set; }

        [Required]
        public string Status {  get; set; }
       

    }
}
