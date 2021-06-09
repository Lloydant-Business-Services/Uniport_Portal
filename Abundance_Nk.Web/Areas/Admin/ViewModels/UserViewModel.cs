using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            SexSelectList = Utility.PopulateSexSelectListItem();
            RoleSelectList = Utility.PopulateRoleSelectListItem();
            SecurityQuestionSelectList = Utility.PopulateSecurityQuestionSelectListItem();
            SessionSelectList = Utility.PopulateSessionSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            ProgrammeSelectList = Utility.PopulateAllProgrammeSelectListItem();
            FacultySelectList = Utility.PopulateFacultySelectListItem();
                }

        public User User { get; set; }
        public HttpPostedFileBase PassportFile { get; set; } 
        public HttpPostedFileBase SignatureFile { get; set; }
        public CourseAllocation CourseAllocation { get; set; }
        public List<User> Users { get; set; }
        public List<SelectListItem> SexSelectList { get; set; }
        public List<SelectListItem> RoleSelectList { get; set; }
        public List<SelectListItem> SecurityQuestionSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> FacultySelectList { get; set; }
        public Faculty Faculty { get; set; }
        public Staff Staff { get; set; }
    }
}