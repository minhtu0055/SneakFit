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
            if (request.GiaTriGiamGia < 0)
                throw new Exception("Giá trị giảm phải lớn hơn hoặc bằng 0");
            if (request.ThoiGianKetThuc <= request.ThoiGianBatDau)
                throw new Exception("Ngày kết thúc phải sau ngày bắt đầu");

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

            //// Kiểm tra tính hợp lệ của các sản phẩm trong danh sách
            //foreach (var sanPhamId in request.SanPhamIds)
            //{
            //    var sanPham = await _context.SanPham.FindAsync(sanPhamId);
            //    if (sanPham == null)
            //    {
            //        throw new Exception($"Sản phẩm với ID {sanPhamId} không tồn tại.");
            //    }

            //    var khuyenMaiChiTiet = new KhuyenMaiChiTiet()
            //    {
            //        SanPhamId = sanPhamId,
            //        KhuyenMaiId = khuyenMai.Id
            //    };
            //    _context.KhuyenMaiChiTiet.Add(khuyenMaiChiTiet);
            //}

            // Sử dụng HashSet để lưu trữ các SanPhamId duy nhất từ các SPCT được chọn
            var uniqueSanPhamIds = new HashSet<Guid>();

            // Duyệt qua danh sách SanPhamChiTietIds được gửi từ frontend (request.SanPhamIds chứa SPCT ID)
            foreach (var spctId in request.SanPhamIds)
            {
                // Tìm SanPhamChiTiet tương ứng và đảm bảo include SanPham gốc
                var sanPhamChiTiet = await _context.SanPhamChiTiet
                                                    .Include(s => s.SanPham) // Rất quan trọng: cần include SanPham để lấy SanPhamId
                                                    .FirstOrDefaultAsync(s => s.ID == spctId);

                if (sanPhamChiTiet == null)
                {
                    // Nếu SPCT không tồn tại, có thể throw lỗi hoặc bỏ qua tùy theo logic nghiệp vụ
                    throw new Exception($"Chi tiết sản phẩm với ID {spctId} không tồn tại.");
                }

                if (sanPhamChiTiet.SanPham == null)
                {
                    // Nếu SanPham gốc của SPCT đó không tồn tại (lỗi dữ liệu)
                    throw new Exception($"Không tìm thấy sản phẩm gốc cho chi tiết sản phẩm ID {spctId}.");
                }

                // Thêm SanPhamId của sản phẩm gốc vào HashSet (chỉ thêm các ID duy nhất)
                uniqueSanPhamIds.Add(sanPhamChiTiet.SanPhamId);
            }

            // Tạo các bản ghi KhuyenMaiChiTiet cho mỗi SanPhamId duy nhất đã thu thập
            foreach (var sanPhamId in uniqueSanPhamIds)
            {
                var khuyenMaiChiTiet = new KhuyenMaiChiTiet()
                {
                    SanPhamId = sanPhamId, // Sử dụng SanPhamId đúng
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
            if (request.GiaTriGiamGia < 0)
                throw new Exception("Giá trị giảm phải lớn hơn hoặc bằng 0");
            if (request.ThoiGianKetThuc <= request.ThoiGianBatDau)
                throw new Exception("Ngày kết thúc phải sau ngày bắt đầu");

            khuyenMai.TenKhuyenMai = request.TenKhuyenMai;
            khuyenMai.MoTa = request.MoTa;
            khuyenMai.ThoiGianBatDau = request.ThoiGianBatDau;
            khuyenMai.ThoiGianKetThuc = request.ThoiGianKetThuc;
            khuyenMai.LoaiGiamGia = request.LoaiGiamGia;
            khuyenMai.GiaTriGiamGia = request.GiaTriGiamGia;


            await CapNhatTrangThaiKhuyenMai(khuyenMai);

            var sanPhamCu = await _context.KhuyenMaiChiTiet
                .Where(x => x.KhuyenMaiId == request.Id)
                .ToListAsync();
            _context.KhuyenMaiChiTiet.RemoveRange(sanPhamCu);

            if (request.SanPhamIds != null && request.SanPhamIds.Any())
            {
                foreach (var sanPhamId in request.SanPhamIds)
                {
                    var sanPham = await _context.SanPham.FindAsync(sanPhamId);
                    if (sanPham == null)
                        throw new Exception($"Sản phẩm với ID {sanPhamId} không tồn tại.");

                    var khuyenMaiChiTiet = new KhuyenMaiChiTiet()
                    {
                        SanPhamId = sanPhamId,
                        KhuyenMaiId = khuyenMai.Id
                    };
                    _context.KhuyenMaiChiTiet.Add(khuyenMaiChiTiet);
                }
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
