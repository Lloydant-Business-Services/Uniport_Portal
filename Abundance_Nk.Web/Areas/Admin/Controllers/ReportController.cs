using System;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using System.Web.Mvc;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using System.Collections.Generic;
using Abundance_Nk.Web.Models;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using System.Linq;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class ReportController :Controller
    {
        public const string ID = "Id";
        public const string NAME = "Name";
        public ReportViewModel ReportViewModel;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApplicationFormSummary()
        {
            return PartialView();
        }

        //
        // GET: /Admin/Report/
        public ActionResult ApplicationSummary()
        {
            return PartialView();
        }

        public ActionResult ListOfApplications()
        {
            return View();
        }

        public ActionResult PhotoCard()
        {
            return View();
        }

        public ActionResult AdmissionProcessing()
        {
            var appliedCourseLogic = new AppliedCourseLogic();
            var admissionCriteriaLogic = new AdmissionCriteriaLogic();

            AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(m => m.Person_Id == 152);

            string rejectReason = admissionCriteriaLogic.EvaluateApplication(appliedCourse);
            ViewBag.RejectReason = rejectReason;
            return View();
        }

        public ActionResult AcceptanceReport()
        {
            return View();
        }

        public ActionResult MatriculationReport()
        {
            return View();
        }

        public ActionResult BiodatatReport()
        {
            return View();
        }
        
        public ActionResult SchoolFeesPayment()
        {
            return View();
        }

        public ActionResult StudentCourseRegistration()
        {
            return View();
        }

        public ActionResult FeesPaymentReport()
        {
            return View();
        }

        public ActionResult ExamRawScoreSheetReport()
        {
            return View();
        }

        public ActionResult CourseRegistrationReport()
        {
            return View();
        }

        public ActionResult Transcript()
        {
            return View();
        }

        public ActionResult StatementOfResult()
        {
            return View();
        }

        public ActionResult AttendanceReportBulk()
        {
            return View();
        }

         public ActionResult AttendanceSlugReportBulk()
        {
            return View();
        }

        public ActionResult NotificationOfResult(long personId,int semesterId,int sessionId,int programmeId,
            int departmentId,int levelId)
        {
            ViewBag.personId = personId;
            ViewBag.semesterId = semesterId;
            ViewBag.sessionId = sessionId;
            ViewBag.programmeId = programmeId;
            ViewBag.departmentId = departmentId;
            ViewBag.semesterId = semesterId;
            ViewBag.levelId = levelId;
            return View();
        }

        public ActionResult NotificationOfResultBulk()
        {
            return View();
        }

        public ActionResult StatementOfResultBulk()
        {
            return View();
        }

        public ActionResult MasterGradeSheet()
        {
            return View();
        }

        public ActionResult ResultSheet()
        {
            ReportViewModel = new ReportViewModel();
            if(TempData["ReportViewModel"] != null)
            {
                ReportViewModel = (ReportViewModel)TempData["ReportViewModel"];
                ViewBag.levelId = ReportViewModel.Level.Id;
                ViewBag.semesterId = ReportViewModel.Semester.Id;
                ViewBag.progId = ReportViewModel.Programme.Id;
                ViewBag.deptId = ReportViewModel.Department.Id;
                ViewBag.sessionId = ReportViewModel.Session.Id;
                ViewBag.courseId = ReportViewModel.Course.Id;
            }

            return View();
        }

        public ActionResult UnregisteredStudentResultSheet(string levelId,string semesterId,string progId,
            string deptId,string sessionId,string courseId)
        {
            ViewBag.levelId = levelId;
            ViewBag.semesterId = semesterId;
            ViewBag.progId = progId;
            ViewBag.deptId = deptId;
            ViewBag.sessionId = sessionId;
            ViewBag.courseId = courseId;
            return View();
        }

        public ActionResult ApplicantResult()
        {
            return View();
        }

        public ActionResult ApplicantsByChoice()
        {
            return View();
        }

        public ActionResult PaymentReportGeneral()
        {
            return View();
        }

        public ActionResult CompositeCourseRegistrationReport()
        {
            return View();
        }

        public ActionResult GSTReport()
        {
            return View();
        }

        public ActionResult SupplementaryApplicantReport()
        {
            return View();
        }
        public ActionResult AcceptanceReportCount()
        {
            return View();
        }
        public ActionResult DebtorsReport()
        {
            return View();
        }
        public ActionResult DebtorsCountReport()
        {
            return View();
        }
        public ActionResult SchoolFeesPaymentReport()
        {
            return View();
        }
        public ActionResult AdmittedStudentReport()
        {
            return View();
        }
        public ActionResult HostelAllocationReport()
        {
            return View();
        }
        public ActionResult StudentRegisteredCourses()
        {
            try
            {
                StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
                Dropdown(viewModel);
                return View();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult CourseEvaluationReport(string downloadPath, string downloadName)
        {
            try
            {
                if (downloadPath != null && downloadName != null)
                {
                    return File(Server.MapPath(downloadPath), "application/zip", downloadName + ".zip");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }
        public ActionResult TranscriptReport()
        {
            try
            {
                
                TranscriptViewModel viewModel = new TranscriptViewModel();
                viewModel.StudentLevels = new List<StudentLevel>();
                KeepDropdownState(viewModel);
                return View(viewModel);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult TranscriptReport(TranscriptViewModel viewModel)
        {
            try
            {
                RetainDropdownState(viewModel);
                StudentLogic studentLogic = new StudentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                CourseRegistration studentCourseRegistration = new CourseRegistration();
                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                viewModel.CourseRegistrations = new List<CourseRegistration>();
                viewModel.StudentLevels = new List<StudentLevel>();
                List<CourseRegistration> studentsInLevels = new List<CourseRegistration>();
                if (viewModel.DepartmentOption?.Id > 0)
                {
                    studentsInLevels = courseRegistrationLogic.GetModelsBy(x => x.Department_Id == viewModel.Department.Id && x.Programme_Id == viewModel.Programme.Id && x.Session_Id == viewModel.Session.Id && x.Level_Id == viewModel.Level.Id && x.Department_Option_Id==viewModel.DepartmentOption.Id);
                }
                else
                {
                    studentsInLevels = courseRegistrationLogic.GetModelsBy(x => x.Department_Id == viewModel.Department.Id && x.Programme_Id == viewModel.Programme.Id && x.Session_Id == viewModel.Session.Id && x.Level_Id == viewModel.Level.Id);
                }
                
                if(studentsInLevels!=null && studentsInLevels.Count > 0)
                {

                    foreach(var studentsInLevel in studentsInLevels)
                    {
                        var studentLevel=studentLevelLogic.GetModelsBy(x => x.Person_Id == studentsInLevel.Student.Id).FirstOrDefault();
                        viewModel.StudentLevels.Add(studentLevel);
                    }
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ViewTranscript(string sid)
        {
            try
            {
                string studentId = Utility.Decrypt(sid);
                if (studentId != "1")
                {
                    ViewBag.StudentId = studentId;
                }
                else if (studentId == "1")
                {
                    ViewBag.StudentId = sid;
                }

            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }
        [NonAction]
        public void Dropdown(StudentCourseRegistrationViewModel viewModel)
        {
            try
            {
                //StudentCourseRegistrationViewModel viewModel = new StudentCourseRegistrationViewModel();
                ViewBag.Sessions = viewModel.SessionSelectList;
                ViewBag.Programmes = viewModel.ProgrammeSelectList;
                ViewBag.Semester = viewModel.SemesterList;
                ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public void RetainDropdownState(TranscriptViewModel viewModel)
        {
            try
            {
                SemesterLogic semesterLogic = new SemesterLogic();
                DepartmentLogic departmentLogic = new DepartmentLogic();
                DepartmentOptionLogic departmentOptionLogic = new DepartmentOptionLogic();
                LevelLogic levelLogic = new LevelLogic();
                if (viewModel != null)
                {
                    if (viewModel.Session != null)
                    {
                        ViewBag.Session = new SelectList(viewModel.SessionSelectList, "Value", "Text", viewModel.Session.Id);
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
                        ViewBag.Programme = new SelectList(viewModel.ProgrammeSelectList, "Value", "Text", viewModel.Programme.Id);
                    }
                    else
                    {
                        ViewBag.Programme = viewModel.ProgrammeSelectList;
                    }
                    if (viewModel.Department != null && viewModel.Programme != null)
                    {
                        ViewBag.Department = new SelectList(departmentLogic.GetBy(viewModel.Programme), "Id", NAME, viewModel.Department.Id);
                        if (viewModel.DepartmentOption != null)
                        {
                            ViewBag.DepartmentOption = new SelectList(departmentOptionLogic.GetBy(viewModel.Department,viewModel.Programme), "Id", NAME, viewModel.DepartmentOption.Id);
                        }
                        else
                        {
                            ViewBag.DepartmentOption = new SelectList(new List<DepartmentOption>(), ID, NAME);
                        }
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                    }
                    if (viewModel.Level != null)
                    {
                        ViewBag.Level = new SelectList(viewModel.LevelSelectList, "Value", "Text", viewModel.Level.Id);
                    }
                    else
                    {
                        ViewBag.Level = new SelectList(viewModel.LevelSelectList, "Value", "Text");
                    }
                   
                    //if (viewModel.ProgrammeCourseRegistrations != null && viewModel.Level != null && viewModel.Semester != null && viewModel.Department != null)
                    //{
                    //    Department department = new Department() { Id = viewModel.Department.Id };
                    //    List<Course> courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.Level, department, viewModel.Semester);
                    //    ViewBag.Course = new SelectList(courseList, ID, NAME, viewModel.Level.Id);
                    //}
                    //else
                    //{
                    //    ViewBag.Course = new SelectList(new List<Course>(), ID, NAME);
                    //}

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void KeepDropdownState(TranscriptViewModel viewModel)
        {
            try
            {
                if (viewModel.Session != null && viewModel.Programme != null && viewModel.Department != null &&
                viewModel.Level != null)
                {
                    ViewBag.Session = new SelectList(viewModel.SessionSelectList, "Value", "Text", viewModel.Session.Id);
                    ViewBag.Semester = new SelectList(new List<Semester>(), "Id", "Name");
                    ViewBag.Programme = new SelectList(viewModel.ProgrammeSelectList, "Value", "Text", viewModel.Programme.Id);
                    ViewBag.Department = new SelectList(viewModel.DepartmentSelectList, "Value", "Text", viewModel.Department.Id);
                    ViewBag.Level = new SelectList(viewModel.LevelSelectList, "Value", "Text", viewModel.Level.Id);
                }
                else
                {
                    ViewBag.Session = viewModel.SessionSelectList;
                    ViewBag.Semester = viewModel.SemesterSelectList;
                    ViewBag.Programme = viewModel.ProgrammeSelectList;
                    ViewBag.Department = new SelectList(new List<Department>(), "Id", "Name");
                    ViewBag.Level = viewModel.LevelSelectList;
                    ViewBag.DepartmentOption = new SelectList(new List<DepartmentOption>(), ID, NAME);
                }
                // ViewBag.ScoreGrade = viewModel.GradeSelectListItems;
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
                List<Department> departments = new List<Department>();
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }
                var loggeduser = new UserLogic();
                var programme = new Programme { Id = Convert.ToInt32(id) };
                var departmentLogic = new DepartmentLogic();
                var user = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                if (user != null && user.Id > 0)
                {
                    StaffLogic staffLogic = new StaffLogic();
                    Staff staff = new Staff();
                    staff = staffLogic.GetBy(user.Id);
                    if (staff != null && staff.Id > 0)
                    {
                        
                        
                        if ((bool)staff.isManagement)
                        {
                            departments = departmentLogic.GetModelsBy(l => l.Faculty_Id == staff.Department.Faculty.Id);
                        }
                        else
                        {
                            departments.Add(staff.Department);
                            //departments = departmentLogic.GetBy(programme);
                        }


                    }
                    else if(user?.Role?.Id==1 || user?.Role?.Id == 22)
                    {
                        ProgrammeDepartmentLogic programmeDepartmentLogic = new ProgrammeDepartmentLogic();
                        var department=programmeDepartmentLogic.GetModelsBy(f => f.Programme_Id == programme.Id).Select(f=>f.Department);
                        departments.AddRange(department);
                    }

                }

                return Json(new SelectList(departments, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ReturningStudentAndNewStudentCount()
        {
            return View();
        }
        public ActionResult ApplicantInformationReportBulk()
        {
            return View();
        }
        public JsonResult GetDepartmentOptions(string id, string departmentId)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(departmentId))
                {
                    return null;
                }

                var programme = new Programme { Id = Convert.ToInt32(id) };
                var department = new Department { Id = Convert.ToInt32(departmentId) };
                var departmentOptionLogic = new DepartmentOptionLogic();
                List<DepartmentOption> departmentOptions = departmentOptionLogic.GetBy(department, programme);

                return Json(new SelectList(departmentOptions, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult IndigenetateReport()
        {
            return View();
        }
        public ActionResult IndigenetateSummaryReport()
        {
            return View();
        }
        public ActionResult NominalReport()
        {
            return View();
        }
        public ActionResult NominalSummaryReport()
        {
            return View();
        }

    }
}