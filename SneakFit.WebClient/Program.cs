using SneakFit.ApiIntegration.Services;
using SneakFit.ApiIntegration.Services.ThuongHieu;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
