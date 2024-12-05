

using System.ComponentModel.DataAnnotations;

namespace AcountService.dto.request.Category
{
    public class CreateCategoryRequest
    {
        [Required]        
        
        public string name {  get; set; }

   
    }
}
