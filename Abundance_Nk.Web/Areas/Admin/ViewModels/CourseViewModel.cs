using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class CourseViewModel
    {
        public CourseViewModel()
        {
            DepartmentSelectListItem = Utility.PopulateAllDepartmentSelectListItem();
            levelSelectListItem = Utility.PopulateLevelSelectListItem();
            CourseTypeSelectListItem = Utility.PopulateCourseTypeSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            SemesterSelectListItem = Utility.PopulateSemesterSelectListItem();
            firstSemesterCourses = new List<Course>();
            secondSemesterCourses = new List<Course>();
        }

        public Semester Semester { get; set; }
        public CourseType CourseType { get; set; }
        public Programme programme { get; set; }
        public Department Department { get; set; }
        public Level level { get; set; }
        public Course course { get; set; }
        public Session Session { get; set; }
        public Model.Model.Student Student { get; set; }
        public CourseRegistration CourseRegistration { get; set; }
        public List<SelectListItem> SemesterSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> CourseTypeSelectListItem { get; set; }
        public List<SelectListItem> levelSelectListItem { get; set; }
        public List<SelectListItem> DepartmentOpionSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<Course> firstSemesterCourses { get; set; }
        public List<Course> secondSemesterCourses { get; set; }
        public List<Course> Courses { get; set; }
    }
}