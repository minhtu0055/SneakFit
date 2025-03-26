using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.ChatLieu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.ChatLieu
{
    public class ChatLieuService : IChatLieuService
    {
        private readonly SneakFitDbContext _context;

        public ChatLieuService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<List<ChatLieuViewModels>> GetAll()
        {
            return await _context.ChatLieu
            .Select(cl => new ChatLieuViewModels
            {
                Id = cl.Id,
                TenChatLieu = cl.TenChatLieu
            })
            .ToListAsync();
        }
        public async Task<ChatLieuViewModels> GetById(Guid id)
        {
            var chatLieu = await _context.ChatLieu.FindAsync(id);
            if (chatLieu == null) throw new Exception("Không tìm thấy!");

            return new ChatLieuViewModels { Id = chatLieu.Id, TenChatLieu = chatLieu.TenChatLieu };
        }
        public async Task<ChatLieuViewModels> Create(ThemChatLieu request)
        {
            var chatLieu = new SneakFit.Data.Entities.ChatLieu
            {
                Id = Guid.NewGuid(),
                TenChatLieu = request.TenChatLieu
            };

            _context.ChatLieu.Add(chatLieu);
            await _context.SaveChangesAsync();

            return new ChatLieuViewModels
            {
                Id = chatLieu.Id,
                TenChatLieu = chatLieu.TenChatLieu
            };
        }
        public async Task<ChatLieuViewModels> Update(SuaChatLieu request)
        {
            var chatLieu = await _context.ChatLieu.FindAsync(request.Id);

            if (chatLieu == null)
            {
                throw new KeyNotFoundException("Không tìm thấy!");
            }
            chatLieu.TenChatLieu = request.TenChatLieu;

            await _context.SaveChangesAsync();

            return new ChatLieuViewModels
            {
                Id = chatLieu.Id,
                TenChatLieu = chatLieu.TenChatLieu
            };
        }
    }
}
