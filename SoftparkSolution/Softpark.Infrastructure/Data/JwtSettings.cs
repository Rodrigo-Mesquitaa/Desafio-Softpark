namespace Softpark.Infrastructure.Data
{
    public sealed class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpirationHours { get; set; }
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
