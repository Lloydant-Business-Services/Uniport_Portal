using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class BloodGroup : Setup
    {
        [Display(Name = "Blood Group")]
        public override int Id { get; set; }

        [Display(Name = "Blood Group")]
        public override string Name { get; set; }
    }
}