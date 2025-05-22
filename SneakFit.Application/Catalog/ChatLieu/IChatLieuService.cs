using SneakFit.ViewModels.Catalog.ChatLieu;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.ChatLieu
{
    public interface IChatLieuService
    {
        Task<PagedResult<ChatLieuViewModels>> GetAllPaging(ChatLieuPagingRequest request);
        Task<ChatLieuViewModels> GetById(Guid id);
        Task<ChatLieuViewModels> Create(ThemChatLieu request);
        Task<ChatLieuViewModels> Update(SuaChatLieu request);
        Task<List<ChatLieuViewModels>> GetAll();
    }
}
