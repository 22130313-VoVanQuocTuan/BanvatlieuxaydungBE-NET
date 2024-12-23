using System.ComponentModel.DataAnnotations;

namespace BanVatLieuXayDung.dto.request.email
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Token không được để trống")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
        [MaxLength(100, ErrorMessage = "Mật khẩu mới không được vượt quá 100 ký tự")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu mới không được để trống")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp với mật khẩu mới")]
        public string ConfirmNewPassword { get; set; }
    }
}
