using AcountService.entity;

namespace AcountService.dto.request.Order
{
    public class OrderRequest
    {
        public string UserId { get; set; } // Thêm UserId
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        
      
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string ShipmentMethod { get; set; }
      
        public string OrderNote {  get; set; }

    }
}
