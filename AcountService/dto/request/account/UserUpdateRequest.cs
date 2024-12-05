using System.ComponentModel.DataAnnotations;

namespace AcountService.dto.request.accountservice
{
    public class UserUpdateRequest
    {
    
        [Required]
     
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }


    }
}

