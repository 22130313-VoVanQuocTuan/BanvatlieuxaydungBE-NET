using AcountService.dto.response.product;

namespace AcountService.dto.response.Cart
{
    public class ProductInCartResponse
    {
        public int CartProductId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlImage { get; set; }
        public decimal Price { get; set; }  // Giá từng sản phẩm
        public int Quantity { get; set; }  // Số lượng trong giỏ
        public decimal TotalPrice { get; set; }  // Tổng giá = Giá * Số lượng





    }
}
