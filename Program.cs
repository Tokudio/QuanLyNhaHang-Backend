using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuanLyNhaHangAPI.Data;
using QuanLyNhaHangAPI.Services;
using System.Text;

namespace QuanLyNhaHangAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

            Console.WriteLine("=================================");
            Console.WriteLine(connStr);
            Console.WriteLine("=================================");

            // 1. Cấu hình kết nối SQL Server Database
            builder.Services.AddDbContext<QuanLyNhaHangDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 2. Kích hoạt Dependency Injection cho AuthService
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 3. Cấu hình bảo mật JWT Bearer Authentication Middleware
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "Key_Bao_Mat_Sieu_Cap_Nha_Hang_Quan_Ly_123456789";
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "NhaHangIssuer",
                    ValidAudience = builder.Configuration["Jwt:Audience"] ?? "NhaHangAudience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            // =========================================================================
            // SỬA TẠI ĐÂY: Gộp chung các domain Frontend vào 1 chính sách CORS duy nhất
            // =========================================================================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontends", policy =>
                {
                    policy.WithOrigins(
                              "http://localhost:4200",  // Cổng chạy Angular mặc định
                              "https://localhost:7144", // Cổng HTTPS của Blazor WASM
                              "http://localhost:5000"   // Cổng HTTP của Blazor WASM
                          )
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Bắt buộc nếu bạn dùng Cookie hoặc xác thực nâng cao qua CORS
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // =========================================================================
            // SỬA TẠI ĐÂY: Chỉ gọi UseCors MỘT LẦN duy nhất với chính sách chung
            // =========================================================================
            app.UseCors("AllowFrontends");

            app.UseHttpsRedirection();

            // Lưu ý: Thứ tự chuẩn là UseCors -> UseAuthentication -> UseAuthorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Đoạn code Hash lại mật khẩu tự động của bạn giữ nguyên
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QuanLyNhaHangDbContext>();
                var users = context.NguoiDung.ToList();
                foreach (var user in users)
                {
                    if (!user.MatKhau.StartsWith("$2"))
                    {
                        user.MatKhau = BCrypt.Net.BCrypt.HashPassword(user.MatKhau);
                    }
                }
                context.SaveChanges();
            }

            app.Run();
        }
    }
}