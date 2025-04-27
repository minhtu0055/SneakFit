using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SneakFit.Application.Catalog.ChatLieu;
using SneakFit.Application.Catalog.DanhMuc;
using SneakFit.Application.Catalog.DeGiay;
using SneakFit.Application.Catalog.KichThuoc;
using SneakFit.Application.Catalog.MauSac;
using SneakFit.Application.Catalog.SanPham;
using SneakFit.Application.Catalog.SanPhamChiTiet;
using SneakFit.Application.Catalog.SanPhamChiTietChiTiet;
using SneakFit.Application.Catalog.KhuyenMai;
using SneakFit.Application.Catalog.ThuongHieu;
using SneakFit.Application.System;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
   

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<SneakFitDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true; //yêu có ít nhất 1 chữ số 
    options.Password.RequireLowercase = true; // yêu cầu có ít nhất chứ thường
    options.Password.RequireUppercase = true; // yêu cầu có ít nhất một chữ hoa
    options.Password.RequireNonAlphanumeric = true; // yêu cầu có ít nhất một ký tự đặc biệt

    options.User.RequireUniqueEmail = true; // yêu cầu mỗi email phải là duy nhất, không thể hai toàn khoản có trùng email
    options.Lockout.MaxFailedAccessAttempts = 5; // sau 5 lần nhập sai mật khẩu 
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // khóa tài khoản trong 5 phút
}).AddEntityFrameworkStores<SneakFitDbContext>() // sử dụng addentity để lưu trữ dữ liệu người dùng vào database
            .AddDefaultTokenProviders(); // cung cấp các token cho tính năng: xác thực  
//Declare DI
builder.Services.AddScoped<IThuongHieuService, ThuongHieuService>(); // khai báo dịch vụ
builder.Services.AddScoped<IDeGiayService, DeGiayService>(); // khai báo dịch vụ
builder.Services.AddScoped<IChatLieuService, ChatLieuService>(); // khai báo dịch vụ
builder.Services.AddScoped<IUserService, UserService>(); // khai báo dịch vụ
builder.Services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
builder.Services.AddScoped<SignInManager<AppUser>, SignInManager<AppUser>>();
builder.Services.AddScoped<RoleManager<IdentityRole>, RoleManager<IdentityRole>>();
builder.Services.AddScoped<IKichThuocService, KichThuocService>(); // khai báo dịch vụ
builder.Services.AddScoped<IMauSacService, MauSacService>(); // khai báo dịch vụ
builder.Services.AddScoped<IDanhMucService, DanhMucService>(); // khai báo dịch vụ
builder.Services.AddScoped<ISanPhamService, SanPhamService>(); // khai báo dịch vụ
builder.Services.AddScoped<ISanPhamChiTetService, SanPhamChiTietChiTetService>(); // khai báo dịch vụ
builder.Services.AddScoped<IKhuyenMaiService, KhuyenMaiService>(); // khai báo dịch vụ


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{       //Thêm bảo mật có JWT Cho swagger
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập 'Bearer' [dấu cách] rồi token của bạn vào ô bên dưới.\r\n\r\nVí dụ: Bearer 12345abcdef",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    // Bắt buộc Swagger yêu cầu JWT Token khi gọi API
    // nếu token không hợp lệ sẽ không được phép gọi API
    x.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
            {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header,
        },
        new List<string>()
        }
    });
});
string issuer = builder.Configuration.GetValue<string>("Tokens:Issuer");
string signingKey = builder.Configuration.GetValue<string>("Tokens:Key");
byte[] signingKeyBytes = System.Text.Encoding.UTF8.GetBytes(signingKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = issuer,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = System.TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
