using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Web.Areas.Applicant.ViewModels
{
    public class PostJAMBProgrammeViewModel
    {
        [Required]
        [Display(Name = "Confirmation Order Number")]
        public string ConfirmationOrderNumber { get; set; }
    }
}