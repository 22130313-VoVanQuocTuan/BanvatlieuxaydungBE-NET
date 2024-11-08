using AcountService.status;
using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class Order
    {
        [Key]
        
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending; // Default status
        public bool IsPaid { get; set; } = false; // Payment status

        public string UserId { get; set; }
        public User User { get; set; }

        // Quan hệ 1-n với OrderDetails
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public Payment Payment { get; set; }
        public Shipment Shipment { get; set; }
        public string? TrackingNumber { get; set; } // Shipping tracking code
        public string OrderNote { get; set; }
        public InfoUserOrder InfoUserOrder { get; set; }




    }

}

