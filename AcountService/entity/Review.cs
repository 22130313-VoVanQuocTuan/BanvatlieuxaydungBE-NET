using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
       public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign keys
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
