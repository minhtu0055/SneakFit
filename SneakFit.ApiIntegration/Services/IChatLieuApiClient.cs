using SneakFit.ViewModels.Catalog.ChatLieu;
using SneakFit.ViewModels.Common;

namespace SneakFit.ApiIntegration.Services
{
    public interface IChatLieuApiClient
    {
        Task<PagedResult<ChatLieuViewModels>> GetAllPaging(ChatLieuPagingRequest request);
        Task<ChatLieuViewModels> GetById(Guid id);
        Task<ChatLieuViewModels> Create(ThemChatLieu request);
        Task<ChatLieuViewModels> Update(SuaChatLieu request);
        Task<List<ChatLieuViewModels>> GetAll();
    }
}
