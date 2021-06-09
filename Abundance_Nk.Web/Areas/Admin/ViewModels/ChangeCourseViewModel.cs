using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class ChangeCourseViewModel
    {
        public ChangeCourseViewModel()
        {
            ProgrammeSelectList = Utility.PopulateAllProgrammeSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
        }

        public string ApplicationFormNumber { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public AppliedCourse AppliedCourse { get; set; }
        public Person Person { get; set; }
        public Payment Payment { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> DepartmentOptionSelectListItem { get; set; }
        public decimal OldSchoolFees { get; set; }
        public decimal ShortFallAmount { get; set; }
        public string ShortFallDescription { get; set; }
        public Level Level { get; set; }
        public Session Session { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
    }
}