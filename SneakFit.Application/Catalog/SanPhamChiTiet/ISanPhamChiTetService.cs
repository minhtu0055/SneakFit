using Microsoft.AspNetCore.Http;
using SneakFit.ViewModels.Catalog.SanPhamChiTiet;
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
        Task<SPCTViewModels> Create(ThemSPCT request);
        Task<SPCTViewModels> Update(SuaSPCT request);
        //Task<bool> UpdateTrangThai(Guid id);
        Task<bool> UpdateGia(Guid id, decimal giaMoi);
        Task<bool> UpdateSoLuong(Guid productId, int themSoLuong);
        Task<int> AddImage(Guid idSanPham, IFormFile image);
        Task<int> RemoveImage(Guid imageId);
        Task<List<string>> GetListImages(Guid idSanPham);
    }
}
