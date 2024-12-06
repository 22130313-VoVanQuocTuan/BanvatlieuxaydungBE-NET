

using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class Category
    {
        [Key]
        public int CategotyId { get; set; }
        public string? Name { get; set; }
        
        // Quan hệ với Product
       public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
