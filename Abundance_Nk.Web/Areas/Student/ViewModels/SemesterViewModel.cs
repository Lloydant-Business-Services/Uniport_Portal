using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Student.ViewModels
{
    public class SemesterViewModel
    {
        public SemesterViewModel()
        {
            SemesterSelectList = Utility.PopulateSemesterSelectListItem();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            levelSelectList = Utility.PopulateLevelSelectListItem();
        }

        public long sid { get; set; }
        public List<SelectListItem> SemesterSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> levelSelectList { get; set; }
        public Session session { get; set; }
        public Semester semester { get; set; }
        public Model.Model.Student Student { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public int semesterId { get; set; }
    }
}