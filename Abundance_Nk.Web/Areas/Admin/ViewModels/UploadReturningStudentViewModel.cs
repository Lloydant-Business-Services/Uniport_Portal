using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class UploadReturningStudentViewModel
    {
        public UploadReturningStudentViewModel()
        {
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
            SessionSelectListItem = Utility.PopulateSessionSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
        }

        public List<SelectListItem> LevelSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public Department Department { get; set; }
        public Level Level { get; set; }
        public Programme Programme { get; set; }
        public Session Session { get; set; }
        public HttpPostedFileBase File { get; set; }
        public List<ReturningStudents> ReturningStudentList { get; set; }
        public List<UploadedStudentModel> UploadedStudents { get; set; }
    }

    public class UploadedStudentModel
    {
        public string Name { get; set; }
        public string MatricNumber { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string Level { get; set; }
        public string Session { get; set; }
    }
}