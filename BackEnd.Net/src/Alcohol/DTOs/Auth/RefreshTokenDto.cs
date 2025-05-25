using System;

namespace Alcohol.DTOs.Auth
{
    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string DeviceInfo { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }

    public class RefreshTokenResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string DeviceInfo { get; set; }
    }
} 