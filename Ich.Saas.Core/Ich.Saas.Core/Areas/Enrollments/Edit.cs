using System.ComponentModel.DataAnnotations;
using Ich.Saas.Core.Code;

namespace Ich.Saas.Core.Areas.Enrollments
{
    public class Edit : BaseModel
    {
        public int Id { get; set; }
        public string EnrollNumber { get; set; }

        [Required(ErrorMessage = "Student is required.")]
        public int? StudentId { get; set; }
        [Required(ErrorMessage = "Class is required.")]
        public int? ClassId { get; set; }
        public int? CourseId { get; set; }
        public string EnrollDate { get; set; }
        public string AmountPaid { get; set; }
        public string Status { get; set; }
    }
}