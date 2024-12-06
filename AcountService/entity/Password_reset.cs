using AcountService.entity;
using System.ComponentModel.DataAnnotations;

namespace BanVatLieuXayDung.entity
{
    public class Password_reset
    {
        [Key]
        public int Id { get; set; } // Đặt tên thuộc tính viết hoa theo chuẩn PascalCase

        public string? ResetToken { get; set; } // Tên thuộc tính viết hoa
        public DateTime TokenExpiry { get; set; }

        // Khóa ngoại liên kết với bảng User
        public string UserId { get; set; } // Đặt theo PascalCase để nhất quán
        public User User { get; set; } // Quan hệ tới thực thể User

    }
}
