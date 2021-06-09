using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Faculty : BasicSetup
    {
        [Display(Name = "Faculty")]
        public override int Id { get; set; }

        [Display(Name = "Faculty")]
        public override string Name { get; set; }
    }
}