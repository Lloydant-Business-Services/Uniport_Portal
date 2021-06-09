using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class StudentCourseRegistrationViewModel
    {
        public StudentCourseRegistrationViewModel()
        {
            ProgrammeSelectList = Utility.PopulateAllProgrammeSelectListItem();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            LevelList = Utility.GetAllLevels();
            SemesterList = Utility.PopulateSemesterSelectListItem();
        }
        public Course Course { get; set; }
        public Department Department { get; set; }
        public Level Level { get; set; }
        public Session Session { get; set; }
        public Programme Programme { get; set; }
        public Semester Semester { get; set; }
        public Model.Model.Student Student { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public List<Level> LevelList { get; set; }
        public List<Course> Courses { get; set; }
        public List<Payment> Payments { get; set; }
        public List<StudentLevel> StudentLevelList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<CourseRegistration> CourseRegistrations { get; set; }
        public List<PaymentEtranzact> PaymentEtranzacts { get; set; }
        public PaymentEtranzact PaymentEtranzact { get; set; }
        public Decimal Amount { get; set; }
        public List<SelectListItem> SemesterList { get; set; }
        public User User { get; set; }
    }
}