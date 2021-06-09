using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class StudentAcademicInformation
    {
        public Student Student { get; set; }
        public ModeOfEntry ModeOfEntry { get; set; }
        public ModeOfStudy ModeOfStudy { get; set; }

        [Display(Name = "Year of Admission")]
        public int YearOfAdmission { get; set; }

        [Display(Name = "Possible Year of Graduation")]
        public int YearOfGraduation { get; set; }

        public Level Level { get; set; }

        public System.DateTime? DateEntered { get; set; }
    }
}