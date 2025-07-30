using SneakFit.ViewModels.Catalog.ThongKe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SneakFit.ApiIntegration.Services
{
    public interface IThongKeApiClient
    {
        Task<List<TopSanPhamBanChayViewModel>> GetTopSanPhamBanChayAsync(int top = 10);
        Task<List<SanPhamChiTietThongKeViewModel>> GetSanPhamChiTietBanChayThongKeAsync(Guid sanPhamId);
    }
} 