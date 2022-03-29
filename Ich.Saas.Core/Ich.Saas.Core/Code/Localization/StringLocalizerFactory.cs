using System;
using Microsoft.Extensions.Localization;

namespace Ich.Saas.Core.Code.Localization
{
    public class StringLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(string baseName, string location)
        {
            return new StringLocalizer();
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new StringLocalizer();
        }
    }
}