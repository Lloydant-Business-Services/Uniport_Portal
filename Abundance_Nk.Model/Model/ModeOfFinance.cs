using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class ModeOfFinance : BasicSetup
    {
        [Display(Name = "Mode of Finance")]
        public override int Id { get; set; }

        [Display(Name = "Mode of Finance")]
        public override string Name { get; set; }
    }
}