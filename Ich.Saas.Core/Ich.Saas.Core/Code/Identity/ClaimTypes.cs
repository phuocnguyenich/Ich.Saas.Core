namespace Ich.Saas.Core.Code.Identity
{
    // Custom claim types

    public static class ClaimTypes
    {
        // Tenant claim types

        public const string TenantId = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/tenantid";
        public const string TenantName = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/tenantname";
        public const string Color = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/color";
        public const string CurrencySymbol = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/currencysymbol";

        // User claim types

        public const string UserId = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/userid";
        public const string FirstName = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/firstname";
        public const string LastName = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/lastname";
        public const string Email = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/email";

        // Culture claim types

        public const string LocaleName = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/localename";
        public const string TimeZoneName = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/timezonename";

        public const string CurrencyId = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/currencyid";
        public const string TimeZoneId = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/timezoneid";
        public const string LocaleId = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/localeid";
        public const string LanguageId = "https://schemas.prouniversity.org/ws/2009/09/identity/claims/languageid";
    }
}
