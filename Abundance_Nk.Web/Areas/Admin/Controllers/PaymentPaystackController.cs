using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System.Transactions;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class PaymentPaystackController : BaseController
    {
        // GET: Admin/PaymentPaystack
        public ActionResult CreatePaystackCommission()
        {
            PaymentPaystackViewModel viewModel = new PaymentPaystackViewModel();
            ViewBag.Session = viewModel.SessionSelectListItem;
            ViewBag.FeeType = viewModel.FeeTypeSelectListItem;
            ViewBag.Fee = viewModel.FeeSelectListItem;
            ViewBag.Programme = viewModel.ProgrammeSelectListItem;
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult CreatePaystackCommission(PaymentPaystackViewModel commission)
        {
           
            try
            {
                var client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var operation = "Create";
                PaymentPaystackCommission created = new PaymentPaystackCommission();
                PaymentPayStackCommissionLogic commissionLogic = new PaymentPayStackCommissionLogic();
                PaymentPayStackCommissionAuditLogic paymentPayStackCommissionAuditLogic = new PaymentPayStackCommissionAuditLogic();
                PaymentPaystackCommissionAudit paymentPaystackCommissionAudit = new PaymentPaystackCommissionAudit();
                UserLogic userLogic = new UserLogic();
                if (commission != null  && commission.PaymentPaystackCommission.FeeType.Id > 0 
                    && commission.PaymentPaystackCommission.Session.Id > 0)
                {
                    var checkifExist =
                        commissionLogic.GetModelsBy(
                            s => s.FeeType_Id == commission.PaymentPaystackCommission.FeeType.Id &&
                                 s.Session_Id == commission.PaymentPaystackCommission.Session.Id && s.Programme_Id==commission.PaymentPaystackCommission.Programme.Id).LastOrDefault();
                    if (checkifExist == null )
                    {
                        using (var transaction = new TransactionScope())
                        {
                            var user= userLogic.GetModelBy(u => u.User_Name == User.Identity.Name);
                            commission.PaymentPaystackCommission.User = new Model.Model.User();
                            commission.PaymentPaystackCommission.User = user;
                            created = commissionLogic.Create(commission.PaymentPaystackCommission);
                            if (created != null)
                            {
                                paymentPaystackCommissionAudit.Amount = commission.PaymentPaystackCommission.Amount;
                                paymentPaystackCommissionAudit.Client = client;
                                paymentPaystackCommissionAudit.Programme = commission.PaymentPaystackCommission.Programme;
                                paymentPaystackCommissionAudit.FeeType = commission.PaymentPaystackCommission.FeeType;
                                paymentPaystackCommissionAudit.PaymentPaystackCommission = created;
                                paymentPaystackCommissionAudit.Session = commission.PaymentPaystackCommission.Session;
                                paymentPaystackCommissionAudit.User = userLogic.GetModelBy(u => u.User_Name == User.Identity.Name);
                                paymentPaystackCommissionAudit.Operation = operation;
                                paymentPaystackCommissionAudit.Action = "Created Commission";
                                paymentPayStackCommissionAuditLogic.Create(paymentPaystackCommissionAudit);
                            }
                            transaction.Complete();
                            
                        }

                        SetMessage(created != null ? "Operation Successful" : "Operation Failed", Message.Category.Information);
                        return RedirectToAction("ViewPaystackCommission", "PaymentPaystack", new {area = "Admin"});
                    }
                    SetMessage("Commission Already exist",Message.Category.Error);
                }
                else
                {
                    SetMessage("Error Could not Complete your Request", Message.Category.Information);
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            PopulatedDropDown();
            return View();
        }
        public ActionResult ViewPaystackCommission()
        {
            PaymentPaystackViewModel viewModel = new PaymentPaystackViewModel();
            try
            {
                PaymentPayStackCommissionLogic paymentPayStackCommissionLogic = new PaymentPayStackCommissionLogic();
                viewModel.PaymentPaystackCommissions = paymentPayStackCommissionLogic.GetAll();
                ViewBag.Session = viewModel.SessionSelectListItem;
                ViewBag.FeeType = viewModel.FeeTypeSelectListItem;
                ViewBag.Fee = viewModel.FeeSelectListItem;
                ViewBag.Programme = viewModel.ProgrammeSelectListItem;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(viewModel);
        }

        public JsonResult EditPaystackCommission(string Id ,string feeType, string addOn,  string session, string  activated,string amount, string programmeId)
        {
            string message;
            bool modifiedCommission = false;
            try
            {
                var client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var operation = "Modify";
                if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(feeType) && !string.IsNullOrEmpty(session) && !string.IsNullOrEmpty(amount) && !string.IsNullOrEmpty(programmeId))
                {
                    PaymentPayStackCommissionAuditLogic paymentPayStackCommissionAuditLogic = new PaymentPayStackCommissionAuditLogic();
                    PaymentPaystackCommissionAudit paymentPaystackCommissionAudit = new PaymentPaystackCommissionAudit();
                    UserLogic userLogic = new UserLogic();
                    PaymentPayStackCommissionLogic paymentCommissionLogic = new PaymentPayStackCommissionLogic();
                    PaymentPaystackCommission paymentPaystackCommission = new PaymentPaystackCommission();
                    paymentPaystackCommission.Session = new Session() { Id = Convert.ToInt32(session) };
                    paymentPaystackCommission.FeeType = new FeeType() { Id = Convert.ToInt32(feeType) };
                    paymentPaystackCommission.Programme = new Programme() { Id = Convert.ToInt32(programmeId) }; 
                    //paymentPaystackCommission.Fee =  new Fee() {Id = Convert.ToInt32(fee)};
                    paymentPaystackCommission.AddOnFee = addOn!=null? Convert.ToDecimal(addOn):0;
                    paymentPaystackCommission.Amount = Convert.ToDecimal(amount);
                    paymentPaystackCommission.Id = Convert.ToInt32(Id);
                    paymentPaystackCommission.Activated = Convert.ToBoolean(activated);

                    var checkIfExist = paymentCommissionLogic.GetModelBy(
                            s => s.FeeType_Id == paymentPaystackCommission.FeeType.Id &&
                                 s.Session_Id == paymentPaystackCommission.Session.Id  && s.Programme_Id==paymentPaystackCommission.Programme.Id);
                    if (checkIfExist!=null)
                    {
                        using (var transaction = new TransactionScope())
                        {
                            modifiedCommission = paymentCommissionLogic.Modify(paymentPaystackCommission);
                            if (modifiedCommission)
                            {
                                paymentPaystackCommissionAudit.Amount = Convert.ToDecimal(amount);
                                paymentPaystackCommissionAudit.Client = client;
                                paymentPaystackCommissionAudit.Programme = new Programme() { Id = Convert.ToInt32(programmeId) };
                                //paymentPaystackCommissionAudit.Fee = new Fee() { Id = Convert.ToInt32(fee) };
                                paymentPaystackCommissionAudit.FeeType = new FeeType() { Id = Convert.ToInt32(feeType) };
                                paymentPaystackCommissionAudit.PaymentPaystackCommission = checkIfExist;
                                paymentPaystackCommissionAudit.Session = new Session() { Id = Convert.ToInt32(session) };
                                paymentPaystackCommissionAudit.User = userLogic.GetModelBy(u => u.User_Name == User.Identity.Name);
                                paymentPaystackCommissionAudit.Operation = operation;
                                paymentPaystackCommissionAudit.Action = "Modified Commission";
                                paymentPayStackCommissionAuditLogic.Create(paymentPaystackCommissionAudit);
                            }
                            transaction.Complete();
                        }
                        
                        message = modifiedCommission ? "Operation Successful" : "Operation Failed";
                    }
                    else
                    {
                        message = " Error:  Commission Already exist";
                    }
                
                }
                else
                {
                    message = "Operation Failed";
                }
            }
            catch (Exception ex)
            {
                message = "Operation Failed" + ex;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletePaystackCommission(string commissionId)
        {
            string message;
            bool deleteCommission = false;
            try
            {
                var client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                var operation = "Delete";
                if (!string.IsNullOrEmpty(commissionId))
                {
                    PaymentPayStackCommissionAuditLogic paymentPayStackCommissionAuditLogic = new PaymentPayStackCommissionAuditLogic();
                    PaymentPaystackCommissionAudit paymentPaystackCommissionAudit = new PaymentPaystackCommissionAudit();
                    UserLogic userLogic = new UserLogic();
                    PaymentPayStackCommissionLogic paymentCommissionLogic = new PaymentPayStackCommissionLogic();
                    PaymentPaystackCommission paymentPaystackCommission = new PaymentPaystackCommission();

                    paymentPaystackCommission.Id = Convert.ToInt32(commissionId);

                    var checkIfExist = paymentCommissionLogic.GetModelBy(
                            s => s.Payment_PayStack_Commission_Id==paymentPaystackCommission.Id);
                    if (checkIfExist != null)
                    {
                        using (var transaction = new TransactionScope())
                        {
                            deleteCommission =paymentCommissionLogic.Delete(x=>x.Payment_PayStack_Commission_Id==checkIfExist.Id);
                            if (deleteCommission)
                            {
                                paymentPaystackCommissionAudit.Amount = checkIfExist.Amount;
                                paymentPaystackCommissionAudit.Client = client;
                                paymentPaystackCommissionAudit.Programme = checkIfExist.Programme;
                                //paymentPaystackCommissionAudit.Fee = new Fee() { Id = Convert.ToInt32(fee) };
                                paymentPaystackCommissionAudit.FeeType = checkIfExist.FeeType;
                                paymentPaystackCommissionAudit.PaymentPaystackCommission = checkIfExist;
                                paymentPaystackCommissionAudit.Session = checkIfExist.Session;
                                paymentPaystackCommissionAudit.User = userLogic.GetModelBy(u => u.User_Name == User.Identity.Name);
                                paymentPaystackCommissionAudit.Operation = operation;
                                paymentPaystackCommissionAudit.Action = "Delete Commission";
                                paymentPayStackCommissionAuditLogic.Create(paymentPaystackCommissionAudit);
                            }
                            transaction.Complete();
                        }

                        message = deleteCommission ? "Operation Successful" : "Operation Failed";
                    }
                    else
                    {
                        message = " Error:  Commission Already exist";
                    }

                }
                else
                {
                    message = "Operation Failed";
                }
            }
            catch (Exception ex)
            {
                message = "Operation Failed" + ex;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        private void PopulatedDropDown()
        {
            try
            {
                ViewBag.Session = Utility.PopulateSessionSelectListItem();
                ViewBag.FeeType = Utility.PopulateFeeTypeSelectListItem();
                ViewBag.Fee =     Utility.PopulateFeeSelectListItem();
                ViewBag.Programme = Utility.PopulateAllProgrammeSelectListItem();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
    }
}