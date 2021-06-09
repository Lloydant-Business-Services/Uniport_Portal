using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class ModeOfStudy : BasicSetup
    {
        [Display(Name = "Mode of Study")]
        public override int Id { get; set; }

        [Display(Name = "Mode of Study")]
        public override string Name { get; set; }
    }
}