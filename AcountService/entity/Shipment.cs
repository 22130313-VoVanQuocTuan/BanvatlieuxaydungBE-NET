using AcountService.status;
using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class Shipment
    {
        [Key]
        public int ShipmentId { get; set; } // Khóa chính cho Shipment
        public string ShippingAddress { get; set; } // Địa chỉ giao hàng
        public DateTime ShippingDate { get; set; } // Ngày giao hàng
        public string ShipmentMethod { get; set; } // Phương thức giao hàng
        public string TrackingNumber { get; set; } // Số theo dõi đơn hàng

        public int OrderId { get; set; } // Khóa ngoại đến đơn hàng
        public Order Order { get; set; } // Liên kết đến đơn hàng
    }
}
