using Microsoft.EntityFrameworkCore;
using SneakFit.Application.Catalog.ChatLieu;
using SneakFit.Application.Catalog.DanhMuc;
using SneakFit.Application.Catalog.DeGiay;
using SneakFit.Application.Catalog.KichThuoc;
using SneakFit.Application.Catalog.MauSac;
using SneakFit.Application.Catalog.SanPham;
using SneakFit.Application.Catalog.SanPhamChiTiet;
using SneakFit.Application.Catalog.SanPhamChiTietChiTiet;
using SneakFit.Application.Catalog.ThuongHieu;
using SneakFit.Data.EF;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<SneakFitDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IThuongHieuService, ThuongHieuService>(); // khai báo dịch vụ
builder.Services.AddScoped<IDeGiayService, DeGiayService>(); // khai báo dịch vụ
builder.Services.AddScoped<IChatLieuService, ChatLieuService>(); // khai báo dịch vụ
builder.Services.AddScoped<IKichThuocService, KichThuocService>(); // khai báo dịch vụ
builder.Services.AddScoped<IMauSacService, MauSacService>(); // khai báo dịch vụ
builder.Services.AddScoped<IDanhMucService, DanhMucService>(); // khai báo dịch vụ
builder.Services.AddScoped<ISanPhamService, SanPhamService>(); // khai báo dịch vụ
builder.Services.AddScoped<ISanPhamChiTetService, SanPhamChiTietChiTetService>(); // khai báo dịch vụ



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
