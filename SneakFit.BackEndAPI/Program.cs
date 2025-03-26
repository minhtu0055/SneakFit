using Microsoft.EntityFrameworkCore;
using SneakFit.Application.Catalog.DeGiay;
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
