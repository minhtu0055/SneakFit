using Microsoft.AspNetCore.Authentication.Cookies;
using SneakFit.ApiIntegration.Services;
using SneakFit.ApiIntegration.Services.ThuongHieu;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Thêm Memory Cache
builder.Services.AddMemoryCache();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Forbidden/Index";
        // Thêm các cấu hình sau
        options.Cookie.Name = "SneakFit.Admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        
        // Cấu hình thời gian sống của cookie
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUserApiClient, UserApiClient>();
builder.Services.AddScoped<IRoleApiClient, RoleApiClient>();
builder.Services.AddScoped<IKhuyenMaiApiClient, KhuyenMaiApiClient>();
builder.Services.AddScoped<IChatLieuApiClient, ChatLieuApiClient>();
builder.Services.AddScoped<IDeGiayApiClient, DeGiayApiClient>();
builder.Services.AddScoped<IMauSacApiClient, MauSacApiClient>();
builder.Services.AddScoped<IKichThuocApiClient, KichThuocApiClient>();
builder.Services.AddScoped<ISanPhamApiClient, SanPhamApiClient>();
builder.Services.AddScoped<ISpctApiClient, SpctApiClient>();
builder.Services.AddScoped<IThuongHieuApiClient, ThuongHieuApiClient>();
builder.Services.AddScoped<IDanhMucApiClient, DanhMucApiClient>();
builder.Services.AddScoped<IVoucherApiClient, VoucherApiClient>();
builder.Services.AddScoped<IGhnApiClient, GhnApiClient>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddRazorPages();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
