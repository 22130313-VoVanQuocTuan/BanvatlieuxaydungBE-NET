using AcountService.entity;
using Microsoft.AspNetCore.Identity;

namespace AcountService.config
{
    public class SeedData
    {

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // Tạo vai trò nếu chưa tồn tại
            string[] roleNames = { "ADMIN", "USER" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Tạo người dùng Admin nếu chưa tồn tại
            var adminUser = new User
            {
                UserName = "Admin",
                Email = "tuanvo0420004@gmail.com", // Nên thêm email
                NormalizedUserName = "ADMIN",
                NormalizedEmail = "TUANVO042004@GMAIL.COM" // Nên thêm email normalized
            };

            // Kiểm tra xem người dùng đã tồn tại chưa
            if (userManager.Users.All(u => u.UserName != adminUser.UserName))
            {
                // Mã hóa mật khẩu
                var passwordHasher = new PasswordHasher<User>();
                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin!");

                await userManager.CreateAsync(adminUser);
                await userManager.AddToRoleAsync(adminUser, "ADMIN");
            }
        }
    }
}