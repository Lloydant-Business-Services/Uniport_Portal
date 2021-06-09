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
    public class CourseRegistrationController : BaseController
    {
        private readonly CourseLogic courseLogic;
        private readonly StudentLogic studentLogic;
        private readonly PGCourseRegistrationViewModel viewModel;
        public CourseRegistrationController()
        {
            courseLogic = new CourseLogic();
            studentLogic = new StudentLogic();
            viewModel = new PGCourseRegistrationViewModel();
        }
        // GET: PGStudent/CourseRegistration
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Form(string sid, PGSemesterViewModel vModel)
        {
            var paymentEtranzactLogic = new PaymentEtranzactLogic();
            var paystackLogic = new PaystackLogic();
            try
            {
                if (TempData["semesterViewModel"] != null)
                {
                    vModel = (PGSemesterViewModel)TempData["semesterViewModel"];
                }
                Int64 studentId = Convert.ToInt64(Utility.Decrypt(sid));
                var session = new Session { Id = vModel.session.Id };
                var feeType = new FeeType { Id = (int)FeeTypes.SchoolFees };
                var sessionLogic = new SessionLogic();
                var levelLogic = new LevelLogic();
                if (vModel.semesterId != 0)
                {
                    vModel.semester = new Semester { Id = vModel.semesterId };
                }
                ApplicantLogic applicantLogic = new ApplicantLogic();
                var ApplicantStep = applicantLogic.GetModelBy(a => a.Person_Id == studentId);
                //if (ApplicantStep != null && ApplicantStep.Status.Id < 10)
                //{
                //    SetMessage("You have not filled your biodata. Please login to the new student area and complete your biodata form",
                //            Message.Category.Error);
                //    return RedirectToAction("Logon", "Registration", new { Area = "PGStudent" });
                //}

                var selectedSession = sessionLogic.GetModelBy(d => d.Session_Id == session.Id);
                if (vModel.semester.Id == 1)
                {
                    if (selectedSession.ActiveCourseRegistration)
                    {
                        if (paymentEtranzactLogic.HasStudentPaidFirstInstallmentOrCompletedFeesForSession(studentId, session, feeType))
                        {
                            PopulateFirstSemeterRegistrationForm(studentId, session.Id);
                        }
                        else
                        {
                            SetMessage("You have not made payment for the selected session! Please pay your fees to continue",
                                Message.Category.Information);
                            return RedirectToAction("CourseRegistration", "Home", new { Area = "PGStudent" });
                        }
                    }
                    else
                    {

                        SetMessage("Registration has closed for the selected session and Semester!",
                                   Message.Category.Information);
                        return RedirectToAction("CourseRegistration", "Home", new { Area = "PGStudent" });
                    }


                }
                else
                {
                    //if (session.Id == 13 || session.Id == 18 || session.Id == 7)
                    if (selectedSession.ActiveCourseRegistration)
                    {
                        if (paymentEtranzactLogic.HasStudentCompletedFeesForSession(studentId, session, feeType))
                        {

                            //var feeTypeSug = new FeeType { Id = (int)FeeTypes.SUGDUESTISHIP };
                            //if (paymentEtranzactLogic.HasStudentPaidSugTishipFeeForSession(studentId, session, feeTypeSug))
                            //{

                            //Check if student is eligible to register for selected session
                            var studentLevelLogic = new StudentLevelLogic();
                            viewModel.StudentLevel = studentLevelLogic.GetBy(studentId);
                            if (viewModel.StudentLevel != null && viewModel.StudentLevel.Id > 0)
                            {
                                //Open past sessions for course registration for selected level
                                if (viewModel.StudentLevel.Session.Id != session.Id && viewModel.StudentLevel.Session.Id > session.Id)
                                {


                                    var previousLevelId = GetStudentPreviousLevelBy(viewModel.StudentLevel.Session.Id, session.Id, viewModel.StudentLevel.Level.Id);
                                    var selectectedSessession = sessionLogic.GetModelBy(f => f.Session_Id == session.Id);
                                    if ((selectectedSessession.Name == "2015/2016" && previousLevelId == 1) || (selectectedSessession.Name == "2016/2017" && previousLevelId == 2) || (selectectedSessession.Name == "2017/2018" && previousLevelId == 3) || (selectectedSessession.Name == "2018/2019" && previousLevelId == 4))
                                    {

                                        //Do nothing
                                    }
                                    else
                                    {
                                        SetMessage("Registration Has Closed for the selected Session",
                                        Message.Category.Information);
                                        return RedirectToAction("CourseRegistration", "Home", new { Area = "PGStudent" });
                                    }
                                }
                            }
                            PopulateSecondSemeterRegistrationForm(studentId, session.Id);
                            //}
                            //else
                            //{
                            //    SetMessage("You have not made SUG/TISHIP payment for the selected session! Please pay your fees to continue",
                            //        Message.Category.Information);
                            //    return RedirectToAction("CourseRegistration", "Home", new { Area = "Student" });
                            //}
                        }
                        else
                        {
                            SetMessage("You have not completed your Fees! Please complete your fees to continue",
                                Message.Category.Information);
                            return RedirectToAction("CourseRegistration", "Home", new { Area = "PGStudent" });
                        }
                    }
                    else
                    {

                        SetMessage("Registration has closed for the selected session and Semester!",
                                   Message.Category.Information);
                        return RedirectToAction("CourseRegistration", "Home", new { Area = "PGStudent" });
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            viewModel.Semester = vModel.semester;
            TempData["CourseRegistrationViewModel"] = viewModel;
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Form(PGCourseRegistrationViewModel viewModel)
        {
            string message = null;

            try
            {
                List<CourseRegistrationDetail> selectedFirstSemesterCourseRegistrationDetails = null;
                List<CourseRegistrationDetail> selectedSecondSemesterCourseRegistrationDetails = null;
                var courseRegistrationDetails = new List<CourseRegistrationDetail>();

                if (viewModel.CarryOverExist)
                {
                    var selectedCarryOverCourseRegistrationDetails = new List<CourseRegistrationDetail>();
                    selectedCarryOverCourseRegistrationDetails = GetSelectedCourses(viewModel.CarryOverCourses);
                    if (selectedCarryOverCourseRegistrationDetails != null)
                    {
                        courseRegistrationDetails.AddRange(selectedCarryOverCourseRegistrationDetails);
                    }

                }

                viewModel.RegisteredCourse.Student = viewModel.Student;

                var courseRegistrationLogic = new CourseRegistrationLogic();
                if (viewModel.CourseAlreadyRegistered) //modify
                {
                    selectedFirstSemesterCourseRegistrationDetails = viewModel.FirstSemesterCourses;
                    selectedSecondSemesterCourseRegistrationDetails = viewModel.SecondSemesterCourses;

                    SetCourseUnit(selectedFirstSemesterCourseRegistrationDetails, selectedSecondSemesterCourseRegistrationDetails);

                    int totalFirstSemesterUnits = GetSelectedUnits(selectedFirstSemesterCourseRegistrationDetails);
                    int totalSecondSemesterUnits = GetSelectedUnits(selectedSecondSemesterCourseRegistrationDetails);

                    if ((totalFirstSemesterUnits > viewModel.FirstSemesterMaximumUnit) || (totalSecondSemesterUnits > viewModel.SecondSemesterMaximumUnit))
                    {
                        message = "Unit is greater than set unit!";
                        return Json(new { message = message }, "text/html", JsonRequestBehavior.AllowGet);

                    }

                    //courseRegistrationDetails = selectedFirstSemesterCourseRegistrationDetails;
                    courseRegistrationDetails.AddRange(selectedFirstSemesterCourseRegistrationDetails);
                    courseRegistrationDetails.AddRange(selectedSecondSemesterCourseRegistrationDetails);
                    viewModel.RegisteredCourse.Details = courseRegistrationDetails;
                    viewModel.RegisteredCourse.Level = viewModel.StudentLevel.Level;
                    viewModel.RegisteredCourse.Department = viewModel.StudentLevel.Department;
                    viewModel.RegisteredCourse.Programme = viewModel.StudentLevel.Programme;
                    viewModel.RegisteredCourse.Session = viewModel.CurrentSessionSemester.SessionSemester.Session;

                    bool modified = courseRegistrationLogic.Modify(viewModel.RegisteredCourse);
                    if (modified)
                    {
                        message = "Selected courses has been successfully modified.";
                    }
                    else
                    {
                        message = "Course Registration Updated!.";
                    }
                }
                else //insert
                {
                    selectedFirstSemesterCourseRegistrationDetails = GetSelectedCourses(viewModel.FirstSemesterCourses);
                    selectedSecondSemesterCourseRegistrationDetails = GetSelectedCourses(viewModel.SecondSemesterCourses);
                    SetCourseUnit(selectedFirstSemesterCourseRegistrationDetails, selectedSecondSemesterCourseRegistrationDetails);
                    int totalFirstSemesterUnits = 0;
                    int totalSecondSemesterUnits = 0;
                    if (selectedFirstSemesterCourseRegistrationDetails != null)
                    {
                        totalFirstSemesterUnits = GetSelectedUnits(selectedFirstSemesterCourseRegistrationDetails);
                    }
                    if (selectedSecondSemesterCourseRegistrationDetails != null)
                    {
                        totalSecondSemesterUnits = GetSelectedUnits(selectedSecondSemesterCourseRegistrationDetails);
                    }



                    if ((totalFirstSemesterUnits > viewModel.FirstSemesterMaximumUnit) || (totalSecondSemesterUnits > viewModel.SecondSemesterMaximumUnit))
                    {
                        message = "Unit is greater than set unit!";
                        return Json(new { message = message }, "text/html", JsonRequestBehavior.AllowGet);

                    }


                    //courseRegistrationDetails = selectedFirstSemesterCourseRegistrationDetails;
                    if (selectedFirstSemesterCourseRegistrationDetails != null)
                    {
                        courseRegistrationDetails.AddRange(selectedFirstSemesterCourseRegistrationDetails);
                    }

                    if (selectedSecondSemesterCourseRegistrationDetails != null)
                    {
                        courseRegistrationDetails.AddRange(selectedSecondSemesterCourseRegistrationDetails);
                    }


                    viewModel.RegisteredCourse.Details = courseRegistrationDetails;
                    viewModel.RegisteredCourse.Level = new Level { Id = viewModel.StudentLevel.Level.Id };
                    viewModel.RegisteredCourse.Programme = new Programme { Id = viewModel.StudentLevel.Programme.Id };
                    viewModel.RegisteredCourse.Department = new Department { Id = viewModel.StudentLevel.Department.Id };
                    viewModel.RegisteredCourse.Session = new Session
                    {
                        Id = viewModel.CurrentSessionSemester.SessionSemester.Session.Id
                    };

                    CourseRegistration courseRegistration = courseRegistrationLogic.Create(viewModel.RegisteredCourse);
                    if (courseRegistration != null)
                    {
                        message = "Selected courses has been successfully registered.";
                    }
                    else
                    {
                        message = "Course Registration Failed! Please try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Error Occurred! " + ex.Message + ". Please try again.";
            }

            return Json(new { message = message }, "text/html", JsonRequestBehavior.AllowGet);

            //var semester = new Semester() { Id = viewModel.RegisteredCourse.Details[0].Semester.Id };
            //viewModel.Semester = semester;

            //var session = new Session() { Id = viewModel.CurrentSessionSemester.SessionSemester.Session.Id };
            //var semesterViewModel = new SemesterViewModel() { semester = semester,session = session };
            //TempData["semesterViewModel"] = semesterViewModel;
            //return RedirectToAction("Form","CourseRegistration",
            //    new
            //    {
            //        Area = "Student",
            //        sid = Utility.Encrypt(viewModel.Student.Id.ToString()),


            //    });
        }
        private List<CourseRegistrationDetail> GetSelectedCourses(List<CourseRegistrationDetail> coursesToRegister)
        {
            try
            {
                List<CourseRegistrationDetail> selectedCourseDetails = null;

                if (coursesToRegister != null && coursesToRegister.Count > 0)
                {
                    selectedCourseDetails = coursesToRegister.Where(c => c.Course.IsRegistered).ToList();
                }

                return selectedCourseDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static int GetSelectedUnits(List<CourseRegistrationDetail> selectedSecondSemesterCourseRegistrationDetails)
        {
            int totalUnits = 0;
            foreach (CourseRegistrationDetail detail in selectedSecondSemesterCourseRegistrationDetails)
            {
                if (detail.Course.IsRegistered)
                {
                    totalUnits += detail.CourseUnit ?? 0;
                }
            }

            return totalUnits;
        }
        private static void SetCourseUnit(List<CourseRegistrationDetail> selectedFirstSemesterCourseRegistrationDetails, List<CourseRegistrationDetail> selectedSecondSemesterCourseRegistrationDetails)
        {
            if (selectedFirstSemesterCourseRegistrationDetails != null)
            {
                for (int i = 0; i < selectedFirstSemesterCourseRegistrationDetails.Count; i++)
                {
                    selectedFirstSemesterCourseRegistrationDetails[i].CourseUnit = selectedFirstSemesterCourseRegistrationDetails[i].Course.Unit;
                }
            }


            if (selectedSecondSemesterCourseRegistrationDetails != null)
            {
                for (int i = 0; i < selectedSecondSemesterCourseRegistrationDetails.Count; i++)
                {
                    selectedSecondSemesterCourseRegistrationDetails[i].CourseUnit = selectedSecondSemesterCourseRegistrationDetails[i].Course.Unit;
                }
            }

        }
        private void PopulateFirstSemeterRegistrationForm(long sid, int sessionId)
        {
            bool UseNewGradingSystem = false;
            List<Course> firstSemesterCourses = null;

            viewModel.Student = studentLogic.GetBy(sid);
            if (viewModel.Student != null && viewModel.Student.Id > 0)
            {
                string[] matricNumbeArray = viewModel.Student.MatricNumber.Split('/');
                int matricNumber = Convert.ToInt32(matricNumbeArray[0]);

                if (matricNumber >= 2015)
                {
                    UseNewGradingSystem = true;
                }
                var firstAttempt = new CourseMode { Id = 1 };
                var carryOver = new CourseMode { Id = 2 };
                var firstSemester = new Semester { Id = 1 };
                var secondSemester = new Semester { Id = 2 };

                var sessionSemester = new SessionSemester();
                var sessionSemesterLogic = new SessionSemesterLogic();
                sessionSemester = sessionSemesterLogic.GetBySessionSemester(firstSemester.Id, sessionId);
                var currentSessionSemesterLogic = new CurrentSessionSemesterLogic();
                var levelLogic = new LevelLogic();
                viewModel.CurrentSessionSemester = new CurrentSessionSemester { SessionSemester = sessionSemester };

                var studentLevelLogic = new StudentLevelLogic();
                viewModel.StudentLevel = studentLevelLogic.GetBy(viewModel.Student.Id);

                if (viewModel.StudentLevel != null && viewModel.StudentLevel.Id > 0)
                {

                        firstSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, firstSemester, viewModel.StudentLevel.Programme);
                        SetMinimumAndMaximumCourseUnit(firstSemester, secondSemester, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, viewModel.StudentLevel.Programme);
                }
                else
                {
                    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                    AdmissionList admissionList = admissionListLogic.GetBy(new Person() { Id = viewModel.Student.Id });
                    if (admissionList != null)
                    {
                        AppliedCourse appliedCourse = appliedCourseLogic.GetBy(new Person() { Id = viewModel.Student.Id });
                        if (appliedCourse != null)
                        {
                            viewModel.StudentLevel = new StudentLevel();
                            viewModel.StudentLevel.Level = new Level() { Id = 1 };
                            viewModel.StudentLevel.Department = admissionList.Deprtment;
                            viewModel.StudentLevel.DepartmentOption = admissionList.DepartmentOption;
                            viewModel.StudentLevel.Programme = appliedCourse.Programme;
                            viewModel.StudentLevel.Session = sessionSemester.Session;
                            viewModel.StudentLevel.Student = viewModel.Student;
                            studentLevelLogic.Create(viewModel.StudentLevel);
                        }
                    }

                }

                //get courses if already registered
                var courseRegistrationLogic = new CourseRegistrationLogic();
                var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                CourseRegistration courseRegistration = courseRegistrationLogic.GetBy(viewModel.Student, viewModel.StudentLevel.Level, viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department,
                    viewModel.CurrentSessionSemester.SessionSemester.Session);
                if (courseRegistration != null && courseRegistration.Id > 0)
                {
                    viewModel.RegisteredCourse = courseRegistration;
                    if (courseRegistration.Details != null && courseRegistration.Details.Count > 0)
                    {
                        //split registered courses by semester
                        List<CourseRegistrationDetail> firstSemesterRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == firstSemester.Id).ToList();
                        List<CourseRegistrationDetail> firstSemesterCarryOverRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == firstSemester.Id && rc.Mode.Id == carryOver.Id).ToList();

                        //get registered courses
                        viewModel.FirstSemesterCourses = GetRegisteredCourse(courseRegistration, firstSemesterCourses, firstSemester, firstSemesterRegisteredCourseDetails, firstAttempt);

                        //get carry over courses
                        List<Course> firstSemesterCarryOverCourses = courseRegistrationDetailLogic.GetCarryOverCoursesBy(courseRegistration, firstSemester);
                        viewModel.FirstSemesterCarryOverCourses = GetRegisteredCourse(courseRegistration, firstSemesterCarryOverCourses, firstSemester, firstSemesterCarryOverRegisteredCourseDetails, carryOver);

                        if (viewModel.FirstSemesterCarryOverCourses != null && viewModel.FirstSemesterCarryOverCourses.Count > 0)
                        {
                            viewModel.CarryOverExist = true;
                            viewModel.CarryOverCourses.AddRange(viewModel.FirstSemesterCarryOverCourses);
                            viewModel.TotalFirstSemesterCarryOverCourseUnit = viewModel.FirstSemesterCarryOverCourses.Where(c => c.Semester.Id == firstSemester.Id).Sum(u => u.Course.Unit);
                        }
                        else
                        {
                            viewModel.CarryOverExist = true;
                            viewModel.CarryOverCourses = courseRegistrationDetailLogic.GetAllCarryOverBy(viewModel.Student, viewModel.StudentLevel.Level, viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department, viewModel.CurrentSessionSemester.SessionSemester.Session, firstSemester);

                        }


                        //set total selected course units
                        viewModel.SumOfFirstSemesterSelectedCourseUnit = SumSemesterSelectedCourseUnit(firstSemesterRegisteredCourseDetails);
                        viewModel.CourseAlreadyRegistered = true;
                    }
                }
                else
                {
                    viewModel.FirstSemesterCourses = GetUnregisteredCourseDetail(firstSemesterCourses, firstSemester);
                    viewModel.CourseAlreadyRegistered = false;

                    //get carry over courses
                    viewModel.CarryOverCourses = courseRegistrationDetailLogic.GetCarryOverBy(viewModel.Student, viewModel.CurrentSessionSemester.SessionSemester.Session, firstSemester, UseNewGradingSystem);
                    if (viewModel.CarryOverCourses != null && viewModel.CarryOverCourses.Count > 0)
                    {
                        viewModel.CarryOverExist = true;
                        viewModel.TotalFirstSemesterCarryOverCourseUnit = viewModel.CarryOverCourses.Where(c => c.Semester.Id == firstSemester.Id).Sum(u => u.Course.Unit);

                        //if(viewModel.TotalFirstSemesterCarryOverCourseUnit <= viewModel.FirstSemesterMaximumUnit && viewModel.TotalSecondSemesterCarryOverCourseUnit <= viewModel.SecondSemesterMaximumUnit)
                        //{
                        foreach (CourseRegistrationDetail carryOverCourse in viewModel.CarryOverCourses)
                        {
                            carryOverCourse.Course.IsRegistered = true;
                        }
                        //}
                    }

                }


                //}
            }
        }
        private void PopulateSecondSemeterRegistrationForm(long sid, int sessionId)
        {
            bool UseNewGradingSystem = false;
            List<Course> secondSemesterCourses = null;

            viewModel.Student = studentLogic.GetBy(sid);
            if (viewModel.Student != null && viewModel.Student.Id > 0)
            {

                string[] matricNumbeArray = viewModel.Student.MatricNumber.Split('/');
                int matricNumber = Convert.ToInt32(matricNumbeArray[0]);

                if (matricNumber >= 2015)
                {
                    UseNewGradingSystem = true;
                }

                var firstAttempt = new CourseMode { Id = 1 };
                var carryOver = new CourseMode { Id = 2 };
                var firstSemester = new Semester();
                var secondSemester = new Semester { Id = 2 };

                var sessionSemester = new SessionSemester();
                var sessionSemesterLogic = new SessionSemesterLogic();
                sessionSemester = sessionSemesterLogic.GetBySessionSemester(secondSemester.Id, sessionId);
                var currentSessionSemesterLogic = new CurrentSessionSemesterLogic();
                var levelLogic = new LevelLogic();
                viewModel.CurrentSessionSemester = new CurrentSessionSemester { SessionSemester = sessionSemester };

                var studentLevelLogic = new StudentLevelLogic();
                viewModel.StudentLevel = studentLevelLogic.GetBy(viewModel.Student.Id);

                if (viewModel.StudentLevel != null && viewModel.StudentLevel.Id > 0)
                {

                        secondSemesterCourses = courseLogic.GetBy(viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, secondSemester, viewModel.StudentLevel.Programme);
                        SetMinimumAndMaximumCourseUnit(firstSemester, secondSemester, viewModel.StudentLevel.Department, viewModel.StudentLevel.Level, viewModel.StudentLevel.Programme);

                }

                //get courses if already registered
                var courseRegistrationLogic = new CourseRegistrationLogic();
                var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                CourseRegistration courseRegistration = courseRegistrationLogic.GetBy(viewModel.Student, viewModel.StudentLevel.Level, viewModel.StudentLevel.Programme, viewModel.StudentLevel.Department,
                    viewModel.CurrentSessionSemester.SessionSemester.Session);
                if (courseRegistration != null && courseRegistration.Id > 0)
                {
                    viewModel.RegisteredCourse = courseRegistration;
                    if (courseRegistration.Details != null)
                    {
                        //split registered courses by semester
                        List<CourseRegistrationDetail> secondSemesterRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == secondSemester.Id).ToList();
                        List<CourseRegistrationDetail> secondSemesterCarryOverRegisteredCourseDetails = courseRegistration.Details.Where(rc => rc.Semester.Id == secondSemester.Id && rc.Mode.Id == carryOver.Id).ToList();

                        //get registered courses
                        viewModel.SecondSemesterCourses = GetRegisteredCourse(courseRegistration, secondSemesterCourses, secondSemester, secondSemesterRegisteredCourseDetails, firstAttempt);

                        //get carry over courses
                        List<Course> secondSemesterCarryOverCourses =
                        courseRegistrationDetailLogic.GetCarryOverCoursesBy(courseRegistration, secondSemester);
                        viewModel.SecondSemesterCarryOverCourses = GetRegisteredCourse(courseRegistration, secondSemesterCarryOverCourses, secondSemester, secondSemesterCarryOverRegisteredCourseDetails, carryOver);

                        if (viewModel.SecondSemesterCarryOverCourses != null && viewModel.SecondSemesterCarryOverCourses.Count > 0)
                        {
                            viewModel.CarryOverExist = true;
                            viewModel.CarryOverCourses.AddRange(viewModel.SecondSemesterCarryOverCourses);
                            viewModel.TotalSecondSemesterCarryOverCourseUnit = viewModel.SecondSemesterCarryOverCourses.Where(c => c.Semester.Id == secondSemester.Id).Sum(u => u.Course.Unit);
                        }




                        //set total selected course units
                        viewModel.SumOfSecondSemesterSelectedCourseUnit = SumSemesterSelectedCourseUnit(secondSemesterRegisteredCourseDetails);
                        viewModel.CourseAlreadyRegistered = true;
                    }
                }
                else
                {
                    viewModel.SecondSemesterCourses = GetUnregisteredCourseDetail(secondSemesterCourses, secondSemester);
                    viewModel.CourseAlreadyRegistered = false;

                    //get carry over courses
                    viewModel.CarryOverCourses = courseRegistrationDetailLogic.GetCarryOverBy(viewModel.Student, viewModel.CurrentSessionSemester.SessionSemester.Session, secondSemester, UseNewGradingSystem);
                    if (viewModel.CarryOverCourses != null && viewModel.CarryOverCourses.Count > 0)
                    {
                        viewModel.CarryOverExist = true;
                        viewModel.TotalSecondSemesterCarryOverCourseUnit = viewModel.CarryOverCourses.Where(c => c.Semester.Id == secondSemester.Id).Sum(u => u.Course.Unit);

                        //if(viewModel.TotalFirstSemesterCarryOverCourseUnit <= viewModel.FirstSemesterMaximumUnit && viewModel.TotalSecondSemesterCarryOverCourseUnit <= viewModel.SecondSemesterMaximumUnit)
                        //{
                        foreach (CourseRegistrationDetail carryOverCourse in viewModel.CarryOverCourses)
                        {
                            carryOverCourse.Course.IsRegistered = true;
                        }
                        //}
                    }



                }


                //}
            }
        }
        public int GetStudentPreviousLevelBy(int currentsessionId, int previoussessionId, int currentLeveId)
        {
            int i = 0;
            try
            {
                SessionLogic sessionLogic = new SessionLogic();
                var currentSession = sessionLogic.GetModelBy(c => c.Session_Id == currentsessionId);
                var previousSession = sessionLogic.GetModelBy(c => c.Session_Id == previoussessionId);
                if (currentSession != null && previousSession != null)
                {
                    var currentSessionFirstSplit = Convert.ToInt32((currentSession.Name.Split('/'))[0]);
                    var previousSessionFirstSplit = Convert.ToInt32((previousSession.Name.Split('/'))[0]);
                    var differenceInSession = currentSessionFirstSplit - previousSessionFirstSplit;
                    i = currentLeveId - differenceInSession;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return i;
        }
        private void SetMinimumAndMaximumCourseUnit(Semester firstSemester, Semester secondSemester, Department departmemt, Level level, Programme programme)
        {
            try
            {

                //Check if level is final year and allow extra units
                //var degreeAwardedLogic = new DegreeAwardedLogic();
                //var duration = degreeAwardedLogic.GetBy(departmemt, programme).Duration ?? 0;

                var courseUnitLogic = new CourseUnitLogic();
                CourseUnit firstSemesterCourseUnit = courseUnitLogic.GetBy(departmemt, level, firstSemester, programme);
                if (firstSemesterCourseUnit != null && firstSemesterCourseUnit.Id > 0 && firstSemester != null)
                {
                    viewModel.FirstSemesterMinimumUnit = firstSemesterCourseUnit.MinimumUnit;
                    viewModel.FirstSemesterMaximumUnit = firstSemesterCourseUnit.MaximumUnit;
                }

                CourseUnit secondSemesterCourseUnit = courseUnitLogic.GetBy(departmemt, level, secondSemester, programme);
                if (secondSemesterCourseUnit != null && secondSemesterCourseUnit.Id > 0 && secondSemester != null)
                {
                    viewModel.SecondSemesterMinimumUnit = secondSemesterCourseUnit.MinimumUnit;
                    viewModel.SecondSemesterMaximumUnit = secondSemesterCourseUnit.MaximumUnit;
                }

                //if ((level.Id * 12) >= duration)
                //{
                //    viewModel.FirstSemesterMaximumUnit += 6;
                //    viewModel.SecondSemesterMaximumUnit += 6;
                //}

                StudentLogic studentLogic = new StudentLogic();
                if (studentLogic.IsStudentInFinalYear(departmemt, level, programme))
                {
                    viewModel.FirstSemesterMaximumUnit += 6;
                    viewModel.SecondSemesterMaximumUnit += 6;
                }


            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }
        private List<CourseRegistrationDetail> GetRegisteredCourse(CourseRegistration courseRegistration, List<Course> courses, Semester semester, List<CourseRegistrationDetail> registeredCourseDetails, CourseMode courseMode)
        {
            try
            {
                List<CourseRegistrationDetail> courseRegistrationDetails = null;
                if (registeredCourseDetails != null && registeredCourseDetails.Count > 0)
                {
                    if (courses != null && courses.Count > 0)
                    {
                        courseRegistrationDetails = new List<CourseRegistrationDetail>();
                        foreach (Course course in courses)
                        {
                            CourseRegistrationDetail registeredCourseDetail = registeredCourseDetails.Where(c => c.Course.Id == course.Id && c.Mode.Id == courseMode.Id).FirstOrDefault();
                            if (registeredCourseDetail != null && registeredCourseDetail.Id > 0)
                            {
                                registeredCourseDetail.Course.IsRegistered = true;
                                courseRegistrationDetails.Add(registeredCourseDetail);
                            }
                            else
                            {
                                var courseRegistrationDetail = new CourseRegistrationDetail();
                                courseRegistrationDetail.Course = course;
                                courseRegistrationDetail.Semester = semester;
                                courseRegistrationDetail.Course.IsRegistered = false;
                                courseRegistrationDetail.CourseUnit = course.Unit;
                                courseRegistrationDetail.Mode = courseMode;
                                courseRegistrationDetail.CourseRegistration = courseRegistration;
                                courseRegistrationDetails.Add(courseRegistrationDetail);
                            }
                        }
                    }
                    return courseRegistrationDetails;
                }

                courseRegistrationDetails = new List<CourseRegistrationDetail>();
                if (courses != null)
                {
                    foreach (Course course in courses)
                    {
                        CourseRegistrationDetail registeredCourseDetail =
                            registeredCourseDetails.Where(c => c.Course.Id == course.Id && c.Mode.Id == courseMode.Id)
                                .SingleOrDefault();
                        if (registeredCourseDetail != null && registeredCourseDetail.Id > 0)
                        {
                            registeredCourseDetail.Course.IsRegistered = true;
                            courseRegistrationDetails.Add(registeredCourseDetail);
                        }
                        else
                        {
                            var courseRegistrationDetail = new CourseRegistrationDetail();
                            courseRegistrationDetail.Course = course;
                            courseRegistrationDetail.Semester = semester;
                            courseRegistrationDetail.Course.IsRegistered = false;
                            //courseRegistrationDetail.Mode = new CourseMode() { Id = 1 };

                            courseRegistrationDetail.Mode = courseMode;
                            courseRegistrationDetail.CourseRegistration = courseRegistration;

                            courseRegistrationDetails.Add(courseRegistrationDetail);
                        }
                    }
                }
                return courseRegistrationDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private int SumSemesterSelectedCourseUnit(List<CourseRegistrationDetail> semesterRegisteredCourseDetails)
        {
            try
            {
                int totalRegisteredCourseUnit = 0;
                if (semesterRegisteredCourseDetails != null && semesterRegisteredCourseDetails.Count > 0)
                {
                    totalRegisteredCourseUnit = semesterRegisteredCourseDetails.Sum(c => c.Course.Unit);
                }

                return totalRegisteredCourseUnit;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private List<CourseRegistrationDetail> GetUnregisteredCourseDetail(List<Course> courses, Semester semester)
        {
            try
            {
                List<CourseRegistrationDetail> courseRegistrationDetails = null;
                if (courses != null && courses.Count > 0)
                {
                    courseRegistrationDetails = new List<CourseRegistrationDetail>();
                    foreach (Course course in courses)
                    {
                        var courseRegistrationDetail = new CourseRegistrationDetail();
                        courseRegistrationDetail.Course = course;
                        courseRegistrationDetail.CourseUnit = course.Unit;
                        courseRegistrationDetail.Semester = semester;
                        courseRegistrationDetail.Course.IsRegistered = false;
                        courseRegistrationDetail.Mode = new CourseMode { Id = 1 };
                        courseRegistrationDetails.Add(courseRegistrationDetail);
                    }
                }

                return courseRegistrationDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult CourseFormPrintOut(long sid, int semesterId, int sessionId)
        {
            try
            {
                PopulateCourseFormPrintOut(sid, semesterId, sessionId);
                if (semesterId == 1)
                {
                    viewModel.SecondSemesterCourses = new List<CourseRegistrationDetail>();
                }
                if (semesterId == 2)
                {
                    viewModel.FirstSemesterCourses = new List<CourseRegistrationDetail>();
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public void PopulateCourseFormPrintOut(long sid, int semesterId, int sessionId)
        {
            try
            {
                viewModel.Student = studentLogic.GetBy(sid);
                if (viewModel.Student != null && viewModel.Student.Id > 0)
                {
                    var studentLevelLogic = new StudentLevelLogic();
                    viewModel.StudentLevel = studentLevelLogic.GetBy(viewModel.Student.Id);
                    //Get the level of the student at the sessionId
                    if (viewModel.StudentLevel.Session.Id != sessionId)
                    {
                        var levelId = GetStudentPreviousLevelBy(viewModel.StudentLevel.Session.Id, sessionId, viewModel.StudentLevel.Level.Id);
                        LevelLogic levelLogic = new LevelLogic();
                        viewModel.StudentLevel.Level = new Level();
                        viewModel.StudentLevel.Level = levelLogic.GetModelBy(d => d.Level_Id == levelId);

                    }

                    var firstAttempt = new CourseMode { Id = 1 };
                    var carryOver = new CourseMode { Id = 2 };
                    var semseter = new Semester { Id = semesterId };
                    var sessionSemester = new SessionSemester();
                    var sessionSemesterLogic = new SessionSemesterLogic();
                    sessionSemester = sessionSemesterLogic.GetBySessionSemester(semseter.Id, sessionId);
                    var currentSessionSemesterLogic = new CurrentSessionSemesterLogic();
                    viewModel.CurrentSessionSemester = new CurrentSessionSemester { SessionSemester = sessionSemester };
                    CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                    if (semesterId == 1)
                    {
                        viewModel.FirstSemesterCourses = courseRegistrationDetailLogic.GetBy(viewModel.Student,
                            viewModel.CurrentSessionSemester.SessionSemester.Session, semseter,
                            new CourseMode() { Id = 1 });

                        viewModel.SumOfFirstSemesterSelectedCourseUnit = (int)
                            viewModel.FirstSemesterCourses.Sum(a => a.CourseUnit);

                        viewModel.FirstSemesterCarryOverCourses = courseRegistrationDetailLogic.GetBy(viewModel.Student,
                            viewModel.CurrentSessionSemester.SessionSemester.Session, semseter,
                            new CourseMode() { Id = 2 });

                    }
                    else
                    {
                        viewModel.SecondSemesterCourses = courseRegistrationDetailLogic.GetBy(viewModel.Student,
                            viewModel.CurrentSessionSemester.SessionSemester.Session, semseter,
                            new CourseMode() { Id = 1 });

                        viewModel.SumOfSecondSemesterSelectedCourseUnit = (int)
                            viewModel.SecondSemesterCourses.Sum(a => a.CourseUnit);

                        viewModel.SecondSemesterCarryOverCourses = courseRegistrationDetailLogic.GetBy(viewModel.Student,
                            viewModel.CurrentSessionSemester.SessionSemester.Session, semseter,
                            new CourseMode() { Id = 2 });
                    }

                    viewModel.CarryOverCourses = new List<CourseRegistrationDetail>();
                    if (viewModel.FirstSemesterCarryOverCourses != null && viewModel.FirstSemesterCarryOverCourses.Count > 0)
                    {
                        viewModel.CarryOverCourses.AddRange(viewModel.FirstSemesterCarryOverCourses);
                        viewModel.TotalFirstSemesterCarryOverCourseUnit = (int)viewModel.FirstSemesterCarryOverCourses.Sum(a => a.CourseUnit);
                    }
                    if (viewModel.SecondSemesterCarryOverCourses != null && viewModel.SecondSemesterCarryOverCourses.Count > 0)
                    {
                        viewModel.CarryOverCourses.AddRange(viewModel.SecondSemesterCarryOverCourses);
                        viewModel.TotalSecondSemesterCarryOverCourseUnit = (int)viewModel.SecondSemesterCarryOverCourses.Sum(a => a.CourseUnit);

                    }



                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}