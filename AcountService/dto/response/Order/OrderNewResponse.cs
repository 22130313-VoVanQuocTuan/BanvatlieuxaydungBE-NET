namespace BanVatLieuXayDung.dto.response.Order
{
    public class OrderNewResponse
    {
        public int OrderId { get; set; }
        public decimal Price { get; set; }
        public string PaymentStatus { get; set; }
        public string Address { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
