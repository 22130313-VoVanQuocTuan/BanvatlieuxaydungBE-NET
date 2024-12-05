namespace BanVatLieuXayDung.dto.response.Order
{
    public class OrderDetailResponse
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
