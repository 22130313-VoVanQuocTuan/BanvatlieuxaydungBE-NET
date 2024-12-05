using System.ComponentModel.DataAnnotations;

namespace BanVatLieuXayDung.dto.request
{
    public class UpdateInfoShippingRequest
    {

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Họ tên không để trống")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không để trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "số diện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; }

        public string Note { get; set; }

    
        public string UserId { get; set; }

    }
}
