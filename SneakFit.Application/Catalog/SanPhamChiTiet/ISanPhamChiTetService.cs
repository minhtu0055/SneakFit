using Microsoft.AspNetCore.Http;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.SanPhamChiTiet
{
    public interface ISanPhamChiTetService
    {
        Task<List<SPCTViewModels>> GetAll();
        Task<SPCTViewModels> GetById(Guid id);
        Task<ApiResult<SPCTViewModels>> Create(ThemSPCT request);
        Task<SPCTViewModels> Update(SuaSPCT request);
        Task<PagedResult<SPCTViewModels>> GetAllPaging(PhanTrangSPCT request);
        Task<PagedResult<SPCTViewModels>> GetAllPagings(PhanTrangSPCT request);
        Task<bool> UpdateTrangThai(Guid id, bool trangThai);
        Task<bool> UpdateGia(Guid id, decimal giaMoi);
        //Task<bool> UpdateSoLuong(Guid productId, int themSoLuong);
        Task<ApiResult<bool>> UpdateSoLuong(Guid productId, int deltaSoLuong);
        Task<int> AddImage(Guid idSanPham, IFormFile image);
        Task<int> RemoveImage(Guid imageId);
        Task<List<string>> GetListImages(Guid idSanPham);
        Task<int> CreateMultiple(ThemNhieuSPCTRequest request);
    }
}
