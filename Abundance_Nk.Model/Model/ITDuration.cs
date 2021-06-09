using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class ITDuration : BasicSetup
    {
        //[Required]
        [Display(Name = "IT Duration")]
        public override int Id { get; set; }
    }
}