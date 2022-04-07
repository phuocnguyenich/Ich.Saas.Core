using System.ComponentModel.DataAnnotations;
using Ich.Saas.Core.Code;

namespace Ich.Saas.Core.Areas.Classes
{
    public class Edit : BaseModel
    {
        public int Id { get; set; }
        public string ClassNumber { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public int? CourseId { get; set; }
        public string Course { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        public string StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        public string EndDate { get; set; }
        public int MaxEnrollments { get; set; }
    }
}