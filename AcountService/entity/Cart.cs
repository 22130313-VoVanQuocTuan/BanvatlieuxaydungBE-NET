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

        // Navigation property to the join table
        public ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();

       
    }
}
