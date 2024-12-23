using System.ComponentModel.DataAnnotations;

namespace AcountService.dto.request.accountservice
{
    public class UserCreateRequest
    {
        [Required(ErrorMessage = "Username không được để trống")]
        [MinLength(3, ErrorMessage = "Username phải có ít nhất 3 ký tự")]
        [MaxLength(20, ErrorMessage = "Username không được vượt quá 20 ký tự")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password không được để trống")]
        [MinLength(3, ErrorMessage = "Password phải có ít nhất 3 ký tự")]
        [MaxLength(20, ErrorMessage = "Password không được vượt quá 20 ký tự")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }



    }
}
