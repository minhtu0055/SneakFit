using Microsoft.AspNetCore.Authentication.Cookies;
using SneakFit.ApiIntegration.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "SneakFit.Client";
    options.LoginPath = "/Login/Index";
    options.AccessDeniedPath = "/Forbidden/Index";
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(300);
    options.Cookie.Name = "SneakFit.Client.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<ISpctApiClient, SpctApiClient>();
builder.Services.AddScoped<IDanhMucApiClient, DanhMucApiClient>();
builder.Services.AddScoped<IMauSacApiClient, MauSacApiClient>();
builder.Services.AddScoped<IKichThuocApiClient, KichThuocApiClient>();
builder.Services.AddScoped<ISanPhamApiClient, SanPhamApiClient>();
builder.Services.AddScoped<IThuongHieuApiClient, ThuongHieuApiClient>();
builder.Services.AddScoped<IDeGiayApiClient, DeGiayApiClient>();
builder.Services.AddScoped<IChatLieuApiClient, ChatLieuApiClient>();
builder.Services.AddScoped<IKhuyenMaiApiClient, KhuyenMaiApiClient>();
builder.Services.AddScoped<IGioHangApiClient, GioHangApiClient>();
builder.Services.AddScoped<IVoucherApiClient, VoucherApiClient>();
builder.Services.AddScoped<IHoaDonClientApiClient, HoaDonClientApiClient>();
builder.Services.AddScoped<IHoaDonChiTietClientApiClient, HoaDonChiTietClientApiClient>();
builder.Services.AddScoped<IGhnApiClient, GhnApiClient>();
builder.Services.AddScoped<IUserApiClient, UserApiClient>();
builder.Services.AddScoped<IDiaChiApiClient, DiaChiApiClient>();
builder.Services.AddScoped<IThanhToanApiClient, ThanhToanApiClient>();
builder.Services.AddScoped<IThongKeApiClient, ThongKeApiClient>();
builder.Services.AddScoped<ITraHangApiClient, TraHangApiClient>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:7211") // Cho phép frontend gọi
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
