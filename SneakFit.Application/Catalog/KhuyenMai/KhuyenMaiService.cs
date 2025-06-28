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
            // Kiểm tra tên khuyến mãi đã tồn tại (không phân biệt hoa thường)
            var existed = await _context.KhuyenMai
                .AnyAsync(x => x.TenKhuyenMai.ToLower() == request.TenKhuyenMai.Trim().ToLower());
            if (existed)
                throw new Exception("Tên khuyến mãi đã tồn tại, vui lòng chọn tên khác.");
            if (request.GiaTriGiamGia < 0)
                throw new Exception("Giá trị giảm phải lớn hơn hoặc bằng 0");
            if (request.LoaiGiamGia == LoaiGiamGia.PhamTram && request.GiaTriGiamGia > 100)
                throw new Exception("Giá trị giảm theo phần trăm không được vượt quá 100%");
            if (request.LoaiGiamGia == LoaiGiamGia.SoTien)
            {
                foreach (var spctId in request.SanPhamIds)
                {
                    var spct = await _context.SanPhamChiTiet.FindAsync(spctId);
                    if (spct == null)
                        throw new Exception($"Chi tiết sản phẩm với ID {spctId} không tồn tại.");
                    if (request.GiaTriGiamGia > spct.Gia)
                        throw new Exception($"Giá trị giảm không được vượt quá giá của sản phẩm chi tiết (ID: {spctId})");
                }
            }
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
            foreach (var spctId in request.SanPhamIds)
            {
                var spct = await _context.SanPhamChiTiet.FindAsync(spctId);
                if (spct == null) continue; // hoặc throw lỗi nếu cần

                var khuyenMaiChiTiet = new KhuyenMaiChiTiet()
                {
                    SanPhamId = spct.SanPhamId, // vẫn lưu SanPhamId để join nếu cần
                    SPCTId = spctId,            // Lưu đúng SPCTId mà người dùng chọn
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
                    NgaySuaDoi = khuyenMai.NgaySuaDoi,
                    ThoiGianBatDau = khuyenMai.ThoiGianBatDau,
                    ThoiGianKetThuc = khuyenMai.ThoiGianKetThuc,              
                    LoaiGiamGia = khuyenMai.LoaiGiamGia,
                    GiaTriGiamGia = khuyenMai.GiaTriGiamGia,
                    TrangThai = khuyenMai.TrangThai,
                    SanPhams = khuyenMai.KhuyenMaiChiTiet.Select(p => new KhuyenMaiSanPhamViewModels
                    {
                        SanPhamId = p.SanPhamId,
                        SPCTId = p.SPCTId,
                        TenSanPham = p.SanPham.TenSanPham,
                        GiaGoc = _context.SanPhamChiTiet
                    .Where(x => x.ID == p.SPCTId)
                    .Select(x => (decimal?)x.Gia)
                    .FirstOrDefault() ?? 0m,


                        GiaKhuyenMai = _context.SanPhamChiTiet.FirstOrDefault(x => x.ID == p.SPCTId) != null
                    ? (khuyenMai.LoaiGiamGia == LoaiGiamGia.PhamTram
                        ? _context.SanPhamChiTiet.FirstOrDefault(x => x.ID == p.SPCTId).Gia * (100 - khuyenMai.GiaTriGiamGia) / 100
                        : _context.SanPhamChiTiet.FirstOrDefault(x => x.ID == p.SPCTId).Gia - khuyenMai.GiaTriGiamGia)
                    : 0
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

            // Lấy danh sách sản phẩm chi tiết
            var sanPhamChiTiets = khuyenMai.KhuyenMaiChiTiet
                .SelectMany(kmct => kmct.SanPham.SanPhamChiTiet)
                .DistinctBy(spct => spct.ID) // <-- THÊM DÒNG NÀY
                .Select(spct => new KhuyenMaiSPCTViewModels
                {
                    SPCTId = spct.ID,
                    MauSacId = spct.MauSacId,
                    KichThuocId = spct.KichThuocId,
                    ChatLieuId = spct.ChatLieuId,
                    DeGiayId = spct.DeGiayId,
                    ThuongHieuId = spct.ThuongHieuId,
                    SanPhamId = spct.SanPhamId,
                    Gia = spct.Gia,
                    SoLuong = spct.SoLuong,
                    TrangThai = spct.TrangThai,
                    NgayTao = spct.NgayTao
                }).ToList();

            return new KhuyenMaiViewModels
            {
                Id = khuyenMai.Id,
                TenKhuyenMai = khuyenMai.TenKhuyenMai,
                MoTa = khuyenMai.MoTa,
                NgayTao = khuyenMai.NgayTao,
                NgaySuaDoi = khuyenMai.NgaySuaDoi,
                ThoiGianBatDau = khuyenMai.ThoiGianBatDau,
                ThoiGianKetThuc = khuyenMai.ThoiGianKetThuc,              
                LoaiGiamGia = khuyenMai.LoaiGiamGia,
                GiaTriGiamGia = khuyenMai.GiaTriGiamGia,
                TrangThai = khuyenMai.TrangThai,
                SanPhams = khuyenMai.KhuyenMaiChiTiet.Select(p => new KhuyenMaiSanPhamViewModels
                {
                    SanPhamId = p.SanPhamId,
                    SPCTId = p.SPCTId,
                    TenSanPham = p.SanPham.TenSanPham,
                    GiaGoc = _context.SanPhamChiTiet
                    .Where(x => x.ID == p.SPCTId)
                    .Select(x => (decimal?)x.Gia)
                    .FirstOrDefault() ?? 0m,


                    GiaKhuyenMai = _context.SanPhamChiTiet.FirstOrDefault(x => x.ID == p.SPCTId) != null
                    ? (khuyenMai.LoaiGiamGia == LoaiGiamGia.PhamTram
                        ? _context.SanPhamChiTiet.FirstOrDefault(x => x.ID == p.SPCTId).Gia * (100 - khuyenMai.GiaTriGiamGia) / 100
                        : _context.SanPhamChiTiet.FirstOrDefault(x => x.ID == p.SPCTId).Gia - khuyenMai.GiaTriGiamGia)
                    : 0
                }).ToList(),
                SanPhamChiTiets = sanPhamChiTiets
            };
        }

        // Phương thức cập nhật khuyến mãi
        public async Task<KhuyenMaiViewModels> Update(SuaKhuyenMai request)
        {
            var khuyenMai = await _context.KhuyenMai.FindAsync(request.Id);

            // Kiểm tra tên khuyến mãi đã tồn tại cho bản ghi khác (không phân biệt hoa thường)
            var existed = await _context.KhuyenMai
                .AnyAsync(x => x.TenKhuyenMai.ToLower() == request.TenKhuyenMai.Trim().ToLower() && x.Id != request.Id);
            if (existed)
                throw new Exception("Tên khuyến mãi đã tồn tại, vui lòng chọn tên khác.");

            if (khuyenMai == null)
                throw new Exception($"Không tìm thấy khuyến mãi có id: {request.Id}");
            if (request.GiaTriGiamGia < 0)
                throw new Exception("Giá trị giảm phải lớn hơn hoặc bằng 0");
            if (request.LoaiGiamGia == LoaiGiamGia.PhamTram && request.GiaTriGiamGia > 100)
                throw new Exception("Giá trị giảm theo phần trăm không được vượt quá 100%");
            if (request.LoaiGiamGia == LoaiGiamGia.SoTien)
            {
                foreach (var spctId in request.SanPhamIds)
                {
                    var spct = await _context.SanPhamChiTiet.FindAsync(spctId);
                    if (spct == null)
                        throw new Exception($"Chi tiết sản phẩm với ID {spctId} không tồn tại.");
                    if (request.GiaTriGiamGia > spct.Gia)
                        throw new Exception($"Giá trị giảm không được vượt quá giá của sản phẩm chi tiết (ID: {spctId})");
                }
            }
            if (request.ThoiGianKetThuc <= request.ThoiGianBatDau)
                throw new Exception("Ngày kết thúc phải sau ngày bắt đầu");

            khuyenMai.TenKhuyenMai = request.TenKhuyenMai;
            khuyenMai.MoTa = request.MoTa;
            khuyenMai.ThoiGianBatDau = request.ThoiGianBatDau;
            khuyenMai.ThoiGianKetThuc = request.ThoiGianKetThuc;
            khuyenMai.LoaiGiamGia = request.LoaiGiamGia;
            khuyenMai.GiaTriGiamGia = request.GiaTriGiamGia;
            khuyenMai.NgaySuaDoi = DateTime.Now;


            await CapNhatTrangThaiKhuyenMai(khuyenMai);

            var sanPhamCu = await _context.KhuyenMaiChiTiet
                .Where(x => x.KhuyenMaiId == request.Id)
                .ToListAsync();
            _context.KhuyenMaiChiTiet.RemoveRange(sanPhamCu);

            if (request.SanPhamIds != null && request.SanPhamIds.Any())
            {
                var uniqueSanPhamIds = new HashSet<Guid>();
                foreach (var spctId in request.SanPhamIds)
                {
                    var sanPhamChiTiet = await _context.SanPhamChiTiet
                        .Include(s => s.SanPham)
                        .FirstOrDefaultAsync(s => s.ID == spctId);

                    if (sanPhamChiTiet == null)
                        throw new Exception($"Chi tiết sản phẩm với ID {spctId} không tồn tại.");

                    if (sanPhamChiTiet.SanPham == null)
                        throw new Exception($"Không tìm thấy sản phẩm gốc cho chi tiết sản phẩm ID {spctId}.");

                    uniqueSanPhamIds.Add(sanPhamChiTiet.SanPhamId);
                }
                foreach (var spctId in request.SanPhamIds)
                {
                    var spct = await _context.SanPhamChiTiet.FindAsync(spctId);
                    if (spct == null) continue; // hoặc throw lỗi nếu cần

                    var khuyenMaiChiTiet = new KhuyenMaiChiTiet()
                    {
                        SanPhamId = spct.SanPhamId, // vẫn lưu SanPhamId để join nếu cần
                        SPCTId = spctId,            // Lưu đúng SPCTId mà người dùng chọn
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
