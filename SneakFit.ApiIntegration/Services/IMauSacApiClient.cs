using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IMauSacApiClient
    {
        Task<PagedResult<MauSacViewModels>> GetAllPaging(MauSacPagingRequest request);
        Task<MauSacViewModels> GetById(Guid id);
        Task<MauSacViewModels> Create(ThemMauSac request);
        Task<MauSacViewModels> Update(SuaMauSac request);
        Task<List<MauSacViewModels>> GetAll();
    }
}
