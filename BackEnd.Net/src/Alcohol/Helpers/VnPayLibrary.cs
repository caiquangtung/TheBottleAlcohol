using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace Alcohol.Helpers;

/// <summary>
/// Canonical helper for building VNPAY requests and verifying signatures following the official docs.
/// </summary>
public sealed class VnPayLibrary
{
    private readonly SortedDictionary<string, string> _requestData = new(StringComparer.Ordinal);

    public void AddRequestData(string key, string value)
    {
        if (string.IsNullOrEmpty(key) || value == null) return;
        if (key.StartsWith("vnp_", StringComparison.Ordinal))
        {
            _requestData[key] = value;
        }
    }

    public SortedDictionary<string, string> GetRequestDataForDebug()
    {
        return new SortedDictionary<string, string>(_requestData, StringComparer.Ordinal);
    }

    /// <summary>
    /// Build signed payment URL. HMAC is computed over key=value pairs WITH URL-encoding values to match VNPAY calculation.
    /// </summary>
    public string CreateRequestUrl(string baseUrl, string secretKey)
    {
        // Filter out vnp_SecureHash and vnp_SecureHashType before computing hash
        var filteredData = new SortedDictionary<string, string>(_requestData
            .Where(kv => kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
            .ToDictionary(k => k.Key, v => v.Value), StringComparer.Ordinal);
        
        // Build encoded data for both signature calculation AND URL
        var encodedData = BuildEncodedData(filteredData);
        var secureHash = ComputeHmacSha512(secretKey, encodedData);

        var url = new StringBuilder();
        url.Append(baseUrl);
        url.Append("?");
        url.Append(encodedData);
        url.Append("&vnp_SecureHash=");
        url.Append(secureHash);
        
        // Store debug info for logging
        LastRawData = encodedData;
        LastSecureHash = secureHash;
        
        return url.ToString();
    }
    
    public string LastRawData { get; private set; } = string.Empty;
    public string LastSecureHash { get; private set; } = string.Empty;

    public static SortedDictionary<string, string> ExtractResponseData(IQueryCollection query)
    {
        var dict = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var kv in query)
        {
            if (kv.Key.StartsWith("vnp_", StringComparison.Ordinal))
            {
                dict[kv.Key] = kv.Value.ToString();
            }
        }
        return dict;
    }

    public static bool ValidateSignature(SortedDictionary<string, string> responseData, string secretKey, out string rawData, out string expectedHash, out string receivedHash)
    {
        responseData.TryGetValue("vnp_SecureHash", out receivedHash);
        // Remove hash fields before building raw data
        var filtered = new SortedDictionary<string, string>(responseData
            .Where(kv => kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
            .ToDictionary(k => k.Key, v => v.Value), StringComparer.Ordinal);

        // For response validation, VNPAY calculates hash on ENCODED values (like when sending the request)
        // But the response comes back decoded by browser, so we need to encode again for validation
        rawData = BuildEncodedData(filtered);
        expectedHash = ComputeHmacSha512(secretKey, rawData);
        return string.Equals(expectedHash, receivedHash, StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildRawData(SortedDictionary<string, string> parameters)
    {
        var sb = new StringBuilder();
        int i = 0;
        foreach (var kv in parameters)
        {
            if (kv.Value == null) continue;
            sb.Append(kv.Key);
            sb.Append('=');
            sb.Append(kv.Value); // raw value per VNPAY doc
            if (i < parameters.Count - 1) sb.Append('&');
            i++;
        }
        return sb.ToString();
    }

    /// <summary>
    /// Build encoded data string for signature calculation - VNPAY calculates signature on encoded values
    /// </summary>
    private static string BuildEncodedData(SortedDictionary<string, string> parameters)
    {
        var sb = new StringBuilder();
        int i = 0;
        foreach (var kv in parameters)
        {
            if (kv.Value == null) continue;
            sb.Append(kv.Key);
            sb.Append('=');
            sb.Append(UrlEncodeUpperCase(kv.Value)); // encoded value for signature
            if (i < parameters.Count - 1) sb.Append('&');
            i++;
        }
        return sb.ToString();
    }

    private static string ComputeHmacSha512(string secretKey, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA512(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// URL encode với uppercase hex theo chuẩn RFC3986 để match với VNPAY
    /// </summary>
    private static string UrlEncodeUpperCase(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        
        // Sử dụng HttpUtility.UrlEncode rồi chuyển hex thành uppercase
        var encoded = HttpUtility.UrlEncode(value);
        if (encoded == null) return string.Empty;
        
        // Chuyển tất cả %xx thành %XX (uppercase)
        var result = new StringBuilder();
        for (int i = 0; i < encoded.Length; i++)
        {
            if (encoded[i] == '%' && i + 2 < encoded.Length)
            {
                result.Append('%');
                result.Append(char.ToUpperInvariant(encoded[i + 1]));
                result.Append(char.ToUpperInvariant(encoded[i + 2]));
                i += 2;
            }
            else
            {
                result.Append(encoded[i]);
            }
        }
        return result.ToString();
    }
}


