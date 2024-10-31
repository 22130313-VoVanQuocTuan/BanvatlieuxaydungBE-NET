using AcountService.AppException;
using AcountService.dto.request.accountservice;
using AcountService.dto.response.account;
using AcountService.entity;
using AcountService.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
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
        public async Task<TokenResponse> loginUserAsync(UserLoginRequest userLoginRequest)
        {
            try
            {
                // Tìm kiếm người dùng theo UserName
                var user = await _userManager.FindByNameAsync(userLoginRequest.UserName);
                if (user == null)
                {
                    throw new CustomException("Tên tài khoản không chính xác", 404);
                }

                var role = await _userManager.GetRolesAsync(user);
                if (role.Contains("USER") && !user.EmailConfirmed)
                {
                    throw new CustomException("Chưa xác thực tài khoản", 401);
                }

                // Kiểm tra mật khẩu bằng cách sử dụng UserManager
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, userLoginRequest.Password);
                if (!isPasswordValid)
                {
                    throw new CustomException("Mật khẩu không chính xác", 401);
                }

                // Gọi phương thức GenerateJwtToken để tạo access token
                var tokenString = GenerateJwtToken(user);

                // Tạo refresh token (bạn có thể thay đổi cách tạo token này theo yêu cầu của bạn)
                var refreshToken = GenerateRefreshToken(); // Gọi phương thức để tạo refresh token
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(2); // Đặt thời gian hết hạn cho refresh token (30 ngày hoặc tùy chọn của bạn)



                // Trả về đối tượng 
                return new TokenResponse
                {
                    Token = await tokenString,
                    RefreshToken = refreshToken, // Trả về refresh token
                    authenticated = true
                }; // Trả về token cho client
            }
            catch (CustomException ex)
            {
                // Thay vì ném ra CustomException mới, trả về thông tin của ex
                throw new CustomException(ex.CustomMessage, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ không xác định (có thể ghi log nếu cần)
                throw new CustomException("Đã xảy ra lỗi không xác định", 500); // Internal Server Error
            }
        }


        // Refresh toke khi hết hạn
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
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
                var refreshToken = request.Token; // Gán token request vào refreshToken

                var storedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken); // Kiểm tra trong database

                if (storedRefreshToken == null || storedRefreshToken.ExpiryDate <= DateTime.UtcNow)
                {
                    throw new CustomException("Refresh token không hợp lệ hoặc đã hết hạn.", 403); // Forbidden
                }

                // Tìm người dùng từ refresh token (bạn có thể cần lưu thông tin người dùng kèm theo refresh token)
                var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId); // Lấy UserId từ storedRefreshToken

                if (user == null)
                {
                    throw new CustomException("Tài khoản không tồn tại", 404); // Not Found
                }

                // Tạo JWT mới cho người dùng
                var newToken = await GenerateJwtToken(user); // Đảm bảo rằng phương thức này trả về Task<string>

               // Cập nhật refresh token mới
                  var newRefreshToken = GenerateRefreshToken();
                  var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(2); // Cập nhật thời gian hết hạn


                // Lưu refresh token mới vào database (bạn cần có bảng để lưu refresh tokens)
                storedRefreshToken.Token = newRefreshToken;
                storedRefreshToken.ExpiryDate = newRefreshTokenExpiry;
                // Trả về response chứa token và trạng thái authenticated
                return new TokenResponse
                {
                    Token = newToken,
                    RefreshToken = newRefreshToken, // Trả về refresh token mới
                    RefreshTokenExpiry = newRefreshTokenExpiry, // Trả về thời gian hết hạn của refresh token
                    authenticated = true
                };
            }
            catch (CustomException ex)
            {
                // Ném lại CustomException với thông điệp và mã lỗi gốc
                throw new CustomException(ex.CustomMessage, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ không xác định (có thể ghi log nếu cần)
                throw new CustomException($"Lỗi khi làm mới token: {ex.Message}", 500); // Internal Server Error
            }
        }

        // Tạo Token
        public async Task<string> GenerateJwtToken(User user)
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
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpirationInMinutes"])), // Thời gian hết hạn
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
                throw new CustomException("Không có quyền truy cập.", 403); // Forbidden
            }

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            // Kiểm tra token có bị vô hiệu hóa hay không
            if (_context.InvalidatedTokens.Any(t => t.id == jti))
            {
                throw new CustomException("Token đã bị vô hiệu hóa.", 403); // Forbidden
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