using Ich.Saas.Core.Code.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Domain
{
    public partial class Translate : IAuditable
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int? ChangedBy { get; set; }

        public virtual Language Language { get; set; }
    }
}
