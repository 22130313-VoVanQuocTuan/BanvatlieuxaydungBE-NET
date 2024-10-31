using System.ComponentModel.DataAnnotations;

namespace AcountService.dto.request.Category
{
    public class UpdateCategoryRequest
    {
        [Required]
        public string Name {  get; set; }
        [Required]
        public string Description { get; set; }
    }
}
