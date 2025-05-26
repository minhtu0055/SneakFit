using Microsoft.EntityFrameworkCore;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.KhuyenMai;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.KhuyenMai
{
    public class KhuyenMaiService : IKhuyenMaiService
    {
        private readonly SneakFitDbContext _context;
        public KhuyenMaiService(SneakFitDbContext context)
        {
            _context = context;
        }
        public async Task<KhuyenMaiViewModels> Create(ThemKhuyenMai request)
        {
            var khuyenMai = new Data.Entities.KhuyenMai()
            {
                Id = Guid.NewGuid(),
                TenKhuyenMai = request.TenKhuyenMai,
                MoTa = request.MoTa,
                NgayTao = DateTime.Now,
                ThoiGianBatDau = request.ThoiGianBatDau,
                ThoiGianKetThuc = request.ThoiGianKetThuc,           
                LoaiGiamGia = request.LoaiGiamGia,
                GiaTriGiamGia = request.GiaTriGiamGia,            
                TrangThai = DateTime.Now >= request.ThoiGianBatDau ? TrangThaiGiamGia.HoatDong : TrangThaiGiamGia.KhongHoatDong,
            };

            // Kiểm tra tính hợp lệ của các sản phẩm trong danh sách
            foreach (var sanPhamId in request.SanPhamIds)
            {
                var sanPham = await _context.SanPham.FindAsync(sanPhamId);
                if (sanPham == null)
                {
                    throw new Exception($"Sản phẩm với ID {sanPhamId} không tồn tại.");
                }

                var khuyenMaiChiTiet = new KhuyenMaiChiTiet()
                {
                    SanPhamId = sanPhamId,
                    KhuyenMaiId = khuyenMai.Id
                };
                _context.KhuyenMaiChiTiet.Add(khuyenMaiChiTiet);
            }           
            _context.KhuyenMai.Add(khuyenMai);
            await _context.SaveChangesAsync();
            return await GetById(khuyenMai.Id);
        }
        // Phương thức lấy tất cả các khuyến mãi
        public async Task<PagedResult<KhuyenMaiViewModels>> GetAllPaging(PhanTrangKhuyenMai request)
        {
            var query = _context.KhuyenMai
                .Include(x => x.KhuyenMaiChiTiet)
                .ThenInclude(x => x.SanPham)
                .ThenInclude(x => x.SanPhamChiTiet)
                .AsQueryable();
            // Cập nhật trạng thái cho tất cả các khuyến mại
            var khuyenMais = await query.ToListAsync();
            foreach (var khuyenMai in khuyenMais)
            {
                await CapNhatTrangThaiKhuyenMai(khuyenMai);
            }
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.TenKhuyenMai.Contains(request.Keyword));
            }

            if (request.TrangThai.HasValue)
            {
                query = query.Where(x => x.TrangThai == request.TrangThai.Value);
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(khuyenMai => new KhuyenMaiViewModels()
                {
                    Id = khuyenMai.Id,
                    TenKhuyenMai = khuyenMai.TenKhuyenMai,
                    MoTa = khuyenMai.MoTa,
                    NgayTao = khuyenMai.NgayTao,
                    ThoiGianBatDau = khuyenMai.ThoiGianBatDau,
                    ThoiGianKetThuc = khuyenMai.ThoiGianKetThuc,              
                    LoaiGiamGia = khuyenMai.LoaiGiamGia,
                    GiaTriGiamGia = khuyenMai.GiaTriGiamGia,
                    TrangThai = khuyenMai.TrangThai,
                    SanPhams = khuyenMai.KhuyenMaiChiTiet.Select(p => new KhuyenMaiSanPhamViewModels()
                    {
                        SanPhamId = p.SanPhamId,
                        TenSanPham = p.SanPham.TenSanPham,
                        GiaGoc = p.SanPham.SanPhamChiTiet.Select(i => i.Gia).FirstOrDefault(),
                        GiaKhuyenMai = khuyenMai.LoaiGiamGia == LoaiGiamGia.PhamTram
                            ? p.SanPham.SanPhamChiTiet.Select(i => i.Gia).FirstOrDefault() * (100 - khuyenMai.GiaTriGiamGia) / 100
                            : p.SanPham.SanPhamChiTiet.Select(i => i.Gia).FirstOrDefault() - khuyenMai.GiaTriGiamGia
                    }).ToList()
                }).ToListAsync();
            var pagedResult = new PagedResult<KhuyenMaiViewModels>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };          
            return pagedResult;
        }

        // Phương thức lấy thông tin khuyến mãi theo ID
        public async Task<KhuyenMaiViewModels> GetById(Guid id)
        {
            var khuyenMai = await _context.KhuyenMai
                .Include(x => x.KhuyenMaiChiTiet)
                .ThenInclude(x => x.SanPham)
                .ThenInclude(x => x.SanPhamChiTiet)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (khuyenMai == null)
                throw new Exception($"Không tìm thấy khuyến mãi có id: {id}");  
            
            return new KhuyenMaiViewModels
            {
                Id = khuyenMai.Id,
                TenKhuyenMai = khuyenMai.TenKhuyenMai,
                MoTa = khuyenMai.MoTa,
                NgayTao = khuyenMai.NgayTao,
                ThoiGianBatDau = khuyenMai.ThoiGianBatDau,
                ThoiGianKetThuc = khuyenMai.ThoiGianKetThuc,              
                LoaiGiamGia = khuyenMai.LoaiGiamGia,
                GiaTriGiamGia = khuyenMai.GiaTriGiamGia,
                TrangThai = khuyenMai.TrangThai,
                SanPhams = khuyenMai.KhuyenMaiChiTiet.Select(p => new KhuyenMaiSanPhamViewModels()
                {
                    SanPhamId = p.SanPhamId,
                    TenSanPham = p.SanPham.TenSanPham,                   
                    GiaGoc = p.SanPham.SanPhamChiTiet.Select(i => i.Gia).FirstOrDefault(),
                    GiaKhuyenMai = khuyenMai.LoaiGiamGia == LoaiGiamGia.PhamTram
                            ? p.SanPham.SanPhamChiTiet.Select(i => i.Gia).FirstOrDefault() * (100 - khuyenMai.GiaTriGiamGia) / 100
                            : p.SanPham.SanPhamChiTiet.Select(i => i.Gia).FirstOrDefault() - khuyenMai.GiaTriGiamGia
                }).ToList()
            };
        }

        // Phương thức cập nhật khuyến mãi
        public async Task<KhuyenMaiViewModels> Update(SuaKhuyenMai request)
        {
            var khuyenMai = await _context.KhuyenMai.FindAsync(request.Id);
            if (khuyenMai == null)
                throw new Exception($"Không tìm thấy khuyến mãi có id: {request.Id}");
            khuyenMai.TenKhuyenMai = request.TenKhuyenMai;
            khuyenMai.MoTa = request.MoTa;
            khuyenMai.ThoiGianBatDau = request.ThoiGianBatDau;
            khuyenMai.ThoiGianKetThuc = request.ThoiGianKetThuc;        
            khuyenMai.LoaiGiamGia = request.LoaiGiamGia;
            khuyenMai.GiaTriGiamGia = request.GiaTriGiamGia;

            // Cập nhật trạng thái dựa trên thời gian
            await CapNhatTrangThaiKhuyenMai(khuyenMai);

            // Xóa các sản phẩm cũ
            var sanPhamCu = await _context.KhuyenMaiChiTiet
                .Where(x => x.KhuyenMaiId == request.Id)
                .ToListAsync();
            _context.KhuyenMaiChiTiet.RemoveRange(sanPhamCu);

            // Thêm các sản phẩm mới
            foreach (var sanPhamId in request.SanPhamIds)
            {
                var sanPham = await _context.SanPham.FindAsync(sanPhamId);
                if (request.SanPhamIds == null || !request.SanPhamIds.Any())
                {
                    throw new Exception("Danh sách sản phẩm không thể trống.");
                }

                var khuyenMaiChiTiet = new KhuyenMaiChiTiet()
                {
                    SanPhamId = sanPhamId,
                    KhuyenMaiId = khuyenMai.Id
                };
                _context.KhuyenMaiChiTiet.Add(khuyenMaiChiTiet);
            }

            await _context.SaveChangesAsync();

            return await GetById(request.Id);
        }

        public async Task<bool> UpdateStatus(Guid id, TrangThaiGiamGia trangThai)
        {
            var khuyenMai = await _context.KhuyenMai.FindAsync(id);
            if (khuyenMai == null)
                throw new Exception($"Không tìm thấy khuyến mãi có id: {id}");
            khuyenMai.TrangThai = trangThai;         
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task CapNhatTrangThaiKhuyenMai(Data.Entities.KhuyenMai khuyenMai)
        {
            var now = DateTime.Now;
            if (now >= khuyenMai.ThoiGianBatDau && now <= khuyenMai.ThoiGianKetThuc)
            {
                khuyenMai.TrangThai = TrangThaiGiamGia.HoatDong;
            }
            else if (now < khuyenMai.ThoiGianBatDau)
            {
                khuyenMai.TrangThai = TrangThaiGiamGia.KhongHoatDong;
            }
            else
            {
                khuyenMai.TrangThai = TrangThaiGiamGia.HetHan;
            }
        }
    }
}
