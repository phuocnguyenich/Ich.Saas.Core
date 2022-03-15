using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Code.Database
{
    interface IAuditable
    {
        DateTime CreatedOn { get; set; }
        int? CreatedBy { get; set; }
        DateTime ChangedOn { get; set; }
        int? ChangedBy { get; set; }
    }
}
