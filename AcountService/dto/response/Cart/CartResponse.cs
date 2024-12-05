namespace BanVatLieuXayDung.dto.response.Cart
{
    public class CartResponse
    {
        public decimal Price { get; set; } 
        public decimal TotalPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public int TotalItem { get; set; }
        public decimal ShippingFee { get; set; }    
    }
}
