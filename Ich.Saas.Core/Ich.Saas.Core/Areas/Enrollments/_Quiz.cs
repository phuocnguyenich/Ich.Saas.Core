namespace Ich.Saas.Core.Areas.Enrollments
{
    public class _Quiz
    {
        public int Id { get; set; }
        public int? EnrollmentId { get; set; }

        public string QuizNumber { get; set; }
        public string QuizDate { get; set; }
        public string Grade { get; set; }
    }
}