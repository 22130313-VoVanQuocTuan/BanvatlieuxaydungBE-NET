using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        // Foreign keys
        public string UserId { get; set; }
        public User User { get; set; }
        public decimal TotalPrice {  get; set; }
        public int TotalItems { get; set; }

        public decimal discount_amount { get; set; }

        public decimal shipping_fee { get; set; }

        // Navigation property to the join table
        public ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();


        // Thêm thuộc tính để theo dõi mã giảm giá đã được áp dụng hay chưa
        public string code { get; set; }
        public decimal code_discount { get; set; } // Giảm giá từ mã giảm giá
        public decimal promotion_discount { get; set; } // Giảm giá từ chương trình khuyến mãi

    }
}
