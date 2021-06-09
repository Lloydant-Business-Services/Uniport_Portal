using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Title : Setup
    {
        [Display(Name = "Title")]
        public int Id { get; set; }
    }
}