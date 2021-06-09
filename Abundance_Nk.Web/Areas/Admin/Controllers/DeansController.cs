using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class DeansController :BaseController
    {
        
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private string FileUploadURL;
        private StaffCourseAllocationViewModel viewModel;

        public DeansController()
        {
            viewModel = new StaffCourseAllocationViewModel();
        }
        // GET: Admin/Deans
       public ActionResult AllocateCourseByDepartment()
        {
            try
            {
               
                var loggeduser = new UserLogic();
                var user = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                if (user != null && user.Id > 0)
                {
                   StaffLogic staffLogic = new StaffLogic();
                   Staff staff = staffLogic.GetBy(user.Id );
                    if (staff != null && staff.Id > 0)
                    {
                        
                        viewModel.CourseAllocation = new CourseAllocation();
                        viewModel.CourseAllocation.Department = staff.Department;
                        if ((bool)staff.isManagement)
                        {
                            ViewBag.Department = Utility.PopulateDepartmentSelectListItemByFacultyId(staff.Department.Faculty.Id);
                        }
                        else
                        {
                            ViewBag.Department = Utility.PopulateDepartmentSelectListItem();
                        }
                        var programme = new Programme { Id = 1 };
                        var departmentOptions = Utility.PopulateDepartmentOptionSelectListItem(staff.Department, programme);
                        
                        if (departmentOptions?.Count > 0)
                        {
                            viewModel.CourseAllocation.DepartmentOption = new DepartmentOption();
                            ViewBag.DepartmentOption = departmentOptions;
                        }
                        viewModel.Staff = staff;

                    }
                    
                }
               

            }
            catch(Exception ex)
            {
                SetMessage("Error: " + ex.Message,Message.Category.Error);
            }

            KeepCourseDropDownState(viewModel);
            return View(viewModel);
        }
       [HttpPost]
       public ActionResult AllocateCourseByDepartment(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                if(viewModel != null)
                {
                    var courseAllocationLogic = new CourseAllocationLogic();
                    List<CourseAllocation> courseAllocationList = null;
                    if (viewModel.CourseAllocation.DepartmentOption?.Id > 0)
                    {
                        courseAllocationList=courseAllocationLogic.GetModelsBy(
                            p =>
                                p.Level_Id == viewModel.CourseAllocation.Level.Id &&
                                p.Programme_Id == viewModel.CourseAllocation.Programme.Id &&
                                p.Session_Id == viewModel.CourseAllocation.Session.Id &&
                                p.Department_Id == viewModel.CourseAllocation.Department.Id
                                && p.Semester_Id == viewModel.CourseAllocation.Semester.Id && p.DepartmentOption_Id==viewModel.CourseAllocation.DepartmentOption.Id);
                    }
                    else
                    {
                        courseAllocationList=courseAllocationLogic.GetModelsBy(
                            p =>
                                p.Level_Id == viewModel.CourseAllocation.Level.Id &&
                                p.Programme_Id == viewModel.CourseAllocation.Programme.Id &&
                                p.Session_Id == viewModel.CourseAllocation.Session.Id &&
                                p.Department_Id == viewModel.CourseAllocation.Department.Id
                                && p.Semester_Id == viewModel.CourseAllocation.Semester.Id);
                    }
                        
                    if(courseAllocationList.Count <= 0 && viewModel.CourseAllocation.Department != null && viewModel.CourseAllocation.Level != null && viewModel.CourseAllocation.Programme != null && viewModel.CourseAllocation.Session != null)
                    {
                        var courseLogic = new CourseLogic();
                        
                        viewModel.Courses = courseLogic.GetBy(viewModel.CourseAllocation.Department,
                            viewModel.CourseAllocation.Level,viewModel.CourseAllocation.Semester,
                            viewModel.CourseAllocation.Programme, viewModel.CourseAllocation.DepartmentOption);
                        //SetMessage("Course Allocated", Message.Category.Information);
                        if(viewModel.Courses != null && viewModel.Courses.Count > 0)
                        {
                            var courseAllocations = new List<CourseAllocation>();
                            foreach(Course course in viewModel.Courses)
                            {
                                var courseAllocation = new CourseAllocation();
                                courseAllocation.Programme = course.Programme;
                                courseAllocation.Department = course.Department;
                                courseAllocation.Level = course.Level;
                                courseAllocation.Semester = course.Semester;
                                courseAllocation.Course = course;
                                courseAllocation.Session = viewModel.CourseAllocation.Session;
                                courseAllocation.DepartmentOption = viewModel.CourseAllocation.DepartmentOption;
                                courseAllocations.Add(courseAllocation);
                            }
                            viewModel.CourseAllocationList = courseAllocations;
                        }
                        var programme = new Programme { Id = viewModel.CourseAllocation.Programme .Id};
                        var departmentOptions = Utility.PopulateDepartmentOptionSelectListItem(viewModel.CourseAllocation.Department, programme);
                        if (departmentOptions?.Count > 0)
                        {
                            viewModel.CourseAllocation.DepartmentOption = new DepartmentOption();
                            ViewBag.DepartmentOption = departmentOptions;
                        }

                        KeepCourseDropDownState(viewModel);
                        return View(viewModel);
                    }
                    viewModel.CourseAllocationList = courseAllocationList;
                    KeepCourseDropDownState(viewModel);
                    var programme2 = new Programme { Id = viewModel.CourseAllocation.Programme.Id };
                    var departmentOptions2 = Utility.PopulateDepartmentOptionSelectListItem(viewModel.CourseAllocation.Department, programme2);
                    if (departmentOptions2?.Count > 0)
                    {
                        viewModel.CourseAllocation.DepartmentOption = new DepartmentOption();
                        ViewBag.DepartmentOption = departmentOptions2;
                    }
                    return View(viewModel);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error: " + ex.Message,Message.Category.Error);
            }
            return View("AllocateCourseByDepartment");
        }
       [HttpPost]
       public ActionResult SavedAllocateCourseByDepartment(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                if(viewModel != null)
                {
                    var courseAllocationLogic = new CourseAllocationLogic();
                    using(var scope = new TransactionScope())
                    {
                        foreach(CourseAllocation courseAllocation in viewModel.CourseAllocationList)
                        {
                            var courseAlreadyAssigned=courseAllocationLogic.GetModelsBy(f => f.Department_Id == courseAllocation.Department.Id && f.Programme_Id == courseAllocation.Programme.Id && f.Course_Id == courseAllocation.Course.Id).FirstOrDefault();
                            if (courseAlreadyAssigned?.Id > 0)
                            {
                                if(courseAlreadyAssigned.User?.Id>0 && courseAlreadyAssigned.User.Id != courseAllocation.User.Id)
                                {
                                    courseAllocationLogic.Modify(courseAllocation);
                                }
                                
                            }
                            else
                            {
                                courseAllocationLogic.Create(courseAllocation);
                            }
                            
                        }
                        scope.Complete();
                        SetMessage("Course Allocated",Message.Category.Information);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error: " + ex.Message,Message.Category.Error);
            }
            return RedirectToAction("AllocateCourseByDepartment");
        }
       public void KeepDropDownState(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                ViewBag.User = viewModel.UserSelectList;
                if(viewModel.Semester != null)
                {
                    var sessionSemesterList = new List<SessionSemester>();
                    var sessionSemesterLogic = new SessionSemesterLogic();
                    sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == viewModel.Session.Id);

                    var semesters = new List<Semester>();
                    foreach(SessionSemester sessionSemester in sessionSemesterList)
                    {
                        semesters.Add(sessionSemester.Semester);
                    }

                    ViewBag.Semester = new SelectList(semesters,ID,NAME,viewModel.Semester.Id);
                }
                if(viewModel.Department != null && viewModel.Department.Id > 0)
                {
                    var departmentLogic = new DepartmentLogic();
                    var departments = new List<Department>();
                    departments = departmentLogic.GetBy(viewModel.Programme);

                    ViewBag.Department = new SelectList(departments,ID,NAME,viewModel.Department.Id);
                }
                if(viewModel.Course != null && viewModel.Course.Id > 0)
                {
                    var courseList = new List<Course>();
                    courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.Level,viewModel.Department,
                        viewModel.Semester,viewModel.Programme);

                    ViewBag.Course = new SelectList(courseList,ID,NAME,viewModel.Course.Id);
                }
            }
            catch(Exception)
            {
                throw;
            }
        }
       public void KeepCourseDropDownState(StaffCourseAllocationViewModel viewModel)
        {
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Level = new SelectList(viewModel.LevelList,ID,NAME);
                ViewBag.User = viewModel.UserSelectList;
                ViewBag.Semester = viewModel.SemesterSelectList;
                if(viewModel.CourseAllocation.Semester != null)
                {
                    var sessionSemesterList = new List<SessionSemester>();
                    var sessionSemesterLogic = new SessionSemesterLogic();
                    sessionSemesterList =
                        sessionSemesterLogic.GetModelsBy(p => p.Session_Id == viewModel.CourseAllocation.Session.Id);

                    var semesters = new List<Semester>();
                    foreach(SessionSemester sessionSemester in sessionSemesterList)
                    {
                        semesters.Add(sessionSemester.Semester);
                    }

                    ViewBag.Semester = new SelectList(semesters,ID,NAME,viewModel.CourseAllocation.Semester.Id);
                }
                if(viewModel.CourseAllocation.Department != null)
                {
                    var departmentLogic = new DepartmentLogic();
                    var departments = new List<Department>();
                    if (viewModel.CourseAllocation.Programme == null)
                    {
                        viewModel.CourseAllocation.Programme = new Programme(){Id = 1};
                    }
                    departments = departmentLogic.GetBy(viewModel.Staff.Department.Faculty,viewModel.CourseAllocation.Programme);

                    ViewBag.Department = new SelectList(departments,ID,NAME,viewModel.CourseAllocation.Department.Id);
                }
                if(viewModel.CourseAllocation.Course != null)
                {
                    var courseList = new List<Course>();
                    courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.CourseAllocation.Level,
                        viewModel.CourseAllocation.Department,viewModel.CourseAllocation.Semester,
                        viewModel.CourseAllocation.Programme);

                    ViewBag.Course = new SelectList(courseList,ID,NAME,viewModel.CourseAllocation.Course.Id);
                }
                SetSelectedLecturer(viewModel);
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void SetSelectedLecturer(StaffCourseAllocationViewModel existingViewModel)
        {
            try
            {
                if(existingViewModel != null && existingViewModel.CourseAllocationList != null &&
                    existingViewModel.CourseAllocationList.Count > 0)
                {
                    int i = 0;
                    foreach(CourseAllocation courseAllocation in existingViewModel.CourseAllocationList)
                    {
                        if(courseAllocation.User != null)
                        {
                            ViewData["lecturer" + i] = new SelectList(existingViewModel.UserSelectList,VALUE,TEXT,courseAllocation.User.Id);
                          
                        }
                        else
                        {
                            ViewData["lecturer" + i] = new SelectList(existingViewModel.UserSelectList,VALUE,TEXT,0);
                            
                        }

                        i++;
                    }
                }

                
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        public ActionResult AttendanceSheet(string Level,string Semester, string programme, string department, string session, string course)
        {
            

            ViewBag.levelId = Utility.Decrypt(Level);
            ViewBag.semesterId = Utility.Decrypt(Semester);
            ViewBag.progId = Utility.Decrypt(programme);
            ViewBag.deptId = Utility.Decrypt(department);
            ViewBag.sessionId = Utility.Decrypt(session);
            ViewBag.courseId = Utility.Decrypt(course);
           

            return View();
        }
        public ActionResult ResultSheet(string Level,string Semester, string programme, string department, string session, string course)
        {
            

            ViewBag.levelId = Utility.Decrypt(Level);
            ViewBag.semesterId = Utility.Decrypt(Semester);
            ViewBag.progId = Utility.Decrypt(programme);
            ViewBag.deptId = Utility.Decrypt(department);
            ViewBag.sessionId = Utility.Decrypt(session);
            ViewBag.courseId = Utility.Decrypt(course);
           

            return View();
        }

        public ActionResult MasterGradeSheet()
        {
            return View();
        }

        public ActionResult CompositeCourseRegistration()
        {
            return View();
        }
    }
}