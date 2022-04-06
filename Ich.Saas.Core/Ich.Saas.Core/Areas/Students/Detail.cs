using System.Collections.Generic;
using Ich.Saas.Core.Code;

namespace Ich.Saas.Core.Areas.Students
{
    public class Detail : BaseModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public int TotalEnrollments { get; set; }
        
        // Related items

        public string Tab { get; set; } = "details";

        public List<_Enrollment> Enrollments { get; set; } = new List<_Enrollment>();
    }
}