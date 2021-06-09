using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Religion : Setup
    {
        [Display(Name = "Religion")]
        public override int Id { get; set; }
    }
}