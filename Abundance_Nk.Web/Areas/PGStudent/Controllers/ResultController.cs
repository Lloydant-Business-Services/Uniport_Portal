using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.PGStudent.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.PGStudent.Controllers
{
    [AllowAnonymous]
    public class ResultController : BaseController
    {
        // GET: PGStudent/Result
        public ActionResult Index()
        {
            return View();
        }
        private readonly StudentLogic studentLogic;
        private StudentLevelLogic studentLevelLogic;
        private PGResultViewModel viewModel;

        public ResultController()
        {
            viewModel = new PGResultViewModel();
            studentLogic = new StudentLogic();
        }
        public ActionResult Check()
        {
            PGResultViewModel resultViewModel = new PGResultViewModel();
            try
            {
                ViewBag.Session = Utility.PopulateSessionSelectListItem();
                ViewBag.Semester = Utility.PopulateSemesterSelectListItem();
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(resultViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Check(PGResultViewModel vModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Model.Model.Student student = studentLogic.GetBy(User.Identity.Name);
                    if (student != null && student.Id > 0)
                    {
                        var courseEvaluationAnswerLogic = new CourseEvaluationAnswerLogic();
                        List<CourseEvaluationAnswer> courseEvaluationAnswers =
                            courseEvaluationAnswerLogic.GetModelsBy(a => a.Person_Id == student.Id && a.Semester_Id == vModel.Semester.Id && a.Session_Id == vModel.StudentLevel.Session.Id);
                        if (courseEvaluationAnswers != null && courseEvaluationAnswers.Count > 0)
                        {
                            var studentResultStatus = new StudentResultStatus();
                            var studentResultStatusLogic = new StudentResultStatusLogic();
                            studentLevelLogic = new StudentLevelLogic();
                            StudentLevel studentLevel = studentLevelLogic.GetBy(student.Id);
                            studentResultStatus =
                                studentResultStatusLogic.GetModelBy(
                                    s =>
                                        s.Department_Id == studentLevel.Department.Id &&
                                        s.Level_Id == studentLevel.Level.Id &&
                                        s.Programme_Id == studentLevel.Programme.Id);
                            if (studentResultStatus != null && studentResultStatus.Id > 0)
                            {
                                return RedirectToAction("Statement", new { sid = Utility.Encrypt(student.Id.ToString()), sesid = Utility.Encrypt(vModel.StudentLevel.Session.Id.ToString()), semid = Utility.Encrypt(vModel.Semester.Id.ToString()) });
                            }
                            //return RedirectToAction("Statement",new { sid = Utility.Encrypt(student.Id.ToString()) });
                            SetMessage("Result for your department hasn't been released or enabled!", Message.Category.Error);
                        }
                        else
                        {
                            return RedirectToAction("SemesterResult", new { sid = Utility.Encrypt(student.Id.ToString()), sesid = Utility.Encrypt(vModel.StudentLevel.Session.Id.ToString()), semid = Utility.Encrypt(vModel.Semester.Id.ToString()) });
                        }
                    }
                    else
                    {
                        SetMessage("Invalid Matric Number!", Message.Category.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            ViewBag.Session = Utility.PopulateSessionSelectListItem();
            ViewBag.Semester = Utility.PopulateSemesterSelectListItem();
            return View(vModel);
        }
        public ActionResult Statement(string sid, string sesid, string semid)
        {
            try
            {
                if (sesid != null && semid != null)
                {
                    int semesterId = Convert.ToInt32(Utility.Decrypt(semid));
                    int sessionId = Convert.ToInt32(Utility.Decrypt(sesid));

                    Model.Model.Student student = studentLogic.GetBy(User.Identity.Name);
                    var courseEvaluationAnswerLogic = new CourseEvaluationAnswerLogic();
                    List<CourseEvaluationAnswer> courseEvaluationAnswers = courseEvaluationAnswerLogic.GetModelsBy(a => a.Person_Id == student.Id && a.Semester_Id == semesterId && a.Session_Id == sessionId);

                    if (courseEvaluationAnswers != null && courseEvaluationAnswers.Count > 0)
                    {

                        CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                        studentLevelLogic = new StudentLevelLogic();
                        StudentLevel studentLevel = studentLevelLogic.GetBy(student.Id);

                        if (studentLevel != null)
                        {
                            CourseRegistration courseRegistration = courseRegistrationLogic.GetModelsBy(s =>
                              s.Session_Id == sessionId && s.Person_Id == student.Id &&
                              s.Department_Id == studentLevel.Department.Id && s.Programme_Id == studentLevel.Programme.Id).LastOrDefault();

                            if (courseRegistration != null)
                            {
                                long Id = Convert.ToInt64(Utility.Decrypt(sid));
                                ViewBag.StudentId = Id;
                                ViewBag.SessionId = sessionId;
                                ViewBag.SemesterId = semesterId;
                            }
                            else
                            {
                                SetMessage("Student courseRegistration record not found", Message.Category.Error);

                                return RedirectToAction("Check");
                            }
                        }
                        else
                        {
                            SetMessage("Student level record not found", Message.Category.Error);

                            return RedirectToAction("Check");
                        }
                    }
                    else
                    {
                        return RedirectToAction("SemesterResult", new { sid = Utility.Encrypt(student.Id.ToString()), semid = Utility.Encrypt(semesterId.ToString()), sesid = Utility.Encrypt(sessionId.ToString()) });
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View();
        }
    }
}