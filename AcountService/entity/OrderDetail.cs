using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; } // Khóa ngoại trỏ tới Order
        public Order Order { get; set; }

        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
   

    }
}
