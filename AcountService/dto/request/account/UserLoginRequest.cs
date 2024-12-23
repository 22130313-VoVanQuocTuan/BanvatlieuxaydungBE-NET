using System.ComponentModel.DataAnnotations;

namespace AcountService.dto.request.accountservice
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "Tên người dùng là bắt buộc.")]
        [MinLength(3, ErrorMessage = "Tên người dùng phải có ít nhất 3 ký tự.")]
        [MaxLength(20, ErrorMessage = "Tên người dùng không được vượt quá 20 ký tự.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(3, ErrorMessage = "Mật khẩu phải có ít nhất 3 ký tự.")]
        [MaxLength(20, ErrorMessage = "Mật khẩu không được vượt quá 20 ký tự.")]
        public string Password { get; set; }

    }
}
