using System;
using Ich.Saas.Core.Code.Identity;
using Ich.Saas.Core.Code.Infrastructure;
using Ich.Saas.Core.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ich.Saas.Core.Code.Logging
{
    public class DbLogger<TLog> : ILogger where TLog : Error, new()
    {
        #region Dependency Injection

        private readonly string _categoryName;
        private readonly Func<string, LogLevel, bool> _filter;

        public DbLogger(string categoryName, Func<string, LogLevel, bool> filter)
        {
            _categoryName = categoryName;
            _filter = filter;
        }

        #endregion

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _filter == null || _filter(_categoryName, logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            try
            {
                var httpContextAccessor = ServiceLocator.Resolve<IHttpContextAccessor>();
                var httpContext = httpContextAccessor?.HttpContext;
                if (httpContext == null) return;  // Nothing to log

                var error = new Error();

                error.Message = exception?.Message ?? "No message";
                error.Exception = exception?.ToString();
                error.UserAgent = httpContext.Request.Headers["User-Agent"];
                error.IpAddress = httpContext.Connection?.LocalIpAddress?.ToString();
                error.Url = httpContext.Request.Path;
                error.HttpReferer = httpContext.Request.Headers["Referer"];

                error.CreatedOn = error.ChangedOn = error.CreatedDate = DateTime.Now;
                try
                {
                    var userId = ((httpContext.RequestServices.GetService(typeof(ICurrentUser)) as CurrentUser)!).Id;
                    if (userId != 0)
                        error.UserId = error.CreatedBy = error.ChangedBy = userId;
                }
                catch
                {
                    // ignored
                }

                // Extract connectionstring from potentially unstable dbcontext.
                var unstable = ServiceLocator.Resolve<SaaSContext>();
                var connectionString = unstable.Database.GetDbConnection().ConnectionString;

                // Use brand new context
                using (var db = new ErrorContext(connectionString))
                {
                    db.Error.Add(error);
                    db.SaveChanges();
                }
            }
            catch
            {
                /* possibly notify by email about logging error */
            }
        }

        private class ErrorContext : SaaSContext
        {
            private readonly string _connectionString;

            public ErrorContext(string connectionString)
            {
                _connectionString = connectionString;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}