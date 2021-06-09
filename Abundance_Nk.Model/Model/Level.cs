using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Level : BasicSetup
    {
        [Display(Name = "Level")]
        public override int Id { get; set; }

        [Display(Name = "Level")]
        public override string Name { get; set; }

        public SchoolType SchoolType { get; set; }
    }
}