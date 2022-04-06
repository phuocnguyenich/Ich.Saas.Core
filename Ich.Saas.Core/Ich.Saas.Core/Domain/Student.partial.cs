using System.ComponentModel.DataAnnotations.Schema;

namespace Ich.Saas.Core.Domain
{
    public partial class Student
    {
        private string fullName;
        [NotMapped]
        public string FullName
        {
            get
            {
                if (fullName == null)
                    fullName = FirstName + " " + LastName;
                
                return fullName;
            }
            set
            {
                fullName = value;
            }
        }
    }
}