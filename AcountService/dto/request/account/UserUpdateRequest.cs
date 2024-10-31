using System.ComponentModel.DataAnnotations;

namespace AcountService.dto.request.accountservice
{
    public class UserUpdateRequest
    {
        [Required]
        [MinLength(3), MaxLength(20)]
        public string UserName { get; set; }
        [Required]
        [MinLength(3), MaxLength(20)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime? Dob { get; set; }

        public string? City { get; set; }
    }
}

