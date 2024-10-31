using System.ComponentModel.DataAnnotations;

namespace AcountService.dto.request.accountservice
{
    public class UserLoginRequest
    {
        [Required]
        [MinLength(3), MaxLength(20)]
        public string UserName { get; set; }
        [Required]
        [MinLength(3), MaxLength(20)]
        public string Password { get; set; }


    }
}
