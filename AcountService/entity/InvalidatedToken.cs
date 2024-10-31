using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class InvalidatedToken
    {
        [Key]
        public string id { get; set; } 
        public DateTime expiryDate { get; set; }

    }
}
