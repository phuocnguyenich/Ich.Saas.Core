using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Code.Infrastructure
{
    public static class ServiceLocator
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Register(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static T Resolve<T>()
        {
            return (T)_httpContextAccessor.HttpContext?.RequestServices.GetService(typeof(T));
        }
    }
}
