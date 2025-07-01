using SneakFit.ViewModels.Catalog.GioHang;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IGioHangApiClient
    {
        Task<PagedResult<GioHangViewModel>> GetAllPaging(GioHangPagingRequest request);
        Task<GioHangViewModel> GetById(Guid id);
        Task<GioHangViewModel> GetByUserId(Guid userId);
        Task<GioHangViewModel> ThemVaoGioHang(ThemVaoGioHangRequest request);
        Task<GioHangViewModel> CapNhatGioHang(CapNhatGioHangRequest request);
        Task<bool> XoaSanPhamKhoiGioHang(Guid gioHangChiTietId);
        Task<bool> XoaGioHang(Guid id);
        Task<ApiResult<bool>> CapNhatSoLuong(CapNhatGioHang request);
    }
}
