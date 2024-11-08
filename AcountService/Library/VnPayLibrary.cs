using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BanVatLieuXayDung.Library
{
    public class VnPayLibrary
    {
        private readonly Dictionary<string, string> _requestData = new Dictionary<string, string>();

        // Thêm dữ liệu vào request
        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && value != null)
            {
                _requestData[key] = value;
            }
        }

        // Tạo URL yêu cầu VNPay với dữ liệu đã thêm
        public string CreateRequestUrl(string vnpUrl, string vnpHashSecret)
        {
            var query = new StringBuilder();
            foreach (var pair in _requestData)
            {
                query.Append(pair.Key + "=" + HttpUtility.UrlEncode(pair.Value) + "&");
            }

            query.Append("vnp_SecureHash=" + GetSecureHash(query.ToString(), vnpHashSecret));

            return vnpUrl + "?" + query.ToString();
        }

        // Tạo mã bảo mật cho yêu cầu
        private string GetSecureHash(string data, string secretKey)
        {
            using (var md5 = MD5.Create())
            {
                var keyBytes = Encoding.UTF8.GetBytes(secretKey);
                var dataBytes = Encoding.UTF8.GetBytes(data);
                var combined = new byte[keyBytes.Length + dataBytes.Length];
                Buffer.BlockCopy(keyBytes, 0, combined, 0, keyBytes.Length);
                Buffer.BlockCopy(dataBytes, 0, combined, keyBytes.Length, dataBytes.Length);
                var hashBytes = md5.ComputeHash(combined);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}

