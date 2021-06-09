using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Sex
    {
        [Required]
        [Display(Name = "Sex")]
        public byte Id { get; set; }

        [Display(Name = "Sex")]
        public string Name { get; set; }
    }
}