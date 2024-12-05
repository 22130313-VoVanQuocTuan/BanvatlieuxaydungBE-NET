using AcountService.entity;

namespace AcountService.dto.response.Category
{
    public class CategoryResponse
    {
        public int CategotyId { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
