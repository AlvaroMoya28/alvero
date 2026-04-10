namespace EventosBackend.Models.Configuration
{
    public class JwtSettings
    {
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string SecretKey { get; set; }
        public int ExpiryInMinutes { get; set; }
    }
}