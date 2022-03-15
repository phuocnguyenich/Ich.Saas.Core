using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Code.Identity
{
    #region Interface

    public interface ICurrentUser
    {
        int? Id { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }

        bool IsAuthenticated { get; }
        bool IsHost { get; }
        bool IsAdmin { get; }
        bool IsUser { get; }

        int LanguageId { get; }
        CultureInfo CultureInfo { get; }
        TimeZoneInfo TimeZoneInfo { get; }
    }

    #endregion
    public class CurrentUser
    {
    }
}
