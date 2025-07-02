namespace Alcohol.DTOs.Auth
{
    public class OAuthLoginDto
    {
        public string Provider { get; set; } = string.Empty; // "Google" or "Facebook"
        public string Token { get; set; } = string.Empty; // ID token for Google, Access token for Facebook
    }
} 