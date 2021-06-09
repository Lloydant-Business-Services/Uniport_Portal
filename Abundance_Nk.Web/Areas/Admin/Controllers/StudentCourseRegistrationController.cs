using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "lloydant,Admin,School Admin,ICT Staff,hod,Dean,ViceChancellor")]
    public class StudentCourseRegistrationController : BaseController
    {
        public const string ID = "Id";
        public const string NAME = "Name";
        // GET: Admin/StudentCourseRegistration
        public ActionResult RegisterCourse()
        {
            try
            {
                StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
                return View();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return View();
            }

        }

        public void RetainDropdownState(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                SemesterLogic semesterLogic = new SemesterLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                SessionLogic sessionLogic = new SessionLogic();
                ProgrammeLogic programmeLogic = new ProgrammeLogic();
                LevelLogic levelLogic = new LevelLogic();
                if (viewModel != null)
                {
                    if (viewModel.Session != null)
                    {

                        ViewBag.Session = new SelectList(sessionLogic.GetAll(), ID, NAME,
                            viewModel.Session.Id);
                    }
                    else
                    {
                        ViewBag.Session = viewModel.SessionSelectList;
                    }
                    if (viewModel.Semester != null)
                    {
                        ViewBag.Semester = new SelectList(semesterLogic.GetAll(), ID, NAME, viewModel.Semester.Id);
                    }
                    else
                    {
                        ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                    }
                    if (viewModel.Programme != null)
                    {
                        ViewBag.Programme = new SelectList(programmeLogic.GetModelsBy(p => p.Activated == true), ID,
                            NAME, viewModel.Programme.Id);
                    }
                    else
                    {
                        ViewBag.Programme = viewModel.ProgrammeSelectList;
                    }
                    if (viewModel.Department != null && viewModel.Programme != null)
                    {
                        ViewBag.Department = new SelectList(departmentLogic.GetBy(viewModel.Programme), ID, NAME,
                            viewModel.Department.Id);
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                    }
                    if (viewModel.Level != null)
                    {
                        ViewBag.Level = new SelectList(levelLogic.GetAll(), ID, NAME, viewModel.Level.Id);
                    }
                    else
                    {
                        ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                    }
                    if (viewModel.Course != null && viewModel.Level != null && viewModel.Semester != null &&
                        viewModel.Department != null)
                    {
                        List<Course> courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.Level,
                            viewModel.Department, viewModel.Semester, viewModel.Programme);
                        ViewBag.Course = new SelectList(courseList, ID, NAME, viewModel.Level.Id);
                    }
                    else
                    {
                        ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "lloydant,Admin,School Admin")]
        public ActionResult RegisterCourse(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                List<CourseRegistration> courseRegistrationListCount = new List<CourseRegistration>();
                int courseRegDetailCheckCount = 0;
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                List<StudentLevel> studentLevelList = new List<StudentLevel>();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                CourseMode courseMode = new CourseMode() { Id = 1 };

                string operation = "INSERT";
                string action = "ADMIN :REGISTRATION FROM ADMIN CONSOLE (StudentCourseRegistration)";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                courseRegistrationDetailAudit.Action = action;
                courseRegistrationDetailAudit.Operation = operation;
                courseRegistrationDetailAudit.Client = client;
                UserLogic loggeduser = new UserLogic();
                courseRegistrationDetailAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);


                studentLevelList =
                    studentLevelLogic.GetModelsBy(
                        p =>
                            p.Department_Id == viewModel.Department.Id && p.Programme_Id == viewModel.Programme.Id &&
                            p.Session_Id == viewModel.Session.Id && p.Level_Id == viewModel.Level.Id);
                foreach (StudentLevel studentLevel in studentLevelList)
                {
                    List<CourseRegistration> courseRegistrationList = new List<CourseRegistration>();
                    courseRegistrationList =
                        courseRegistrationLogic.GetModelsBy(
                            p =>
                                p.Person_Id == studentLevel.Student.Id && p.Programme_Id == studentLevel.Programme.Id &&
                                p.Department_Id == studentLevel.Department.Id && p.Level_Id == studentLevel.Level.Id &&
                                p.Session_Id == studentLevel.Session.Id);
                    if (courseRegistrationList != null && courseRegistrationList.Count() > 0)
                    {
                        foreach (CourseRegistration item in courseRegistrationList)
                        {
                            CourseRegistrationDetail courseRegistrationDetailCheck = new CourseRegistrationDetail();
                            courseRegistrationDetailCheck =
                                courseRegistrationDetailLogic.GetModelsBy(
                                    p =>
                                        p.Student_Course_Registration_Id == item.Id &&
                                        p.Course_Id == viewModel.Course.Id && p.Semester_Id == viewModel.Semester.Id &&
                                        p.Course_Mode_Id == 1).FirstOrDefault();
                            if (courseRegistrationDetailCheck != null)
                            {
                                courseRegDetailCheckCount += 1;

                            }
                        }
                    }
                    if (courseRegDetailCheckCount == 0)
                    {
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            CourseRegistration courseRegistration = new CourseRegistration();
                            courseRegistration.Student = studentLevel.Student;
                            courseRegistration.Session = studentLevel.Session;
                            courseRegistration.Programme = studentLevel.Programme;
                            courseRegistration.Department = studentLevel.Department;
                            courseRegistration.Level = studentLevel.Level;
                            courseRegistration = courseRegistrationLogic.CreateCourseRegistration(courseRegistration);
                            courseRegistrationDetail.CourseRegistration = courseRegistration;
                            courseRegistrationDetail.Course = viewModel.Course;
                            courseRegistrationDetail.Mode = courseMode;
                            courseRegistrationDetail.Semester = viewModel.Semester;
                            courseRegistrationDetail = courseRegistrationDetailLogic.Create(courseRegistrationDetail, courseRegistrationDetailAudit);
                            courseRegistrationListCount.Add(courseRegistration);
                            transaction.Complete();
                        }
                    }
                    courseRegDetailCheckCount = 0;
                }
                TempData["Action"] = courseRegistrationListCount.Count + " Students registered  successfully";
                return RedirectToAction("RegisterCourse", new { controller = "StudentCourseRegistration", area = "Admin" });
            }
            catch (Exception ex)
            {
                RetainDropdownState(viewModel);
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return View();
            }

        }

        public ActionResult RegisterCourseBulkForPastLevel()
        {
            try
            {
                StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
                return View();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return View();
            }

        }
        [HttpPost]
        [Authorize(Roles = "lloydant,Admin,School Admin")]
        public ActionResult RegisterCourseBulkForPastLevel(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                int totalRegistrationDone = 0;

                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                CourseMode courseMode = new CourseMode() { Id = 1 };
                ProgrammeDepartmentLogic programmeDepartmentLogic = new ProgrammeDepartmentLogic();


                string operation = "INSERT";
                string action = "ADMIN :REGISTRATION FROM ADMIN CONSOLE (StudentCourseRegistration)";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                courseRegistrationDetailAudit.Action = action;
                courseRegistrationDetailAudit.Operation = operation;
                courseRegistrationDetailAudit.Client = client;
                UserLogic loggeduser = new UserLogic();
                courseRegistrationDetailAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);

                List<CourseRegistration> courseRegistrationListCount = new List<CourseRegistration>();
                List<CourseRegistration> courseRegistration = new List<CourseRegistration>();


                courseRegistration =
                                courseRegistrationLogic.GetModelsBy(
                                    p =>
                                        p.Programme_Id == viewModel.Programme.Id &&
                                        p.Department_Id == viewModel.Department.Id && p.Level_Id == viewModel.Level.Id &&
                                        p.Session_Id == viewModel.Session.Id);

                foreach (var item in courseRegistration)
                {
                    int courseRegDetailCheckCount = 0;
                    List<CourseRegistration> courseRegistrationList = new List<CourseRegistration>();
                    courseRegistrationList =
                        courseRegistrationLogic.GetModelsBy(
                            p =>
                                p.Person_Id == item.Student.Id && p.Programme_Id == item.Programme.Id &&
                                p.Department_Id == item.Department.Id && p.Level_Id == item.Level.Id &&
                                p.Session_Id == viewModel.Session.Id);
                    if (courseRegistrationList != null && courseRegistrationList.Count() > 0)
                    {
                        foreach (CourseRegistration regItem in courseRegistrationList)
                        {
                            CourseRegistrationDetail courseRegistrationDetailCheck = new CourseRegistrationDetail();
                            courseRegistrationDetailCheck =
                                courseRegistrationDetailLogic.GetModelsBy(
                                    p =>
                                        p.Student_Course_Registration_Id == regItem.Id &&
                                        p.Course_Id == viewModel.Course.Id && p.Semester_Id == viewModel.Semester.Id &&
                                        p.Course_Mode_Id == 1).FirstOrDefault();
                            if (courseRegistrationDetailCheck != null)
                            {
                                courseRegDetailCheckCount += 1;

                            }
                            if (courseRegDetailCheckCount == 0)
                            {
                                using (TransactionScope transaction = new TransactionScope())
                                {
                                    //CourseRegistration courseRegistration = new CourseRegistration();
                                    //courseRegistration.Student = studentLevel.Student;
                                    //courseRegistration.Session = viewModel.Session;
                                    //courseRegistration.Programme = studentLevel.Programme;
                                    //courseRegistration.Department = studentLevel.Department;
                                    //courseRegistration.Level = viewModel.Level;
                                    //courseRegistration = courseRegistrationLogic.CreateCourseRegistration(courseRegistration);
                                    courseRegistrationDetail.CourseRegistration = regItem;
                                    courseRegistrationDetail.Course = viewModel.Course;
                                    courseRegistrationDetail.Mode = courseMode;
                                    courseRegistrationDetail.Semester = viewModel.Semester;
                                    courseRegistrationDetail = courseRegistrationDetailLogic.Create(courseRegistrationDetail, courseRegistrationDetailAudit);
                                    //courseRegistrationListCount.Add(regItem);
                                    totalRegistrationDone = +1;
                                    transaction.Complete();
                                    
                                }
                                
                            }
                            courseRegDetailCheckCount = 0;

                        }
                    }

                    
                }

                TempData["Action"] = totalRegistrationDone + " Students registered  successfully";
                return RedirectToAction("RegisterCourseBulkForPastLevel", new { controller = "StudentCourseRegistration", area = "Admin" });
            }
            catch (Exception ex)
            {
                RetainDropdownState(viewModel);
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return View();
            }

        }
        [Authorize(Roles = "lloydant,Admin,School Admin")]
        public ActionResult UnRegisterCourse()
        {
            try
            {
                StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
                return View();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return View();
            }

        }
        [Authorize(Roles = "lloydant,Admin,School Admin")]
        [HttpPost]
        public ActionResult UnRegisterCourse(StudentCourseRegistrationViewModel viewModel)
        {

            try
            {

                CourseRegistrationDetailAuditLogic courseRegistrationDetailAuditLogic = new CourseRegistrationDetailAuditLogic();
                List<CourseRegistration> courseRegistrationListCount = new List<CourseRegistration>();
                int courseRegistrationDeleteCount = 0;
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                List<StudentLevel> studentLevelList = new List<StudentLevel>();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                CourseMode courseMode = new CourseMode() { Id = 1 };

                string operation = "Unregister";
                string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (CoursesController)";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                courseRegistrationDetailAudit.Action = action;
                courseRegistrationDetailAudit.Operation = operation;
                courseRegistrationDetailAudit.Client = client;
                UserLogic loggeduser = new UserLogic();
                courseRegistrationDetailAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);

                studentLevelList =
                    studentLevelLogic.GetModelsBy(
                        p =>
                            p.Department_Id == viewModel.Department.Id && p.Programme_Id == viewModel.Programme.Id &&
                            p.Session_Id == viewModel.Session.Id && p.Level_Id == viewModel.Level.Id);
                foreach (StudentLevel studentLevel in studentLevelList)
                {
                    List<CourseRegistration> courseRegistrationList = new List<CourseRegistration>();
                    courseRegistrationList =
                        courseRegistrationLogic.GetModelsBy(
                            p =>
                                p.Person_Id == studentLevel.Student.Id && p.Programme_Id == studentLevel.Programme.Id &&
                                p.Department_Id == studentLevel.Department.Id && p.Level_Id == studentLevel.Level.Id &&
                                p.Session_Id == studentLevel.Session.Id);
                    if (courseRegistrationList != null && courseRegistrationList.Count() > 0)
                    {
                        foreach (CourseRegistration item in courseRegistrationList)
                        {
                            List<CourseRegistrationDetail> courseRegistrationDetailCheckList =
                                new List<CourseRegistrationDetail>();

                            courseRegistrationDetailCheckList =
                                courseRegistrationDetailLogic.GetModelsBy(
                                    p =>
                                        p.Student_Course_Registration_Id == item.Id && p.Test_Score == 0.00M &&
                                        p.Exam_Score == 0.00M);

                            CourseRegistrationDetail courseRegistrationDetailCheck = courseRegistrationDetailLogic.GetModelsBy(p => p.Student_Course_Registration_Id == item.Id && (p.Course_Id == viewModel.Course.Id || p.COURSE.Course_Code == viewModel.Course.Code) && p.Semester_Id == viewModel.Semester.Id && p.Course_Mode_Id == (int)CourseModes.FirstAttempt).LastOrDefault();


                            using (TransactionScope scope = new TransactionScope())
                            {
                                if (courseRegistrationDetailCheckList.Count == 0)
                                {

                                    courseRegistrationDetailAudit.Course = courseRegistrationDetailCheck.Course;
                                    courseRegistrationDetailAudit.CourseRegistration = item;
                                    courseRegistrationDetailAudit.CourseUnit = courseRegistrationDetailCheck.CourseUnit;
                                    courseRegistrationDetailAudit.Mode = new CourseMode() { Id = (int)CourseModes.FirstAttempt };
                                    courseRegistrationDetailAudit.Semester = viewModel.Semester;
                                    courseRegistrationDetailAudit.Time = DateTime.Now;
                                    courseRegistrationDetailAudit.ExamScore = courseRegistrationDetailCheck.ExamScore;
                                    courseRegistrationDetailAudit.TestScore = courseRegistrationDetailCheck.TestScore;
                                    courseRegistrationDetailAudit.SpecialCase = courseRegistrationDetailCheck.SpecialCase;
                                    courseRegistrationDetailAuditLogic.Create(courseRegistrationDetailAudit);

                                    bool isCourseRegistrationDetailDeleted =
                                        courseRegistrationDetailLogic.Delete(
                                            p =>
                                                p.Student_Course_Registration_Id == item.Id &&
                                                p.Course_Id == viewModel.Course.Id &&
                                                p.Semester_Id == viewModel.Semester.Id && p.Course_Mode_Id == 1);
                                    if (isCourseRegistrationDetailDeleted)
                                    {
                                        courseRegistrationDeleteCount += 1;
                                    }
                                    scope.Complete();
                                }

                            }


                        }
                    }


                }
                TempData["Action"] = courseRegistrationDeleteCount + " Students unregistered  successfully";
                return RedirectToAction("UnRegisterCourse",
                    new { controller = "StudentCourseRegistration", area = "Admin" });
            }
            catch (Exception ex)
            {
                RetainDropdownState(viewModel);
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return View();
            }

        }
        [Authorize(Roles = "lloydant,Admin,School Admin")]
        public ActionResult AddExtraCourse(string studentId, string semesterId, string sessionId)
        {
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {

                Model.Model.Student studentRecord = new Model.Model.Student() { Id = Convert.ToInt64(Utility.Decrypt(studentId)) };
                Model.Model.Session session = new Model.Model.Session() { Id = Convert.ToInt32(Utility.Decrypt(sessionId)) };
                Model.Model.Semester semester = new Model.Model.Semester() { Id = Convert.ToInt32(Utility.Decrypt(semesterId)) };

                if (studentRecord.Id <= 0 || session.Id <= 0 || semester.Id <= 0)
                {
                    return RedirectToAction("StudentDetails");
                }

                StudentLogic studentLogic = new StudentLogic();
                CourseLogic courseLogic = new CourseLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                studentRecord = studentLogic.GetBy(studentRecord.Id);
                viewModel.Student = studentRecord;
                viewModel.StudentLevel = studentLevelLogic.GetBy(studentRecord.Id);
                if (viewModel.StudentLevel == null || viewModel.StudentLevel.Id <= 0)
                {
                    SetMessage("Student level could not be loaded!", Message.Category.Error);
                    RetainDropdownState(viewModel);
                    return View(viewModel);
                }

                SessionLogic sessionLogic = new SessionLogic();
                viewModel.Session = sessionLogic.GetModelBy(a => a.Session_Id == session.Id);

                viewModel.Courses = courseLogic.GetModelsBy(c => c.Department_Id == viewModel.StudentLevel.Department.Id && c.Programme_Id == viewModel.StudentLevel.Programme.Id && c.Level_Id <= viewModel.StudentLevel.Level.Id && c.Semester_Id == semester.Id && c.Activated == true);
                viewModel.Courses.OrderBy(c => c.Level).ThenBy(c => c.Code);
                viewModel.Semester = viewModel.Courses.FirstOrDefault().Semester;

                RetainDropdownState(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred!" + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        //[HttpPost]
        //public ActionResult AddExtraCourse(StudentCourseRegistrationViewModel viewModel)
        //{
        //    try
        //    {
        //        if (viewModel != null)
        //        {
        //            StudentLogic studentLogic = new StudentLogic();
        //            CourseLogic courseLogic = new CourseLogic();
        //            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

        //            List<Model.Model.Student> students =
        //                studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber);
        //            if (students.Count != 1)
        //            {
        //                SetMessage("Duplicate Matric Number OR Matric Number does not exist!", Message.Category.Error);
        //                RetainDropdownState(viewModel);
        //                return View(viewModel);
        //            }

        //            Model.Model.Student student = students.FirstOrDefault();
        //            List<StudentLevel> studentLevels =
        //                studentLevelLogic.GetModelsBy(
        //                    sl =>
        //                        sl.Person_Id == student.Id && sl.Department_Id == viewModel.Department.Id &&
        //                        sl.Programme_Id == viewModel.Programme.Id);
        //            if (studentLevels.Count <= 0)
        //            {
        //                SetMessage("Student is not in this Programme, Department!", Message.Category.Error);
        //                RetainDropdownState(viewModel);
        //                return View(viewModel);
        //            }

        //            viewModel.Courses = courseLogic.GetModelsBy(c => c.Department_Id == viewModel.Department.Id && c.Programme_Id == viewModel.Programme.Id && c.Activated == true);
        //            viewModel.Courses.OrderBy(c => c.Level).ThenBy(c => c.Semester.Id).ThenBy(c => c.Name);

        //            RetainDropdownState(viewModel);
        //            return View(viewModel);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
        //    }

        //    RetainDropdownState(viewModel);
        //    return View(viewModel);
        //}

        [Authorize(Roles = "lloydant,Admin,School Admin")]
        public ActionResult SaveAddedCourse(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLogic studentLogic = new StudentLogic();
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();

                    CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                    CourseMode carryOverCourseMode = new CourseMode() { Id = 2 };
                    CourseMode firstAttemprCourseMode = new CourseMode() { Id = 1 };

                    List<StudentLevel> studentLevelList = new List<StudentLevel>();
                    List<CourseRegistration> courseRegistrationList = new List<CourseRegistration>();

                    List<Model.Model.Student> students = studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber);
                    if (students.Count != 1)
                    {
                        SetMessage("Duplicate Matric Number OR Matric Number does not exist!", Message.Category.Error);
                        RetainDropdownState(viewModel);
                        return View("AddExtraCourse", viewModel);
                    }

                    Model.Model.Student student = students.FirstOrDefault();
                    StudentLevel studentLevel = studentLevelLogic.GetBy(student.Id);
                    if (studentLevel == null)
                    {
                        SetMessage("Student has not been registered in this level for this session!",
                            Message.Category.Error);
                        RetainDropdownState(viewModel);
                        return View("AddExtraCourse", viewModel);
                    }

                    //int sessionStart = GetSessionStartYear(viewModel.Session);

                    //if (viewModel.Session.Id != 13 && (viewModel.Session.Id != 7) && (viewModel.Session.Id != 1))
                    //if (sessionStart < 2015)
                    //{
                    //    SetMessage("Session Registration has been closed. Please register the course as a carry over for the student!",
                    //        Message.Category.Error);
                    //    RetainDropdownState(viewModel);
                    //    return View("AddExtraCourse", viewModel);
                    //}

                    courseRegistrationList =
                        courseRegistrationLogic.GetModelsBy(
                            p =>
                                p.Person_Id == studentLevel.Student.Id &&
                                p.Programme_Id == studentLevel.Programme.Id &&
                                p.Department_Id == studentLevel.Department.Id &&
                                p.Session_Id == viewModel.Session.Id);

                    if (courseRegistrationList.Count() != 1)
                    {
                        SetMessage("Student has not registered course for this session!", Message.Category.Error);
                        RetainDropdownState(viewModel);
                        return View("AddExtraCourse", viewModel);
                    }

                    string operation = "INSERT";
                    string action = "REGISTRATION :REGISTER ALL - ADMIN";
                    string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                    var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                    courseRegistrationDetailAudit.Action = action;
                    courseRegistrationDetailAudit.Operation = operation;
                    courseRegistrationDetailAudit.Client = client;
                    UserLogic loggeduser = new UserLogic();
                    courseRegistrationDetailAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);

                    CourseRegistration courseRegistration = courseRegistrationList.FirstOrDefault();
                    var checkedCourses = viewModel.Courses.Where(x => x.IsRegistered || x.isCarryOverCourse).ToList();
                    for (int i = 0; i < checkedCourses.Count; i++)
                    {
                        long courseId = checkedCourses[i].Id;
                        CourseRegistrationDetail courseRegistrationDetailCheck =
                            courseRegistrationDetailLogic.GetModelsBy(
                                crd =>
                                    crd.Course_Id == courseId &&
                                    crd.Student_Course_Registration_Id == courseRegistration.Id).LastOrDefault();
                        int maxUnit = 24;
                        int maxAnnualUnit = 54;
                        int maxSemesterUnit = 30;
                        int UnitCount = courseRegistrationDetailLogic.GetModelsBy(crd => crd.Student_Course_Registration_Id == courseRegistration.Id && crd.Semester_Id == viewModel.Semester.Id).Sum(a => a.CourseUnit) ?? 0;
                        if (studentLogic.IsStudentInFinalYear(studentLevel.Department, studentLevel.Level, studentLevel.Programme))
                        {
                            int firstSemester = courseRegistrationDetailLogic.GetModelsBy(crd => crd.Student_Course_Registration_Id == courseRegistration.Id && crd.Semester_Id == 1).Sum(a => a.CourseUnit) ?? 0;
                            int secondSemester = courseRegistrationDetailLogic.GetModelsBy(crd => crd.Student_Course_Registration_Id == courseRegistration.Id && crd.Semester_Id == 2).Sum(a => a.CourseUnit) ?? 0;
                            int TotalUnitsRegistered = firstSemester + secondSemester;
                            if (TotalUnitsRegistered < maxAnnualUnit)
                            {
                                int outstandingUnits = maxAnnualUnit - TotalUnitsRegistered;
                                if (viewModel.Semester.Id == 1)
                                {
                                    maxUnit = maxSemesterUnit;
                                }
                                else
                                {
                                    maxUnit = TotalUnitsRegistered + outstandingUnits;
                                }
                            }
                        }

                        if (UnitCount < maxUnit)
                        {
                            if (courseRegistrationDetailCheck == null)
                            {
                                if (checkedCourses[i].IsRegistered || checkedCourses[i].isCarryOverCourse)
                                {
                                    courseRegistrationDetail.CourseRegistration = courseRegistration;
                                    courseRegistrationDetail.Course = checkedCourses[i];
                                    if (checkedCourses[i].isCarryOverCourse)
                                    {
                                        courseRegistrationDetail.Mode = carryOverCourseMode;
                                    }
                                    else
                                    {
                                        courseRegistrationDetail.Mode = firstAttemprCourseMode;
                                    }

                                    courseRegistrationDetail.Semester = viewModel.Semester;
                                    courseRegistrationDetail.CourseUnit = checkedCourses[i].Unit;

                                    courseRegistrationDetail = courseRegistrationDetailLogic.Create(courseRegistrationDetail, courseRegistrationDetailAudit);

                                }
                            }

                            SetMessage("Operation Successful!", Message.Category.Information);
                        }
                        else
                        {

                            SetMessage("Already Exceeded Maximum Unit!", Message.Category.Information);
                        }



                    }
                    RetainDropdownState(viewModel);
                    return RedirectToAction("StudentDetails");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }
            RetainDropdownState(viewModel);
            return View("StudentDetails", viewModel);
        }

        private int GetSessionStartYear(Session session)
        {
            try
            {
                int sessionStart = 0;

                SessionLogic sessionLogic = new SessionLogic();

                Model.Model.Session selectedSession = sessionLogic.GetModelBy(s => s.Session_Id == session.Id);
                if (selectedSession != null)
                {
                    return Convert.ToInt32(selectedSession.Name.Split('/')[0]);
                }

                return sessionStart;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult GetDepartments(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Programme programme = new Programme() { Id = Convert.ToInt32(id) };
                DepartmentLogic departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                return Json(new SelectList(departments, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetSemester(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                Session session = new Session() { Id = Convert.ToInt32(id) };
                SemesterLogic semesterLogic = new SemesterLogic();
                List<SessionSemester> sessionSemesterList = new List<SessionSemester>();
                SessionSemesterLogic sessionSemesterLogic = new SessionSemesterLogic();
                sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == session.Id);

                List<Semester> semesters = new List<Semester>();
                foreach (SessionSemester sessionSemester in sessionSemesterList)
                {
                    semesters.Add(sessionSemester.Semester);
                }

                return Json(new SelectList(semesters, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetCourses(int[] ids)
        {
            try
            {
                if (ids.Count() == 0)
                {
                    return null;
                }
                Level level = new Level() { Id = Convert.ToInt32(ids[1]) };
                Department department = new Department() { Id = Convert.ToInt32(ids[1]) };
                Semester semester = new Semester() { Id = Convert.ToInt32(ids[1]) };
                Programme programme = new Programme() { Id = Convert.ToInt32(ids[1]) };
                List<Course> courseList = Utility.GetCoursesByLevelDepartmentAndSemester(level, department, semester, programme);

                return Json(new SelectList(courseList, ID, NAME), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult RegisterAll()
        {
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return View(viewModel);
            }

        }

        [HttpPost]
        public ActionResult RegisterAll(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                CourseLogic courseLogic = new CourseLogic();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();

                viewModel.CourseRegistrations = courseRegistrationLogic.GetUnregisteredStudents(viewModel.Session,
                    viewModel.Programme, viewModel.Department, viewModel.Level);
                TempData["viewModel"] = viewModel;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }

            RetainDropdownState(viewModel);
            return View(viewModel);
        }

        public ActionResult SaveAllRegisteredStudents()
        {
            StudentCourseRegistrationViewModel viewModel = (StudentCourseRegistrationViewModel)TempData["viewModel"];
            try
            {
                CourseLogic courseLogic = new CourseLogic();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();

                List<CourseRegistration> courseRegistrations = viewModel.CourseRegistrations;

                if (courseRegistrations != null && courseRegistrations.Count > 0)
                {
                    List<Course> SemesterCourses = courseLogic.GetBy(viewModel.Department, viewModel.Level,
                        viewModel.Semester, viewModel.Programme);

                    List<CourseRegistrationDetail> courseRegistrationDetails = new List<CourseRegistrationDetail>();

                    foreach (Course SemesterCourse in SemesterCourses)
                    {
                        CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                        courseRegistrationDetail.Course = SemesterCourse;
                        courseRegistrationDetail.CourseUnit = SemesterCourse.Unit;
                        courseRegistrationDetail.Mode = new CourseMode() { Id = 1 };
                        courseRegistrationDetail.Semester = viewModel.Semester;
                        courseRegistrationDetails.Add(courseRegistrationDetail);
                    }

                    string operation = "INSERT";
                    string action = "REGISTRATION :REGISTER ALL - ADMIN";
                    string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                    var courseRegistrationDetailAudit = new CourseRegistrationDetailAudit();
                    courseRegistrationDetailAudit.Action = action;
                    courseRegistrationDetailAudit.Operation = operation;
                    courseRegistrationDetailAudit.Client = client;
                    UserLogic loggeduser = new UserLogic();
                    courseRegistrationDetailAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);

                    if (courseRegistrationDetails.Count > 0)
                    {
                        foreach (CourseRegistration courseRegistration in courseRegistrations)
                        {
                            CourseRegistration registeredCourse = new CourseRegistration();
                            registeredCourse = courseRegistrationLogic.GetBy(courseRegistration.Student,
                                courseRegistration.Level, courseRegistration.Programme, courseRegistration.Department,
                                courseRegistration.Session);
                            if (registeredCourse == null)
                            {
                                courseRegistration.Details = courseRegistrationDetails;
                                //registeredCourse = courseRegistrationLogic.Create(courseRegistration);
                                registeredCourse = courseRegistrationLogic.Create(courseRegistration, courseRegistrationDetailAudit);

                            }
                        }
                    }

                    SetMessage("Operation Succesful!", Message.Category.Information);
                }

                RetainDropdownState(viewModel);
                return RedirectToAction("RegisterAll");
            }
            catch (Exception ex)
            {
                RetainDropdownState(viewModel);
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
                return RedirectToAction("RegisterAll");
            }

        }

        public ActionResult StudentsToRegister()
        {
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult StudentsToRegister(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    List<StudentLevel> studentLevelList = new List<StudentLevel>();

                    List<CourseRegistration> courseRegistrations =
                        courseRegistrationLogic.GetModelsBy(
                            s =>
                                s.Session_Id == viewModel.Session.Id && s.Department_Id == viewModel.Department.Id &&
                                s.Programme_Id == viewModel.Programme.Id && s.Level_Id == viewModel.Level.Id);
                    List<StudentLevel> studentLevels =
                        studentLevelLogic.GetModelsBy(
                            s =>
                                s.Session_Id == viewModel.Session.Id && s.Department_Id == viewModel.Department.Id &&
                                s.Programme_Id == viewModel.Programme.Id && s.Level_Id == viewModel.Level.Id);

                    List<long> courseRegPersonIdList = courseRegistrations.Select(c => c.Student.Id).ToList();

                    for (int i = 0; i < studentLevels.Count; i++)
                    {
                        if (!courseRegPersonIdList.Contains(studentLevels[i].Student.Id))
                        {
                            string matricNumber = studentLevels[i].Student.MatricNumber;
                            string session = studentLevels[i].Session.Name;
                            string[] splitRegNumber = matricNumber.Split('/');
                            string matricYearNumber = splitRegNumber[2];
                            string[] sessionSplit = session.Split('/');
                            string sessionYear = sessionSplit[0].Substring(2, 2);
                            string prevSessionYear = (Convert.ToInt32(sessionYear) - 1).ToString();
                            string[] matricNumberYearsToPull = { sessionYear, prevSessionYear };
                            if (matricNumberYearsToPull.Contains(matricYearNumber))
                            {
                                studentLevelList.Add(studentLevels[i]);
                            }

                            //studentLevelList.Add(studentLevels[i]);
                        }
                    }

                    viewModel.StudentLevelList = studentLevelList;
                    TempData["viewModel"] = viewModel;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }

            RetainDropdownState(viewModel);
            return View(viewModel);
        }
        public ActionResult GetPayments(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                StudentCourseRegistrationViewModel viewModel =
                    (StudentCourseRegistrationViewModel)TempData["viewModel"];
                TempData.Keep("viewModel");
                long personId = Convert.ToInt64(id);

                PaymentLogic paymentLogic = new PaymentLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                List<Payment> paymentList = new List<Payment>();

                List<Payment> payments = paymentLogic.GetModelsBy(p => p.Person_Id == personId);

                for (int i = 0; i < payments.Count; i++)
                {
                    Payment currentPayment = payments[i];
                    PaymentEtranzact paymentEtranzact =
                        paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == currentPayment.Id);
                    RemitaPayment remitaPayment = remitaPaymentLogic.GetModelBy(r => r.Payment_Id == currentPayment.Id);
                    if (paymentEtranzact != null)
                    {
                        paymentList.Add(currentPayment);
                    }
                    if (remitaPayment != null)
                    {
                        paymentList.Add(currentPayment);
                    }
                }

                viewModel.Payments = paymentList;
                return PartialView("_StudentPayment", viewModel);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult RegisteStudent(string id)
        {
            StudentCourseRegistrationViewModel viewModel = (StudentCourseRegistrationViewModel)TempData["viewModel"];
            TempData.Keep("viewModel");
            try
            {
                long personId = Convert.ToInt64(id);
                Model.Model.Student student = new Model.Model.Student() { Id = personId };

                CourseLogic courseLogic = new CourseLogic();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();

                Semester firstSemester = new Semester() { Id = 1 };
                Semester secondSemester = new Semester() { Id = 2 };

                List<Course> firstSemesterCourses = courseLogic.GetBy(viewModel.Department, viewModel.Level,
                    firstSemester, viewModel.Programme);
                List<Course> secondSemesterCourses = courseLogic.GetBy(viewModel.Department, viewModel.Level,
                    secondSemester, viewModel.Programme);

                List<CourseRegistrationDetail> courseRegistrationDetails = new List<CourseRegistrationDetail>();

                foreach (Course SemesterCourse in firstSemesterCourses)
                {
                    CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                    courseRegistrationDetail.Course = SemesterCourse;
                    courseRegistrationDetail.CourseUnit = SemesterCourse.Unit;
                    courseRegistrationDetail.Mode = new CourseMode() { Id = 1 };
                    courseRegistrationDetail.Semester = firstSemester;
                    courseRegistrationDetails.Add(courseRegistrationDetail);
                }

                foreach (Course SemesterCourse in secondSemesterCourses)
                {
                    CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
                    courseRegistrationDetail.Course = SemesterCourse;
                    courseRegistrationDetail.CourseUnit = SemesterCourse.Unit;
                    courseRegistrationDetail.Mode = new CourseMode() { Id = 1 };
                    courseRegistrationDetail.Semester = secondSemester;
                    courseRegistrationDetails.Add(courseRegistrationDetail);
                }


                if (courseRegistrationDetails.Count > 0)
                {
                    CourseRegistration registeredCourse = new CourseRegistration();
                    registeredCourse = courseRegistrationLogic.GetBy(student, viewModel.Level, viewModel.Programme,
                        viewModel.Department, viewModel.Session);
                    if (registeredCourse == null)
                    {
                        registeredCourse = new CourseRegistration();
                        registeredCourse.Student = student;
                        registeredCourse.Department = viewModel.Department;
                        registeredCourse.Details = courseRegistrationDetails;
                        registeredCourse.Level = viewModel.Level;
                        registeredCourse.Programme = viewModel.Programme;
                        registeredCourse.Session = viewModel.Session;
                        courseRegistrationLogic.Create(registeredCourse);
                    }
                }

                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult StudentDetails()
        {
            return View();
        }
        [HttpPost]
        public ActionResult StudentDetails(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                StudentLogic studentLogic = new StudentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                List<Model.Model.Student> students = studentLogic.GetModelsBy(x => x.Matric_Number == viewModel.Student.MatricNumber);
                Model.Model.Student myStudent = new Model.Model.Student();

                var loggeduser = new UserLogic();
                viewModel.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                
                if (students.Count > 1)
                {
                    SetMessage("Matric Number is duplicate", Message.Category.Error);
                    return View();
                }
                if (students.Count == 0)
                {
                    Model.Model.Student appliedStudent = studentLogic.GetModelsBy(s => s.APPLICATION_FORM.Application_Form_Number == viewModel.Student.MatricNumber).LastOrDefault();
                    if (appliedStudent == null)
                    {
                        SetMessage("No record found", Message.Category.Error);
                        return View();
                    }
                    else
                    {
                        myStudent = appliedStudent;
                    }
                }

                if (students.Count == 1)
                {
                    myStudent = students.FirstOrDefault();

                }
                var departmentLogic = new DepartmentLogic();
                var user = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                if (user != null && user.Id > 0)
                {
                    if (user.Role.Name != "lloydant" && user.Role.Name != "Admin" && user.Role.Name != "School Admin" && user.Role.Name != "ICT Staff" && user.Role.Name != "ViceChancellor")
                    {
                        //ViceChancellor
                        StaffLogic staffLogic = new StaffLogic();
                        Staff staff = new Staff();
                        staff = staffLogic.GetBy(user.Id);
                        if ((bool)staff.isManagement)
                        {
                            var faculty = staff.Department.Faculty.Id;
                            var stdDept = GetStudentDept(myStudent.Id);
                            if (faculty != stdDept.Faculty.Id)
                            {
                                SetMessage("You are not allowed to View this student's Detail", Message.Category.Error);
                                return View();
                            }
                        }
                        else if ((bool)staff.isHead)
                        {
                            var department = staff.Department.Id;
                            var stdDept = GetStudentDept(myStudent.Id);
                            if (department != stdDept.Id)
                            {
                                SetMessage("You are not allowed to View this student's Detail", Message.Category.Error);
                                return View();
                            }
                        }
                    }
                }

                List<CourseRegistration> courseRegistrationlist = courseRegistrationLogic.GetModelsBy(x => x.Person_Id == myStudent.Id);

                for (int i = 0; i < courseRegistrationlist.Count; i++)
                {
                    long courseRegId = courseRegistrationlist[i].Id;
                    List<CourseRegistrationDetail> courseRegistrationDetaillist = courseRegistrationDetailLogic.GetModelsBy(x => x.Student_Course_Registration_Id == courseRegId);
                    if (courseRegistrationDetaillist.Count != 0)
                    {
                        courseRegistrationlist[i].Details = courseRegistrationDetaillist.OrderBy(c => c.Semester.Id).ToList();

                    }
                    else
                    {
                        //var courseRegistrationToDelete = courseRegistrationlist[i];
                        //courseRegistrationlist.Remove(courseRegistrationlist[i]);
                        //courseRegistrationLogic.Delete(p => p.Student_Course_Registration_Id == courseRegistrationToDelete.Id);

                    }
                }
                //remove course registration without coursedetail
                courseRegistrationlist.RemoveAll(item => item.Details == null);
                List<Payment> paymentList = paymentLogic.GetModelsBy(x => x.Person_Id == myStudent.Id);
                List<Payment> ConfirmedPayments = new List<Payment>();

                for (int i = 0; i < paymentList.Count; i++)
                {
                    long PaymentId = paymentList[i].Id;
                    Payment payment = paymentLogic.GetModelBy(p => p.Payment_Id == PaymentId);

                    PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelBy(x => x.Payment_Id == PaymentId);
                    if (paymentEtranzact != null)
                    {
                        payment.ConfirmationNumber = paymentEtranzact.ConfirmationNo;
                        payment.Amount = paymentEtranzact.TransactionAmount.ToString();
                        if (paymentEtranzact.TransactionDate != null)
                        {
                            payment.DatePaid = paymentEtranzact.TransactionDate.Value;
                        }
                        ConfirmedPayments.Add(payment);
                    }

                }

                viewModel.StudentLevelList = studentLevelLogic.GetModelsBy(s => s.STUDENT.Person_Id == myStudent.Id);
                if (viewModel.StudentLevelList.Count <= 0)
                {
                    StudentLevel studentLevel = new StudentLevel();
                    studentLevel.Student = myStudent;
                    studentLevel.Session = new Session() { Id = 7 };
                    studentLevel.Level = new Level() { Id = 1 };
                    studentLevel.Programme = new Programme() { Id = 1 };
                    studentLevel.Department = new Department() { Id = 1 };
                    studentLevelLogic.Create(studentLevel);
                    viewModel.StudentLevelList = studentLevelLogic.GetModelsBy(s => s.STUDENT.Person_Id == myStudent.Id);

                }

                viewModel.Student = myStudent;
                viewModel.CourseRegistrations = courseRegistrationlist;
                viewModel.Payments = ConfirmedPayments;

                return View(viewModel);
            }

            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("StudentDetails");
        }
        public JsonResult DeleteStudentLevel(string id)
        {
            try
            {
                long studentLevelId = Convert.ToInt64(id);
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                var studentLevel = studentLevelLogic.GetModelBy(a => a.Student_Level_Id == studentLevelId);
                if (studentLevel != null)
                {
                    var studentLevels =
                        studentLevelLogic.GetModelsBy(
                            a => a.Person_Id == studentLevel.Student.Id && a.Session_Id == studentLevel.Session.Id);
                    if (studentLevels != null && studentLevels.Count > 1)
                    {
                        studentLevelLogic.Delete(s => s.Student_Level_Id == studentLevelId);
                        return Json(new { result = "Success" });
                    }

                }
                return Json(new { result = "Failed! Student Level can't be deleted" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult RemoveCourse(string id)
        {
            try
            {
                string postBackResult = "";
                long courseRegdetailId = Convert.ToInt64(id);

                UserLogic userLogic = new UserLogic();
                CourseRegistrationDetailAuditLogic courseRegistrationDetailAuditLogic = new CourseRegistrationDetailAuditLogic();

                CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                CourseRegistrationDetail RegDetailCheck = courseRegistrationDetailLogic.GetModelBy(cr => cr.Student_Course_Registration_Detail_Id == courseRegdetailId);
                //if ((RegDetailCheck.TestScore == null && RegDetailCheck.ExamScore == null) ||
                //    (RegDetailCheck.TestScore + RegDetailCheck.ExamScore <= 0))
                //{
                //if ((RegDetailCheck.TestScore == null && RegDetailCheck.ExamScore == null) ||
                //    (RegDetailCheck.TestScore + RegDetailCheck.ExamScore <= 0) || ) 
                //{
                using (TransactionScope scope = new TransactionScope())
                {
                    CourseRegistrationDetailAudit courseRegistrationDetailAudit =
                        new CourseRegistrationDetailAudit();
                    string operation = "REMOVED COURSE REGISTRATION";
                    string action = "ADMIN :CHANGES FROM ADMIN CONSOLE (StudentCourseRegistrationController)";
                    string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress +
                                    ")";
                    courseRegistrationDetailAudit.Action = action;
                    courseRegistrationDetailAudit.Operation = operation;
                    courseRegistrationDetailAudit.Client = client;
                    courseRegistrationDetailAudit.User =
                        userLogic.GetModelsBy(u => u.User_Name == User.Identity.Name).LastOrDefault();
                    courseRegistrationDetailAudit.Time = DateTime.Now;

                    courseRegistrationDetailAudit.Course = RegDetailCheck.Course;
                    courseRegistrationDetailAudit.CourseUnit = RegDetailCheck.CourseUnit;
                    courseRegistrationDetailAudit.Mode = RegDetailCheck.Mode;
                    courseRegistrationDetailAudit.Semester = RegDetailCheck.Semester;
                    courseRegistrationDetailAudit.TestScore = RegDetailCheck.TestScore;
                    courseRegistrationDetailAudit.ExamScore = RegDetailCheck.ExamScore;
                    courseRegistrationDetailAudit.SpecialCase = RegDetailCheck.SpecialCase;
                    courseRegistrationDetailAudit.CourseRegistrationDetail = RegDetailCheck;
                    courseRegistrationDetailAudit.Student = RegDetailCheck.CourseRegistration.Student;

                    courseRegistrationDetailAuditLogic.Create(courseRegistrationDetailAudit);

                    courseRegistrationDetailLogic.Delete(
                        s => s.Student_Course_Registration_Detail_Id == courseRegdetailId);

                    scope.Complete();
                }
                return Json(new { success = true, responseText = "Course was removed!" },
                    JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    List<CourseRegistrationDetail> regDetailChecks =
                //        courseRegistrationDetailLogic.GetModelsBy(cr => cr.Course_Id == RegDetailCheck.Course.Id &&
                //                                                        cr.Student_Course_Registration_Id ==
                //                                                        RegDetailCheck.CourseRegistration.Id);
                //    if (regDetailChecks.Count > 1)
                //    {
                //        var firstCourseReg = regDetailChecks.FirstOrDefault();
                //        if (firstCourseReg != null)
                //        {
                //            using (TransactionScope scope = new TransactionScope())
                //            {
                //                CourseRegistrationDetailAudit courseRegistrationDetailAudit =
                //                    new CourseRegistrationDetailAudit();
                //                string operation = "REMOVED COURSE REGISTRATION";
                //                string action =
                //                    "ADMIN :CHANGES FROM ADMIN CONSOLE (StudentCourseRegistrationController)";
                //                string client = Request.LogonUserIdentity.Name + " (" +
                //                                HttpContext.Request.UserHostAddress + ")";
                //                courseRegistrationDetailAudit.Action = action;
                //                courseRegistrationDetailAudit.Operation = operation;
                //                courseRegistrationDetailAudit.Client = client;
                //                courseRegistrationDetailAudit.User =
                //                    userLogic.GetModelsBy(u => u.User_Name == User.Identity.Name).LastOrDefault();
                //                courseRegistrationDetailAudit.Time = DateTime.Now;

                //                courseRegistrationDetailAudit.Course = firstCourseReg.Course;
                //                courseRegistrationDetailAudit.CourseUnit = firstCourseReg.CourseUnit;
                //                courseRegistrationDetailAudit.Mode = firstCourseReg.Mode;
                //                courseRegistrationDetailAudit.Semester = firstCourseReg.Semester;
                //                courseRegistrationDetailAudit.TestScore = firstCourseReg.TestScore;
                //                courseRegistrationDetailAudit.ExamScore = firstCourseReg.ExamScore;
                //                courseRegistrationDetailAudit.SpecialCase = firstCourseReg.SpecialCase;
                //                courseRegistrationDetailAudit.CourseRegistrationDetail = firstCourseReg;
                //                courseRegistrationDetailAudit.Student = firstCourseReg.CourseRegistration.Student;

                //                courseRegistrationDetailAuditLogic.Create(courseRegistrationDetailAudit);

                //                courseRegistrationDetailLogic.Delete(
                //                    s => s.Student_Course_Registration_Detail_Id == firstCourseReg.Id);

                //                scope.Complete();
                //            }
                //            return Json(new {success = true, responseText = "Course was removed!"},
                //                JsonRequestBehavior.AllowGet);
                //        }

                //    }
                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return Json(new { success = false, responseText = "Operation was not successful." }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditStudentLevel(int sid)
        {
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                if (sid > 0)
                {
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLevel studentLevel = studentLevelLogic.GetModelBy(sl => sl.Student_Level_Id == sid);

                    viewModel.StudentLevel = studentLevel;
                    ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME, studentLevel.Level.Id);
                    ViewBag.Session = viewModel.SessionSelectList;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditStudentLevel(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentLevel != null)
                {
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLevel studentLevel = studentLevelLogic.GetModelBy(sl => sl.Student_Level_Id == viewModel.StudentLevel.Id);

                    studentLevel.Session = viewModel.StudentLevel.Session;
                    studentLevel.Level = viewModel.StudentLevel.Level;

                    studentLevelLogic.Modify(studentLevel);

                    SetMessage("Operation Successful! ", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("StudentDetails");
        }
        public ActionResult ViewConfirmedpayments()
        {
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {

            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ViewConfirmedpayments(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.Student.MatricNumber != null)
                {
                    StudentLogic studentLogic = new StudentLogic();
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

                    Model.Model.Student student = studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber).LastOrDefault();
                    if (student == null)
                    {
                        SetMessage("Error! Student does not exist", Message.Category.Error);
                    }

                    viewModel.PaymentEtranzacts = paymentEtranzactLogic.GetModelsBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == student.Id);

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        public ActionResult EditConfirmedPayment(int pid)
        {
            StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
            try
            {
                if (pid > 0)
                {
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    viewModel.PaymentEtranzact = paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == pid);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult EditConfirmedPayment(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.PaymentEtranzact != null)
                {
                    string operation = "UPDATE";
                    string action = "UPDATE PIN";
                    string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress +
                                    ")";

                    var paymentEtranzactAudit = new PaymentEtranzactAudit();
                    var loggeduser = new UserLogic();
                    paymentEtranzactAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                    paymentEtranzactAudit.Operation = operation;
                    paymentEtranzactAudit.Action = action;
                    paymentEtranzactAudit.Time = DateTime.Now;
                    paymentEtranzactAudit.Client = client;

                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    viewModel.PaymentEtranzact = paymentEtranzactLogic.GetModelBy(p => p.ONLINE_PAYMENT.PAYMENT.Payment_Id == viewModel.PaymentEtranzact.Payment.Payment.Id);
                    viewModel.PaymentEtranzact.TransactionAmount = Convert.ToDecimal(viewModel.Amount);

                    paymentEtranzactLogic.Modify(viewModel.PaymentEtranzact, paymentEtranzactAudit);
                    SetMessage("Operation Successful! ", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ViewConfirmedpayments");
        }
        public Department GetStudentDept(long sId)
        {
            Department dept;
            try
            {

                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                var studentLevel = studentLevelLogic.GetModelBy(x => x.Person_Id == sId);
                dept = studentLevel.Department;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dept;
        }

    }
}