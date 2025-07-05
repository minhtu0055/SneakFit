using SneakFit.ViewModels.Catalog.GioHang;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.GioHang
{
    public interface IGioHangService
    {
        Task<PagedResult<GioHangViewModel>> GetAllPaging(GioHangPagingRequest request);
        Task<GioHangViewModel> GetById(Guid id);
        Task<GioHangViewModel> GetByUserId(Guid userId);
        Task<GioHangViewModel> ThemVaoGioHang(ThemVaoGioHangRequest request);
        Task<GioHangViewModel> CapNhatGioHang(CapNhatGioHangRequest request);
        Task<bool> XoaSanPhamKhoiGioHang(Guid gioHangChiTietId);
        Task<bool> XoaSanPhamDaMuaKhoiGioHang(Guid userId, List<Guid> sanPhamChiTietIds);
        Task<bool> XoaGioHang(Guid id);
        Task<ApiResult<bool>> CapNhatSoLuongAsync(CapNhatGioHang request);
    }
}
