using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Ich.Saas.Core.Code.Extensions
{
    public static class HttpExtensions
    {
        public static string Referer(this IHttpContextAccessor httpContextAccessor)
        {
            var referer = httpContextAccessor.HttpContext.Request?.Headers[HeaderNames.Referer];
            if (string.IsNullOrEmpty(referer)) return "";

            var uri = new Uri(referer);
            return uri.GetComponents(UriComponents.PathAndQuery, UriFormat.UriEscaped);
        }

        public static string PostedReferer(this IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext.Request.Method == "POST" && 
                httpContextAccessor.HttpContext.Request.Form.ContainsKey("Referer"))
                return httpContextAccessor.HttpContext.Request.Form["Referer"].ToString();

            return null;
        }
    }
}