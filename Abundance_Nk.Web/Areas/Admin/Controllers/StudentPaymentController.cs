using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class StudentPaymentController : BaseController
    {

        public ActionResult Index()
        {
            StudentPaymentViewModel viewModel = new StudentPaymentViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(Payment viewmodel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    Payment payment = InvalidInvoiceNumber(viewmodel.InvoiceNumber);
                    if (payment == null || payment.Id < 0)
                    {
                        return RedirectToAction("Index", viewmodel);
                    }
                    StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                    StudentPayment studentPayment = studentPaymentLogic.GetModelBy(sp => sp.Payment_Id == payment.Id && sp.Person_Id == payment.Person.Id && sp.Session_Id == payment.Session.Id);
                    StudentPaymentViewModel studentPaymentViewModel = new StudentPaymentViewModel();
                   
                    if (studentPayment != null)
                    {
                        studentPaymentViewModel.Payment_Id = studentPayment.Id;
                        studentPaymentViewModel.person = payment.Person;
                        studentPaymentViewModel.Person_Id = payment.Person.Id;
                        studentPaymentViewModel.Level_Id = studentPayment.Level.Id;
                        studentPaymentViewModel.Level = studentPayment.Level;
                        studentPaymentViewModel.FullName = payment.Person.FullName;
                        studentPaymentViewModel.Session_Id = studentPayment.Session.Id;
                        studentPaymentViewModel.Session = studentPayment.Session;
                        studentPaymentViewModel.Amount = studentPayment.Amount;
                    }
                    else
                    {
                        SetMessage("The Invoice Number does not have a co-responding payment record", Message.Category.Error);
                        
                    }

                    TempData["studentPaymentViewModel"] = studentPaymentViewModel;
                    return RedirectToAction("EditStudentPayment", "StudentPayment");

                }

            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction("Index", viewmodel);
        }

        public ActionResult EditStudentPayment()
        {
            StudentPaymentViewModel studentPaymentViewModel = (StudentPaymentViewModel)TempData["studentPaymentViewModel"];
            try
            {

                if (studentPaymentViewModel != null)
                {
                    ViewBag.Level = new SelectList(studentPaymentViewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT, studentPaymentViewModel.Level.Id);
                    ViewBag.Session = new SelectList(studentPaymentViewModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT, studentPaymentViewModel.Session.Id);
                  
                    TempData["viewModel"] = studentPaymentViewModel;
                    return View(studentPaymentViewModel);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Index", "StudentPayment");
        }

        [HttpPost]
        public ActionResult EditPayment(StudentPayment viewModel)
        {

            StudentPaymentViewModel studentPaymentViewModel = (StudentPaymentViewModel)TempData["viewModel"];
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                {

                    if (studentPaymentViewModel != null)
                    {
                        StudentPayment studentPayment = new StudentPayment();
                        StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();

                        studentPayment.Id = studentPaymentViewModel.Payment_Id;
                        studentPayment.Person = new Person() { Id = studentPaymentViewModel.Person_Id };
                        studentPayment.Amount = viewModel.Amount;
                        studentPayment.Level = new Level() { Id = viewModel.Level.Id };
                        studentPayment.Session = new Session() { Id = viewModel.Session.Id };

                        studentPayment = studentPaymentLogic.Modify(studentPayment);
                        if (studentPayment != null  && studentPayment.Id > 0)
                        {
                            StudentPaymentAudit studentPaymentAudit = new StudentPaymentAudit();
                            var userLogic = new UserLogic();
                            User user = userLogic.GetModelBy(p => p.User_Name == User.Identity.Name);
                            studentPaymentAudit.Operation = "Modify Payment For" +" "+ studentPayment.Student.FullName;
                            studentPaymentAudit.Action = "Modified Student Payment (StudentPayment controller)";
                            studentPaymentAudit.Client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                            studentPaymentAudit.User = user;
                            studentPaymentAudit.StudentPayment = new StudentPayment(){Id = studentPaymentViewModel.Payment_Id};
                            studentPaymentAudit.Person = new Person() { Id = studentPaymentViewModel.Person_Id };
                            studentPaymentAudit.Level = new Level() { Id = studentPayment.Level.Id };
                            studentPaymentAudit.Session = new Session() { Id = studentPayment.Session.Id };
                            studentPaymentAudit.Amount = studentPayment.Amount;
                            studentPaymentAudit.Status = studentPayment.Status;
                            studentPaymentAudit.Time = DateTime.Now;
                            CreateStudentPaymentAudit(studentPaymentAudit,studentPaymentViewModel);

                            SetMessage("Operation Successful", Message.Category.Information);
                        }
                    }
                    
                    transaction.Complete();
                }


            }
            catch (Exception)
            {

                throw;
            }
            return View("Index");
        }
        public Payment InvalidInvoiceNumber(string InvoiceNumber)
        {
            Payment studentPayment = new Payment();
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                studentPayment = paymentLogic.GetBy(InvoiceNumber);
                if (studentPayment != null)
                {
                    return studentPayment;
                }


                else
                {
                    SetMessage("Invalid Invoice Number Kindly Confirm the Invoice Number  ", Message.Category.Information);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return studentPayment;
        }

        private void CreateStudentPaymentAudit(StudentPaymentAudit studentPaymentAudit, StudentPaymentViewModel viewModel)
        {
            try
            {
                if (studentPaymentAudit != null)
                {
                    StudentPaymentAudit audit = new StudentPaymentAudit();
                    StudentPaymentAuditLogic auditLogic = new StudentPaymentAuditLogic();

                    
                    audit.StudentPayment = studentPaymentAudit.StudentPayment;
                    audit.Person = studentPaymentAudit.Person;
                    audit.Id = studentPaymentAudit.Id;
                    audit.Session = studentPaymentAudit.Session ;
                    audit.Level = studentPaymentAudit.Level;
                    audit.Amount = studentPaymentAudit.Amount;
                    audit.Status = studentPaymentAudit.Status;
                    audit.OldPerson = viewModel.person;
                    audit.OldSession = viewModel.Session;
                    audit.OldLevel = viewModel.Level;
                    audit.OldAmount = viewModel.Amount;
                    audit.OldStatus = viewModel.Status;
                    audit.Operation = studentPaymentAudit.Operation;
                    audit.Action = studentPaymentAudit.Action;
                    audit.Client = studentPaymentAudit.Client;
                    audit.User = studentPaymentAudit.User;
                    audit.Time = studentPaymentAudit.Time;

                    auditLogic.Create(audit);


                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}