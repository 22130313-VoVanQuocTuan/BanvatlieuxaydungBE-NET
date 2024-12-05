using System.ComponentModel.DataAnnotations;

namespace AcountService.dto.request.product
{
    public class CreateProductRequest
    {

        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        public string Name { get; set; } = "";

    
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn một hình ảnh.")]
        public IFormFile? UrlImage { get; set; } // Sử dụng IFormFile để nhận tệp hình ảnh từ người dùng

        [Required(ErrorMessage = "Danh mục sản phẩm không được để trống.")]
        public int CategoryId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải không âm.")]
        public int StockQuantity { get; set; }

        public string Status { get; set; }


    }
}
