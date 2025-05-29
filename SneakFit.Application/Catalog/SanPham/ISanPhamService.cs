using Microsoft.AspNetCore.Http;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Catalog.ThuongHieu;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.SanPham
{
    public interface ISanPhamService
    {
        Task<PagedResult<SanPhamViewModels>> GetAllPaging(SanPhamPagingRequest request);
        Task<List<SanPhamViewModels>> GetAll();
        Task<SanPhamViewModels> GetById(Guid id);
        Task<SanPhamViewModels> Create(ThemSanPham request);
        Task<SanPhamViewModels> Update(SuaSanPham request);
        Task<bool> UpdateTrangThai(Guid id, bool trangThai);
        Task<bool> UpdateSPCT(List<SanPhamChiTietCapNhat> updates);
        Task<List<SPCTViewModels>> GetSPCTByProductName(string productName);
        Task<SPCTDetailViewModel> GetSPCTDetail(Guid spctId);
        Task<bool> UpdateSPCTDetail(SuaSPCTDetailViewModel model);
        
        // Thêm các phương thức xử lý ảnh
        Task<bool> UploadImages(UploadImageRequest request);
        Task<bool> DeleteImage(DeleteImageRequest request);
    }
}
