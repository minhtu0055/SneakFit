using SneakFit.ViewModels.Catalog.ThongKe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.ThongKe
{
    public interface IThongKeService
    {
        Task<ThongKeTongQuanViewModel> GetThongKeTongQuanAsync(
        string filter,
        string ngay = null,
        string tuan = null,
        string thang = null,
        string tuNgay = null,
        string denNgay = null
    );
        Task<byte[]> ExportExcelAsync(string filter); // thêm dòng này
        Task<ThongKeHoaDonSanPhamChartViewModel> GetThongKeHoaDonSanPhamChartAsync(
             string filter,
             string ngay = null,
             string tuan = null,
             string thang = null,
             string tuNgay = null,
             string denNgay = null
         );
        Task<List<TopSanPhamBanChayViewModel>> GetTopSanPhamBanChayAsync(int top, string filter, string ngay = null, string tuan = null, string thang = null, string tuNgay = null, string denNgay = null);
        Task<List<TrangThaiDonHangViewModel>> GetTrangThaiDonHangAsync(string filter, string ngay = null, string tuan = null, string thang = null, string tuNgay = null, string denNgay = null);
        Task<List<SanPhamSapHetHangViewModel>> GetSanPhamSapHetHangAsync(int soLuongCanhBao = 5);
        Task<List<TocDoTangTruongViewModel>> GetTocDoTangTruongAsync();
        Task<List<SanPhamChiTietThongKeViewModel>> GetSanPhamChiTietBanChayThongKe(
            Guid sanPhamId, string filter, string ngay = null, string tuan = null, string thang = null, string tuNgay = null, string denNgay = null);
        Task<List<SanPhamChiTietThongKeViewModel>> GetSanPhamChiTietHetHangThongKe(Guid sanPhamId, int soLuongCanhBao = 5);
    }
}
