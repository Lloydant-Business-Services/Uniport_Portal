using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class FeeType : BasicSetup
    {
        [Required]
        [Display(Name = "Fee Type")]
        public override string Name { get; set; }
        public bool? Active { get; set; }
    }
}