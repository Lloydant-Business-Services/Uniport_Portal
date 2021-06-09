using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class StudentResultType : BasicSetup
    {
        [Display(Name = "Result Type")]
        public override int Id { get; set; }
    }
}