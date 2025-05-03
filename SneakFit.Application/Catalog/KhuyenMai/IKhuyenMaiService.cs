using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.KhuyenMai
{
    public interface IKhuyenMaiService
    {
        Task<PagedResult<KhuyenMaiViewModels>> GetAllPaging(PhanTrangKhuyenMai request);
        Task<KhuyenMaiViewModels> GetById(Guid id);
        Task<KhuyenMaiViewModels> Create(ThemKhuyenMai request);
        Task<KhuyenMaiViewModels> Update(SuaKhuyenMai request);
        Task<bool> UpdateStatus(Guid id, TrangThaiGiamGia trangThai);
    }
}
