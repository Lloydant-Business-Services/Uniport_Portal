using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class OLevelResult
    {
        public long Id { get; set; }
        public Person Person { get; set; }

        public PersonType PersonType { get; set; }

        [Display(Name = "Exam No")]
        public string ExamNumber { get; set; }

        [Required]
        [Display(Name = "Exam Year")]
        public int ExamYear { get; set; }

        public OLevelExamSitting Sitting { get; set; }
        public OLevelType Type { get; set; }

        public ApplicationForm ApplicationForm { get; set; }

        [Display(Name = "Scanned Copy")]
        public string ScannedCopyUrl { get; set; }
    }
}