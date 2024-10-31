namespace AcountService.dto.request.cart
{
    public class UpdateQuantityInCartRequest
    {
        public int CartProductId { get; set; } // ID sản phẩm trong giỏ hàng
        public int Quantity { get; set; } // Số lượng mới
    }
}
