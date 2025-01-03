using System.Security.Cryptography;
using System.Text;

public class VnPayLibrary
{
    private SortedList<string, string> _requestData = new SortedList<string, string>();

    public void AddRequestData(string key, string value)
    {
        if (!_requestData.ContainsKey(key))
        {
            _requestData.Add(key, value);
        }
    }

    public string CreateRequestUrl(string baseUrl, string hashSecret)
    {
        StringBuilder data = new StringBuilder();
        foreach (KeyValuePair<string, string> kvp in _requestData)
        {
            if (data.Length > 0)
            {
                data.Append("&");
            }
            data.AppendFormat("{0}={1}", kvp.Key, kvp.Value);
        }

        string queryString = data.ToString();
        string vnpSecureHash = HmacSHA512(hashSecret, queryString);
        string paymentUrl = $"{baseUrl}?{queryString}&vnp_SecureHash={vnpSecureHash}";

        return paymentUrl;
    }

    private string HmacSHA512(string key, string inputData)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
        }
    }
}
