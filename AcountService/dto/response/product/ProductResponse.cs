namespace AcountService.dto.response.product
{
    public class ProductResponse
    {
        public int ProductId { get; set; }
        public string UrlImage { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } // Số lượng trong giỏ
    }
}
