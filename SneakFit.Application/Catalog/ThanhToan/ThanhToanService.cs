using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
        public ThanhToanService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public string CreateVNPayPaymentUrl(VNPayPaymentRequest request)
        {
            // Lấy cấu hình từ appsettings.json
            var vnp_Url = _config["VNPay:BaseUrl"];
            var vnp_TmnCode = _config["VNPay:TmnCode"];
            var vnp_HashSecret = _config["VNPay:HashSecret"];

            if (string.IsNullOrEmpty(vnp_Url) || string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
                throw new InvalidOperationException("Thiếu cấu hình VNPay.");

            // Kiểm tra dữ liệu đầu vào
            if (request.Amount <= 0)
                throw new ArgumentException("Số tiền phải lớn hơn 0.");
            if (string.IsNullOrEmpty(request.OrderId))
                throw new ArgumentException("OrderId không được để trống.");
            if (string.IsNullOrEmpty(request.ReturnUrl))
                throw new ArgumentException("ReturnUrl không được để trống.");

            // Lấy IP thực tế của client
            var ipAddr = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            // Tạo danh sách tham số, không mã hóa URL trước
            var vnp_Params = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", ((int)(request.Amount * 100)).ToString() },
                { "vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") }, // Sử dụng GMT+7
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", ipAddr },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", request.OrderDescription }, // Không mã hóa trước
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", request.ReturnUrl }, // Không mã hóa trước
                { "vnp_TxnRef", request.OrderId },
                { "vnp_ExpireDate", DateTime.UtcNow.AddHours(7).AddMinutes(15).ToString("yyyyMMddHHmmss") }
            };

            // Tạo chuỗi ký
            var signData = string.Join("&", vnp_Params.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));

            // Tạo chữ ký HMAC-SHA512
            string vnp_SecureHash;
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(vnp_HashSecret)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signData));
                vnp_SecureHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            // Thêm chữ ký vào tham số
            vnp_Params.Add("vnp_SecureHash", vnp_SecureHash);

            // Tạo query string cho URL
            var queryString = string.Join("&", vnp_Params.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
            var paymentUrl = $"{vnp_Url}?{queryString}";

            return paymentUrl;
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
    }
}
