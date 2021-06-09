using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Genotype : Setup
    {
        [Display(Name = "Genotype")]
        public override int Id { get; set; }

        [Display(Name = "Genotype")]
        public override string Name { get; set; }
    }
}