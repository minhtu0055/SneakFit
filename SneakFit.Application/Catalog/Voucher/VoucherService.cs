using Microsoft.EntityFrameworkCore;
using SneakFit.Application.Email;
using SneakFit.Data.EF;
using SneakFit.Data.Entities;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.Voucher;
using SneakFit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SneakFit.Application.Catalog.Voucher
{
    public class VoucherService : IVoucherService
    {
        public readonly SneakFitDbContext _context;
        private readonly IEmailSender _emailSender;
        public VoucherService(SneakFitDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // Hàm kiểm tra và cập nhật trạng thái voucher
        private TrangThaiGiamGia GetVoucherStatus(DateTime startTime, DateTime endTime)
        {
            var now = DateTime.Now;
            if (now < startTime)
                return TrangThaiGiamGia.KhongHoatDong;
            if (now > endTime)
                return TrangThaiGiamGia.HetHan;
            return TrangThaiGiamGia.HoatDong;
        }

        public async Task<VoucherViewModels> Create(CreateVoucher request)
        {
            //Tự động tạo mã 
            string newCode = await GetNextVoucherCode();

            // Kiểm tra mã voucher trùng lặp
            var existingVoucher = await _context.Voucher.FirstOrDefaultAsync(x => x.MaVoucher == request.MaVoucher);
            if (existingVoucher != null)
            {
                throw new Exception($"Mã voucher '{request.MaVoucher}' đã tồn tại trong hệ thống.");
            }

            // Kiểm tra nếu là voucher riêng tư thì phải có người dùng được chọn
            if (request.LoaiVoucher == LoaiVoucher.RiengTu)
            {
                if (request.SelectedUserIds == null || !request.SelectedUserIds.Any())
                {
                    throw new Exception("Voucher riêng tư phải có ít nhất một người dùng được chọn.");
                }
            }

            var vc = new Data.Entities.Voucher()
            {
                Id = Guid.NewGuid(),
                MaVoucher = request.MaVoucher,
                LoaiGiamGia = request.LoaiGiamGia,
                GiaTriGiamGia = request.GiaTriGiamGia,
                DieuKienApDung = request.DieuKienApDung,
                GiaTriToiDa = request.LoaiGiamGia == LoaiGiamGia.PhamTram
                    ? request.GiaTriToiDa ?? 0
                    : request.GiaTriGiamGia,
                SoLuong = request.LoaiVoucher == LoaiVoucher.CongKhai ? request.SoLuong ?? 0 : request.SelectedUserIds?.Count ?? 0,
                NgayTao = DateTime.Now,
                ThoiGianBatDau = request.ThoiGianBatDau ?? throw new Exception("Thời gian bắt đầu không được để trống"),
                ThoiGianKetThuc = request.ThoiGianKetThuc ?? throw new Exception("Thời gian kết thúc không được để trống"),
                TrangThai = GetVoucherStatus(
                    request.ThoiGianBatDau ?? throw new Exception("Thời gian bắt đầu không được để trống"),
                    request.ThoiGianKetThuc ?? throw new Exception("Thời gian kết thúc không được để trống")
                ),
                loaiVoucher = request.LoaiVoucher,
            };
            _context.Voucher.Add(vc);

            // Chỉ xử lý SelectedUserIds nếu là voucher riêng tư
            if (request.LoaiVoucher == LoaiVoucher.RiengTu && request.SelectedUserIds != null && request.SelectedUserIds.Any())
            {
                var voucherUsers = request.SelectedUserIds.Select(userId => new VoucherUser
                {
                    Id = Guid.NewGuid(),
                    VoucherId = vc.Id,
                    UserId = userId,
                    IsUsed = false
                }).ToList();

                vc.SoLuong = voucherUsers.Count;
                _context.VoucherUser.AddRange(voucherUsers);

                // Lấy thông tin người dùng để gửi email
                var users = await _context.Users
                    .Where(u => request.SelectedUserIds.Contains(u.Id))
                    .ToListAsync();

                // Gửi email cho từng người dùng
                foreach (var user in users)
                {
                    var emailBody = $@"
                        <h2>Xin chào {user.HoVaTen},</h2>
                        <p>Bạn đã nhận được một voucher mới từ SneakFit:</p>
                        <ul>
                            <li><strong>Mã voucher:</strong> {vc.MaVoucher}</li>
                            <li><strong>Loại giảm giá:</strong> {(vc.LoaiGiamGia == LoaiGiamGia.PhamTram ? "Giảm theo phần trăm" : "Giảm theo số tiền")}</li>
                            <li><strong>Giá trị giảm giá:</strong> {vc.GiaTriGiamGia}{(vc.LoaiGiamGia == LoaiGiamGia.PhamTram ? "%" : " VNĐ")}</li>
                            <li><strong>Điều kiện áp dụng:</strong> {vc.DieuKienApDung:N0} VNĐ</li>
                            <li><strong>Điều kiện áp dụng:</strong> {vc.GiaTriToiDa:N0} VNĐ</li>
                            <li><strong>Thời gian sử dụng:</strong> từ {vc.ThoiGianBatDau:dd/MM/yyyy HH:mm} đến {vc.ThoiGianKetThuc:dd/MM/yyyy HH:mm}</li>
                        </ul>
                        <p>Vui lòng sử dụng mã voucher này khi thanh toán đơn hàng của bạn.</p>
                        <p>Trân trọng,<br>SneakFit Team</p>";

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        $"Voucher mới từ SneakFit - {vc.MaVoucher}",
                        emailBody
                    );
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Voucher_MaVoucher"))
                {
                    throw new Exception("Mã voucher đã tồn tại, vui lòng chọn mã khác.");
                }
                throw;
            }
            return await GetById(vc.Id);
        }

        public async Task<string> GetNextVoucherCode()
        {
            var lastVoucher = await _context.Voucher
                .OrderByDescending(v => v.NgayTao)
                .FirstOrDefaultAsync();

            string newCode = "VC001";
            if (lastVoucher != null && lastVoucher.MaVoucher.StartsWith("VC"))
            {
                var numberPart = lastVoucher.MaVoucher.Substring(2);
                if (int.TryParse(numberPart, out int num))
                {
                    newCode = $"VC{(num + 1):D3}";
                }
            }
            return newCode;
        }

        public async Task<PagedResult<VoucherViewModels>> GetAllPaging(GetVoucherPagingRequest request)
        {
            var query = _context.Voucher.AsQueryable();

            // Cập nhật trạng thái cho tất cả voucher trước khi lấy danh sách
            var vouchers = await query.ToListAsync();
            foreach (var voucher in vouchers)
            {
                var newStatus = GetVoucherStatus(voucher.ThoiGianBatDau, voucher.ThoiGianKetThuc);
                // Kiểm tra trạng thái dựa trên loại voucher
                if (voucher.loaiVoucher == LoaiVoucher.CongKhai && voucher.SoLuong <= 0)
                {
                    voucher.SoLuong = 0;
                    voucher.TrangThai = TrangThaiGiamGia.HetHan;
                }
                else if (voucher.loaiVoucher == LoaiVoucher.RiengTu && voucher.SoLuong <= 0)
                {
                    voucher.SoLuong = 0;
                    voucher.TrangThai = TrangThaiGiamGia.HetHan;
                }
                else
                {
                    voucher.TrangThai = newStatus;
                }
            }
            await _context.SaveChangesAsync();

            // Lọc theo từ khóa
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.MaVoucher.Contains(request.Keyword));
            }

            // Lọc theo trạng thái
            if (request.Status.HasValue)
            {
                query = query.Where(x => x.TrangThai == request.Status.Value);
            }

            // Tính toán tổng số bản ghi
            int totalRow = await query.CountAsync();

            // Sắp xếp theo ngày tạo từ mới đến cũ
            query = query.OrderByDescending(x => x.NgayTao);

            // Lấy dữ liệu theo trang
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new VoucherViewModels()
                {
                    Id = x.Id,
                    MaVoucher = x.MaVoucher,
                    LoaiGiamGia = x.LoaiGiamGia,
                    loaiVoucher = x.loaiVoucher,
                    GiaTriGiamGia = x.GiaTriGiamGia,
                    DieuKienApDung = x.DieuKienApDung,
                    GiaTriToiDa = x.GiaTriToiDa,
                    SoLuong = x.SoLuong,
                    NgayTao = x.NgayTao,
                    ThoiGianBatDau = x.ThoiGianBatDau,
                    ThoiGianKetThuc = x.ThoiGianKetThuc,
                    TrangThai = x.TrangThai
                }).ToListAsync();

            // Tạo đối tượng phân trang
            var pagedResult = new PagedResult<VoucherViewModels>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };

            return pagedResult;
        }

        public async Task<VoucherViewModels> GetById(Guid id)
        {
            var voucher = await _context.Voucher.FindAsync(id);
            if (voucher == null) return null;

            // Cập nhật trạng thái trước khi trả về
            var newStatus = GetVoucherStatus(voucher.ThoiGianBatDau, voucher.ThoiGianKetThuc);
            if (voucher.TrangThai != newStatus)
            {
                voucher.TrangThai = newStatus;
                await _context.SaveChangesAsync();
            }

            return new VoucherViewModels()
            {
                Id = voucher.Id,
                MaVoucher = voucher.MaVoucher,
                LoaiGiamGia = voucher.LoaiGiamGia,
                loaiVoucher = voucher.loaiVoucher,
                GiaTriGiamGia = voucher.GiaTriGiamGia,
                DieuKienApDung = voucher.DieuKienApDung,
                GiaTriToiDa = voucher.GiaTriToiDa,
                SoLuong = voucher.SoLuong,
                NgayTao = voucher.NgayTao,
                ThoiGianBatDau = voucher.ThoiGianBatDau,
                ThoiGianKetThuc = voucher.ThoiGianKetThuc,
                TrangThai = voucher.TrangThai
            };
        }

        public async Task<VoucherViewModels> GetByCode(string code)
        {
            var voucher = await _context.Voucher.FirstOrDefaultAsync(x => x.MaVoucher == code);
            if (voucher == null) return null;

            // Cập nhật trạng thái trước khi trả về
            var newStatus = GetVoucherStatus(voucher.ThoiGianBatDau, voucher.ThoiGianKetThuc);
            if (voucher.TrangThai != newStatus)
            {
                voucher.TrangThai = newStatus;
                await _context.SaveChangesAsync();
            }

            return new VoucherViewModels()
            {
                Id = voucher.Id,
                MaVoucher = voucher.MaVoucher,
                LoaiGiamGia = voucher.LoaiGiamGia,
                loaiVoucher = voucher.loaiVoucher,
                GiaTriGiamGia = voucher.GiaTriGiamGia,
                DieuKienApDung = voucher.DieuKienApDung,
                GiaTriToiDa = voucher.GiaTriToiDa,
                SoLuong = voucher.SoLuong,
                NgayTao = voucher.NgayTao,
                ThoiGianBatDau = voucher.ThoiGianBatDau,
                ThoiGianKetThuc = voucher.ThoiGianKetThuc,
                TrangThai = voucher.TrangThai
            };
        }

        public async Task<VoucherViewModels> Update(UpdateVoucher request)
        {
            var voucher = await _context.Voucher.FindAsync(request.Id);
            if (voucher == null) return null;

            if (request.LoaiVoucher == LoaiVoucher.RiengTu &&
                (request.SelectedUserIds == null || !request.SelectedUserIds.Any()))
            {
                throw new Exception("Voucher riêng tư phải có ít nhất một khách hàng được chọn.");
            }

            // Kiểm tra nếu đang cố gắng chuyển từ riêng tư sang công khai
            if (voucher.loaiVoucher == LoaiVoucher.RiengTu && request.LoaiVoucher == LoaiVoucher.CongKhai)
            {
                throw new Exception("Không thể chuyển voucher riêng tư sang công khai.");
            }
            // Cập nhật thông tin voucher
            voucher.LoaiGiamGia = request.LoaiGiamGia;
            voucher.GiaTriGiamGia = request.GiaTriGiamGia;
            voucher.DieuKienApDung = request.DieuKienApDung;
            voucher.GiaTriToiDa = request.LoaiGiamGia == LoaiGiamGia.PhamTram
                ? request.GiaTriToiDa ?? 0
                : request.GiaTriGiamGia;
            voucher.SoLuong = request.LoaiVoucher == LoaiVoucher.CongKhai ? request.SoLuong ?? 0 : request.SelectedUserIds?.Count ?? 0;
            voucher.ThoiGianBatDau = request.ThoiGianBatDau ?? throw new Exception("Thời gian bắt đầu không được để trống");
            voucher.ThoiGianKetThuc = request.ThoiGianKetThuc ?? throw new Exception("Thời gian kết thúc không được để trống");
            voucher.loaiVoucher = request.LoaiVoucher;
            voucher.TrangThai = GetVoucherStatus(
                request.ThoiGianBatDau ?? throw new Exception("Thời gian bắt đầu không được để trống"),
                request.ThoiGianKetThuc ?? throw new Exception("Thời gian kết thúc không được để trống")
            );

            // Xử lý khi chuyển từ voucher công khai sang riêng tư
            if (voucher.loaiVoucher == LoaiVoucher.RiengTu && request.SelectedUserIds != null && request.SelectedUserIds.Any())
            {
                // Lấy danh sách khách hàng hiện tại của voucher
                var existingVoucherUsers = await _context.VoucherUser
                    .Where(vu => vu.VoucherId == voucher.Id)
                    .Select(vu => vu.UserId)
                    .ToListAsync();

                // Xác định khách hàng mới và khách hàng cũ
                var newUserIds = request.SelectedUserIds.Except(existingVoucherUsers).ToList();
                var removedUserIds = existingVoucherUsers.Except(request.SelectedUserIds).ToList();

                // Thêm khách hàng mới
                foreach (var userId in newUserIds)
                {
                    var voucherUser = new VoucherUser
                    {
                        VoucherId = voucher.Id,
                        UserId = userId
                    };
                    _context.VoucherUser.Add(voucherUser);

                    // Cập nhật số lượng voucher riêng tư
                    voucher.SoLuong = request.SelectedUserIds.Count;

                    // Gửi email thông báo cho khách hàng mới
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        var emailBody = $@"
                            <h2>Xin chào {user.HoVaTen},</h2>
                            <p>Bạn đã nhận được một voucher mới từ SneakFit:</p>
                            <ul>
                                <li><strong>Mã voucher:</strong> {voucher.MaVoucher}</li>
                                <li><strong>Loại giảm giá:</strong> {(voucher.LoaiGiamGia == LoaiGiamGia.PhamTram ? "Giảm theo phần trăm" : "Giảm theo số tiền")}</li>
                                <li><strong>Giá trị giảm giá:</strong> {voucher.GiaTriGiamGia}{(voucher.LoaiGiamGia == LoaiGiamGia.PhamTram ? "%" : " VNĐ")}</li>
                                <li><strong>Điều kiện áp dụng:</strong> {voucher.DieuKienApDung:N0} VNĐ</li>
                                <li><strong>Điều kiện áp dụng:</strong> {voucher.GiaTriToiDa:N0} VNĐ</li>
                                <li><strong>Thời gian sử dụng:</strong> từ {voucher.ThoiGianBatDau:dd/MM/yyyy HH:mm} đến {voucher.ThoiGianKetThuc:dd/MM/yyyy HH:mm}</li>
                            </ul>
                            <p>Vui lòng sử dụng mã voucher này khi thanh toán đơn hàng của bạn.</p>
                            <p>Trân trọng,<br>SneakFit Team</p>";

                        await _emailSender.SendEmailAsync(
                            user.Email,
                            $"Voucher mới từ SneakFit - {voucher.MaVoucher}",
                            emailBody
                        );
                    }
                }

                //// Xóa khách hàng bị loại bỏ
                //foreach (var userId in removedUserIds)
                //{
                //    var voucherUser = await _context.VoucherUser
                //        .FirstOrDefaultAsync(vu => vu.VoucherId == voucher.Id && vu.UserId == userId);
                //    if (voucherUser != null)
                //    {
                //        _context.VoucherUser.Remove(voucherUser);
                //    }
                //}

                // Gửi email thông báo cập nhật cho khách hàng cũ
                var existingUsers = await _context.Users
                    .Where(u => existingVoucherUsers.Contains(u.Id))
                    .ToListAsync();

                foreach (var user in existingUsers)
                {
                    var emailBody = $@"
                        <h2>Xin chào {user.HoVaTen},</h2>
                        <p>Voucher của bạn đã được cập nhật với thông tin mới:</p>
                        <ul>
                            <li><strong>Mã voucher:</strong> {voucher.MaVoucher}</li>
                            <li><strong>Loại giảm giá:</strong> {(voucher.LoaiGiamGia == LoaiGiamGia.PhamTram ? "Giảm theo phần trăm" : "Giảm theo số tiền")}</li>
                            <li><strong>Giá trị giảm giá:</strong> {voucher.GiaTriGiamGia}{(voucher.LoaiGiamGia == LoaiGiamGia.PhamTram ? "%" : " VNĐ")}</li>
                            <li><strong>Điều kiện áp dụng:</strong> {voucher.DieuKienApDung:N0} VNĐ</li>
                            <li><strong>Điều kiện áp dụng:</strong> {voucher.GiaTriToiDa:N0} VNĐ</li>
                            <li><strong>Thời gian sử dụng:</strong> từ {voucher.ThoiGianBatDau:dd/MM/yyyy HH:mm} đến {voucher.ThoiGianKetThuc:dd/MM/yyyy HH:mm}</li>
                        </ul>
                        <p>Vui lòng kiểm tra thông tin mới của voucher trước khi sử dụng.</p>
                        <p>Trân trọng,<br>SneakFit Team</p>";

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        $"Cập nhật voucher SneakFit - {voucher.MaVoucher}",
                        emailBody
                    );
                }
            }
            // Xử lý khi là voucher riêng tư và có thay đổi thông tin
            else if (voucher.loaiVoucher == LoaiVoucher.RiengTu)
            {
                // Cập nhật số lượng voucher riêng tư
                voucher.SoLuong = request.SelectedUserIds?.Count ?? 0;

                // Lấy danh sách khách hàng được gán voucher này
                var voucherUsers = await _context.VoucherUser
                    .Where(vu => vu.VoucherId == voucher.Id)
                    .Include(vu => vu.User)
                    .ToListAsync();

                // Gửi email cho từng khách hàng
                foreach (var voucherUser in voucherUsers)
                {
                    var user = voucherUser.User;
                    var emailBody = $@"
                        <h2>Xin chào {user.HoVaTen},</h2>
                        <p>Voucher của bạn đã được cập nhật với thông tin mới:</p>
                        <ul>
                            <li><strong>Mã voucher:</strong> {voucher.MaVoucher}</li>
                            <li><strong>Loại giảm giá:</strong> {(voucher.LoaiGiamGia == LoaiGiamGia.PhamTram ? "Giảm theo phần trăm" : "Giảm theo số tiền")}</li>
                            <li><strong>Giá trị giảm giá:</strong> {voucher.GiaTriGiamGia}{(voucher.LoaiGiamGia == LoaiGiamGia.PhamTram ? "%" : " VNĐ")}</li>
                            <li><strong>Điều kiện áp dụng:</strong> {voucher.DieuKienApDung:N0} VNĐ</li>
                            <li><strong>Điều kiện áp dụng:</strong> {voucher.GiaTriToiDa:N0} VNĐ</li>
                            <li><strong>Thời gian sử dụng:</strong> từ {voucher.ThoiGianBatDau:dd/MM/yyyy HH:mm} đến {voucher.ThoiGianKetThuc:dd/MM/yyyy HH:mm}</li>
                        </ul>
                        <p>Vui lòng kiểm tra thông tin mới của voucher trước khi sử dụng.</p>
                        <p>Trân trọng,<br>SneakFit Team</p>";

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        $"Cập nhật voucher SneakFit - {voucher.MaVoucher}",
                        emailBody
                    );
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Voucher_MaVoucher"))
                {
                    throw new Exception("Mã voucher đã tồn tại, vui lòng chọn mã khác.");
                }
                throw;
            }

            return new VoucherViewModels()
            {
                Id = voucher.Id,
                MaVoucher = voucher.MaVoucher,
                LoaiGiamGia = voucher.LoaiGiamGia,
                loaiVoucher = voucher.loaiVoucher,
                GiaTriGiamGia = voucher.GiaTriGiamGia,
                DieuKienApDung = voucher.DieuKienApDung,
                GiaTriToiDa = voucher.GiaTriToiDa,
                SoLuong = voucher.SoLuong,
                NgayTao = voucher.NgayTao,
                ThoiGianBatDau = voucher.ThoiGianBatDau,
                ThoiGianKetThuc = voucher.ThoiGianKetThuc,
                TrangThai = voucher.TrangThai
            };
        }

        public async Task<bool> UpdateTrangThai(Guid Id, TrangThaiGiamGia status)
        {
            var voucher = await _context.Voucher.FindAsync(Id);
            if (voucher == null) return false;

            // Chỉ cho phép cập nhật trạng thái thủ công nếu voucher chưa hết hạn
            if (DateTime.Now <= voucher.ThoiGianKetThuc)
            {
                voucher.TrangThai = status;
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UseVoucher(string code, Guid userId)
        {
            var voucher = await _context.Voucher.FirstOrDefaultAsync(x => x.MaVoucher == code);
            if (voucher == null) return false;

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.TrangThai == false) return false; // Không hoạt động hoặc không tồn tại

            // Nếu là voucher riêng tư → kiểm tra xem user có được gán không
            if (voucher.loaiVoucher == LoaiVoucher.RiengTu)
            {
                var isAssigned = await _context.VoucherUser
                    .AnyAsync(vu => vu.VoucherId == voucher.Id && vu.UserId == userId);

                if (!isAssigned) return false; // Không có quyền sử dụng
            }

            // Cập nhật trạng thái voucher trước khi kiểm tra
            var newStatus = GetVoucherStatus(voucher.ThoiGianBatDau, voucher.ThoiGianKetThuc);
            if (voucher.TrangThai != newStatus)
            {
                voucher.TrangThai = newStatus;
                await _context.SaveChangesAsync();
            }

            // Kiểm tra điều kiện sử dụng voucher
            if (voucher.SoLuong <= 0 || voucher.TrangThai != TrangThaiGiamGia.HoatDong)
            {
                return false;
            }

            // ✅ Giảm số lượng
            voucher.SoLuong--;

            // ✅ Nếu hết số lượng thì cập nhật trạng thái
            if (voucher.SoLuong <= 0)
            {
                voucher.SoLuong = 0;
                voucher.TrangThai = TrangThaiGiamGia.HetHan;
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> GiamSoLuongVoucher(Guid id, int soLuong)
        {
            var voucher = await _context.Voucher.FindAsync(id);
            if (voucher == null) return false;
            if (voucher.SoLuong < soLuong) return false;
            voucher.SoLuong -= soLuong;
            if (voucher.SoLuong <= 0)
            {
                voucher.SoLuong = 0;
                voucher.TrangThai = TrangThaiGiamGia.HetHan;
            }
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<VoucherViewModels>> GetVouchersForUser(Guid userId, decimal tongTienHoaDon)
        {
            var now = DateTime.Now;
            var privateVouchers = await (from v in _context.Voucher
                                         join vu in _context.VoucherUser on v.Id equals vu.VoucherId
                                         where vu.UserId == userId
                                            && v.loaiVoucher == LoaiVoucher.RiengTu
                                            && v.TrangThai == TrangThaiGiamGia.HoatDong
                                            && v.SoLuong > 0
                                            && v.ThoiGianBatDau <= now
                                            && v.ThoiGianKetThuc >= now
                                            && v.DieuKienApDung <= tongTienHoaDon
                                            && !vu.IsUsed // Chỉ lấy voucher chưa được sử dụng
                                         select new { Voucher = v, VoucherUser = vu }).ToListAsync();

            // Map sang ViewModel
            var result = privateVouchers.Select(x => new VoucherViewModels
            {
                Id = x.Voucher.Id,
                MaVoucher = x.Voucher.MaVoucher,
                LoaiGiamGia = x.Voucher.LoaiGiamGia,
                loaiVoucher = x.Voucher.loaiVoucher,
                GiaTriGiamGia = x.Voucher.GiaTriGiamGia,
                DieuKienApDung = x.Voucher.DieuKienApDung,
                GiaTriToiDa = x.Voucher.GiaTriToiDa,
                SoLuong = 1, // Mỗi user chỉ có 1 voucher riêng tư
                NgayTao = x.Voucher.NgayTao,
                ThoiGianBatDau = x.Voucher.ThoiGianBatDau,
                ThoiGianKetThuc = x.Voucher.ThoiGianKetThuc,
                TrangThai = x.Voucher.TrangThai
            }).ToList();

            return result;
        }

        public async Task<List<VoucherViewModels>> GetPublicVouchers(decimal tongTienHoaDon)
        {
            var now = DateTime.Now;
            var publicVouchers = await _context.Voucher
                .Where(v => v.loaiVoucher == LoaiVoucher.CongKhai
                && v.TrangThai == TrangThaiGiamGia.HoatDong
                && v.SoLuong > 0
                && v.ThoiGianBatDau <= now
                && v.ThoiGianKetThuc >= now
                && v.DieuKienApDung <= tongTienHoaDon) // Thêm điều kiện này
            .ToListAsync();

            return publicVouchers.Select(x => new VoucherViewModels
            {
                Id = x.Id,
                MaVoucher = x.MaVoucher,
                LoaiGiamGia = x.LoaiGiamGia,
                loaiVoucher = x.loaiVoucher,
                GiaTriGiamGia = x.GiaTriGiamGia,
                DieuKienApDung = x.DieuKienApDung,
                GiaTriToiDa = x.GiaTriToiDa,
                SoLuong = x.SoLuong,
                NgayTao = x.NgayTao,
                ThoiGianBatDau = x.ThoiGianBatDau,
                ThoiGianKetThuc = x.ThoiGianKetThuc,
                TrangThai = x.TrangThai
            }).ToList();
        }

        public async Task<List<VoucherUserViewModel>> GetUsersForVoucher(Guid? voucherId = null)
        {
            var query = _context.Users
                .Where(u => u.TrangThai == true) // Chỉ lấy những khách hàng đang hoạt động
                .Join(_context.UserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole })
                .Join(_context.Roles,
                    ur => ur.userRole.RoleId,
                    role => role.Id,
                    (ur, role) => new { ur.user, role })
                .Where(x => x.role.Name.ToUpper() == "KHÁCH HÀNG"); // Chỉ lấy user có role là khách hàng

            // Nếu có voucherId, lọc theo khách hàng của voucher đó
            if (voucherId.HasValue)
            {
                var voucherUserIds = await _context.VoucherUser
                    .Where(vu => vu.VoucherId == voucherId.Value)
                    .Select(vu => vu.UserId)
                    .ToListAsync();

                query = query.Where(x =>
                voucherUserIds.Contains(x.user.Id) &&
                x.user.TrangThai == true); // ⚠️ Thêm điều kiện này lại
            }

            var users = await query
                .Select(x => new VoucherUserViewModel
                {
                    Id = x.user.Id,
                    UserName = x.user.UserName,
                    HoVaTen = x.user.HoVaTen,
                    Email = x.user.Email,
                    SoDienThoai = x.user.PhoneNumber,
                    TrangThai = x.user.TrangThai
                })
                .ToListAsync();

            return users;
        }

        public async Task<PagedResult<VoucherUserViewModel>> GetUsersForVoucherPaging(GetVoucherUserPagingRequest request)
        {
            var query = _context.Users
                .Where(u => u.TrangThai == true)
                .Join(_context.UserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole })
                .Join(_context.Roles,
                    ur => ur.userRole.RoleId,
                    role => role.Id,
                    (ur, role) => new { ur.user, role })
                .Where(x => x.role.Name.ToUpper() == "KHÁCH HÀNG" && x.user.TrangThai == true);

            // Lọc theo từ khóa nếu có
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.user.HoVaTen.Contains(request.Keyword) ||
                                       x.user.Email.Contains(request.Keyword) ||
                                       x.user.PhoneNumber.Contains(request.Keyword));
            }

            // Lấy danh sách userId đã thuộc về voucher (nếu có voucherId)
            List<Guid> existingUserIds = new List<Guid>();
            //if (request.VoucherId != null && request.VoucherId != Guid.Empty)
            //{
            //    existingUserIds = await _context.VoucherUser
            //        .Where(vu => vu.VoucherId == request.VoucherId)
            //        .Select(vu => vu.UserId)
            //        .ToListAsync();
            //}

            // Tính toán tổng số bản ghi
            int totalRow = await query.CountAsync();

            // Lấy dữ liệu theo trang
            var data = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new VoucherUserViewModel
                {
                    Id = x.user.Id,
                    UserName = x.user.UserName,
                    HoVaTen = x.user.HoVaTen,
                    Email = x.user.Email,
                    SoDienThoai = x.user.PhoneNumber,
                    TrangThai = x.user.TrangThai,
                    IsExistingUser = false // tạm, sẽ cập nhật bên dưới
                })
                .ToListAsync();

            // Cập nhật IsExistingUser cho từng user
            foreach (var user in data)
            {
                if (existingUserIds.Contains(user.Id))
                {
                    user.IsExistingUser = true;
                }
            }

            // Tạo đối tượng phân trang
            var pagedResult = new PagedResult<VoucherUserViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return pagedResult;
        }
    }
}