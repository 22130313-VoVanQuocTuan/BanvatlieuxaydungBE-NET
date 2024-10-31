using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using AcountService.entity;
using Microsoft.EntityFrameworkCore;
using AcountService.Repository;
using AcountService.dto.response;
using AutoMapper;
using AcountService.dto.request.email;

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

        public void SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var smtpClient = new SmtpClient(smtpSettings["Server"])
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = true
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
        public async Task<string> ConfirmEmailAsync( EmailConfrimRequest request)
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
    }
}
