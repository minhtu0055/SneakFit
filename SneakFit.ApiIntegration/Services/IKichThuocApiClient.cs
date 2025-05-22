using SneakFit.ViewModels.Catalog.ChatLieu;
using SneakFit.ViewModels.Catalog.KichThuoc;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IKichThuocApiClient
    {
        Task<PagedResult<KichThuocViewModels>> GetAllPaging(KichThuocPagingRequest request);
        Task<KichThuocViewModels> GetById(Guid id);
        Task<KichThuocViewModels> Create(ThemKichThuoc request);
        Task<KichThuocViewModels> Update(SuaKichThuoc request);
    }
}
