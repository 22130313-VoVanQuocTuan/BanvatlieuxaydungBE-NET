using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using AcountService.entity;
using Microsoft.EntityFrameworkCore;
using AcountService.Repository;
using AutoMapper;
using AcountService.dto.request.email;
using BanVatLieuXayDung.entity;
using ForgotPasswordRequest = BanVatLieuXayDung.dto.request.email.ForgotPasswordRequest;
using ResetPasswordRequest = BanVatLieuXayDung.dto.request.email.ResetPasswordRequest;
using System.Web;

namespace AcountService.service
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;
        public readonly IMapper _mapper;

        public EmailService(IConfiguration configuration, UserManager<User> userManager, DataContext context, IMapper mapper)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var smtpClient = new SmtpClient(smtpSettings["Server"])
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = true  // bật ssl để kết nối tới SMTP
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["SenderEmail"], smtpSettings["SenderName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);
            smtpClient.Send(mailMessage);
        }


        //ConfirmEmailAsync
        public async Task<string> ConfirmEmailAsync(EmailConfrimRequest request)
        {
            try
            {

                // Tìm mã xác thực trong cơ sở dữ liệu
                var verificationCode = await _context.EmailVerificationCodes
                        .FirstOrDefaultAsync(v => v.Email == request.Email && v.VerificationCode == request.Code);
                if (verificationCode == null)
                {
                    throw new Exception("Mã xác thực không hợp lệ.");
                }
                if (DateTime.Now > verificationCode.ExpiresAt || verificationCode.IsUsed == true)
                {
                    // Xóa mã khỏi cơ sở dữ liệu khi đã hết hạn
                    _context.EmailVerificationCodes.Remove(verificationCode);
                    await _context.SaveChangesAsync();

                    var userNoEmailConfirmed = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                    if (userNoEmailConfirmed != null && !userNoEmailConfirmed.EmailConfirmed)
                    {
                        // Xóa người dùng nếu chưa được xác thực
                        _context.Users.Remove(userNoEmailConfirmed);
                    }
                    await _context.SaveChangesAsync();
                    throw new Exception("Mã xác thực đã hết hạn.");

                }

                // Cập nhật EmailConfirmed thành true
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    throw new Exception("Không tìm thấy người dùng.");
                }
                // Thiết lập EmailConfirmed  là true khi xác thực thành công
                user.EmailConfirmed = true;
                await _context.SaveChangesAsync();

                _context.EmailVerificationCodes.Remove(verificationCode);
                // Tìm người dùng chưa được xác thực


                return "Xác thực thành công";
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần thiết và trả về thông báo lỗi
                throw new Exception($"Đã xảy ra lỗi: {ex.Message}");


            }


        }

        // Xác thực mã code
        public async Task<EmailVerificationCode> GenerateVerificationCode(string email, string id)
        {
            // Tạo mã xác thực ngẫu nhiên (ví dụ: 4 chữ số)
            var random = new Random();
            var verificationCode = random.Next(1000, 9999).ToString(); // Tạo mã từ 1000 đến 9999

            // Tạo đối tượng EmailVerificationCode
            var emailVerificationCode = new EmailVerificationCode
            {

                Email = email,
                UserId = id,
                VerificationCode = verificationCode,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(1) // Ví dụ: mã sẽ hết hạn sau 1 phút
            };

            // Lưu mã vào cơ sở dữ liệu
            await _context.EmailVerificationCodes.AddAsync(emailVerificationCode);
            await _context.SaveChangesAsync();

            return emailVerificationCode;
        }


        // Phương thức gửi email chứa liên kết đặt lại mật khẩu
        public async Task SendPasswordResetEmailAsync(ForgotPasswordRequest request)
        {
            // Kiểm tra xem email có tồn tại trong cơ sở dữ liệu không
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
           
                // Tránh tiết lộ thông tin người dùng
                throw new Exception("email không tồn tại");
            }

            // Kiểm tra xem đã có token reset cho user này chưa
            var passwordReset = await _context.password_Resets.FirstOrDefaultAsync(e => e.UserId == user.Id);

            // Tạo token reset mật khẩu
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (passwordReset == null)
            {
                // Nếu chưa có token, tạo mới
                var newPasswordReset = new Password_reset
                {
                    ResetToken = resetToken,
                    TokenExpiry = DateTime.UtcNow.AddHours(1), // Token có hiệu lực trong 1 giờ
                    UserId = user.Id
                };

                _context.password_Resets.Add(newPasswordReset);
                await _context.SaveChangesAsync(); // Lưu thông tin token vào cơ sở dữ liệu
            }
            else
            {
                // Nếu đã có token, cập nhật lại token và thời gian hết hạn
                passwordReset.ResetToken = resetToken;
                passwordReset.TokenExpiry = DateTime.UtcNow.AddHours(1); // Cập nhật token và thời gian hết hạn

                await _context.SaveChangesAsync(); // Lưu thông tin token vào cơ sở dữ liệu
            }

            // Tạo liên kết đặt lại mật khẩu
            var resetLink = $"{_configuration["AppUrl"]}/reset-password.html?token={HttpUtility.UrlEncode(passwordReset?.ResetToken)}";
            var subject = "Yêu cầu đặt lại mật khẩu";
            var body = $"Nhấn vào <a href='{resetLink}'>đây</a> để đặt lại mật khẩu.";

            // Gửi email cho người dùng
            await SendEmailAsync(request.Email, subject, body);
        }

        //Thay đổi lại mật khẩu
        // Thay đổi lại mật khẩu
        public async Task<string> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                // Kiểm tra xem mật khẩu mới và xác nhận mật khẩu có khớp không
                if (request.NewPassword != request.ConfirmNewPassword)
                {
                    throw new Exception("Mật khẩu mới và xác nhận mật khẩu không khớp.");
                }

                // Tìm token trong cơ sở dữ liệu
                var passwordReset = await _context.password_Resets.FirstOrDefaultAsync(pr => pr.ResetToken == request.Token);

                if (passwordReset == null || passwordReset.TokenExpiry < DateTime.UtcNow)
                {
                    throw new Exception("Token không hợp lệ hoặc đã hết hạn.");
                }

                // Tìm người dùng liên quan đến token
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == passwordReset.UserId);
                if (user == null)
                {
                    throw new Exception("Người dùng không tồn tại.");
                }

                // Mã hóa mật khẩu mới
                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);

                // Cập nhật người dùng và xóa token
                _context.Users.Update(user);
                _context.password_Resets.Remove(passwordReset);

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return "Thay đổi mật khẩu thành công.";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
