﻿namespace BanVatLieuXayDung.dto.request.email
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
       
    }
}
