using Microsoft.AspNetCore.Http;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Catalog.SanPham;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface ISanPhamApiClient
    {
        Task<PagedResult<SanPhamViewModels>> GetAllPaging(SanPhamPagingRequest request);
        Task<SanPhamViewModels> GetById(Guid id);
        Task<SanPhamViewModels> Create(ThemSanPham request);
        Task<SanPhamViewModels> Update(SuaSanPham request);
        Task<List<SanPhamViewModels>> GetAll();
        Task<bool> UpdateTrangThai(Guid id, bool trangThai);
        Task<bool> UpdateSPCT(List<SanPhamChiTietCapNhat> updates);
        Task<List<SPCTViewModels>> GetSPCTByProductName(string productName);
        Task<SPCTDetailViewModel> GetSPCTDetail(Guid spctId);
        Task<bool> UpdateSPCTDetail(SuaSPCTDetailViewModel model);
        
        // Thêm các phương thức xử lý ảnh
        Task<bool> UploadImages(Guid sanPhamChiTietId, List<IFormFile> files);
        Task<bool> DeleteImage(Guid imageId, Guid sanPhamChiTietId);

        // THÊM KHAI BÁO PHƯƠNG THỨC NÀY cho sửa khuyến mại
        Task<List<SPCTViewModels>> GetSPCTByListIds(List<Guid> ids);
    }
}
