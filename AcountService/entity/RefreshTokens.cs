using System.ComponentModel.DataAnnotations;

namespace BanVatLieuXayDung.entity
{
    public class RefreshTokens
    {
        [Key] 
        
        public int Id { get; set; } // Khóa chính
        public string Token { get; set; }
        public string UserId { get; set; } // ID người dùng
        public DateTime ExpiryDate { get; set; } // Ngày hết hạn
    }
}
