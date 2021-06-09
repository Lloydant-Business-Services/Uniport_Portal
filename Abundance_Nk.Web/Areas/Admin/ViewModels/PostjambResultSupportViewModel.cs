using Abundance_Nk.Model.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class PostjambResultSupportViewModel
    {
        public PostjambResultSupportViewModel()
        {
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            ProgrammeSelectList = Utility.PopulateAllProgrammeSelectListItem();
        }
        public PutmeResult putmeResult { get; set; }
        public List<PutmeResult> AllResults { get; set; }
        public ApplicantJambDetail jambDetail { get; set; }
        public ApplicationForm ApplicationDetail { get; set; }

        [Display(Name = "Jamb Number")]
        public string JambNumber { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public Session Session { get; set; }
        public Programme Programme { get; set; }
        public List<PutmeResultModel> PutmeResults { get; set; }
        public List<StudentDetails> StudentDetailList { get; set; }
    }
}