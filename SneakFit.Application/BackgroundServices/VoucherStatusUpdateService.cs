using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SneakFit.Data.EF;
using SneakFit.Data.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SneakFit.Application.BackgroundServices
{
    public class VoucherStatusUpdateService : BackgroundService
    {
        private readonly ILogger<VoucherStatusUpdateService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public VoucherStatusUpdateService(
            ILogger<VoucherStatusUpdateService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<SneakFitDbContext>();
                        var now = DateTime.Now;

                        // Cập nhật trạng thái cho các voucher
                        var vouchers = dbContext.Voucher
                            .Where(v => v.TrangThai != TrangThaiGiamGia.HetHan)
                            .ToList();

                        foreach (var voucher in vouchers)
                        {
                            if (now >= voucher.ThoiGianBatDau && now <= voucher.ThoiGianKetThuc)
                            {
                                if (voucher.TrangThai != TrangThaiGiamGia.HoatDong)
                                {
                                    voucher.TrangThai = TrangThaiGiamGia.HoatDong;
                                    _logger.LogInformation($"Voucher {voucher.MaVoucher} đã được kích hoạt");
                                }
                            }
                            else if (now > voucher.ThoiGianKetThuc)
                            {
                                if (voucher.TrangThai != TrangThaiGiamGia.HetHan)
                                {
                                    voucher.TrangThai = TrangThaiGiamGia.HetHan;
                                    _logger.LogInformation($"Voucher {voucher.MaVoucher} đã hết hạn");
                                }
                            }
                            else
                            {
                                if (voucher.TrangThai != TrangThaiGiamGia.KhongHoatDong)
                                {
                                    voucher.TrangThai = TrangThaiGiamGia.KhongHoatDong;
                                    _logger.LogInformation($"Voucher {voucher.MaVoucher} chưa đến thời gian kích hoạt");
                                }
                            }
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật trạng thái voucher");
                }

                // Chờ 1 phút trước khi kiểm tra lại
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
} 