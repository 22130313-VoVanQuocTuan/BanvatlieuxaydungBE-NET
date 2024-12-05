using AcountService.AppException;
using AcountService.config;
using AcountService.entity;
using AcountService.mapper;
using AcountService.Repository;
using AcountService.service;
using BanVatLieuXayDung.entity;
using BanVatLieuXayDung.service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


var builder = WebApplication.CreateBuilder(args);




builder.Services.AddEndpointsApiExplorer(); // Cho phép ứng dụng phát hiện và liệt kê các endpoint API, cần thiết nếu bạn dùng Swagger để hiển thị tài liệu API.
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<Authenticate>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<DiscountService>();
builder.Services.AddScoped<PromotionalService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<InfoUserShippingService>();
builder.Services.AddScoped<ReviewService>();

builder.Services.AddHttpContextAccessor(); // Đăng ký IHttpContextAccessor
// Cấu hình DbContext
builder.Services.AddDbContext<DataContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// Thêm AutoMapper vào DI container
builder.Services.AddAutoMapper(typeof(UserMapper)); // Sử dụng UserProfile để cấu hình

// Cấu hình JWT Authentication
// Đăng ký TokenValidationParameters dưới dạng Singleton
builder.Services.AddSingleton(provider =>
{
    return new TokenValidationParameters
    {
        ValidateIssuer = true,   // người tạo token
        ValidateAudience = true,  //đối tượng được cấp token
        ValidateLifetime = true,   //thời hạn token
        ValidateIssuerSigningKey = true,  //chữ ký của token 
        ValidIssuer = builder.Configuration["Jwt:Issuer"],  //Đây là chuỗi đại diện cho người phát hành token
        ValidAudience = builder.Configuration["Jwt:Audience"],  
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))  //Tạo chìa khóa bí mật để ký token bằng mã hóa Symmetric Security Key.
    };
});

// Đăng ký trong cấu hình JwtBearer
builder.Services.AddAuthentication(options =>
{
options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Lấy TokenValidationParameters từ dịch vụ đã cấu hình trước đó
    var serviceProvider = builder.Services.BuildServiceProvider();
    var tokenValidationParameters = serviceProvider.GetRequiredService<TokenValidationParameters>();   //chứa các tham số cấu hình dùng để xác thực JW

    // Thiết lập các tham số xác thực token
    options.TokenValidationParameters = tokenValidationParameters;

    // Xử lý sự kiện khi token được xác thực
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var jwtToken = context.SecurityToken as JwtSecurityToken;
            var jti = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value; //là một claim dùng để xác định một token duy nhất.

            // Kiểm tra token đã bị vô hiệu hóa chưa
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<DataContext>();
            var invalidatedToken = await dbContext.InvalidatedTokens.FirstOrDefaultAsync(t => t.id == jti);

            if (invalidatedToken != null)
            {
                context.Fail("Token đã bị vô hiệu hóa");
            }
        }
    };
});

//cấu hình CORS (Cross-Origin Resource Sharin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
       builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


// Cấu hình dịch vụ ủy quyền và chính sách
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("ADMIN"));

    options.AddPolicy("UserOnly", policy =>
        policy.RequireRole("USER"));
});


// Cấu hình các dịch vụ khác
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



var app = builder.Build();
// Khởi động ứng dụng
using (var scope = app.Services.CreateScope())    // khởi tạo dữ liệu mặc định trong csdl
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}


// Configure the HTTP request pipeline.

// Bật CORS
app.UseCors("AllowAllOrigins");
// Bật Routing
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//Bật Middleware cho lỗi
app.UseMiddleware<StatusErrorCode>();

// Bật HTTPS Redirection
app.UseHttpsRedirection();

//// Bật Xác thực
app.UseAuthentication();

// Bật Ủy quyền
app.UseAuthorization();

// Ánh xạ các controller
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

//cấu hình thư mục tĩnh
app.UseStaticFiles();

app.Run();
