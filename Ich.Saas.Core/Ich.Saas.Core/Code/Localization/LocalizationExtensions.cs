using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Ich.Saas.Core.Code.Localization
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddStringLocalization(this IServiceCollection services)
        {
            services.AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>();
            services.AddSingleton<IStringLocalizer, StringLocalizer>();

            return services;
        }
    }
}