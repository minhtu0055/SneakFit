using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.ViewModels.Catalog.ChatLieu;
using SneakFit.ViewModels.Catalog.MauSac;
using SneakFit.ViewModels.Common;
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
        public async Task<PagedResult<ChatLieuViewModels>> GetAllPaging(ChatLieuPagingRequest request)
        {
            var query = _context.ChatLieu.AsQueryable();
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.TenChatLieu.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ChatLieuViewModels()
                {
                    Id = x.Id,
                    TenChatLieu = x.TenChatLieu
                }).ToListAsync();
            var PageResult = new PagedResult<ChatLieuViewModels>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data,
            };
            return PageResult;
        }
        public async Task<ChatLieuViewModels> GetById(Guid id)
        {
            var chatLieu = await _context.ChatLieu.FindAsync(id);
            if (chatLieu == null) throw new Exception("Không tìm thấy!");

            return new ChatLieuViewModels { Id = chatLieu.Id, TenChatLieu = chatLieu.TenChatLieu };
        }
        public async Task<ChatLieuViewModels> Create(ThemChatLieu request)
        {
            // Kiểm tra xem tên chất liệu đã tồn tại chưa
            var existingChatLieu = await _context.ChatLieu
                .FirstOrDefaultAsync(x => x.TenChatLieu.ToLower() == request.TenChatLieu.ToLower());
            if (existingChatLieu != null)
            {
                throw new Exception("Tên chất liệu đã tồn tại!");
            }
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
            // Kiểm tra xem tên chất liệu đã tồn tại chưa (trừ chính bản ghi hiện tại)
            var existingChatLieu = await _context.ChatLieu
                .FirstOrDefaultAsync(x => x.TenChatLieu.ToLower() == request.TenChatLieu.ToLower() 
                                        && x.Id != request.Id);
            if (existingChatLieu != null)
            {
                throw new Exception("Tên chất liệu đã tồn tại!");
            }
            chatLieu.TenChatLieu = request.TenChatLieu;

            await _context.SaveChangesAsync();

            return new ChatLieuViewModels
            {
                Id = chatLieu.Id,
                TenChatLieu = chatLieu.TenChatLieu
            };
        }
        public async Task<List<ChatLieuViewModels>> GetAll()
        {
            return await _context.ChatLieu.Select(x => new ChatLieuViewModels
            {
                Id = x.Id,
                TenChatLieu = x.TenChatLieu
            }).ToListAsync();
        }
    }
}
