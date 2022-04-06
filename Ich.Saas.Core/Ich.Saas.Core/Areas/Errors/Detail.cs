using Ich.Saas.Core.Domain;

namespace Ich.Saas.Core.Areas.Errors
{
    public class Detail 
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public string CreatedDate { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string IpAddress { get; set; }
        public string Url { get; set; }
        public string HttpReferer { get; set; }
        public string UserAgent { get; set; }
        public string ServerName { get; set; }
    }
}