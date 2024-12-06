using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class Order
    {
        [Key]
        
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
       public string? Status { get; set; }
        public decimal shipping_fee { get; set; }
        public decimal   discount_amount { get; set; }
        public string? payment_method { get; set; }
        public string? payment_status { get; set; }
        public string? shipping_address {get; set; }
        public string UserId { get; set; }

        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Note { get; set; }
        public User User { get; set; }

        // Quan hệ 1-n với OrderDetails
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
      
        public string? TrackingNumber { get; set; } // Shipping tracking code
    
       



    }

}

