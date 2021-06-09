using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private Model.Model.Student _Student;
        private Model.Model.StudentLevel _StudentLevel;
        private StudentLevelLogic studentLevelLogic;
        private StudentLogic studentLogic;
        public HomeController()
        {
            try
            {
                if (System.Web.HttpContext.Current.Session["student"] != null)
                {
                    studentLogic = new StudentLogic();
                    _Student =  System.Web.HttpContext.Current.Session["student"] as Model.Model.Student ;
                    _Student = studentLogic.GetBy(_Student.Id);
                    studentLevelLogic = new StudentLevelLogic();
                    _StudentLevel = studentLevelLogic.GetBy(_Student.Id);
                }
                else
                {
                     FormsAuthentication.SignOut();
                     System.Web.HttpContext.Current.Response.Redirect("/Security/Account/Login");

                }
               
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        // GET: Student/Home
        public ActionResult Index()
        {
            try
            {
                Model.Model.Student currentStudent = System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;
                currentStudent = studentLogic.GetBy(currentStudent.Id);
                ViewBag.Email = StudentEmail(currentStudent.Id);
                
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return View();
        }
        public ActionResult Profile()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                   return RedirectToAction("Form", "Registration", new {Area = "Student", sid = Utility.Encrypt(_Student.Id.ToString()),pid = Utility.Encrypt(_StudentLevel.Programme.Id.ToString())});
                }
                else
                {
                     FormsAuthentication.SignOut();
                     RedirectToAction("Login","Account",new { Area = "Security" });
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return RedirectToAction("Index");
        }
        public ActionResult Fees()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                     return RedirectToAction("GenerateInvoice", "Payment", new {Area = "Student", sid = Utility.Encrypt(_Student.Id.ToString())});
              
                }
                else
                {
                     FormsAuthentication.SignOut();
                     RedirectToAction("Login","Account",new { Area = "Security" });
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return RedirectToAction("Index");
        }
        public ActionResult PaymentHistory()
        {
            var paymentHistory = new PaymentHistory();
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    var paymentLogic = new PaymentLogic();
                    
                    //paymentHistory.Payments = paymentLogic.GetBy(_Student);
                    //List<PaymentView> paystackPayments = paymentLogic.GetPaystackPaymentBy(_Student);
                    //List<PaymentView> paymentInterswitch = paymentLogic.GetInterswitchPaymentBy(_Student);
                    //List<PaymentView> paymentMonnify = paymentLogic.GetMonnifyPaymentBy(_Student);
                    paymentHistory.Payments=paymentLogic.GETRemitaPayment(_Student);

                    //List<PaymentView> manualPayment = paymentLogic.GetManualPaymentBy(_Student);

                    //paymentHistory.Payments = paymentHistory.Payments.Where(p => p.ConfirmationOrderNumber != null).ToList();
                    //paymentHistory.Payments.AddRange(paystackPayments);
                    //paymentHistory.Payments.AddRange(paymentInterswitch);
                    //paymentHistory.Payments.AddRange(paymentMonnify);
                    //if (manualPayment != null &&  manualPayment.Count > 0)
                    //{
                    //    paymentHistory.Payments.AddRange(manualPayment);
                    //}

                    

                    paymentHistory.Student = _Student;
                }
                else
                {
                     FormsAuthentication.SignOut();
                     RedirectToAction("Login","Account",new { Area = "Security" });
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return View(paymentHistory);
        }
        public ActionResult CourseRegistration()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                   return RedirectToAction("CourseRegistration", "Registration", new {Area = "Student", sid = Utility.Encrypt(_Student.Id.ToString()),pid = Utility.Encrypt(_StudentLevel.Programme.Id.ToString())});
              
                }
                else
                {
                     FormsAuthentication.SignOut();
                     RedirectToAction("Login","Account",new { Area = "Security" });
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return RedirectToAction("Index");
        }
        public ActionResult Result()
        {
             try
            {
                 if (_Student != null && _StudentLevel != null)
                {
                     return RedirectToAction("Check", "Result", new {Area = "Student", sid = Utility.Encrypt(_Student.Id.ToString())});
              
                }
                 else
                {
                     FormsAuthentication.SignOut();
                     RedirectToAction("Login","Account",new { Area = "Security" });
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return RedirectToAction("Index");
        }
        public ActionResult ChangePassword()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    StudentViewModel studentViewModel = new StudentViewModel();
                    studentViewModel.Student = _Student;
                    return View(studentViewModel);
                }
                else
                {
                     FormsAuthentication.SignOut();
                     RedirectToAction("Login","Account",new { Area = "Security" });
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return View();
        } 
        [HttpPost]
        public ActionResult ChangePassword(StudentViewModel studentViewModel)
        {
            try
            {
                ModelState.Remove("Student.FirstName");
                ModelState.Remove("Student.LastName");
                ModelState.Remove("Student.MobilePhone");
                if(ModelState.IsValid)
                {
                    var studentLogic = new StudentLogic();
                    var LoggedInUser = new Model.Model.Student();
                    LoggedInUser = studentLogic.GetModelBy(
                            u =>
                                u.Matric_Number == studentViewModel.Student.MatricNumber &&
                                u.Password_hash == studentViewModel.OldPassword);
                    if(LoggedInUser != null)
                    {
                        LoggedInUser.PasswordHash = studentViewModel.NewPassword;
                        studentLogic.ChangeUserPassword(LoggedInUser);
                        TempData["Message"] = "Password Changed successfully! Please keep password in a safe place";
                        return RedirectToAction("Index","Home",new { Area = "Student" });
                    }
                    SetMessage("Please log off and log in then try again.",Message.Category.Error);

                    return View(studentViewModel);
                }
            }
            catch(Exception)
            {
                throw;
            }
            return View();
        }

        public ActionResult PayFees()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                   return RedirectToAction("Logon", "Registration", new {Area = "Student"});
                }
                else
                {
                     FormsAuthentication.SignOut();
                     RedirectToAction("Login","Account",new { Area = "Security" });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Index");
        }

        public ActionResult OtherFees()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                     return RedirectToAction("OldFees", "Payment", new {Area = "Student", Detail = Utility.Encrypt(_Student.Id.ToString())});
              
                }
                else
                {
                     FormsAuthentication.SignOut();
                     RedirectToAction("Login","Account",new { Area = "Security" });
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return RedirectToAction("Index");

        }

        public ActionResult PaymentReceipt()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("PrintReceipt", "Registration", new { Area = "Student" });
                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Index");


        }
        public string StudentEmail(long studentId)
        {
            PersonLogic personLogic = new PersonLogic();
            if (studentId > 0)
            {
                var person = personLogic.GetModelsBy(f => f.Person_Id == studentId).FirstOrDefault();
                if (person?.Id > 0)
                {
                    return person.Email;
                }
            }

            return null;
        }
        public JsonResult UpdateEmail(string emailAddress)
        {
            JsonResultModel result = new JsonResultModel();
            Model.Model.Student currentStudent = System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;
            try
            {
                if (currentStudent?.Id > 0)
                {
                    PersonLogic personLogic = new PersonLogic();
                    var person = personLogic.GetModelsBy(f => f.Person_Id == currentStudent.Id).FirstOrDefault();
                    if (person?.Id > 0)
                    {
                        person.Email = emailAddress;
                        personLogic.Modify(person);
                        result.IsError = false;
                        result.Message = "You have successfully Updated your email address!";
                    }
                }

            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}