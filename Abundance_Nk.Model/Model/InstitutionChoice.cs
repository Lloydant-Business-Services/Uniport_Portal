using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class InstitutionChoice : Setup
    {
        [Display(Name = "Choice")]
        public override int Id { get; set; }
    }
}