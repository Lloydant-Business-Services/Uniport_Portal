using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Abundance_Nk.Web.Areas.PGStudent.Controllers
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
                    _Student = System.Web.HttpContext.Current.Session["student"] as Model.Model.Student;
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
            return View();
        }
        public ActionResult Profile()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("Form", "Registration", new { Area = "PGStudent", sid = Utility.Encrypt(_Student.Id.ToString()), pid = Utility.Encrypt(_StudentLevel.Programme.Id.ToString()) });
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
        public ActionResult Fees()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("GenerateInvoice", "Payment", new { Area = "PGStudent", sid = Utility.Encrypt(_Student.Id.ToString()) });

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
        public ActionResult PaymentHistory()
        {
            var paymentHistory = new PaymentHistory();
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    var paymentLogic = new PaymentLogic();

                    paymentHistory.Payments = paymentLogic.GetBy(_Student);
                    List<PaymentView> paystackPayments = paymentLogic.GetPaystackPaymentBy(_Student);
                    List<PaymentView> paymentInterswitch = paymentLogic.GetInterswitchPaymentBy(_Student);
                    List<PaymentView> manualPayment = paymentLogic.GetManualPaymentBy(_Student);

                    paymentHistory.Payments = paymentHistory.Payments.Where(p => p.ConfirmationOrderNumber != null).ToList();
                    paymentHistory.Payments.AddRange(paystackPayments);
                    paymentHistory.Payments.AddRange(paymentInterswitch);
                    
                    if (manualPayment != null && manualPayment.Count > 0)
                    {
                        paymentHistory.Payments.AddRange(manualPayment);

                        
                    }

                    //Check if student has payment history as undergraduate level in same institution
                    PersonMergerLogic personMergerLogic = new PersonMergerLogic();
                    PersonMerger personMerger=personMergerLogic.GetModelsBy(l => l.New_Person_Id == _Student.Id).LastOrDefault();
                    if (personMerger?.PersonMergerId > 0)
                    {
                        paymentHistory.PreviousPayments = paymentLogic.GetBy(personMerger.OldPerson);
                        List<PaymentView> previousPaystackPayments = paymentLogic.GetPaystackPaymentBy(_Student);
                        List<PaymentView> previousPaymentInterswitch = paymentLogic.GetInterswitchPaymentBy(_Student);
                        List<PaymentView> previousManualPayment = paymentLogic.GetManualPaymentBy(_Student);

                        paymentHistory.PreviousPayments = paymentHistory.PreviousPayments.Where(p => p.ConfirmationOrderNumber != null).ToList();
                        paymentHistory.PreviousPayments.AddRange(previousPaystackPayments);
                        paymentHistory.PreviousPayments.AddRange(previousPaymentInterswitch);
                        if (previousManualPayment != null && previousManualPayment.Count > 0)
                        {
                            paymentHistory.PreviousPayments.AddRange(previousManualPayment);
                        }
                    }

                    paymentHistory.Student = _Student;
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
            return View(paymentHistory);
        }
        public ActionResult CourseRegistration()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("CourseRegistration", "Registration", new { Area = "PGStudent", sid = Utility.Encrypt(_Student.Id.ToString()), pid = Utility.Encrypt(_StudentLevel.Programme.Id.ToString()) });

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
        public ActionResult Result()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("Check", "Result", new { Area = "PGStudent", sid = Utility.Encrypt(_Student.Id.ToString()) });

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
                    RedirectToAction("Login", "Account", new { Area = "Security" });
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
                if (ModelState.IsValid)
                {
                    var studentLogic = new StudentLogic();
                    var LoggedInUser = new Model.Model.Student();
                    LoggedInUser = studentLogic.GetModelBy(
                            u =>
                                u.Matric_Number == studentViewModel.Student.MatricNumber &&
                                u.Password_hash == studentViewModel.OldPassword);
                    if (LoggedInUser != null)
                    {
                        LoggedInUser.PasswordHash = studentViewModel.NewPassword;
                        studentLogic.ChangeUserPassword(LoggedInUser);
                        TempData["Message"] = "Password Changed successfully! Please keep password in a safe place";
                        return RedirectToAction("Index", "Home", new { Area = "PGStudent" });
                    }
                    SetMessage("Please log off and log in then try again.", Message.Category.Error);

                    return View(studentViewModel);
                }
            }
            catch (Exception)
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
                    return RedirectToAction("Logon", "Registration", new { Area = "PGStudent" });
                }
                else
                {
                    FormsAuthentication.SignOut();
                    RedirectToAction("Login", "Account", new { Area = "Security" });
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
                    return RedirectToAction("OldFees", "Payment", new { Area = "PGStudent", Detail = Utility.Encrypt(_Student.Id.ToString()) });

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

        public ActionResult PaymentReceipt()
        {
            try
            {
                if (_Student != null && _StudentLevel != null)
                {
                    return RedirectToAction("PrintReceipt", "Registration", new { Area = "PGStudent" });
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

    }
}