using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class EmailVerificationCode
    {
        [Key]
        public int Id { get; set; } // Khóa chính (ID) cho mã xác thực
        public User User { get; set; }
        public string UserId { get; set; } // ID của người dùng mà mã xác thực này liên kết
        public string? Email { get; set; } // Địa chỉ email của người dùng
        public string? VerificationCode { get; set; } // Mã xác thực email (ví dụ: 4 hoặc 6 chữ số)
        public DateTime CreatedAt { get; set; } // Thời gian tạo mã xác thực
        public DateTime ExpiresAt { get; set; } // Thời gian hết hạn của mã xác thực
        public bool IsUsed { get; set; } // Trạng thái cho biết mã xác thực đã được sử dụng chưa
    }
}
