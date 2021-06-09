using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    public class ResultController : BaseController
    {
        private readonly StudentLogic studentLogic;
        private StudentLevelLogic studentLevelLogic;
        private ResultViewModel viewModel;

        public ResultController()
        {
            viewModel = new ResultViewModel();
            studentLogic = new StudentLogic();
        }

        public ActionResult Check()
        {
            ResultViewModel resultViewModel = new ResultViewModel();
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
        public ActionResult Check(ResultViewModel vModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Model.Model.Student student = studentLogic.GetBy(User.Identity.Name);
                    if (student != null && student.Id > 0)
                    {
                        //var courseEvaluationAnswerLogic = new CourseEvaluationAnswerLogic();
                        //List<CourseEvaluationAnswer> courseEvaluationAnswers =
                        //    courseEvaluationAnswerLogic.GetModelsBy(a => a.Person_Id == student.Id && a.Semester_Id == vModel.Semester.Id && a.Session_Id == vModel.StudentLevel.Session.Id);
                        //if (courseEvaluationAnswers != null && courseEvaluationAnswers.Count > 0)
                        //{
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
                            SetMessage("Result for your department hasn't been released or enabled!",Message.Category.Error);
                        //}
                        //else
                        //{
                        //    return RedirectToAction("SemesterResult", new { sid = Utility.Encrypt(student.Id.ToString()), sesid = Utility.Encrypt(vModel.StudentLevel.Session.Id.ToString()), semid = Utility.Encrypt(vModel.Semester.Id.ToString()) });
                        //}
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
                    //var courseEvaluationAnswerLogic = new CourseEvaluationAnswerLogic();
                    //List<CourseEvaluationAnswer> courseEvaluationAnswers = courseEvaluationAnswerLogic.GetModelsBy(a => a.Person_Id == student.Id && a.Semester_Id == semesterId && a.Session_Id == sessionId);

                    //if (courseEvaluationAnswers != null && courseEvaluationAnswers.Count > 0)
                    //{

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
                            SetMessage("Student level record not found",Message.Category.Error);

                            return RedirectToAction("Check");
                        }
                    ////}
                    ////else
                    ////{
                    ////    return RedirectToAction("SemesterResult", new { sid = Utility.Encrypt(student.Id.ToString()), semid = Utility.Encrypt(semesterId.ToString()), sesid = Utility.Encrypt(sessionId.ToString()) });
                    ////}
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View();
        }

        public ActionResult SemesterResult(string sid, string semid,string sesid)
        {
                  viewModel = new ResultViewModel();
            try
            {
                if (sid == null && semid == null && sesid == null)
                {
                    return RedirectToAction("Check");
                }

                long Id = Convert.ToInt64(Utility.Decrypt(sid));
                int semesterId = Convert.ToInt32(Utility.Decrypt(semid));

                studentLevelLogic = new StudentLevelLogic();
                var studentLevel = new StudentLevel();
                if (Id > 0)
                {
                    studentLevel = studentLevelLogic.GetBy(Id);
                    viewModel.Semester = new Semester() {Id = semesterId};
                    studentLevel.Session.Id = Convert.ToInt32(Utility.Decrypt(sesid));
                }

                var courseLogic = new CourseLogic();
                var courseEvaluationQuestionLogic = new CourseEvaluationQuestionLogic();
                ViewBag.ScoreId = viewModel.ScoreSelectListItem;
                viewModel.StudentLevel = studentLevel;
                viewModel.CourseEvaluationQuestionsForSectionOne = courseEvaluationQuestionLogic.GetModelsBy(a => a.Section == 1 && a.Activated);
                viewModel.CourseEvaluationQuestionsForSectionTwo = courseEvaluationQuestionLogic.GetModelsBy(a => a.Section == 2 && a.Activated);
                viewModel.CourseEvaluations = new List<CourseEvaluation>();
                viewModel.CourseEvaluationsTwo = new List<CourseEvaluation>();
                viewModel.Courses = courseLogic.GetBy(studentLevel.Department, studentLevel.Level, new Semester { Id = semesterId },studentLevel.Programme,studentLevel.DepartmentOption);
                if (viewModel.Courses != null && viewModel.Courses.Count > 0)
                {
                    foreach (Course course in viewModel.Courses)
                    {
                        var courseEvaluation = new CourseEvaluation();
                        courseEvaluation.Course = course;
                        courseEvaluation.CourseEvaluationQuestion = viewModel.CourseEvaluationQuestionsForSectionOne;
                        viewModel.CourseEvaluations.Add(courseEvaluation);
                    }

                    foreach (Course course in viewModel.Courses)
                    {
                        var courseEvaluation = new CourseEvaluation();
                        courseEvaluation.Course = course;
                        courseEvaluation.CourseEvaluationQuestion = viewModel.CourseEvaluationQuestionsForSectionTwo;
                        viewModel.CourseEvaluationsTwo.Add(courseEvaluation);
                    }
                }
                TempData["resultViewModel"] = viewModel;
            }
            catch (Exception)
            {
                throw;
            }

            return View(viewModel);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SemesterResult(FormCollection resultViewModel)
        {
            try
            {
                int count = 0;
                long studentId = Convert.ToInt64(resultViewModel["StudentLevel.Student.Id"]);
                ResultViewModel resultModel = (ResultViewModel)TempData["resultViewModel"];
                var st = new StringBuilder();
                var answeredQuestions = new List<string>();
                var answeredQuestionsSection2 = new List<string>();
                foreach (string key in resultViewModel.Keys)
                {
                    if (key.StartsWith("Question_"))
                    {
                        answeredQuestions.Add(key);
                    }
                    if (key.StartsWith("SectionQuestion_"))
                    {
                        answeredQuestionsSection2.Add(key);
                    }
                }
                using (var scope = new TransactionScope())
                {
                    foreach (string s in answeredQuestions)
                    {
                        var courseEvaluationAnswerLogic = new CourseEvaluationAnswerLogic();
                        var courseEvaluationAnswer = new CourseEvaluationAnswer();

                        int courseIndex = s.IndexOf("Course_Id_");
                        string Coursestring = s.Substring(courseIndex);
                        string CourseId = s.Substring(courseIndex).Replace("Course_Id_", "");
                        string QuestionId = s.Replace("Question_", "").Replace(Coursestring, "");
                        int value = 0;
                        Int32.TryParse(resultViewModel[s], out value);
                        if (value > 0)
                        {
                            if (resultModel != null)
                            {
                                courseEvaluationAnswer.Semester = resultModel.Semester;
                                courseEvaluationAnswer.Session = resultModel.StudentLevel.Session;
                            }
                            courseEvaluationAnswer.CourseEvaluationQuestion = new CourseEvaluationQuestion();
                            courseEvaluationAnswer.CourseEvaluationQuestion.Id = Convert.ToInt32(QuestionId);
                            courseEvaluationAnswer.Course = new Course();
                            courseEvaluationAnswer.Course.Id = Convert.ToInt32(CourseId);
                            courseEvaluationAnswer.Score = value;
                            courseEvaluationAnswer.Student = new Model.Model.Student() {Id = studentId};
                            var created = courseEvaluationAnswerLogic.Create(courseEvaluationAnswer);
                         
                                if (created != null)
                                {
                                    count += 1;
                                }
                        }
                    }

                    foreach (string s in answeredQuestionsSection2)
                    {
                        int value = 0;
                        Int32.TryParse(resultViewModel[s], out value);
                        if (value > 0)
                        {
                            var courseEvaluationAnswerLogic = new CourseEvaluationAnswerLogic();
                            var courseEvaluationAnswer = new CourseEvaluationAnswer();
                            int courseIndex = s.IndexOf("Course_Id_");
                            string Coursestring = s.Substring(courseIndex);
                            string CourseId = s.Substring(courseIndex).Replace("Course_Id_", "");
                            string QuestionId = s.Replace("SectionQuestion_", "").Replace(Coursestring, "");

                            courseEvaluationAnswer.CourseEvaluationQuestion = new CourseEvaluationQuestion();
                            courseEvaluationAnswer.CourseEvaluationQuestion.Id = Convert.ToInt32(QuestionId);
                            courseEvaluationAnswer.Course = new Course();
                            courseEvaluationAnswer.Course.Id = Convert.ToInt32(CourseId);
                            courseEvaluationAnswer.Score = value;
                            courseEvaluationAnswer.Student = new Model.Model.Student() {Id = studentId};
                         
                            if (resultModel != null)
                            {
                                courseEvaluationAnswer.Semester = resultModel.Semester;
                                courseEvaluationAnswer.Session = resultModel.StudentLevel.Session;
                            }
                            var created = courseEvaluationAnswerLogic.Create(courseEvaluationAnswer);

                            if (created != null)
                            {
                                count += 1;
                            }
                        }
                    }
                    scope.Complete();
                    SetMessage("Thank you for filling the survey, You may now proceed to check your result!",
                        Message.Category.Information);

                    return RedirectToAction("Check");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }
    }
}