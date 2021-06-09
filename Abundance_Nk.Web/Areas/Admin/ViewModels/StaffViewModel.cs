using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class StaffViewModel
    {
        public StaffViewModel()
        {
            SessionSelectList = Utility.PopulateSessionSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            ProgrammeSelectList = Utility.PopulateAllProgrammeSelectListItem();
            LevelList = Utility.GetAllLevels();
            UserSelectList = Utility.PopulateStaffSelectListItem();
            CourseModeSelectList = Utility.PopulateCourseModeSelectListItem();
        }

        public List<SelectListItem> CourseModeSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> UserSelectList { get; set; }
        public List<ResultFormat> resultFormatList { get; set; }
        public List<Level> LevelList { get; set; }
        public Level Level { get; set; }
        public Programme Programme { get; set; }
        public CourseMode CourseMode { get; set; }
        public Department Department { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public Course Course { get; set; }
        public User User { get; set; }
        public CourseAllocation CourseAllocation { get; set; }
        public List<CourseAllocation> CourseAllocationList { get; set; }
        public long Cid { get; set; }
        public bool IsAlternate { get; set; }
        public List<CourseAllocation> CourseAllocations { get; set; }
        public List<UploadedCourseFormat> UploadedCourses { get; set; }
    }
}