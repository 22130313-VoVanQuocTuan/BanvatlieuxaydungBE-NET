namespace AcountService.dto.response.account
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken {  get; set; }
        public bool authenticated { get; set; }
        public DateTime RefreshTokenExpiry { get; set; } // Thời gian hết hạn của refresh token

        public string Role {  get; set; }
    }
}
