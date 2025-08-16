using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Alcohol.Helpers;

public static class VnPayHelper
{
    public static SortedDictionary<string, string> ExtractVnpParams(IQueryCollection query)
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

    public static string BuildHashData(SortedDictionary<string, string> parameters)
    {
        // Exclude hash fields
        var filtered = parameters
            .Where(kv => kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
            .ToList();

        var sb = new StringBuilder();
        for (int i = 0; i < filtered.Count; i++)
        {
            var kv = filtered[i];
            if (kv.Value is null) continue;
            sb.Append(kv.Key);
            sb.Append('=');
            // IMPORTANT: use URI percent-encoding (not '+') for signature building
            sb.Append(Uri.EscapeDataString(kv.Value));
            if (i < filtered.Count - 1)
            {
                sb.Append('&');
            }
        }
        return sb.ToString();
    }

    public static string ComputeHmacSha512(string secretKey, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA512(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static string BuildQueryString(IDictionary<string, string> parameters, bool includeHashType = true)
    {
        var parts = new List<string>();
        foreach (var kv in parameters)
        {
            if (kv.Key == "vnp_SecureHashType" && !includeHashType) continue;
            var encoded = Uri.EscapeDataString(kv.Value ?? string.Empty);
            parts.Add($"{kv.Key}={encoded}");
        }
        return string.Join("&", parts);
    }
}


