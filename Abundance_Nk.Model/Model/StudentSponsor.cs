using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class StudentSponsor
    {
        public Student Student { get; set; }
        public Relationship Relationship { get; set; }

        [Required]
        [Display(Name = "Sponsor")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Sponsor's Address")]
        public string ContactAddress { get; set; }

        [Required]
        [Display(Name = "Mobile Phone")]
        public string MobilePhone { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}