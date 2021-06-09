using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class EducationalQualification
    {
        [Display(Name = "Qualification")]
        public int Id { get; set; }

        public string ShortName { get; set; }

        public string Name { get; set; }
    }
}