using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SneakFit.Application.Catalog.HoaDon;
using SneakFit.Application.Catalog.HoaDonClient;
using SneakFit.Application.Payments;
using SneakFit.Data.Enums;
using SneakFit.ViewModels.Catalog.HoaDon;
using SneakFit.ViewModels.Catalog.HoaDonClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

namespace SneakFit.Application.Catalog.ThanhToan
{
    public class ThanhToanService : IThanhToanService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor; // Thêm để lấy IP thực tế
        private readonly IHoaDonService _hoaDonService;
        private readonly IHoaDonClientService _hoaDonClientService;

        public ThanhToanService(
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor,
            IHoaDonService hoaDonService,
            IHoaDonClientService hoaDonClientService)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _hoaDonService = hoaDonService;
            _hoaDonClientService = hoaDonClientService;
        }
        public async Task<string> CreateMomoPaymentUrl(MomoPaymentRequest request)
        {
            var endpoint = _config["Momo:Endpoint"];
            var partnerCode = _config["Momo:PartnerCode"];
            var accessKey = _config["Momo:AccessKey"];
            var secretKey = _config["Momo:SecretKey"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(partnerCode) ||
                string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
                throw new InvalidOperationException("Thiếu cấu hình Momo.");

            var orderId = request.OrderId;
            var orderInfo = request.OrderInfo;
            var amount = request.Amount.ToString("0");
            var returnUrl = request.ReturnUrl;
            var notifyUrl = request.NotifyUrl;
            var requestId = Guid.NewGuid().ToString();

            // Chuỗi raw data để ký
            var rawHash = $"accessKey={accessKey}&amount={amount}&extraData=&ipnUrl={notifyUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType=captureWallet";

            // Tạo signature
            string signature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
                signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            var payload = new
            {
                partnerCode,
                accessKey,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl = returnUrl,
                ipnUrl = notifyUrl,
                extraData = "",
                requestType = "captureWallet",
                signature
            };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            if (doc.RootElement.TryGetProperty("payUrl", out var payUrlElement))
            {
                return payUrlElement.GetString();
            }
            throw new Exception("Không lấy được payUrl từ Momo. Response: " + responseString);
        }
        public async Task<string> CreateVnPayPaymentUrl(VNPayPaymentRequest request)
        {
            // 1. Cập nhật hóa đơn sang trạng thái chờ thanh toán và trạng thái thanh toán = 1 (Chưa thanh toán)
            if (Guid.TryParse(request.OrderId, out var hoaDonId))
            {
                var hoaDon = await _hoaDonService.GetById(hoaDonId);
                if (hoaDon != null)
                {

                    hoaDon.TrangThai = TrangThaiHoaDon.ChoXacNhan; // Chờ xác nhận (string)
                    hoaDon.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan; // Chưa thanh toán (string)
                    hoaDon.PhuongThucThanhToan = PhuongThucThanhToan.VnPay; // Chuyển khoản (string)
                    hoaDon.NgayThanhToan = null;
                    // Cập nhật các trường khác nếu cần
                    await _hoaDonService.Update(new SuaHoaDon
                    {
                        Id = hoaDon.Id,
                        TongTien = request.Amount,
                        TrangThai = hoaDon.TrangThai,
                        DiaChi = hoaDon.DiaChi,
                        SoDienThoai = hoaDon.SoDienThoai,
                        Email = hoaDon.Email,
                        HoTen = hoaDon.HoTen,
                        UserId = hoaDon.UserId,
                        GiaoHang = hoaDon.GiaoHang,
                        GhiChu = hoaDon.GhiChu,
                        PhuongThucThanhToan = hoaDon.PhuongThucThanhToan,
                        LoaiHoaDon = hoaDon.LoaiHoaDon,
                        NgayThanhToan = hoaDon.NgayThanhToan,
                        MaHoaDon = hoaDon.MaHoaDon,
                        PhiVanChuyen = hoaDon.PhiVanChuyen,
                        TrangThaiThanhToan = hoaDon.TrangThaiThanhToan,
                        VoucherId = hoaDon.VoucherId,
                        TienKhachDua = hoaDon.TienKhachDua
                    });
                }
            }
            // 2. Tạo link VNPay như cũ
            var vnp_TmnCode = _config["VNPay:TmnCode"];
            var vnp_HashSecret = _config["VNPay:HashSecret"];
            var vnp_Url = _config["VNPay:BaseUrl"];

            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret) || string.IsNullOrEmpty(vnp_Url))
                throw new InvalidOperationException("Thiếu cấu hình VNPay.");

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)(request.Amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_httpContextAccessor.HttpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", request.OrderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", request.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", request.OrderId);

            var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        public async Task<bool> XuLyVnPayCallbackAsync(Dictionary<string, string> vnp_Params)
        {
            // 2. Lấy mã đơn hàng
            var orderId = vnp_Params.ContainsKey("vnp_TxnRef") ? vnp_Params["vnp_TxnRef"] : null;
            if (string.IsNullOrEmpty(orderId))
                return false;

            // 3. Lấy hóa đơn từ DB
            var hoaDon = await _hoaDonService.GetById(Guid.Parse(orderId));
            if (hoaDon == null)
                return false;

            // 4. Kiểm tra trạng thái giao dịch thành công
            var responseCode = vnp_Params.ContainsKey("vnp_ResponseCode") ? vnp_Params["vnp_ResponseCode"] : null;
            if (responseCode == "00")
            {
                if (hoaDon.GiaoHang == true)
                {
                    hoaDon.TrangThai = TrangThaiHoaDon.DaXacNhan; // Đã thanh toán                   
                }
                else
                {
                    hoaDon.TrangThai = TrangThaiHoaDon.ThanhCong; // Đã thanh toán    
                }
                hoaDon.GhiChu = vnp_Params.ContainsKey("vnp_TransactionNo") ? vnp_Params["vnp_TransactionNo"] : null;
                hoaDon.NgayThanhToan = DateTime.Now;
                hoaDon.TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan; // Đã thanh toán
                var suaHoaDon = new SuaHoaDon
                {
                    Id = hoaDon.Id,
                    TongTien = hoaDon.TongTien - hoaDon.PhiVanChuyen,
                    TrangThai = hoaDon.TrangThai,
                    DiaChi = hoaDon.DiaChi,
                    SoDienThoai = hoaDon.SoDienThoai,
                    Email = hoaDon.Email,
                    HoTen = hoaDon.HoTen,
                    UserId = hoaDon.UserId,
                    GiaoHang = hoaDon.GiaoHang,
                    GhiChu = hoaDon.GhiChu,
                    PhuongThucThanhToan = hoaDon.PhuongThucThanhToan,
                    LoaiHoaDon = hoaDon.LoaiHoaDon,
                    NgayThanhToan = hoaDon.NgayThanhToan,
                    MaHoaDon = hoaDon.MaHoaDon,
                    PhiVanChuyen = hoaDon.PhiVanChuyen,
                    TrangThaiThanhToan = hoaDon.TrangThaiThanhToan,
                    VoucherId = hoaDon.VoucherId,
                    TienKhachDua = hoaDon.TienKhachDua
                };
                await _hoaDonService.Update(suaHoaDon);
                return true;
            }
            else
            {
                // Có thể cập nhật trạng thái thất bại nếu muốn
                // hoaDon.TrangThaiThanhToan = 3;
                // await _hoaDonService.UpdateAsync(hoaDon);
                return false;
            }
        }
        // Tạo link thanh toán VNPay cho HoaDonClient
        public async Task<string> CreateVnPayPaymentUrlClient(VNPayPaymentRequest request)
        {
            // 1. Cập nhật hóa đơn sang trạng thái chờ thanh toán và trạng thái thanh toán = 1 (Chưa thanh toán)
            if (Guid.TryParse(request.OrderId, out var hoaDonId))
            {
                // Lấy HoaDonClient thay vì HoaDon
                var hoaDonClientService = _hoaDonClientService;
                var hoaDon = await hoaDonClientService.GetById(hoaDonId);
                if (hoaDon != null)
                {
                    hoaDon.TrangThai = TrangThaiHoaDon.ChoXacNhan;
                    hoaDon.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;
                    hoaDon.PhuongThucThanhToan = PhuongThucThanhToan.VnPay;
                    hoaDon.NgayThanhToan = null;
                    await hoaDonClientService.Update(new SuaHoaDonClient
                    {
                        Id = hoaDon.Id,
                        TongTien = request.Amount,
                        TrangThai = hoaDon.TrangThai,
                        DiaChi = hoaDon.DiaChi,
                        SoDienThoai = hoaDon.SoDienThoai,
                        Email = hoaDon.Email,
                        HoTen = hoaDon.HoTen,
                        UserId = hoaDon.UserId,
                        GhiChu = null,
                        PhuongThucThanhToan = hoaDon.PhuongThucThanhToan,
                        LoaiHoaDon = hoaDon.LoaiHoaDon,
                        NgayThanhToan = hoaDon.NgayThanhToan,
                        MaHoaDon = hoaDon.MaHoaDon,
                        PhiVanChuyen = hoaDon.PhiVanChuyen,
                        DonViVanChuyen = hoaDon.DonViVanChuyen,
                        TrangThaiThanhToan = hoaDon.TrangThaiThanhToan,
                        VoucherId = hoaDon.VoucherId
                    });
                }
            }
            // 2. Tạo link VNPay như cũ
            var vnp_TmnCode = _config["VNPay:TmnCode"];
            var vnp_HashSecret = _config["VNPay:HashSecret"];
            var vnp_Url = _config["VNPay:BaseUrl"];

            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret) || string.IsNullOrEmpty(vnp_Url))
                throw new InvalidOperationException("Thiếu cấu hình VNPay.");

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)(request.Amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(_httpContextAccessor.HttpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", request.OrderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", request.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", request.OrderId);

            var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        // Xử lý callback VNPay cho HoaDonClient
        public async Task<bool> XuLyVnPayCallBackClientAsync(Dictionary<string, string> vnp_Params)
        {
            var orderId = vnp_Params.ContainsKey("vnp_TxnRef") ? vnp_Params["vnp_TxnRef"] : null;
            if (string.IsNullOrEmpty(orderId))
                return false;

            var hoaDonClientService = _hoaDonClientService;
            var hoaDon = await hoaDonClientService.GetById(Guid.Parse(orderId));
            if (hoaDon == null)
                return false;

            var responseCode = vnp_Params.ContainsKey("vnp_ResponseCode") ? vnp_Params["vnp_ResponseCode"] : null;
            if (responseCode == "00")
            {
                hoaDon.TrangThai = TrangThaiHoaDon.ChoVanChuyen; // Đổi từ ThanhCong sang ChoVanChuyen
                hoaDon.GhiChu = vnp_Params.ContainsKey("vnp_TransactionNo") ? vnp_Params["vnp_TransactionNo"] : null;
                hoaDon.NgayThanhToan = DateTime.Now;
                hoaDon.TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan;
                var suaHoaDon = new SuaHoaDonClient
                {
                    Id = hoaDon.Id,
                    TongTien = hoaDon.TongTien,
                    TrangThai = hoaDon.TrangThai,
                    DiaChi = hoaDon.DiaChi,
                    SoDienThoai = hoaDon.SoDienThoai,
                    Email = hoaDon.Email,
                    HoTen = hoaDon.HoTen,
                    UserId = hoaDon.UserId,
                    GhiChu = hoaDon.GhiChu,
                    PhuongThucThanhToan = hoaDon.PhuongThucThanhToan,
                    LoaiHoaDon = hoaDon.LoaiHoaDon,
                    NgayThanhToan = hoaDon.NgayThanhToan,
                    MaHoaDon = hoaDon.MaHoaDon,
                    PhiVanChuyen = hoaDon.PhiVanChuyen,
                    DonViVanChuyen = hoaDon.DonViVanChuyen,
                    TrangThaiThanhToan = hoaDon.TrangThaiThanhToan,
                    VoucherId = hoaDon.VoucherId
                };
                await hoaDonClientService.Update(suaHoaDon);
                return true;
            }
            else
            {
                // Có thể cập nhật trạng thái thất bại nếu muốn
                return false;
            }
        }
        public async Task<bool> XuLyMomoCallBackClientAsync(Dictionary<string, string> momoParams)
        {
            // Lấy orderId từ momoParams (tùy thuộc vào key MoMo trả về, ví dụ: orderId)
            var orderId = momoParams.ContainsKey("orderId") ? momoParams["orderId"] : null;
            if (string.IsNullOrEmpty(orderId))
                return false;
            var hoaDon = await _hoaDonClientService.GetById(Guid.Parse(orderId));
            if (hoaDon == null)
                return false;
            // Kiểm tra trạng thái giao dịch thành công (MoMo trả về resultCode == 0 là thành công)
            var resultCode = momoParams.ContainsKey("resultCode") ? momoParams["resultCode"] : null;
            if (resultCode == "0")
            {
                hoaDon.TrangThai = TrangThaiHoaDon.ChoVanChuyen;
                hoaDon.NgayThanhToan = DateTime.Now;
                hoaDon.TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan;
                hoaDon.GhiChu = momoParams.ContainsKey("transId") ? momoParams["transId"] : null;
                var suaHoaDon = new SuaHoaDonClient
                {
                    Id = hoaDon.Id,
                    TongTien = hoaDon.TongTien,
                    TrangThai = hoaDon.TrangThai,
                    DiaChi = hoaDon.DiaChi,
                    SoDienThoai = hoaDon.SoDienThoai,
                    Email = hoaDon.Email,
                    HoTen = hoaDon.HoTen,
                    UserId = hoaDon.UserId,
                    GhiChu = hoaDon.GhiChu,
                    PhuongThucThanhToan = hoaDon.PhuongThucThanhToan,
                    LoaiHoaDon = hoaDon.LoaiHoaDon,
                    NgayThanhToan = hoaDon.NgayThanhToan,
                    MaHoaDon = hoaDon.MaHoaDon,
                    PhiVanChuyen = hoaDon.PhiVanChuyen,
                    DonViVanChuyen = hoaDon.DonViVanChuyen,
                    TrangThaiThanhToan = hoaDon.TrangThaiThanhToan,
                    VoucherId = hoaDon.VoucherId
                };
                await _hoaDonClientService.Update(suaHoaDon);
                return true;
            }
            else
            {
                // Có thể cập nhật trạng thái thất bại nếu muốn
                return false;
            }
        }
    }
}