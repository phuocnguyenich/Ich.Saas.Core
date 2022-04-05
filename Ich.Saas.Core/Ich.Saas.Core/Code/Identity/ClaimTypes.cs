namespace Ich.Saas.Core.Code.Identity
{
    // Custom claim types

    public static class ClaimTypes
    {
        // Tenant claim types

        public const string TenantId = "tenantid";
        public const string TenantName = "tenantname";
        public const string Color = "color";
        public const string CurrencySymbol = "currencysymbol";

        // User claim types

        public const string UserId = "userid";
        public const string FirstName = "firstname";
        public const string LastName = "lastname";
        public const string Email = "email";

        // Culture claim types

        public const string LocaleName = "localename";
        public const string TimeZoneName = "timezonename";

        public const string CurrencyId = "currencyid";
        public const string TimeZoneId = "timezoneid";
        public const string LocaleId = "localeid";
        public const string LanguageId = "languageid";
    }
}
