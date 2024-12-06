using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class InfoUserOrder
    {
        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }    
        public string? UserId { get; set; }
        public User Users { get; set; }
    }
}
