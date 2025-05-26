using SneakFit.ViewModels.Catalog.DeGiay;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IDeGiayApiClient
    {
        Task<PagedResult<DeGiayViewModels>> GetAllPaging(DeGiayPagingRequest request);
        Task<DeGiayViewModels> GetById(Guid id);
        Task<DeGiayViewModels> Create(ThemDeGiay request);
        Task<DeGiayViewModels> Update(SuaDeGiay request);
        Task<List<DeGiayViewModels>> GetAll();
    }
}
