using Microsoft.EntityFrameworkCore;
using SneakFit.Application.Catalog.ChatLieu;
using SneakFit.Application.Catalog.DeGiay;
using SneakFit.Application.Catalog.KhuyenMai;
using SneakFit.Application.Catalog.ThuongHieu;
using SneakFit.Data.EF;
using System.Text.Json.Serialization;

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
builder.Services.AddScoped<IKhuyenMaiService, KhuyenMaiService>(); // khai báo dịch vụ



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
