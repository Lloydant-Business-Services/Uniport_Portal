using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Areas.Security.Models;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;


namespace Abundance_Nk.Web.Areas.Security.Controllers
{
    public class AccountController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        [AllowAnonymous]
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            var manageUserviewModel = new ManageUserViewModel();

            try
            {
                ViewBag.UserId = User.Identity.Name;
                manageUserviewModel.Username = User.Identity.Name;
            }
            catch (Exception)
            {
                throw;
            }
            return View(manageUserviewModel);
        }

        [HttpPost]
        public ActionResult ChangePassword(ManageUserViewModel manageUserviewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userLogic = new UserLogic();
                    var LoggedInUser = new User();
                    LoggedInUser =
                        userLogic.GetModelBy(
                            u =>
                                u.User_Name == manageUserviewModel.Username &&
                                u.Password == manageUserviewModel.OldPassword);
                    if (LoggedInUser != null)
                    {
                        LoggedInUser.Password = manageUserviewModel.NewPassword;
                        if (manageUserviewModel.OldPassword != manageUserviewModel.NewPassword)
                        {
                            userLogic.ChangeUserPassword(LoggedInUser);
                            TempData["Message"] = "Password Changed successfully! Please keep password in a safe place";
                            return RedirectToAction("Home", "Account", new { Area = "Security" });
                        }
                        else
                        {
                            TempData["Message"] = "You cannot use the same password again!";
                            SetMessage("You cannot use the same password again!", Message.Category.Error);
                            return View(manageUserviewModel);
                        }

                    }
                    SetMessage("Please log off and log in then try again.", Message.Category.Error);

                    return View(manageUserviewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel viewModel, string returnUrl)
        {
            try
            {
                if (Regex.IsMatch(viewModel.UserName, @"^\d"))
                //if (viewModel.UserName.Contains("/"))
                {
                    StudentLogic studentLogic = new StudentLogic();
                    PersonMergerLogic personMergerLogic = new PersonMergerLogic();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    if (studentLogic.ValidateUser(viewModel.UserName, viewModel.Password))
                    {
                        FormsAuthentication.SetAuthCookie(viewModel.UserName, false);
                        var student = studentLogic.GetBy(viewModel.UserName);
                        //check if student has more than one student record
                        var newStudent = CheckForMultipleStudentRecord(viewModel.UserName);
                        if (newStudent?.Id > 0)
                        {
                            student = newStudent;
                        }
                        Session["student"] = student;
                        var studentLevel = studentLevelLogic.GetModelsBy(f => f.Person_Id == student.Id).LastOrDefault();
                        if (studentLevel?.Id > 0 && studentLevel.Department?.Id == 83)
                        {
                            //have not updated academic detail
                            return RedirectToAction("UpdateAcademicRecord", new { sid = Utility.Encrypt(studentLevel.Student.Id.ToString()) });
                        }
                        if (string.IsNullOrEmpty(returnUrl))
                        {
                            bool isPGStudent = false;
                            if (studentLevel?.Id > 0)
                            {

                                if (studentLevel.Programme.Name != null && studentLevel.Programme.Name.Contains("PG"))
                                {
                                    isPGStudent = true;
                                }

                            }
                            if (isPGStudent)
                            {
                                //return RedirectToAction("Index", "Home", new { Area = "PGStudent" });
                                return RedirectToAction("Index", "Home", new { Area = "Student" });
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home", new { Area = "Student" });
                            }

                        }
                        return RedirectToLocal(returnUrl);
                    }
                }
                else
                {
                    var userLogic = new UserLogic();
                    if (userLogic.ValidateUser(viewModel.UserName, viewModel.Password))
                    {
                        FormsAuthentication.SetAuthCookie(viewModel.UserName, false);
                        if (userLogic.isPasswordDueForChange(viewModel.UserName))
                        {
                            return RedirectToAction("ChangePassword");
                        }

                        if (string.IsNullOrEmpty(returnUrl))
                        {
                            return RedirectToAction("Home", "Account", new { Area = "Security" });
                        }
                        return RedirectToLocal(returnUrl);
                    }
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            SetMessage("Invalid Username or Password!", Message.Category.Error);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", new { Area = "Security" });
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult SignOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", new { Area = "Security" });
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home", new { Area = "" });
        }

        [AllowAnonymous]
        public ActionResult Unauthorized()
        {
            FormsAuthentication.SignOut();
            return View();
        }
        public Model.Model.Student CheckForMultipleStudentRecord(string matricNo)
        {
            StudentLogic studentLogic = new StudentLogic();
            Model.Model.Student student = new Model.Model.Student();
            PersonMergerLogic personMergerLogic = new PersonMergerLogic();
            try
            {
                var studentRecords = studentLogic.GetModelsBy(f => f.Matric_Number == matricNo);
                if (studentRecords?.Count > 1)
                {
                    foreach (var item in studentRecords)
                    {
                        var existMerge = personMergerLogic.GetModelsBy(f => f.New_Person_Id == item.Id).LastOrDefault();
                        if (existMerge?.PersonMergerId > 0)
                        {
                            student = item;
                        }
                    }
                }
                else
                {
                    if (studentRecords?.Count > 0 && studentRecords.FirstOrDefault().Id > 0)
                    {
                        student = studentRecords.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return student;
        }
        public ActionResult UpdateAcademicRecord(string sid)
        {
            PostJambViewModel model = new PostJambViewModel();
            if (string.IsNullOrEmpty(sid))
            {
                SetMessage("Student does not exist",Message.Category.Error);
                return RedirectToAction("SignOff");
            }
            long studentId = Convert.ToInt64(Utility.Decrypt(sid));
            ViewBag.StateId = model.StateSelectList;
            ViewBag.SexId = model.SexSelectList;
            ViewBag.ProgrammeId = model.ProgrammeSelectListItem;
            ViewBag.LevelId = model.LevelSelectListItem;
            ViewBag.ReligionId = model.ReligionSelectList;
            ViewBag.LgaId = new SelectList(new List<LocalGovernment>(), ID, NAME);
            ViewBag.DepartmentId = new SelectList(new List<Department>().OrderBy(D => D.Name), ID, NAME);
            PersonLogic personLogic = new PersonLogic();
            StudentLogic studentLogic = new StudentLogic();
            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
            model.Student=studentLogic.GetModelsBy(f => f.Person_Id == studentId).FirstOrDefault();
            model.StudentLevel = studentLevelLogic.GetModelsBy(f => f.Person_Id == studentId).FirstOrDefault();
            model.Person = personLogic.GetModelsBy(f => f.Person_Id == studentId).FirstOrDefault();
            if (model.StudentLevel?.Programme?.Id > 0)
            {
                var departmentsList = Utility.PopulateDepartmentSelectListItem(model.StudentLevel?.Programme);
                    ViewBag.DepartmentId = new SelectList(departmentsList, VALUE, TEXT);
            }
            if (model.Person?.State?.Id != null)
            {
               var lgas= Utility.PopulateLocalGovernmentSelectListItemByStateId(model.Person.State.Id);
                ViewBag.LgaId = new SelectList(lgas, VALUE, TEXT);
            }
            
            model.StudentId = studentId;
            return View(model);
        }
        [HttpPost]
        public ActionResult UpdateAcademicRecord(PostJambViewModel postJambViewModel)
        {
            if(postJambViewModel?.Student?.MatricNumber==null || string.IsNullOrEmpty(postJambViewModel?.Student?.MatricNumber))
            {
               return RedirectToAction("UpdateAcademicRecord", new { studentId = postJambViewModel.StudentId});
            }

            PersonLogic personLogic = new PersonLogic();
            StudentLogic studentLogic = new StudentLogic();
            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
            var student=studentLogic.GetModelsBy(f => f.Person_Id == postJambViewModel.StudentId).FirstOrDefault();
            if (student?.Id > 0)
            {
                studentLogic.Modify(postJambViewModel.Student);
                personLogic.Modify(postJambViewModel.Person);
                SessionLogic sessionLogic = new SessionLogic();
                var session=sessionLogic.GetModelsBy(f => f.Activated == true).LastOrDefault();
                if (session?.Id > 0)
                {
                    postJambViewModel.StudentLevel.Session = new Session() { Id = session.Id };
                }
                studentLevelLogic.ModifyIncludingDepartmentAndProgrammme(postJambViewModel.StudentLevel);
                SetMessage("Update was successful, you can now login with your matric no", Message.Category.Information);
                EmailModel emailModel = new EmailModel();
                emailModel.Username = postJambViewModel.Student.MatricNumber;
                emailModel.Password = student.PasswordHash;
                emailModel.FullName = postJambViewModel.Person.FirstName;
                var template = Server.MapPath("/Areas/Common/Views/Credential/LoginCredentialTemplate.cshtml");
                EmailMessage message = new EmailMessage();
                message.Email = postJambViewModel.Person.Email;
                message.Name = postJambViewModel.Person.FirstName;
                message.Subject = "Profile Update";
                emailModel.link = $"http://192.169.152.37";
                EmailSenderLogic<EmailModel> receiptEmailSenderLogic = new EmailSenderLogic<EmailModel>(template, emailModel);

                receiptEmailSenderLogic.Send(message);
                return RedirectToAction("SignOff");
            }

            return View(postJambViewModel);
        }
        public JsonResult GetDepartmentByProgrammeId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var programme = new Programme { Id = Convert.ToInt32(id) };

                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                return Json(new SelectList(departments, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetLocalGovernmentsByState(string id)
        {
            try
            {
                var lgaLogic = new LocalGovernmentLogic();
                List<LocalGovernment> lgas = lgaLogic.GetModelsBy(f => f.State_Id == id);
                return Json(new SelectList(lgas, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AllowAnonymous]
        public ActionResult UserLogin()
        {
            return View();
        }

    }
}