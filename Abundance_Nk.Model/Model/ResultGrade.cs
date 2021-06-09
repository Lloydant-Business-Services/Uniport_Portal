using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class ResultGrade
    {
        [Display(Name = "Class of Degree")]
        public int Id { get; set; }

        public decimal CGPAFrom { get; set; }
        public decimal CGPATo { get; set; }
        public string LevelOfPassCode { get; set; }

        [Display(Name = "Level of Pass")]
        public string LevelOfPass { get; set; }
    }
}