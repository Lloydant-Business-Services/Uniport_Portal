using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class SlugViewModel
    {
        public SlugViewModel()
        {
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            if(Programme != null && Programme.Id > 0)
            {
                DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(Programme);
            }
            SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
        }

        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public AppliedCourse appliedCourse { get; set; }
        public List<Slug> applicantDetails { get; set; }
        public bool IsBulk { get; set; }
        public Session Session { get; set; }
    }
}