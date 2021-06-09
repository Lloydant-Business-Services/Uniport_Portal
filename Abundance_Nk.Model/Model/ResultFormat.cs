using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class ResultFormat
    {
        public ResultFormat()
        {
            ResultSpecialCaseMessages = new ResultSpecialCaseMessages();
        }

        public int SN { get; set; }

        [Display(Name = "Matric No")]
        public string MatricNo { get; set; }

        public string Fullname { get; set; }

        [Display(Name = "Student Department")]
        public string Department { get; set; }

        [Display(Name = "In Course")]
        public decimal? CA { get; set; }

        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Display(Name = "Exam Score")]
        public decimal? Exam { get; set; }

        public ResultSpecialCaseMessages ResultSpecialCaseMessages { get; set; }
    }

    public class CourseUploadFormat
    {
        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Display(Name = "Course Unit")]
        public int CourseUnit { get; set; }

        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Display(Name ="Semester")]
        public int SemesterId { get; set; }
    }
}