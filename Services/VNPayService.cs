using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using QuanLyNhaHangAPI.Models;

namespace QuanLyNhaHangAPI.Services;

public class VNPayService
{
    private readonly IConfiguration _config;

    public VNPayService(IConfiguration config)
    {
        _config = config;
    }

    public string CreatePaymentUrl(OrderRequestDTO order, string orderId, string clientIp)
    {
        var vnp = new VNPayLibrary();

        vnp.AddRequestData("vnp_Version", "2.1.0");
        vnp.AddRequestData("vnp_Command", "pay");
        vnp.AddRequestData("vnp_TmnCode", _config["VNPay:TmnCode"]!);
        vnp.AddRequestData("vnp_Amount", ((long)(order.ChiTietDonHang.Sum(x => x.SoLuong * x.GiaLucDat) * 100)).ToString());
        vnp.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        vnp.AddRequestData("vnp_CurrCode", "VND");
        vnp.AddRequestData("vnp_IpAddr", clientIp);
        vnp.AddRequestData("vnp_Locale", "vn");

        // MẸO: Đổi khoảng trắng thành gạch dưới để an toàn tuyệt đối với mọi bộ mã hóa
        vnp.AddRequestData("vnp_OrderInfo", "Thanh_toan_don_hang_" + orderId);

        vnp.AddRequestData("vnp_OrderType", "food");
        vnp.AddRequestData("vnp_ReturnUrl", _config["VNPay:ReturnUrl"]!);
        vnp.AddRequestData("vnp_TxnRef", orderId);

        return vnp.CreateRequestUrl(_config["VNPay:Url"]!, _config["VNPay:HashSecret"]!);
    }

    public bool ValidateSignature(IQueryCollection query)
    {
        return new VNPayLibrary().ValidateSignature(query, _config["VNPay:HashSecret"]!);
    }
}

public class VNPayLibrary
{
    private SortedList<string, string> _requestData = new(new VnPayCompare());

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            _requestData[key] = value;
    }

    public string CreateRequestUrl(string baseUrl, string hashSecret)
    {
        var data = new StringBuilder();
        foreach (var kv in _requestData)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                // ĐỔI THÀNH: WebUtility.UrlEncode
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }
        }

        string queryString = data.ToString().TrimEnd('&');
        string secureHash = HmacSHA512(hashSecret, queryString);

        return baseUrl + "?" + queryString + "&vnp_SecureHash=" + secureHash;
    }

    public bool ValidateSignature(IQueryCollection query, string hashSecret)
    {
        var vnpSecureHash = query["vnp_SecureHash"].ToString();
        var data = new SortedList<string, string>(new VnPayCompare());

        foreach (var kv in query)
        {
            if (!string.IsNullOrEmpty(kv.Value.ToString()) && kv.Key.StartsWith("vnp_") && kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
            {
                data[kv.Key] = kv.Value.ToString()!;
            }
        }

        var sb = new StringBuilder();
        foreach (var kv in data)
        {
            // ĐỔI THÀNH: WebUtility.UrlEncode
            sb.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
        }

        string signData = sb.ToString().TrimEnd('&');
        string myChecksum = HmacSHA512(hashSecret, signData);

        return myChecksum.Equals(vnpSecureHash, StringComparison.InvariantCultureIgnoreCase);
    }

    private string HmacSHA512(string key, string inputData)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    private class VnPayCompare : IComparer<string>
    {
        public int Compare(string? x, string? y) => string.CompareOrdinal(x, y);
    }
}