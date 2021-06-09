﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    public class ElearningController : BaseController
    {
        // GET: Student/Elearning
        private ELearningViewModel viewModel;
        private EChatTopicLogic eChatTopicLogic;
        private EChatResponseLogic eChatResponseLogic;
        private EContentTypeLogic eContentTypeLogic;
        private CourseAllocationLogic courseAllocationLogic;
        private string FileUploadURL;
        public ElearningController()
        {
            viewModel = new ELearningViewModel();
            eChatTopicLogic = new EChatTopicLogic();
            eChatResponseLogic = new EChatResponseLogic();
            eContentTypeLogic = new EContentTypeLogic();
            courseAllocationLogic = new CourseAllocationLogic();
        }
        public ActionResult Index()
        {
            viewModel = new ELearningViewModel();
            ViewBag.Session = viewModel.SessionSelectList;
            ViewBag.Semester = new SelectList(new List<Semester>(), Utility.ID, Utility.NAME);
            ViewBag.Level = new SelectList(viewModel.LevelList, Utility.ID, Utility.NAME);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(ELearningViewModel viewModel)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                Model.Model.Student student = studentLogic.GetBy(User.Identity.Name);

                var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                viewModel.ELearningViews = courseRegistrationDetailLogic.GetEcontentBy(viewModel.Level ,viewModel.Semester,viewModel.Session,student);
                
                if (viewModel.ELearningViews.Count <= 0)
                {
                    SetMessage("No Econtent Found for the selected parameter",Message.Category.Error);
                }
                KeepDropDownState(viewModel);
                TempData["vModel"] = viewModel;
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }

        public void KeepDropDownState(ELearningViewModel viewModel)
        {
            try
            {
                ViewBag.Session = viewModel.SessionSelectList;
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.Level = new SelectList(viewModel.LevelList, Utility.ID, Utility.NAME);
                ViewBag.User = viewModel.UserSelectList;
                if (viewModel.Semester != null)
                {
                    var sessionSemesterList = new List<SessionSemester>();
                    var sessionSemesterLogic = new SessionSemesterLogic();
                    sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == viewModel.Session.Id);

                    var semesters = new List<Semester>();
                    foreach (SessionSemester sessionSemester in sessionSemesterList)
                    {
                        semesters.Add(sessionSemester.Semester);
                    }

                    ViewBag.Semester = new SelectList(semesters, Utility.ID, Utility.NAME, viewModel.Semester.Id);
                }
                if (viewModel.Department != null && viewModel.Department.Id > 0)
                {
                    var departmentLogic = new DepartmentLogic();
                    var departments = new List<Department>();
                    departments = departmentLogic.GetBy(viewModel.Programme);

                    ViewBag.Department = new SelectList(departments, Utility.ID, Utility.NAME, viewModel.Department.Id);
                }
                if (viewModel.Course != null && viewModel.Course.Id > 0)
                {
                    var courseList = new List<Course>();
                    courseList = Utility.GetCoursesByLevelDepartmentAndSemester(viewModel.Level, viewModel.Department,
                        viewModel.Semester, viewModel.Programme);

                    ViewBag.Course = new SelectList(courseList, Utility.ID, Utility.NAME, viewModel.Course.Id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult Assignment()
        {
            viewModel = new ELearningViewModel();
            ViewBag.Session = viewModel.SessionSelectList;
            ViewBag.Semester = new SelectList(new List<Semester>(), Utility.ID, Utility.NAME);
            ViewBag.Level = new SelectList(viewModel.LevelList, Utility.ID, Utility.NAME);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Assignment(ELearningViewModel viewModel)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                Model.Model.Student student = studentLogic.GetBy(User.Identity.Name);

                var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                List<CourseRegistrationDetail> courseRegs = courseRegistrationDetailLogic.GetModelsBy(c => c.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id &&
                                                c.STUDENT_COURSE_REGISTRATION.Session_Id == viewModel.Session.Id && c.Semester_Id == viewModel.Semester.Id && c.STUDENT_COURSE_REGISTRATION.Level_Id == viewModel.Level.Id);

                if (courseRegs == null || courseRegs.Count <= 0)
                {
                    SetMessage("You have no course registration.", Message.Category.Error);
                    KeepDropDownState(viewModel);
                    return View(viewModel);
                }

                EAssignmentLogic assignmentLogic = new EAssignmentLogic();
                EAssignmentSubmissionLogic eAssignmentSubmissionLogic = new EAssignmentSubmissionLogic();

                viewModel.ELearningViews = new List<ELearningView>();

                for (int i = 0; i < courseRegs.Count; i++)
                {
                    CourseRegistrationDetail registrationDetail = courseRegs[i];
                    List<EAssignment> assignments = assignmentLogic.GetModelsBy(s => s.Course_Id == registrationDetail.Course.Id && s.IsDelete == false);
                    foreach (var assignment in assignments)
                    {

                        if (assignment != null)
                        {
                            var submittedAssignment = eAssignmentSubmissionLogic.GetModelsBy(f => f.Assignment_Id == assignment.Id && f.Student_Id == student.Id).FirstOrDefault();
                            ELearningView eLearningView = new ELearningView();
                            eLearningView.CourseCode = assignment.Course.Code;
                            eLearningView.CourseName = assignment.Course.Name;
                            eLearningView.Name = assignment.Assignment;
                            eLearningView.Description = assignment.Instructions;
                            eLearningView.SetDate = assignment.DateSet.ToLongDateString();
                            eLearningView.DueDate = assignment.DueDate.ToLongDateString();
                            eLearningView.Url = assignment.URL;
                            eLearningView.AssignmentId = assignment.Id;
                            eLearningView.HasSubmitted = submittedAssignment?.Id > 0 ? true : false;
                            eLearningView.SubmittedAssignmentId = submittedAssignment?.Id > 0 ? submittedAssignment.Id : 0;
                            eLearningView.isPublished = assignment.Publish;
                            if (assignment.Publish)
                            {

                                eLearningView.AssignmentScore = submittedAssignment?.Score != null ? Decimal.Truncate((decimal)submittedAssignment.Score) + "/" + assignment.MaxScore : "";
                            }

                            viewModel.ELearningViews.Add(eLearningView);
                        }
                    }
                }

                KeepDropDownState(viewModel);
                TempData["vModel"] = viewModel;
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }

            KeepDropDownState(viewModel);
            return View(viewModel);
        }
        public JsonResult GetSemester(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var session = new Session { Id = Convert.ToInt32(id) };
                var semesterLogic = new SemesterLogic();
                var sessionSemesterList = new List<SessionSemester>();
                var sessionSemesterLogic = new SessionSemesterLogic();
                sessionSemesterList = sessionSemesterLogic.GetModelsBy(p => p.Session_Id == session.Id);

                var semesters = new List<Semester>();
                foreach (SessionSemester sessionSemester in sessionSemesterList)
                {
                    semesters.Add(sessionSemester.Semester);
                }

                return Json(new SelectList(semesters, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult DownloadCount(string EcourseId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                ECourseContentDownloadLogic eCourseContentDownloadLogic = new ECourseContentDownloadLogic();
                ECourseLogic eCourseLogic = new ECourseLogic();
                Model.Model.Student currentStudent = System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;
                if (!String.IsNullOrEmpty(EcourseId))
                {
                    var id = Convert.ToInt64(EcourseId);
                    if (currentStudent?.Id > 0)
                    {
                        var exist = eCourseContentDownloadLogic.GetModelsBy(f => f.E_Course_Content_Id == id && f.Person_Id == currentStudent.Id).FirstOrDefault();
                        if (exist == null)
                        {
                            var ecourse = eCourseLogic.GetModelBy(f => f.Id == id);
                            ecourse.views = ecourse.views == null ? 1 : ecourse.views + 1;
                            eCourseLogic.Modify(ecourse);
                            ECourseContentDownload eCourseContentDownload = new ECourseContentDownload()
                            {
                                DateViewed = DateTime.Now,
                                Person = new Person() { Id = currentStudent.Id },
                                ECourse = new ECourse() { Id = id },

                            };
                            eCourseContentDownloadLogic.Create(eCourseContentDownload);
                        }
                    }
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit(string id)
        {
            ELearningViewModel viewModel = new ELearningViewModel();
            try
            {
                if (id != null)
                {
                    long assignmentId = Convert.ToInt64(Utility.Decrypt(id));
                    EAssignmentLogic eAssignmentLogic = new EAssignmentLogic();
                    EAssignmentSubmissionLogic eAssignmentSubmissionLogic = new EAssignmentSubmissionLogic();
                    StudentLogic studentLogic = new StudentLogic();
                    var student = studentLogic.GetModelBy(u => u.Matric_Number == User.Identity.Name);

                    var getAssignment = eAssignmentLogic.GetModelBy(s => s.Id == assignmentId);
                    if (getAssignment != null)
                    {
                        var checkIfStudentHasSubmitted = eAssignmentSubmissionLogic.GetModelsBy(s => s.Student_Id == student.Id && s.Assignment_Id == getAssignment.Id);
                        if (checkIfStudentHasSubmitted.Count > 0)
                        {
                            SetMessage("Sorry you have already submitted this assignment", Message.Category.Warning);
                            return RedirectToAction("Assignment");
                        }
                        if (getAssignment.DueDate < DateTime.Now)
                        {
                            SetMessage("Sorry you can no longer submit this assignment", Message.Category.Warning);
                            return RedirectToAction("Assignment");
                        }

                        viewModel.eAssignment = getAssignment;
                    }
                }
                else
                {
                    return RedirectToAction("Assignment");
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }
            return View(viewModel);

        }
        [HttpPost]
        public ActionResult Submit(ELearningViewModel viewModel)
        {
            try
            {
                if (viewModel != null && viewModel.eAssignment != null)
                {
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase hpf = Request.Files[file];
                        string pathForSaving = Server.MapPath("/Content/ELearning/Submission/");
                        if (!Directory.Exists(pathForSaving))
                        {
                            Directory.CreateDirectory(pathForSaving);
                        }

                        StudentLogic studentLogic = new StudentLogic();
                        EAssignmentSubmissionLogic eAssignmentSubmissionLogic = new EAssignmentSubmissionLogic();

                        var student = studentLogic.GetModelsBy(u => u.Matric_Number == User.Identity.Name).LastOrDefault();
                        if (student != null)
                        {
                            string extension = Path.GetExtension(hpf.FileName);
                            string newFilename = student.Name.Replace("/", "_").Replace(" ", "_") + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + extension;
                            string savedFileName = Path.Combine(pathForSaving, newFilename);
                            FileUploadURL = savedFileName;
                            hpf.SaveAs(savedFileName);

                            var existingSubmission = eAssignmentSubmissionLogic.GetModelsBy(s => s.Student_Id == student.Id && s.Assignment_Id == viewModel.eAssignment.Id).LastOrDefault();
                            if (existingSubmission != null)
                            {
                                existingSubmission.AssignmentContent = "~/Content/ELearning/Submission/" + newFilename;
                                existingSubmission.DateSubmitted = DateTime.Now;

                                eAssignmentSubmissionLogic.Modify(existingSubmission);
                            }
                            else
                            {
                                EAssignmentSubmission eAssignmentSubmission = new EAssignmentSubmission();
                                eAssignmentSubmission.AssignmentContent = "~/Content/ELearning/Submission/" + newFilename;
                                eAssignmentSubmission.DateSubmitted = DateTime.Now;
                                eAssignmentSubmission.EAssignment = viewModel.eAssignment;
                                eAssignmentSubmission.Student = student;
                                eAssignmentSubmission.TextSubmission = viewModel.TextSubmission;

                                eAssignmentSubmissionLogic.Create(eAssignmentSubmission);
                            }

                            SetMessage("Assignment Submitted Succesfully", Message.Category.Information);
                        }
                    }
                }
                else
                {
                    SetMessage("Error Please Choose a course before submitting", Message.Category.Error);

                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);


            }
            return RedirectToAction("Assignment");
        }
        public ActionResult RegisteredCourse()
        {
            try
            {
                viewModel.CourseRegistrationSemesters = RegisteredCourseForActiveSession();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(viewModel);
        }
        public ActionResult ECourseContent(string id)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    long registrationId = Convert.ToInt64(Utility.Decrypt(id));
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                    ECourseLogic eCourseLogic = new ECourseLogic();
                    var courseregistrations = courseRegistrationDetailLogic.GetModelBy(f => f.Student_Course_Registration_Detail_Id == registrationId);
                    if (courseregistrations?.Id > 0)
                    {
                        var courseAllocation = courseAllocationLogic.GetModelsBy(s => s.Department_Id == courseregistrations.CourseRegistration.Department.Id &&
                                                           s.Programme_Id == courseregistrations.CourseRegistration.Programme.Id && s.Level_Id == courseregistrations.CourseRegistration.Level.Id
                                                           && s.Session_Id == courseregistrations.CourseRegistration.Session.Id && s.Course_Id == courseregistrations.Course.Id).LastOrDefault();
                        if (courseAllocation?.Id > 0)
                        {
                            viewModel.ECourseList = eCourseLogic.GetModelsBy(f => f.E_CONTENT_TYPE.Course_Allocation_Id == courseAllocation.Id && f.E_CONTENT_TYPE.IsDelete == false && f.IsDelete == false);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(viewModel);
        }
        public ActionResult EnterChatRoom(string id)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                {


                    long registrationId = Convert.ToInt64(Utility.Decrypt(id));
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    CourseAllocationLogic courseAllocationLogic = new CourseAllocationLogic();
                    var courseregistrations = courseRegistrationDetailLogic.GetModelBy(f => f.Student_Course_Registration_Detail_Id == registrationId);
                    if (courseregistrations?.Id > 0)
                    {

                        var courseAllocation = courseAllocationLogic.GetModelsBy(s => s.Department_Id == courseregistrations.CourseRegistration.Department.Id &&
                                                           s.Programme_Id == courseregistrations.CourseRegistration.Programme.Id && s.Level_Id == courseregistrations.CourseRegistration.Level.Id
                                                           && s.Session_Id == courseregistrations.CourseRegistration.Session.Id && s.Course_Id == courseregistrations.Course.Id).LastOrDefault();
                        if (courseAllocation?.Id > 0)
                        {
                            StudentLogic studentLogic = new StudentLogic();
                            viewModel.Student = studentLogic.GetBy(User.Identity.Name);

                            viewModel.CourseAllocation = courseAllocationLogic.GetModelBy(f => f.Course_Allocation_Id == courseAllocation.Id);
                            var existingChatTopic = eChatTopicLogic.GetModelsBy(f => f.Course_Allocation_Id == courseAllocation.Id).LastOrDefault();
                            if (existingChatTopic == null)
                            {
                                EChatTopic eChatTopic = new EChatTopic()
                                {
                                    CourseAllocation = courseAllocation,
                                    Active = true
                                };
                                eChatTopicLogic.Create(eChatTopic);
                            }
                            viewModel.EChatResponses = eChatResponseLogic.GetModelsBy(f => f.E_CHAT_TOPIC.Course_Allocation_Id == courseAllocation.Id);
                        }

                    }




                }



            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(viewModel);
        }
        public JsonResult AjaxLoadChatBoard(long courseAllocationId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                viewModel.Student = studentLogic.GetBy(User.Identity.Name);
                if (viewModel.Student != null && courseAllocationId > 0)
                {

                    result.EChatBoards = eChatResponseLogic.LoadChatBoard(viewModel.Student, null, courseAllocationId);
                    result.IsError = false;

                }

            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }
        public JsonResult SaveChatResponse(long courseAllocationId, string response)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                viewModel.Student = studentLogic.GetBy(User.Identity.Name);
                if (viewModel.Student != null && courseAllocationId > 0 && response != null && response != " ")
                {

                    eChatResponseLogic.SaveChatResponse(courseAllocationId, viewModel.Student, response, null);
                    result.IsError = false;

                }
            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result);
        }
        public CourseRegistrationSemesters RegisteredCourseForActiveSession()
        {
            CourseRegistrationSemesters courseRegistrationSemesters = new CourseRegistrationSemesters();
            courseRegistrationSemesters.CourseRegistrationFirstSemester = new List<CourseRegistrationDetail>();
            courseRegistrationSemesters.CourseRegistrationSecondSemester = new List<CourseRegistrationDetail>();
            SessionLogic sessionLogic = new SessionLogic();
            CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
            StudentLogic studentLogic = new StudentLogic();
            var activeSession = sessionLogic.GetModelsBy(f => f.Activated == true).LastOrDefault();
            Model.Model.Student student = studentLogic.GetBy(User.Identity.Name);
            if (activeSession?.Id > 0 && student?.Id > 0)
            {
                courseRegistrationSemesters.CourseRegistrationFirstSemester = courseRegistrationDetailLogic.GetModelsBy(f => f.STUDENT_COURSE_REGISTRATION.Session_Id == activeSession.Id && f.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && f.Semester_Id == 1);
                courseRegistrationSemesters.CourseRegistrationSecondSemester = courseRegistrationDetailLogic.GetModelsBy(f => f.STUDENT_COURSE_REGISTRATION.Session_Id == activeSession.Id && f.STUDENT_COURSE_REGISTRATION.Person_Id == student.Id && f.Semester_Id == 2);
            }
            return courseRegistrationSemesters;
        }
        public ActionResult DownloadElearningGuide()
        {
            try
            {


                try
                {
                    return File(Server.MapPath("~/Content/ELearning/Manual/") + "Absu_E-Learning_Student_Guide.pdf", "application/pdf", "Student_Elearning_User_Manual.pdf");
                }
                catch (Exception ex)
                {
                    SetMessage("Error Occured. " + ex.Message, Message.Category.Error);
                    return RedirectToAction("Home", "Account", new { Area = "Security" });
                }


            }
            catch (Exception ex) { throw ex; }
        }
        public ActionResult ViewSubmittedAssignment(string id)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    long assignmentSubmissionId = Convert.ToInt64(Utility.Decrypt(id));
                    EAssignmentSubmissionLogic eAssignmentSubmissionLogic = new EAssignmentSubmissionLogic();
                    viewModel.EAssignmentSubmission = eAssignmentSubmissionLogic.GetModelBy(f => f.Id == assignmentSubmissionId);
                    if (viewModel.EAssignmentSubmission != null && viewModel.EAssignmentSubmission.Score != null)
                    {
                        viewModel.EAssignmentSubmission.Score = Decimal.Truncate((decimal)viewModel.EAssignmentSubmission.Score);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(viewModel);
        }
    }
}