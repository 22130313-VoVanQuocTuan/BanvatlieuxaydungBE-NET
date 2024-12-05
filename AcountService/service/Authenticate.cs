using AcountService.AppException;
using AcountService.dto.request.accountservice;
using AcountService.dto.response.account;
using AcountService.entity;
using AcountService.Repository;
using BanVatLieuXayDung.Migrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AcountService.service
{
    public class Authenticate

    {
        public readonly DataContext _context;
        private readonly IConfiguration _configuration; //: Cho phép lớp của bạn truy cập vào cấu hình của ứng dụng
        private readonly UserManager<User> _userManager; //cung cấp các phương thức và thuộc tính để quản lý người dùng
        private readonly TokenValidationParameters _tokenValidationParameters;
        public Authenticate(DataContext context, IConfiguration configuration, UserManager<User> userManager, TokenValidationParameters tokenValidationParameters)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _tokenValidationParameters = tokenValidationParameters;
        }

        //Login và tạo token
        public async Task<TokenResponse> LoginAsync(UserLoginRequest request)
        {
            // Kiểm tra thông tin đăng nhập
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new CustomException("Tên đăng nhập hoặc mật khẩu không chính xác", 401);
            }

            // Tìm refresh token cũ trong cơ sở dữ liệu
            var oldRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == user.Id);

            // Nếu có refresh token cũ và nó chưa hết hạn, xóa nó
            if (oldRefreshToken != null)
            {
                _context.RefreshTokens.Remove(oldRefreshToken);
                await _context.SaveChangesAsync();
            }

            // Tạo token mới
            var newToken = await GenerateJwtToken(user);

            // Tạo refresh token mới
            var newRefreshToken = await GenerateRefreshTokenAsync();

            // Lưu refresh token mới vào cơ sở dữ liệu
            var storedNewRefreshToken = new RefreshTokens
            {
                Token = newRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(2),
                UserId = user.Id
            };
            await _context.RefreshTokens.AddAsync(storedNewRefreshToken);
            await _context.SaveChangesAsync();


            var role = await _userManager.GetRolesAsync(user);
            var userRole = role.FirstOrDefault( );
            return new TokenResponse
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(2),
                authenticated = true,
                Role = userRole
            };
        }


        // Refresh toke khi hết hạn
        private async Task<string> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        //logout 
        public async Task<string> LogoutUserAsync(LogoutRequest request)
        {
            try
            {
                // Xác thực token
                var validatedToken = VerifyToken(request.Token, true);
                var jti = validatedToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                // Kiểm tra xem token đã bị đánh dấu là không hợp lệ chưa
                if (await _context.InvalidatedTokens.AnyAsync(t => t.id == jti))
                {
                    throw new CustomException("Token đã được vô hiệu hóa trước đó.", 400); // Bad Request
                }

                // Thêm token vào danh sách không hợp lệ
                await _context.InvalidatedTokens.AddAsync(new InvalidatedToken
                {
                    id = jti,
                    expiryDate = validatedToken.ValidTo
                });

                await _context.SaveChangesAsync();
                return "Đăng xuất thành công";
            }
            catch (CustomException ex)
            {
                // Ném lại CustomException với thông điệp gốc và mã lỗi
                throw new CustomException(ex.CustomMessage, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ không xác định (có thể ghi log nếu cần)
                throw new CustomException($"Lỗi khi đăng xuất: {ex.Message}", 500); // Internal Server Error
            }
        }

        // Refresh token
        public async Task<TokenResponse> RefreshTokenAsync(TokenRefreshRequest request)
        {
            try
            {
                var refreshToken = request.RefreshToken;

                // Tìm refresh token cũ trong cơ sở dữ liệu
                var storedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);

                if (storedRefreshToken == null || storedRefreshToken.ExpiryDate <= DateTime.UtcNow)
                {
                    throw new CustomException("Refresh token không hợp lệ hoặc đã hết hạn.", 403);
                }

                var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
                if (user == null)
                {
                    throw new CustomException("Tài khoản không tồn tại", 404);
                }

                var newToken = await GenerateJwtToken(user, isRefresh: true);  // Đảm bảo tham số isRefresh là true

                // Tạo refresh token mới
                var newRefreshToken = await GenerateRefreshTokenAsync();

                // Xóa refresh token cũ khỏi cơ sở dữ liệu
                _context.RefreshTokens.Remove(storedRefreshToken);

                // Thêm refresh token mới vào cơ sở dữ liệu
                var storedNewRefreshToken = new RefreshTokens
                {
                    Token = newRefreshToken,
                    ExpiryDate = DateTime.UtcNow.AddDays(2),
                    UserId = user.Id // Ghi lại UserId của người dùng liên quan
                };
                await _context.RefreshTokens.AddAsync(storedNewRefreshToken);

                await _context.SaveChangesAsync();

                return new TokenResponse
                {
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiry = DateTime.UtcNow.AddDays(2),
                    authenticated = true
                };
            }
            catch (CustomException ex)
            {
                throw new CustomException(ex.CustomMessage, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                throw new CustomException($"Lỗi khi làm mới token: {ex.Message}", 500);
            }
        }
        // Tạo Token
        public async Task<string> GenerateJwtToken(User user, bool isRefresh = false)
        {
            // Kiểm tra xem người dùng có phải là null không
            if (user == null)
            {
                throw new CustomException("Người dùng không hợp lệ.", 400); // Bad Request
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]);

            // Lấy danh sách vai trò của người dùng
            var roles = await _userManager.GetRolesAsync(user);

            // Tạo danh sách claims bao gồm thông tin người dùng và vai trò
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("UserId", user.Id.ToString()) // Thêm UserID vào claim
    };

            // Thêm các vai trò vào claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Tạo SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Sử dụng danh sách claims đã tạo ở trên
                Expires = isRefresh
            ? DateTime.UtcNow.AddHours(1)  // Nếu là refresh token, set thời gian lớn hơn (ví dụ: 1 giờ)
            : DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpirationInMinutes"])), // Thời gian sống mặc định cho access token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"], // Issuer từ appsettings.json
                Audience = _configuration["Jwt:Audience"] // Audience từ appsettings.json
            };

            // Tạo token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token); // Trả về token JWT dưới dạng string
        }

        //Xác thực token
        public JwtSecurityToken VerifyToken(string token, bool isRefresh)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;

            try
            {
                // Xác thực token và lấy claims
                var claimsPrincipal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out validatedToken);
            }
            catch (SecurityTokenExpiredException)
            {
                throw new CustomException("Token đã hết hạn.", 401); // Unauthorized
            }
            catch (SecurityTokenException)
            {
                throw new CustomException("Token không hợp lệ.", 401); // Unauthorized
            }

            // Kiểm tra loại của validatedToken
            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                throw new CustomException("Không có quyền truy cập.", 401); // Forbidden
            }

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            // Kiểm tra token có bị vô hiệu hóa hay không
            if (_context.InvalidatedTokens.Any(t => t.id == jti))
            {
                throw new CustomException("Token đã bị vô hiệu hóa.", 401); // Forbidden
            }

            // Kiểm tra thời hạn của token
            var expiration = isRefresh
                ? jwtToken.ValidFrom.AddMinutes(double.Parse(_configuration["Jwt:refreshable-duration"]))
                : jwtToken.ValidTo;

            if (expiration <= DateTime.UtcNow)
            {
                throw new CustomException("Token đã hết hạn.", 401); // Unauthorized
            }

            return jwtToken;
        }
    }
}