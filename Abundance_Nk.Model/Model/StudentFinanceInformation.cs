using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class StudentFinanceInformation
    {
        public Student Student { get; set; }
        public ModeOfFinance Mode { get; set; }

        [Display(Name = "Scholarship Title")]
        public string ScholarshipTitle { get; set; }
    }
}