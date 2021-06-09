using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.PGApplicant.Controllers
{
    [AllowAnonymous]
    public class PGAdmissionController : BaseController
    {
        private AdmissionViewModel viewModel;

        public PGAdmissionController()
        {
            viewModel = new AdmissionViewModel();
        }
        public ActionResult PGCheckStatus()
        {
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult PGCheckStatus(AdmissionViewModel vModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var applicationFormLogic = new ApplicationFormLogic();
                    AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                    AdmissionList admissionList = admissionListLogic.GetBy(vModel.ApplicationForm.Number);
                    if (admissionList?.Id > 0)
                    {

                        return RedirectToAction("Index", new { fid = Utility.Encrypt(admissionList.Form.Id.ToString()) });
                    }
                    else
                    {

                        SetMessage("Sorry, your name is not yet on the admission list! Kindly check back later.", Message.Category.Error);

                    }

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! Unabled to validate your details, please contact ICS or Support", Message.Category.Error);
            }

            return View(vModel);
        }
        public ActionResult Index(string fid)
        {
            try
            {
                TempData["FormViewModel"] = null;
                Int64 formId = Convert.ToInt64(Utility.Decrypt(fid));
                viewModel.GetApplicationBy(formId);
                PopulateAllDropDowns(viewModel);
                viewModel.GetOLevelResultBy(formId);
                SetSelectedSittingSubjectAndGrade(viewModel);

                //check that scratch card was used
                //if (viewModel.ApplicationForm.Setting.Session.Id == (int)Sessions.CurrentSession && viewModel.AppliedCourse.Programme.Id == 1)
                //{
                //    ScratchCardLogic scratchCardLogic = new ScratchCardLogic();
                //    if (!scratchCardLogic.DidUserBuyCard(viewModel.ApplicationForm.Person.Id, new ScratchCardBatch { Id = 2 }))
                //    {
                //        PaymentLogic paymentLogic = new PaymentLogic();
                //        var payment = paymentLogic.GetBy(viewModel.AppliedCourse.Person, new FeeType { Id = (int)FeeTypes.AdmissionChecking });
                //        if (payment != null && payment.Id > 0)
                //        {
                //            PaystackLogic paystackLogic = new PaystackLogic();
                //            if (!paystackLogic.ValidateAmountPaid(payment.InvoiceNumber, 210000))
                //            {
                //                return RedirectToAction("CheckStatus");
                //            }
                //        }
                //    }



                //}

            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        private void PopulateAllDropDowns(AdmissionViewModel existingViewModel)
        {
            try
            {
                if (existingViewModel.Loaded)
                {
                    if (existingViewModel.FirstSittingOLevelResult.Type == null)
                    {
                        existingViewModel.FirstSittingOLevelResult.Type = new OLevelType();
                    }

                    ViewBag.FirstSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList,
                        Utility.VALUE, Utility.TEXT, existingViewModel.FirstSittingOLevelResult.Type.Id);
                    ViewBag.FirstSittingExamYearId = new SelectList(existingViewModel.ExamYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.FirstSittingOLevelResult.ExamYear);
                    ViewBag.SecondSittingExamYearId = new SelectList(existingViewModel.ExamYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.SecondSittingOLevelResult.ExamYear);

                    if (existingViewModel.SecondSittingOLevelResult.Type != null)
                    {
                        ViewBag.SecondSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList,
                            Utility.VALUE, Utility.TEXT, existingViewModel.SecondSittingOLevelResult.Type.Id);
                    }
                    else
                    {
                        ViewBag.SecondSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList,
                            Utility.VALUE, Utility.TEXT, 0);
                    }
                    ViewBag.PaymentModes = viewModel.PaymentModeSelectListItem;
                    SetSelectedSittingSubjectAndGrade(existingViewModel);
                }
                else
                {
                    viewModel = new AdmissionViewModel();

                    ViewBag.FirstSittingOLevelTypeId = viewModel.OLevelTypeSelectList;
                    ViewBag.SecondSittingOLevelTypeId = viewModel.OLevelTypeSelectList;
                    ViewBag.FirstSittingExamYearId = viewModel.ExamYearSelectList;
                    ViewBag.SecondSittingExamYearId = viewModel.ExamYearSelectList;

                    //SetDefaultSelectedSittingSubjectAndGrade(viewModel);
                    ViewBag.PaymentModes = viewModel.PaymentModeSelectListItem;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }
        private void SetSelectedSittingSubjectAndGrade(AdmissionViewModel olevelViewModel)
        {
            try
            {
                if (olevelViewModel != null && olevelViewModel.FirstSittingOLevelResultDetails != null &&
                    olevelViewModel.FirstSittingOLevelResultDetails.Count > 0)
                {
                    int i = 0;
                    foreach (
                        OLevelResultDetail firstSittingOLevelResultDetail in
                            olevelViewModel.FirstSittingOLevelResultDetails)
                    {
                        if (firstSittingOLevelResultDetail.Subject != null &&
                            firstSittingOLevelResultDetail.Grade != null)
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(olevelViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT,
                                    firstSittingOLevelResultDetail.Subject.Id);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(olevelViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT,
                                    firstSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(olevelViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, 0);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(olevelViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, 0);
                        }

                        i++;
                    }
                }

                if (olevelViewModel != null && olevelViewModel.SecondSittingOLevelResultDetails != null &&
                    olevelViewModel.SecondSittingOLevelResultDetails.Count > 0)
                {
                    int i = 0;
                    foreach (
                        OLevelResultDetail secondSittingOLevelResultDetail in
                            olevelViewModel.SecondSittingOLevelResultDetails)
                    {
                        if (secondSittingOLevelResultDetail.Subject != null &&
                            secondSittingOLevelResultDetail.Grade != null)
                        {
                            ViewData["SecondSittingOLevelSubjectId" + i] =
                                new SelectList(olevelViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT,
                                    secondSittingOLevelResultDetail.Subject.Id);
                            ViewData["SecondSittingOLevelGradeId" + i] =
                                new SelectList(olevelViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT,
                                    secondSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            ViewData["SecondSittingOLevelSubjectId" + i] =
                                new SelectList(olevelViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, 0);
                            ViewData["SecondSittingOLevelGradeId" + i] =
                                new SelectList(olevelViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, 0);
                        }

                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }
        }
        [HttpPost]
        public ActionResult GenerateAcceptanceInvoice(string applicationFormNo)
        {

            Payment payment = null;
            try
            {
                ApplicationForm form = viewModel.GetApplicationFormBy(applicationFormNo);
                if (form?.Id > 0)
                {

                    PaymentMode paymentMode = new PaymentMode { Id = (int)PaymentModes.FullInstallment };
                    var appliedCourse = new AppliedCourse();
                    var appliedCourseLogic = new AppliedCourseLogic();
                    appliedCourse = appliedCourseLogic.GetBy(form.Person);
                    var feeType = new FeeType { Id = (int)FeeTypes.AcceptanceFee };
                    var status = ApplicantStatus.Status.GeneratedAcceptanceInvoice;
                    using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    {
                        payment = GenerateInvoiceHelperAsync(form, feeType, paymentMode, status).Result;
                        if (payment != null)
                        {
                            transaction.Complete();
                        }
                        else
                        {
                            transaction.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                return Json(new { }, "text/html", JsonRequestBehavior.AllowGet);
            }

            return Json(new { payment.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> GenerateAcceptanceInvoiceAsync(string applicationFormNo)
        {

            Payment payment = null;
            try
            {
                ApplicationForm form = viewModel.GetApplicationFormBy(applicationFormNo);
                if (form?.Id > 0)
                {

                    PaymentMode paymentMode = new PaymentMode { Id = (int)PaymentModes.FullInstallment };
                    var appliedCourse = new AppliedCourse();
                    var appliedCourseLogic = new AppliedCourseLogic();
                    appliedCourse = appliedCourseLogic.GetBy(form.Person);

                    var feeType = new FeeType { Id = (int)FeeTypes.AcceptanceFee };
                    var status = ApplicantStatus.Status.GeneratedAcceptanceInvoice;
                    //using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    //{
                    payment = await GenerateInvoiceHelperAsync(form, feeType, paymentMode, status);
                    if (payment != null)
                    {
                        // transaction.Complete();
                    }
                    else
                    {
                        SetMessage("Error Occurred! ", Message.Category.Error);
                        //transaction.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                return Json(new { }, "text/html", JsonRequestBehavior.AllowGet);
            }

            return Json(new { payment.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GenerateSchoolFeesInvoice(string formNo, string paymentModeId)
        {
            Payment payment = null;
            var feeType = new FeeType { Id = (int)FeeTypes.SchoolFees };
            PaymentMode paymentMode = new PaymentMode() { Id = Convert.ToInt16(paymentModeId) };

            try
            {
                ApplicationForm form = viewModel.GetApplicationFormBy(formNo);
                if (form?.Id>0)
                {

                    PaymentLogic paymentLogic = new PaymentLogic();
                    //Check if student is in admission list 
                    var list = new AdmissionList();
                    var listLogic = new AdmissionListLogic();
                    list = listLogic.GetBy(form.Id);
                    if (list?.Id > 0)
                    {
                        //Check if Acceptance has been paid.
                        if (!paymentLogic.HasPaidAcceptance(form.Person))
                        {
                            return Json("Acceptance fee is required before proceeding! Contact ICS or admin", "text/html", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json("Cannot validate your admission status! Contact ICS or admin", "text/html", JsonRequestBehavior.AllowGet);
                    }

                    payment = paymentLogic.GetBy(feeType, form.Person, form.Setting.Session);
                    if (payment != null)
                    {
                        if (payment.PaymentMode.Id == paymentMode.Id)
                        {
                            return Json(new { InvoiceNumber = payment.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            payment.PaymentMode = paymentMode;
                            payment = paymentLogic.Modify(payment);
                            return Json(new { InvoiceNumber = payment.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        ApplicantStatus.Status status = ApplicantStatus.Status.GeneratedSchoolFeesInvoice;

                        //Change

                        payment = GenerateInvoiceHelperAsync(form, feeType, paymentMode, status).Result;
                        return Json(new { InvoiceNumber = payment.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return Json(new { payment.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> GenerateSchoolFeesInvoiceAsync(string formNo, string paymentModeId)
        {
            Payment payment = null;
            var feeType = new FeeType { Id = (int)FeeTypes.SchoolFees };
            PaymentMode paymentMode = new PaymentMode() { Id = Convert.ToInt16(paymentModeId) };

            try
            {

                PaymentLogic paymentLogic = new PaymentLogic();
                ApplicationForm form = viewModel.GetApplicationFormBy(formNo);
                if (form != null && form.Id > 0)
                {
                    //Check if student is in admission list 
                    var list = new AdmissionList();
                    var listLogic = new AdmissionListLogic();
                    list = listLogic.GetBy(form.Id);
                    if (list?.Id > 0)
                    {
                        //Check if Acceptance has been paid.
                        if (!paymentLogic.HasPaidAcceptance(form.Person))
                        {
                            return Json("Acceptance fee is required before proceeding! Contact ICS or admin", "text/html", JsonRequestBehavior.AllowGet);

                        }
                    }
                    else
                    {
                        return Json("Cannot validate your admission status! Contact ICS or admin", "text/html", JsonRequestBehavior.AllowGet);
                    }
                    payment = paymentLogic.GetBy(feeType, form.Person, form.Setting.Session);
                    if (payment?.Id > 0 )
                    {
                        if (payment.PaymentMode.Id == paymentMode.Id)
                        {
                            return Json(new { InvoiceNumber = payment.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            payment.PaymentMode = paymentMode;
                            payment = paymentLogic.Modify(payment);
                            return Json(new { InvoiceNumber = payment.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        ApplicantStatus.Status status = ApplicantStatus.Status.GeneratedSchoolFeesInvoice;

                        //Change

                        payment = await GenerateInvoiceHelperAsync(form, feeType, paymentMode, status);
                        return Json(new { InvoiceNumber = payment.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return Json(new { payment.InvoiceNumber }, "text/html", JsonRequestBehavior.AllowGet);
        }

        private async Task<Payment> GenerateInvoiceHelperAsync(ApplicationForm form, FeeType feeType, PaymentMode paymentMode, ApplicantStatus.Status status)
        {
            try
            {

                Payment payment = viewModel.GenerateInvoice(form, status, feeType, paymentMode);

                if (payment == null)
                {
                    SetMessage("Operation Failed! Invoice could not be generated. Refresh browser & try again",
                        Message.Category.Error);
                }
                StudentPayment studentPayment = CreateStudentPaymentLog(payment);
                viewModel.Invoice.Payment = payment;
                viewModel.Invoice.Person = form.Person;

                if (payment.FeeType.Id == (int)FeeTypes.AcceptanceFee)
                {
                    viewModel.AcceptanceInvoiceNumber = payment.InvoiceNumber;

                }
                else if (payment.FeeType.Id == (int)FeeTypes.SchoolFees)
                {
                    viewModel.SchoolFeesInvoiceNumber = payment.InvoiceNumber;
                    viewModel.PaymentMode = payment.PaymentMode;
                }

                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult GenerateAcceptanceReceipt(long fid, string ivn, string con, int st)
        {
            try
            {
                string successMeassge =
                    "Acceptance Receipt has been successfully generated and ready for printing. Print the Acceptance Receipt and Acceptance Letter by clicking on the Print Receipt or Print Acceptance Letter button.";
                if (!GenerateReceiptHelper(fid, ivn, con, st, successMeassge))
                {
                    return PartialView("_Message",
                        "Confirmation order number seems not to be valid! Please crosscheck and try again");
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }

            return PartialView("_Message", TempData["Message"]);
        }
        public ActionResult GenerateSchoolFeesReceipt(long fid, string ivn, string con, int st)
        {

            try
            {
                string successMeassge =
                    "School Fees Receipt has been successfully generated and ready for printing. Click on the Print Receipt button to print receipt.";
                var applicantLogic = new ApplicantLogic();

                //using (TransactionScope transaction = new TransactionScope())
                //{
                bool isSuccessfull = GenerateReceiptHelper(fid, ivn, con, st, successMeassge);
                if (isSuccessfull)
                {
                    //assign matric number

                    ApplicationFormView applicant = applicantLogic.GetBy(fid);
                    if (applicant != null && (applicant.ProgrammeId != (int)Programmes.Undergraduate && applicant.ProgrammeId != (int)Programmes.IAS))
                    {
                       
                        var studentLogic = new StudentLogic();
                        bool matricNoAssigned = studentLogic.NewAssignPGMatricNumber(applicant);
                        if (matricNoAssigned)
                        {
                            //  transaction.Complete();

                        }
                    }
                }

                //}

            }
            catch (Exception ex)
            {

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }


            return PartialView("_Message", TempData["Message"]);
        }
        private bool GenerateReceiptHelper(long fid, string ivn, string con, int st, string successMeassge)
        {
            try
            {
                //var session = new Session { Id = (int)Sessions.CurrentSession };
                SessionLogic sessionLogic = new SessionLogic();
                var session=sessionLogic.GetModelsBy(g => g.Active_Application).LastOrDefault();
                FeeType feetype;
                if (st <= 4)
                {
                    feetype = new FeeType { Id = (int)FeeTypes.AcceptanceFee };
                }
                else
                {
                    feetype = new FeeType { Id = (int)FeeTypes.SchoolFees };
                }
                var paymentLogic = new PaymentLogic();
                Payment payment = paymentLogic.InvalidConfirmationOrderNumber(con, session, feetype);
                if (payment != null && payment.Id > 0)
                {
                    if (payment.InvoiceNumber == ivn)
                    {
                        Receipt receipt = GetReceipt(ivn, fid, st);
                        if (receipt != null)
                        {
                            SetMessage(successMeassge, Message.Category.Information);
                            return true;
                        }
                    }
                    else
                    {
                        SetMessage(
                            "Your Acceptance Receipt generation failed because the Confirmation Order Number (" + con +
                            ") entered belongs to another invoice number! Please enter your Confirmation Order Number correctly.",
                            Message.Category.Error);
                    }
                }

                SetMessage(
                    "Your Receipt generation failed because the Confirmation Number (" + con +
                    ") entered could not be verified. Please confirm and try again", Message.Category.Error);
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private Receipt GetReceipt(string ivn, long fid, int st)
        {
            Receipt receipt = null;

            try
            {
                var status = (ApplicantStatus.Status)st;

                ApplicationForm applicationForm = new ApplicationForm() { Id = fid };
                StudentLogic studentLogic = new StudentLogic();
                Model.Model.Student student = studentLogic.GetBy(applicationForm);
                if (student != null)
                {
                    student.Activated = true;
                    studentLogic.Modify(student);
                }

                if (IsNextApplicationStatus(fid, st))
                {
                    receipt = viewModel.GenerateReceipt(ivn, fid, status);
                }
                else
                {
                    receipt = viewModel.GetReceiptBy(ivn);
                }

                if (receipt == null)
                {
                    SetMessage("No receipt found for Invoice No " + ivn, Message.Category.Error);
                }
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult Invoice(string ivn)
        {
            try
            {
                if (string.IsNullOrEmpty(ivn))
                {
                    SetMessage("Invoice Not Found! Refresh and Try again ", Message.Category.Error);
                }

                viewModel.GetInvoiceBy(ivn);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel.Invoice);
        }
        public ActionResult Receipt(string ivn, long fid, int st)
        {
            Receipt receipt = null;

            try
            {
                receipt = GetReceipt(ivn, fid, st);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(receipt);
        }
        [HttpPost]
        public ActionResult VerifyOlevelResult(AdmissionViewModel viewModel)
        {
            string enc = Utility.Encrypt(viewModel.ApplicationForm.Id.ToString());

            try
            {
                //validate o-level result entry
                if (InvalidOlevelResult(viewModel))
                {
                    return RedirectToAction("Index", new { fid = enc });
                }

                using (var transaction = new TransactionScope())
                {
                    //get applicant's applied course
                    if (viewModel.FirstSittingOLevelResult == null || viewModel.FirstSittingOLevelResult.Id <= 0)
                    {
                        viewModel.FirstSittingOLevelResult.ApplicationForm = viewModel.ApplicationForm;
                        viewModel.FirstSittingOLevelResult.Person = viewModel.ApplicationForm.Person;
                        viewModel.FirstSittingOLevelResult.Sitting = new OLevelExamSitting { Id = 1 };
                    }

                    if (viewModel.SecondSittingOLevelResult == null || viewModel.SecondSittingOLevelResult.Id <= 0)
                    {
                        viewModel.SecondSittingOLevelResult.ApplicationForm = viewModel.ApplicationForm;
                        viewModel.SecondSittingOLevelResult.Person = viewModel.ApplicationForm.Person;
                        viewModel.SecondSittingOLevelResult.Sitting = new OLevelExamSitting { Id = 2 };
                    }

                    ModifyOlevelResult(viewModel.FirstSittingOLevelResult, viewModel.FirstSittingOLevelResultDetails);
                    ModifyOlevelResult(viewModel.SecondSittingOLevelResult, viewModel.SecondSittingOLevelResultDetails);

                    //check if applicant has been previously cleared
                    var applicantClearanceLogic = new ApplicantClearanceLogic();
                    if (applicantClearanceLogic.IsCleared(viewModel.ApplicationForm))
                    {
                        if (string.IsNullOrWhiteSpace(viewModel.ApplicationForm.RejectReason))
                        {
                            SetMessage(
                                "You have already been successfully cleared. Congratulations! kindly move on to next step.",
                                Message.Category.Information);
                            transaction.Complete();
                            return RedirectToAction("Index", new { fid = enc });
                        }
                    }


                    //set applicant new status
                    var newStatus = ApplicantStatus.Status.ClearedAndAccepted;
                    viewModel.ApplicationForm.Rejected = false;
                    viewModel.ApplicationForm.Release = false;
                    var applicantLogic = new ApplicantLogic();
                    applicantLogic.UpdateStatus(viewModel.ApplicationForm, newStatus);


                    //save clearance metadata
                    var applicantClearance = new ApplicantClearance();
                    applicantClearance = applicantClearanceLogic.GetBy(viewModel.ApplicationForm);
                    if (applicantClearance == null)
                    {
                        applicantClearance = new ApplicantClearance();
                        applicantClearance.ApplicationForm = viewModel.ApplicationForm;
                        applicantClearance.Cleared = string.IsNullOrWhiteSpace(viewModel.ApplicationForm.RejectReason)
                            ? true
                            : false;
                        applicantClearance.DateCleared = DateTime.Now;
                        applicantClearanceLogic.Create(applicantClearance);
                    }
                    else
                    {
                        applicantClearance.Cleared = string.IsNullOrWhiteSpace(viewModel.ApplicationForm.RejectReason)
                            ? true
                            : false;
                        applicantClearance.DateCleared = DateTime.Now;
                        applicantClearanceLogic.Modify(applicantClearance);
                    }

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }

            return RedirectToAction("Index", new { fid = enc });
        }
        private void ModifyOlevelResult(OLevelResult oLevelResult, List<OLevelResultDetail> oLevelResultDetails)
        {
            try
            {
                var oLevelResultDetailLogic = new OLevelResultDetailLogic();
                if (oLevelResult != null && oLevelResult.ExamNumber != null && oLevelResult.Type != null &&
                    oLevelResult.ExamYear > 0)
                {
                    if (oLevelResult != null && oLevelResult.Id > 0)
                    {
                        oLevelResultDetailLogic.DeleteBy(oLevelResult);
                        var oLevelResultLogic = new OLevelResultLogic();
                        oLevelResultLogic.Modify(oLevelResult);
                    }
                    else
                    {
                        var oLevelResultLogic = new OLevelResultLogic();
                        OLevelResult newOLevelResult = oLevelResultLogic.Create(oLevelResult);
                        oLevelResult.Id = newOLevelResult.Id;
                    }

                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        List<OLevelResultDetail> olevelResultDetails =
                            oLevelResultDetails.Where(
                                m => m.Grade != null && m.Grade.Id > 0 && m.Subject != null && m.Subject.Id > 0)
                                .ToList();
                        foreach (OLevelResultDetail oLevelResultDetail in olevelResultDetails)
                        {
                            oLevelResultDetail.Header = oLevelResult;
                            oLevelResultDetailLogic.Create(oLevelResultDetail);
                        }

                        //oLevelResultDetailLogic.Create(olevelResultDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool InvalidOlevelResult(AdmissionViewModel viewModel)
        {
            try
            {
                if (InvalidNumberOfOlevelSubject(viewModel.FirstSittingOLevelResultDetails,
                    viewModel.SecondSittingOLevelResultDetails))
                {
                    return true;
                }

                if (InvalidOlevelSubjectOrGrade(viewModel.FirstSittingOLevelResultDetails, viewModel.OLevelSubjects,
                    viewModel.OLevelGrades, Utility.FIRST_SITTING))
                {
                    return true;
                }

                if (viewModel.SecondSittingOLevelResult != null)
                {
                    if (viewModel.SecondSittingOLevelResult.ExamNumber != null &&
                        viewModel.SecondSittingOLevelResult.Type != null &&
                        viewModel.SecondSittingOLevelResult.Type.Id > 0 &&
                        viewModel.SecondSittingOLevelResult.ExamYear > 0)
                    {
                        if (InvalidOlevelSubjectOrGrade(viewModel.SecondSittingOLevelResultDetails,
                            viewModel.OLevelSubjects, viewModel.OLevelGrades, Utility.SECOND_SITTING))
                        {
                            return true;
                        }
                    }
                }

                if (InvalidOlevelResultHeaderInformation(viewModel.FirstSittingOLevelResultDetails,
                    viewModel.FirstSittingOLevelResult, Utility.FIRST_SITTING))
                {
                    return true;
                }

                if (InvalidOlevelResultHeaderInformation(viewModel.SecondSittingOLevelResultDetails,
                    viewModel.SecondSittingOLevelResult, Utility.SECOND_SITTING))
                {
                    return true;
                }

                if (NoOlevelSubjectSpecified(viewModel.FirstSittingOLevelResultDetails,
                    viewModel.FirstSittingOLevelResult, Utility.FIRST_SITTING))
                {
                    return true;
                }
                if (NoOlevelSubjectSpecified(viewModel.SecondSittingOLevelResultDetails,
                    viewModel.SecondSittingOLevelResult, Utility.SECOND_SITTING))
                {
                    return true;
                }

                //if (InvalidOlevelType(viewModel.FirstSittingOLevelResult.Type, viewModel.SecondSittingOLevelResult.Type))
                //{
                //    return true;
                //}

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool NoOlevelSubjectSpecified(List<OLevelResultDetail> oLevelResultDetails, OLevelResult oLevelResult,
            string sitting)
        {
            try
            {
                if (!string.IsNullOrEmpty(oLevelResult.ExamNumber) ||
                    (oLevelResult.Type != null && oLevelResult.Type.Id > 0) || (oLevelResult.ExamYear > 0))
                {
                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        List<OLevelResultDetail> oLevelResultDetailsEntered =
                            oLevelResultDetails.Where(r => r.Subject.Id > 0).ToList();
                        if (oLevelResultDetailsEntered == null || oLevelResultDetailsEntered.Count <= 0)
                        {
                            SetMessage(
                                "No O-Level Subject specified for " + sitting +
                                "! At least one subject must be specified when Exam Number, O-Level Type and Year are all specified for the sitting.",
                                Message.Category.Error);
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool InvalidNumberOfOlevelSubject(List<OLevelResultDetail> firstSittingResultDetails,
            List<OLevelResultDetail> secondSittingResultDetails)
        {
            const int FIVE = 5;

            try
            {
                int totalNoOfSubjects = 0;

                List<OLevelResultDetail> firstSittingSubjectList = null;
                List<OLevelResultDetail> secondSittingSubjectList = null;

                if (firstSittingResultDetails != null && firstSittingResultDetails.Count > 0)
                {
                    firstSittingSubjectList = firstSittingResultDetails.Where(r => r.Subject.Id > 0).ToList();
                    if (firstSittingSubjectList != null)
                    {
                        totalNoOfSubjects += firstSittingSubjectList.Count;
                    }
                }

                if (secondSittingResultDetails != null && secondSittingResultDetails.Count > 0)
                {
                    secondSittingSubjectList = secondSittingResultDetails.Where(r => r.Subject.Id > 0).ToList();
                    if (secondSittingSubjectList != null)
                    {
                        totalNoOfSubjects += secondSittingSubjectList.Count;
                    }
                }

                if (totalNoOfSubjects == 0)
                {
                    SetMessage("No O-Level Result Details found for both sittings!", Message.Category.Error);
                    return true;
                }
                if (totalNoOfSubjects < FIVE)
                {
                    SetMessage("O-Level Result cannot be less than " + FIVE + " subjects in both sittings!",
                        Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool InvalidOlevelResultHeaderInformation(List<OLevelResultDetail> resultDetails,
            OLevelResult oLevelResult, string sitting)
        {
            try
            {
                if (resultDetails != null && resultDetails.Count > 0)
                {
                    List<OLevelResultDetail> subjectList = resultDetails.Where(r => r.Subject.Id > 0).ToList();

                    if (subjectList != null && subjectList.Count > 0)
                    {
                        if (string.IsNullOrEmpty(oLevelResult.ExamNumber))
                        {
                            SetMessage("O-Level Exam Number not set for " + sitting + " ! Please modify",
                                Message.Category.Error);
                            return true;
                        }
                        if (oLevelResult.Type == null || oLevelResult.Type.Id <= 0)
                        {
                            SetMessage("O-Level Exam Type not set for " + sitting + " ! Please modify",
                                Message.Category.Error);
                            return true;
                        }
                        if (oLevelResult.ExamYear <= 0)
                        {
                            SetMessage("O-Level Exam Year not set for " + sitting + " ! Please modify",
                                Message.Category.Error);
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool InvalidOlevelSubjectOrGrade(List<OLevelResultDetail> oLevelResultDetails,
            List<OLevelSubject> subjects, List<OLevelGrade> grades, string sitting)
        {
            try
            {
                List<OLevelResultDetail> subjectList = null;
                if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                {
                    subjectList = oLevelResultDetails.Where(r => r.Subject.Id > 0 || r.Grade.Id > 0).ToList();
                }

                foreach (OLevelResultDetail oLevelResultDetail in subjectList)
                {
                    OLevelSubject subject = subjects.Where(s => s.Id == oLevelResultDetail.Subject.Id).SingleOrDefault();
                    OLevelGrade grade = grades.Where(g => g.Id == oLevelResultDetail.Grade.Id).SingleOrDefault();

                    List<OLevelResultDetail> results =
                        subjectList.Where(o => o.Subject.Id == oLevelResultDetail.Subject.Id).ToList();
                    if (results != null && results.Count > 1)
                    {
                        SetMessage(
                            "Duplicate " + subject.Name.ToUpper() + " Subject detected in " + sitting +
                            "! Please modify.", Message.Category.Error);
                        return true;
                    }
                    if (oLevelResultDetail.Subject.Id > 0 && oLevelResultDetail.Grade.Id <= 0)
                    {
                        SetMessage(
                            "No Grade specified for Subject " + subject.Name.ToUpper() + " in " + sitting +
                            "! Please modify.", Message.Category.Error);
                        return true;
                    }
                    if (oLevelResultDetail.Subject.Id <= 0 && oLevelResultDetail.Grade.Id > 0)
                    {
                        SetMessage(
                            "No Subject specified for Grade" + grade.Name.ToUpper() + " in " + sitting +
                            "! Please modify.", Message.Category.Error);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool IsNextApplicationStatus(long formId, int status)
        {
            try
            {
                var form = new ApplicationForm { Id = formId };
                var applicantLogic = new ApplicantLogic();
                Model.Model.Applicant applicant = applicantLogic.GetBy(form);
                if (applicant != null)
                {
                    if (applicant.Status.Id < status)
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception("Applicant Status not found!");
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private StudentPayment CreateStudentPaymentLog(Payment payment)
        {
            try
            {
                var studentPaymentLogic = new StudentPaymentLogic();
                var student = new Model.Model.Student();
                var studentLogic = new StudentLogic();
                var studentPayment = new StudentPayment();
                var existStudentPayment = studentPaymentLogic.GetModelsBy(f => f.Payment_Id == payment.Id).FirstOrDefault();
                if (existStudentPayment == null)
                {
                    studentPayment.Id = payment.Id;
                    studentPayment.Level = new Level { Id = 1 };
                    studentPayment.Session = payment.Session;
                    studentPayment.Student = new Model.Model.Student { Id = payment.Person.Id };
                    studentPayment.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                    studentPayment.Status = false;
                    studentPayment = studentPaymentLogic.Create(studentPayment);
                }
                else
                {
                    return existStudentPayment;
                }

                return studentPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}