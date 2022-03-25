using System.ComponentModel.DataAnnotations.Schema;

namespace Ich.Saas.Core.Domain
{
    public partial class User
    {
        #region FullName

        private string fullName;

        [NotMapped]
        public string FullName
        {
            get { return fullName ??= FirstName + "" + LastName; }
            set => fullName = value;
        }

        #endregion
    }
}