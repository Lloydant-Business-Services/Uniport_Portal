using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Ability : BasicSetup
    {
        [Required]
        [Display(Name = "Disability")]
        public override int Id { get; set; }
    }
}