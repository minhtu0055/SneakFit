using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.LichSuHoaDon;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.HoaDon
{
    public interface IHoaDonService
    {
        Task<PagedResult<HoaDonViewModel>> GetAllPaging(PhanTrangHoaDon request);
        Task<HoaDonViewModel> GetById(Guid id);
        Task<HoaDonViewModel> Create(ThemHoaDon request, string tenNguoiTao);
        Task<HoaDonViewModel> Update(SuaHoaDon request);
        Task<Dictionary<TrangThaiHoaDon, int>> GetCountByStatusAsync();
        Task<List<HoaDonViewModel>> GetHoaDonChoByNguoiTao(string nguoiTao);
        Task<bool> Delete(Guid id);
        Task<bool> RevertToPreviousStatusAsync(Guid hoaDonId, string nguoiThucHien);
        Task<bool> UpdateStatusAndLogAsync(Guid hoaDonId, TrangThaiHoaDon newStatus, Guid userId, string nguoiChinhSua, string ghiChu);
        // Lịch sử hóa đơn
        Task<List<LichSuHoaDonViewModel>> GetByHoaDonIdAsync(Guid hoaDonId);
        Task<Guid> CreateAsync(CreateLichSuHoaDonRequest request);
        
    }
}
