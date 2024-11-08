using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class CartProduct
    {
        [Key]
        public int CartProductId { get; set; }

        // Foreign keys
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        // Thêm thuộc tính giá
        public decimal Price { get; set; } // Giá của sản phẩm tại thời điểm thêm vào giỏ hàng
        public decimal TotalPrice { get; set; }     // Giá tổng
    }
}
