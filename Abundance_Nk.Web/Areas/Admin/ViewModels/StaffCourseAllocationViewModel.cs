using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class StaffCourseAllocationViewModel
    {
        public StaffCourseAllocationViewModel()
        {
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            ProgrammeSelectList = Utility.PopulateAllProgrammeSelectListItem();
            LevelList = Utility.GetAllLevels();
            UserSelectList = Utility.PopulateStaffSelectListItem();
            SemesterSelectList = Utility.PopulateSemesterSelectListItem();
        }

        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<SelectListItem> SemesterSelectList { get; set; }
        public List<SelectListItem> UserSelectList { get; set; }
        public List<ResultFormat> resultFormatList { get; set; }
        public List<Level> LevelList { get; set; }
        public Level Level { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public Course Course { get; set; }
        public List<Course> Courses { get; set; }
        public User User { get; set; }
        public CourseAllocation CourseAllocation { get; set; }
        public List<CourseAllocation> CourseAllocationList { get; set; }
        public long cid { get; set; }
        public List<CourseAllocation> CourseAllocations { get; set; }
        public List<UploadedCourseFormat> UploadedCourses { get; set; }
        public Staff Staff { get; set; }
        public CourseEvaluation CourseEvaluation { get; set; }
        public List<CourseEvaluationReport> CourseEvaluationReports { get; set; }

        public List<CourseEvaluationAnswer> CourseEvaluationAnswers { get; set; }
        public List<DepartmentOption> DepartmentOptions { get; set; }
    }
}