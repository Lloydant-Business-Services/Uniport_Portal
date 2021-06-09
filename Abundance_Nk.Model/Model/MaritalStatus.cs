using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class MaritalStatus : Setup
    {
        [Display(Name = "Marital Status")]
        public override int Id { get; set; }

        [Display(Name = "Marital Status")]
        public override string Name { get; set; }
    }
}