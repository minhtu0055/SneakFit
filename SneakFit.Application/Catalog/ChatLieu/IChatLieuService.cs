using SneakFit.ViewModels.Catalog.ChatLieu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.ChatLieu
{
    public interface IChatLieuService
    {
        Task<List<ChatLieuViewModels>> GetAll();
        Task<ChatLieuViewModels> GetById(Guid id);
        Task<ChatLieuViewModels> Create(ThemChatLieu request);
        Task<ChatLieuViewModels> Update(SuaChatLieu request);
    }
}
