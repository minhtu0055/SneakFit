using SneakFit.ViewModels.Catalog.DanhMuc;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.DanhMuc
{
    public interface IDanhMucService
    {
        Task<PagedResult<DanhMucViewModels>> GetAllPaging(DanhMucPagingRequest request);
        Task<List<DanhMucViewModels>> GetAll();
        Task<DanhMucViewModels> GetById(Guid id);
        Task<DanhMucViewModels> Create(ThemDanhMuc request);
        Task<DanhMucViewModels> Update(SuaDanhMuc request);
    }
}
