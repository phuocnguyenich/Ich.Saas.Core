namespace Ich.Saas.Core.Areas.Auth
{
    public class _User
    {
        public string Tenant { get; set; }
        public int? UserId { get; set; }
        public string User { get; set; }
        public string Role { get; set; }
        public string Color { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public string Language { get; set; }
        public string Locale { get; set; }
        public string TimeZone { get; set; }
        public string Currency { get; set; }
    }
}