using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BanVatLieuXayDung.Library
{
    public class VnPayLibrary
    {
        private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());

        // Add key-value data to request
        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            // Tạo query string từ request data
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            // Xóa dấu "&" thừa ở cuối query string
            string queryString = data.ToString();
            if (queryString.Length > 0)
            {
                queryString = queryString.Remove(queryString.Length - 1, 1);
            }

            // Tính toán chữ ký HMAC SHA-512
            string vnp_SecureHash = HmacSHA512(vnp_HashSecret, queryString);

            // Kiểm tra nếu baseUrl đã có dấu "?" để tránh thêm dấu "?" thừa
            if (baseUrl.Contains("?"))
            {
                baseUrl += "&" + queryString + "&vnp_SecureHash=" + vnp_SecureHash;
            }
            else
            {
                baseUrl += "?" + queryString + "&vnp_SecureHash=" + vnp_SecureHash;
            }

            return baseUrl;
        }
        // HMACSHA512 hash calculation
        public static string HmacSHA512(string secretKey, string rawData)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey); // Secret key
            byte[] messageBytes = Encoding.UTF8.GetBytes(rawData); // Data to sign

            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(messageBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // Chuyển byte sang chuỗi hex
                }
                return sb.ToString();
            }
        }

        // Custom comparator for sorting keys
        public class VnPayCompare : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                var vnpCompare = CompareInfo.GetCompareInfo("en-US");
                return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
            }
        }
    }
}
