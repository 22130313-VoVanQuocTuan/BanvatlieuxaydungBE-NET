using AcountService.dto.request.accountservice;
using AcountService.dto.response.account;
using AcountService.dto.response.product;
using AcountService.entity;
using AcountService.Repository;
using AutoMapper;
using BanVatLieuXayDung.dto.response.account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace AcountService.service
{
    public class UserService
    {

        public readonly DataContext _context;
        public readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;
        
       
        public UserService(DataContext context, IMapper mapper, UserManager<User> userManager, EmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
       
        }

        //TẠO TÀI KHOẢN
        public async Task<UserResponse> CreateUserAsync(UserCreateRequest userCreateRequest)
        {
            try
            {
                // Kiểm tra nếu user đã tồn tại
                if (await _context.Users.AnyAsync(u => u.UserName == userCreateRequest.UserName))
                {
                    // Ném ngoại lệ nếu người dùng đã tồn tại
                    throw new Exception("Tên tài khoản đã tồn tại");
                }
                if (await _context.Users.AnyAsync(e => e.Email == userCreateRequest.Email ))
                {
                    // Ném ngoại lệ nếu người dùng đã tồn tại
                    throw new Exception("Email đã tồn tại");
                }

                // Map dữ liệu qua User
                var user = _mapper.Map<User>(userCreateRequest);

                
                // Mã hóa mật khẩu
                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, userCreateRequest.Password);

                user.EmailConfirmed = false;

                // Thêm người dùng vào DbSet
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync(); // Lưu tất cả thay đổi vào cơ sở dữ liệu

                

                // Lấy ID của user mới tạo
               
                var verificationCode = await _emailService.GenerateVerificationCode(userCreateRequest.Email, user.Id);
              


                // Gửi mã xác thực qua email
                var subject = "Mã xác thực email";
                var body = $"Mã xác thực của bạn là: {verificationCode.VerificationCode}";
                _emailService.SendEmailAsync(user.Email, subject, body);




                // Gán vai trò "User" cho người dùng mới
                var roleResult = await _userManager.AddToRoleAsync(user, "USER");
                if (!roleResult.Succeeded)
                {
                    throw new Exception("Không thể gán vai trò cho người dùng");
                }




                var userResponse = _mapper.Map<UserResponse>(user);
                return userResponse;
            }
            catch (Exception ex)
            {
                // Có thể ném lại ngoại lệ gốc thay vì thay đổi thông báo lỗi
                throw new Exception(ex.Message); // Hoặc giữ nguyên ex
            }


        }

        

        //XÓA NGƯỜI DÙNG
            public async Task<string> DeleteUserAsync(string id)
        {
            try
            {
                // Tìm người dùng theo ID
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                // Xóa người dùng
                var result = await _userManager.DeleteAsync(user);

                var email = await _context.EmailVerificationCodes.FirstOrDefaultAsync(e => e.UserId == id);
                if(email != null) {
                    _context.EmailVerificationCodes.Remove(email);
                    await _context.SaveChangesAsync();
                }
                 

                return "User has been deleted";
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting user: " + ex.Message);
            }
        }

        //LẤY DANH SÁCH TÀI KHOẢN
        public async Task<List<UserResponse>> getAllUserAsync()
        {
            try
            {

                var result = await _userManager.Users.ToListAsync();
                var response = _mapper.Map<List<UserResponse>>(result);
                return response;

            }
            catch
            (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // LẤY RA TÀI KHOẢN
        public async Task<UserResponse> getUserAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new Exception("User không tồn tại");
                }
                var response = _mapper.Map<UserResponse>(user);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        // CẬP NHẬT TÀI KHOẢN
        public async Task<UserResponse> UpdateUserAsync(string id, UserUpdateRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new Exception("Tài khoản không tồn tại");
                }

  
                // Cập nhật các thuộc tính của thực thể đã được truy vấn
                user.Email = request.Email;
                user.PhoneNumber = request.PhoneNumber;
                user.Address = request.Address;
                user.FullName = request.FullName;
               
               
               
                // Cập nhật người dùng trong cơ sở dữ liệu
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Cập nhật thông tin người dùng thất bại");
                }

                var response = _mapper.Map<UserResponse>(user);
                response.Id = id;
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

             

          
        }
        // TÍNH TỔNG TÀI KHOẢN, ĐÁNH GIÁ, DOANH THU
        public async Task<TotalUserRateRevenueResponse> GetTotalUserRateRevenueAsync()
        {
            // Đếm tổng số khách hàng trong bảng User
            var totalCustomers = await _context.Users.CountAsync();

            // Đếm tổng số lượt xem (nếu là Review thì cần xác định đúng ý nghĩa)
            var totalView = await _context.Reviews.CountAsync();

            // Tính tổng doanh thu từ các hóa đơn đã thanh toán
            var totalRevenue = await _context.Orders
                .Where(o => o.payment_status == "đã thanh toán") // Điều kiện chỉ tính hóa đơn đã thanh toán
                .SumAsync(o => o.TotalPrice);         // Tổng tiền dựa trên cột TotalPrice

            // Trả về kết quả dưới dạng đối tượng response
            return new TotalUserRateRevenueResponse
            {
                totalUser = totalCustomers,
                Rate = totalView,
                Revenue = totalRevenue
            };
        }

    }
}