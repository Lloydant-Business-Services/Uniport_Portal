using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Areas.Admin.Views
{
    [RoleBasedAttribute]
    public class SupportController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private const string FIRST_SITTING = "FIRST SITTING";
        private const string SECOND_SITTING = "SECOND SITTING";
        private Abundance_NkEntities db = new Abundance_NkEntities();
        private SupportViewModel viewmodel;
        protected Expression<Func<APPLICANT_JAMB_DETAIL, bool>> Selector { get; set; }
        // GET: /Admin/Support/
        public ActionResult Index()
        {
            viewmodel = new SupportViewModel();
            try
            {
                ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.SettingId = new SelectList(new List<ApplicationFormSetting>(), ID, NAME);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(SupportViewModel vModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vModel.InvoiceNumber != null)
                    {
                        var payment = new Payment();
                        var paymentLogic = new PaymentLogic();
                        var p = new PaymentEtranzact();
                        var pe = new PaymentEtranzactLogic();
                        p = pe.GetModelBy(n => n.Confirmation_No == vModel.InvoiceNumber);
                        if (p != null && p.ConfirmationNo != null)
                        {
                            payment = paymentLogic.GetModelBy(m => m.Invoice_Number == p.CustomerID);
                        }
                        else
                        {
                            payment = paymentLogic.GetModelBy(m => m.Invoice_Number == vModel.InvoiceNumber);
                        }

                        if (payment != null && payment.Id > 0)
                        {
                            var appliedcourse = new AppliedCourse();
                            var appliedCourseLogic = new AppliedCourseLogic();
                            var applicantjambdetail = new ApplicantJambDetail();
                            var applicantjambdetailLogic = new ApplicantJambDetailLogic();

                            appliedcourse = appliedCourseLogic.GetModelBy(n => n.Person_Id == payment.Person.Id);
                            if (appliedcourse != null && appliedcourse.Department.Id > 0)
                            {
                                applicantjambdetail = applicantjambdetailLogic.GetModelBy(x => x.Person_Id == payment.Person.Id);
                                if (applicantjambdetail != null && applicantjambdetail.Person.Id > 0)
                                {
                                    vModel.ApplicantJambDetail = applicantjambdetail;
                                }

                                vModel.AppliedCourse = appliedcourse;
                                vModel.DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(appliedcourse.Programme);
                                vModel.FormSettingSelectListItem = Utility.PopulateFormSettingSelectListItem();
                                vModel.ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();

                                if (vModel.FormSettingSelectListItem != null)
                                {
                                    if (appliedcourse.Setting != null)
                                    {
                                        ViewBag.SettingId = new SelectList(vModel.FormSettingSelectListItem, VALUE,
                                            TEXT, appliedcourse.Setting.Id);
                                    }
                                    else
                                    {
                                        ViewBag.SettingId = new SelectList(vModel.FormSettingSelectListItem, VALUE,
                                            TEXT, 5);
                                    }
                                }
                                else
                                {
                                    ViewBag.SettingId = new SelectList(new List<ApplicationFormSetting>(), ID, NAME);
                                }

                                if (vModel.DepartmentSelectListItem != null)
                                {
                                    ViewBag.DepartmentId = new SelectList(vModel.DepartmentSelectListItem, VALUE, TEXT,
                                        appliedcourse.Department.Id);
                                }
                                else
                                {
                                    ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                                }
                                if (vModel.ProgrammeSelectListItem != null)
                                {
                                    ViewBag.ProgrammeId = new SelectList(vModel.ProgrammeSelectListItem, VALUE, TEXT,
                                        appliedcourse.Programme.Id);
                                }
                                else
                                {
                                    ViewBag.ProgrammeId = new SelectList(new List<Programme>(), ID, NAME);
                                }

                                vModel.Payment = payment;
                                vModel.Person = payment.Person;
                                vModel.InvoiceNumber = payment.InvoiceNumber;
                                TempData["SupportViewModel"] = viewmodel;
                                return View(vModel);
                            }
                        }
                        else
                        {
                            SetMessage("Invoice does not exist! ", Message.Category.Error);
                            return View(vModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(vModel);
        }

        [HttpPost]
        public ActionResult UpdateInvoice(SupportViewModel vModel)
        {
            try
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                    }
                }

                ModelState.Remove("Person.DateEntered");
                ModelState.Remove("Person.DateOfBirth");
                ModelState.Remove("Person.FirstName");
                ModelState.Remove("Person.LastName");
                ModelState.Remove("Person.MobilePhone");
                ModelState.Remove("Person.Sex.Id");
                ModelState.Remove("Person.Religion.Id");
                ModelState.Remove("Person.LocalGovernment.Id");
                ModelState.Remove("AppliedCourse.Option.Id");
                ModelState.Remove("AppliedCourse.ApplicationForm.Id");
                ModelState.Remove("ApplicantJambDetail.Person.Id");
                ModelState.Remove("ApplicantJambDetail.JambRegistrationNumber");

                if (ModelState.IsValid)
                {
                    using (var transaction = new TransactionScope())
                    {
                        string operation = "UPDATE";
                        string action = "MODIFY APPLICANT PERSON AND APPLIED COURSE DETAILS";
                        string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress +
                                        ")";

                        if (vModel.Person != null && vModel.AppliedCourse != null)
                        {
                            var personAudit = new PersonAudit();
                            var loggeduser = new UserLogic();
                            personAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                            personAudit.Operation = operation;
                            personAudit.Action = action;
                            personAudit.Time = DateTime.Now;
                            personAudit.Client = client;

                            var personLogic = new PersonLogic();
                            bool personChanged = personLogic.Modify(vModel.Person, personAudit);

                            var appliedCourseAudit = new AppliedCourseAudit();
                            appliedCourseAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                            appliedCourseAudit.Operation = operation;
                            appliedCourseAudit.Action = action;
                            appliedCourseAudit.Time = DateTime.Now;
                            appliedCourseAudit.Client = client;

                            var jambAudit = new ApplicantJambDetailAudit();
                            jambAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                            jambAudit.Operation = operation;
                            jambAudit.Action = action;
                            jambAudit.Time = DateTime.Now;
                            jambAudit.Client = client;

                            var appliedCourseLogic = new AppliedCourseLogic();
                            vModel.AppliedCourse.Person = vModel.Person;
                            bool appliedCourseChanged = appliedCourseLogic.Modify(vModel.AppliedCourse, appliedCourseAudit);

                            //if (vModel.AppliedCourse.Department.Id != appliedCourseAudit.OldDepartment.Id)
                            //{
                            //    var appForm = new ApplicationForm();
                            //    var applicationFormLogic = new ApplicationFormLogic();
                            //    appForm = applicationFormLogic.GetModelBy(m => m.Person_Id == vModel.Person.Id);
                            //    if (appForm != null)
                            //    {
                            //        var department = new Department();
                            //        var departmentLogic = new DepartmentLogic();
                            //        department = departmentLogic.GetModelBy(d => d.Department_Id == vModel.AppliedCourse.Department.Id);
                            //        vModel.AppliedCourse.Department = department;
                            //        applicationFormLogic.Modify(appForm);
                            //    }
                            //}

                            if (vModel.ApplicantJambDetail.JambRegistrationNumber != null && ((appliedCourseAudit.OldProgramme.Id == 1 && appliedCourseAudit.Programme.Id == 1) || (appliedCourseAudit.OldProgramme.Id == 4 && appliedCourseAudit.Programme.Id == 4)))
                            {
                                var appDetail = new ApplicantJambDetail();
                                var appJambDetailLogic = new ApplicantJambDetailLogic();
                                appDetail =
                                    appJambDetailLogic.GetModelBy(
                                        m => m.Person_Id == appliedCourseAudit.AppliedCourse.Person.Id);

                                if (appDetail != null)
                                {
                                    appDetail.JambRegistrationNumber = vModel.ApplicantJambDetail.JambRegistrationNumber;
                                    appJambDetailLogic.Modify(appDetail, jambAudit);
                                }
                                else
                                {
                                    vModel.ApplicantJambDetail.Person = appliedCourseAudit.AppliedCourse.Person;
                                    appJambDetailLogic.Create(vModel.ApplicantJambDetail);
                                }
                            }
                            else if (vModel.ApplicantJambDetail.JambRegistrationNumber != null &&
                                     appliedCourseAudit.OldProgramme.Id != 1 && appliedCourseAudit.Programme.Id == 1)
                            {
                                var appJambDetailLogic = new ApplicantJambDetailLogic();
                                vModel.ApplicantJambDetail.Person = appliedCourseAudit.AppliedCourse.Person;
                                appJambDetailLogic.Create(vModel.ApplicantJambDetail);
                            }
                            else if (vModel.ApplicantJambDetail.JambRegistrationNumber != null && appliedCourseAudit.Programme.Id != 1)
                            {
                                var appJambDetailLogic = new ApplicantJambDetailLogic();
                                Selector = r => r.PERSON.Person_Id == appliedCourseAudit.AppliedCourse.Person.Id;
                                appJambDetailLogic.Delete(Selector);
                            }

                            string number = vModel.Payment.InvoiceNumber;
                            var payment = new Payment();
                            var paymentLogic = new PaymentLogic();
                            var p = new PaymentEtranzact();
                            var pe = new PaymentEtranzactLogic();
                            p = pe.GetModelBy(n => n.Confirmation_No == number);
                            if (p != null && p.ConfirmationNo != null)
                            {
                                payment = paymentLogic.GetModelBy(m => m.Invoice_Number == p.CustomerID);
                            }
                            else
                            {
                                payment = paymentLogic.GetModelBy(m => m.Invoice_Number == number);
                            }
                            var ApplicationProgrammeFee = new ApplicationProgrammeFee();
                            var ApplicationProgrammeFeeLogic = new ApplicationProgrammeFeeLogic();
                            ApplicationProgrammeFee = ApplicationProgrammeFeeLogic.GetModelBy(z => z.Programme_Id == appliedCourseAudit.Programme.Id && z.Session_Id == payment.Session.Id && z.Fee_Type_Id == payment.FeeType.Id);
                            if (ApplicationProgrammeFee != null)
                            {
                                payment.FeeType = ApplicationProgrammeFee.FeeType;
                                paymentLogic.Modify(payment);
                            }
                            transaction.Complete();
                        }
                    }

                    SetMessage("Record was successfully updated", Message.Category.Information);
                    return RedirectToAction("Index", "Support");
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "System Message :" + ex.Message;
                return RedirectToAction("Index", "Support");
            }

            TempData["Message"] = "Record was not updated! Crosscheck entries and try again";
            return RedirectToAction("Index", "Support");
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

        public ActionResult SendSms()
        {
            viewmodel = new SupportViewModel();
            try
            {
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View();
        }

        public ActionResult ViewAuditReport()
        {
            try
            {
                var supportModel = new SupportViewModel();

                var personAudit = new List<PersonAudit>();
                var personAuditLogic = new PersonAuditLogic();

                personAudit = personAuditLogic.GetAll();
                supportModel.PersonAudit = personAudit;
                return View(supportModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult ViewAuditReportDetails(long Id)
        {
            try
            {
                var supportModel = new SupportViewModel();
                if (Id > 0)
                {
                    var personAudit = new PersonAudit();
                    var personAuditLogic = new PersonAuditLogic();

                    personAudit = personAuditLogic.GetModelBy(p => p.Person_Audit_Id == Id);
                    supportModel.PersonAuditDetails = personAudit;

                    return View(supportModel);
                }

                return View(supportModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult CorrectOlevel()
        {
            viewmodel = new SupportViewModel();
            try
            {
                PopulateOlevelDropdowns(viewmodel);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult CorrectOlevel(SupportViewModel supportModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (supportModel.InvoiceNumber != null)
                    {
                        var payment = new Payment();
                        var paymentLogic = new PaymentLogic();
                        var p = new PaymentEtranzact();
                        var pe = new PaymentEtranzactLogic();
                        p = pe.GetModelBy(n => n.Confirmation_No == supportModel.InvoiceNumber);
                        if (p != null && p.ConfirmationNo != null)
                        {
                            payment = paymentLogic.GetModelBy(m => m.Invoice_Number == p.CustomerID);
                        }
                        else
                        {
                            payment = paymentLogic.GetModelBy(m => m.Invoice_Number == supportModel.InvoiceNumber);
                        }

                        if (payment != null && payment.Id > 0)
                        {
                            var appliedcourse = new AppliedCourse();
                            var appliedCourseLogic = new AppliedCourseLogic();
                            var resultDetail = new OLevelResultDetail();

                            appliedcourse = appliedCourseLogic.GetModelBy(n => n.Person_Id == payment.Person.Id);
                            if (appliedcourse != null && appliedcourse.Department.Id > 0)
                            {
                                ApplicationForm applicationform = supportModel.GetApplicationFormBy(payment.Person,
                                    payment);
                                if (applicationform != null && applicationform.Id > 0)
                                {
                                    var olevelResult = new OLevelResult();
                                    var olevelResultLogic = new OLevelResultLogic();
                                    olevelResult =
                                        olevelResultLogic.GetModelBy(
                                            m =>
                                                m.Person_Id == payment.Person.Id && m.O_Level_Exam_Sitting_Id == 1 &&
                                                m.Application_Form_Id != null);
                                    if (olevelResult != null)
                                    {
                                        var olevelResultdetails = new List<OLevelResultDetail>();
                                        var olevelResultdetailsLogic = new OLevelResultDetailLogic();

                                        olevelResultdetails =
                                            olevelResultdetailsLogic.GetModelsBy(
                                                m => m.Applicant_O_Level_Result_Id == olevelResult.Id);
                                        supportModel.FirstSittingOLevelResult = olevelResult;
                                        supportModel.FirstSittingOLevelResultDetails = olevelResultdetails;
                                    }

                                    olevelResult =
                                        olevelResultLogic.GetModelBy(
                                            m =>
                                                m.Person_Id == payment.Person.Id && m.O_Level_Exam_Sitting_Id == 2 &&
                                                m.Application_Form_Id != null);
                                    if (olevelResult != null)
                                    {
                                        var olevelResultdetails = new List<OLevelResultDetail>();
                                        var olevelResultdetailsLogic = new OLevelResultDetailLogic();

                                        olevelResultdetails =
                                            olevelResultdetailsLogic.GetModelsBy(
                                                m => m.Applicant_O_Level_Result_Id == olevelResult.Id);
                                        supportModel.SecondSittingOLevelResult = olevelResult;
                                        supportModel.SecondSittingOLevelResultDetails = olevelResultdetails;
                                    }
                                }

                                supportModel.AppliedCourse = appliedcourse;
                                supportModel.Payment = payment;
                                supportModel.Person = payment.Person;
                                supportModel.InvoiceNumber = payment.InvoiceNumber;
                                //SetSelectedSittingSubjectAndGrade(supportModel);
                                PopulateOlevelDropdowns(supportModel);

                                TempData["SupportViewModel"] = supportModel;
                                return View(supportModel);
                            }
                        }
                        else
                        {
                            SetMessage("Invoice does not exist! ", Message.Category.Error);
                            return View(supportModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewmodel);
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

                        //if (string.IsNullOrEmpty(oLevelResult.ExamNumber) || oLevelResult.Type == null || oLevelResult.Type.Id <= 0 || oLevelResult.ExamYear <= 0)
                        //{
                        //    SetMessage("Header Information not set for " + sitting + " O-Level Result Details! ", Message.Category.Error);
                        //    return true;
                        //}
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

                    //else if (oLevelResultDetail.Grade.Id > 0 && oLevelResultDetail.Subject.Id <= 0)
                    //{
                    //    SetMessage("No Grade specified for " + subject.Name.ToUpper() + " for " + sitting + "! Please modify.", Message.Category.Error);
                    //    return true;
                    //}
                    //else if (oLevelResultDetail.Subject.Id <= 0 && oLevelResultDetail.Grade.Id > 0)
                    //{
                    //    SetMessage("No Subject specified for " + grade.Name.ToUpper() + " for " + sitting + "! Please modify.", Message.Category.Error);
                    //    return true;
                    //}
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

        private void PopulateOlevelDropdowns(SupportViewModel viewmodel)
        {
            try
            {
                ViewBag.FirstSittingOLevelTypeId = viewmodel.OLevelTypeSelectList;
                ViewBag.SecondSittingOLevelTypeId = viewmodel.OLevelTypeSelectList;
                ViewBag.FirstSittingExamYearId = viewmodel.ExamYearSelectList;
                ViewBag.SecondSittingExamYearId = viewmodel.ExamYearSelectList;
                ViewBag.ResultGradeId = viewmodel.ResultGradeSelectList;

                SetSelectedSittingSubjectAndGrade(viewmodel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void SetDefaultSelectedSittingSubjectAndGrade(SupportViewModel viewModel)
        {
            try
            {
                if (viewModel != null && viewModel.FirstSittingOLevelResultDetails != null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        ViewData["FirstSittingOLevelSubjectId" + i] = viewModel.OLevelSubjectSelectList;
                        ViewData["FirstSittingOLevelGradeId" + i] = viewModel.OLevelGradeSelectList;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetSelectedSittingSubjectAndGrade(SupportViewModel existingViewModel)
        {
            try
            {
                if (existingViewModel != null && existingViewModel.FirstSittingOLevelResultDetails != null &&
                    existingViewModel.FirstSittingOLevelResultDetails.Count > 0)
                {
                    int i = 0;
                    foreach (
                        OLevelResultDetail firstSittingOLevelResultDetail in
                            existingViewModel.FirstSittingOLevelResultDetails)
                    {
                        if (firstSittingOLevelResultDetail.Subject != null &&
                            firstSittingOLevelResultDetail.Grade != null)
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT,
                                    firstSittingOLevelResultDetail.Subject.Id);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, VALUE, TEXT,
                                    firstSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT, 0);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, VALUE, TEXT, 0);
                        }

                        i++;
                    }
                }

                if (existingViewModel != null && existingViewModel.SecondSittingOLevelResultDetails != null &&
                    existingViewModel.SecondSittingOLevelResultDetails.Count > 0)
                {
                    int i = 0;
                    foreach (
                        OLevelResultDetail secondSittingOLevelResultDetail in
                            existingViewModel.SecondSittingOLevelResultDetails)
                    {
                        if (secondSittingOLevelResultDetail.Subject != null &&
                            secondSittingOLevelResultDetail.Grade != null)
                        {
                            ViewData["SecondSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT,
                                    secondSittingOLevelResultDetail.Subject.Id);
                            ViewData["SecondSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, VALUE, TEXT,
                                    secondSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            ViewData["SecondSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT, 0);
                            ViewData["SecondSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, VALUE, TEXT, 0);
                        }

                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        [HttpPost]
        public ActionResult UpdateOlevel(SupportViewModel viewModel)
        {
            try
            {
                ModelState.Remove("Person.DateEntered");
                ModelState.Remove("Person.DateOfBirth");
                ModelState.Remove("Person.FirstName");
                ModelState.Remove("Person.LastName");
                ModelState.Remove("Person.MobilePhone");
                ModelState.Remove("Person.Sex.Id");
                ModelState.Remove("Person.Religion.Id");
                ModelState.Remove("Person.LocalGovernment.Id");
                ModelState.Remove("AppliedCourse.Option.Id");
                ModelState.Remove("AppliedCourse.ApplicationForm.Id");
                ModelState.Remove("ApplicantJambDetail.Person.Id");
                ModelState.Remove("ApplicationForm.Id");
                ModelState.Remove("ApplicationForm.Person.Id");

                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        //DoSomethingWith(error);
                        SetMessage(error.ErrorMessage, Message.Category.Information);
                    }
                }

                if (InvalidOlevelSubjectOrGrade(viewModel.FirstSittingOLevelResultDetails, viewModel.OLevelSubjects,
                    viewModel.OLevelGrades, FIRST_SITTING))
                {
                    TempData["Message"] = "O-level Subjects contains duplicates";
                    return RedirectToAction("CorrectOlevel");
                }

                if (InvalidNumberOfOlevelSubject(viewModel.FirstSittingOLevelResultDetails,
                    viewModel.SecondSittingOLevelResultDetails))
                {
                    TempData["Message"] = "Invalid number of O-level Subjects";
                    return RedirectToAction("CorrectOlevel");
                }
                if (InvalidOlevelSubjectOrGrade(viewModel.SecondSittingOLevelResultDetails, viewModel.OLevelSubjects,
                    viewModel.OLevelGrades, SECOND_SITTING))
                {
                    TempData["Message"] = "Second sitting O-level Subjects contains duplicates";
                    return RedirectToAction("CorrectOlevel");
                }

                using (var transaction = new TransactionScope())
                {
                    var oLevelResultLogic = new OLevelResultLogic();
                    var oLevelResultDetailLogic = new OLevelResultDetailLogic();

                    var appFormLogic = new ApplicationFormLogic();
                    viewModel.ApplicationForm = appFormLogic.GetBy(viewModel.AppliedCourse.ApplicationForm.Id);

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

                    //get applicant's applied course
                    var appliedCourseLogic = new AppliedCourseLogic();
                    var admissionListLogic = new AdmissionListLogic();
                    var applicantLogic = new ApplicantLogic();
                    AppliedCourse appliedCourse = appliedCourseLogic.GetBy(viewModel.ApplicationForm.Person);

                    //Set department to admitted department since it might vary
                    var admissionList = new AdmissionList();
                    admissionList = admissionListLogic.GetBy(viewModel.ApplicationForm.Person);
                    if (admissionList != null)
                    {
                        appliedCourse.Department = admissionList.Deprtment;

                        if (appliedCourse == null)
                        {
                            SetMessage(
                                "Your O-Level was successfully verified, but could not be cleared because no Applied Course was not found for your application",
                                Message.Category.Error);
                            return RedirectToAction("CorrectOlevel");
                        }

                        //set reject reason if exist
                        ApplicantStatus.Status newStatus;
                        var admissionCriteriaLogic = new AdmissionCriteriaLogic();
                        string rejectReason = admissionCriteriaLogic.EvaluateApplication(appliedCourse);
                        if (string.IsNullOrWhiteSpace(rejectReason))
                        {
                            var applicant = new Model.Model.Applicant();
                            applicant = applicantLogic.GetModelBy(p => p.Person_Id == appliedCourse.Person.Id);
                            newStatus = (ApplicantStatus.Status)applicant.Status.Id;
                            //newStatus = ApplicantStatus.Status.ClearedAndAccepted;
                            viewModel.ApplicationForm.Rejected = false;
                            viewModel.ApplicationForm.Release = false;
                        }
                        else
                        {
                            newStatus = ApplicantStatus.Status.ClearedAndRejected;
                            viewModel.ApplicationForm.Rejected = true;
                            viewModel.ApplicationForm.Release = true;
                        }

                        viewModel.ApplicationForm.RejectReason = rejectReason;
                        var applicationFormLogic = new ApplicationFormLogic();
                        applicationFormLogic.SetRejectReason(viewModel.ApplicationForm);

                        //set applicant new status
                        //ApplicantLogic applicantLogic = new ApplicantLogic();
                        applicantLogic.UpdateStatus(viewModel.ApplicationForm, newStatus);

                        //save clearance metadata
                        var applicantClearance = new ApplicantClearance();
                        var applicantClearanceLogic = new ApplicantClearanceLogic();
                        applicantClearance = applicantClearanceLogic.GetBy(viewModel.ApplicationForm);
                        if (applicantClearance == null)
                        {
                            applicantClearance = new ApplicantClearance();
                            applicantClearance.ApplicationForm = viewModel.ApplicationForm;
                            applicantClearance.Cleared =
                                string.IsNullOrWhiteSpace(viewModel.ApplicationForm.RejectReason) ? true : false;
                            applicantClearance.DateCleared = DateTime.Now;
                            applicantClearanceLogic.Create(applicantClearance);
                        }
                        else
                        {
                            applicantClearance.Cleared =
                                string.IsNullOrWhiteSpace(viewModel.ApplicationForm.RejectReason) ? true : false;
                            applicantClearance.DateCleared = DateTime.Now;
                            applicantClearanceLogic.Modify(applicantClearance);
                        }
                    }
                    transaction.Complete();
                }
            }

            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            TempData["Message"] = "O-Level result updated";
            return RedirectToAction("CorrectOlevel");
        }

        private void ModifyOlevelResult(OLevelResult oLevelResult, List<OLevelResultDetail> oLevelResultDetails)
        {
            try
            {
                var oLevelResultLogic = new OLevelResultLogic();

                var oLevelResultDetailLogic = new OLevelResultDetailLogic();
                if (oLevelResult != null && oLevelResult.ExamNumber != null && oLevelResult.Type != null &&
                    oLevelResult.ExamYear > 0)
                {
                    if (oLevelResult != null && oLevelResult.Id > 0)
                    {
                        oLevelResultDetailLogic.DeleteBy(oLevelResult);
                        oLevelResultLogic.Modify(oLevelResult);
                    }
                    else
                    {
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

        [HttpGet]
        public ActionResult PinRetrieval()
        {
            viewmodel = new SupportViewModel { EtranzactPins = new List<PaymentEtranzact>() };
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult PinRetrieval(SupportViewModel viewmodel)
        {
            try
            {
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                PersonLogic personLogic = new PersonLogic();
                ScratchCardLogic scratchCardLogic = new ScratchCardLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                List<PaymentEtranzact> paymentEtranzact = new List<PaymentEtranzact>();
                List<ScratchCard> scratchCards = new List<ScratchCard>();

                viewmodel.ApplicationForm = applicationFormLogic.GetModelBy(p => p.Application_Form_Number == viewmodel.ApplicationForm.Number);
                if (viewmodel.ApplicationForm != null && viewmodel.ApplicationForm.Person.Id > 0)
                {
                    paymentEtranzact = paymentEtranzactLogic.GetModelsBy(p => p.Used_By_Person_Id == viewmodel.ApplicationForm.Person.Id);
                    scratchCards = scratchCardLogic.GetModelsBy(s => s.Person_Id == viewmodel.ApplicationForm.Person.Id);
                    viewmodel.EtranzactPins = paymentEtranzact;
                    viewmodel.ScratchCards = scratchCards;

                    if ((scratchCards == null || scratchCards.Count <= 0) && (paymentEtranzact == null || paymentEtranzact.Count <= 0))
                    {
                        SetMessage("No pin was found for this application number.", Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage("Application number was not found. Kindly check and try again.", Message.Category.Error);
                    viewmodel = new SupportViewModel { EtranzactPins = new List<PaymentEtranzact>() };
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }

            return View(viewmodel);
        }

        public ActionResult ResetInvoice()
        {
            try
            {
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Warning);
            }
            return View();
        }

        [HttpPost]
        public ActionResult ResetInvoice(SupportViewModel viewmodel)
        {
            try
            {
                var payment = new Payment();
                var paymentLogic = new PaymentLogic();
                payment = paymentLogic.GetModelBy(n => n.Invoice_Number == viewmodel.InvoiceNumber && n.Fee_Type_Id != 1);
                if (payment != null && payment.Id != null)
                {
                    var remitaPayment = new RemitaPayment();
                    var remitaPaymentLogic = new RemitaPaymentLogic();

                    remitaPayment = remitaPaymentLogic.GetBy(payment.Id);
                    if (remitaPayment != null)
                    {
                        if (remitaPayment.Status.Contains("01:"))
                        {
                            SetMessage("This invoice has been paid for and can't be deleted!", Message.Category.Warning);
                            return View(viewmodel);
                        }
                        var appFormLogic = new ApplicationFormLogic();
                        viewmodel.ApplicationForm = appFormLogic.GetBy(payment.Person);
                        var newStatus = new ApplicantStatus.Status();
                        if (payment.FeeType.Id == 2 || payment.FeeType.Id == 9)
                        {
                            newStatus = ApplicantStatus.Status.SubmittedApplicationForm;
                        }
                        else if (payment.FeeType.Id == 3)
                        {
                            newStatus = ApplicantStatus.Status.GeneratedAcceptanceReceipt;
                        }

                        var onlinePayment = new OnlinePayment();
                        var onlinePaymentLogic = new OnlinePaymentLogic();
                        onlinePayment = onlinePaymentLogic.GetBy(payment.Id);
                        if (onlinePayment != null)
                        {
                            using (var scope = new TransactionScope())
                            {
                                onlinePaymentLogic.DeleteBy(onlinePayment.Payment.Id);
                                remitaPaymentLogic.DeleteBy(remitaPayment.payment.Id);
                                paymentLogic.DeleteBy(payment.Id);

                                var applicantLogic = new ApplicantLogic();
                                applicantLogic.UpdateStatus(viewmodel.ApplicationForm, newStatus);

                                scope.Complete();
                                SetMessage(
                                    "The " + payment.FeeType.Name + "  invoice  " + payment.InvoiceNumber + "  for " +
                                    payment.Person.FullName +
                                    " has been deleted! Please log into your profile and generate a new one",
                                    Message.Category.Information);
                                return View(viewmodel);
                            }
                        }
                    }
                    else
                    {
                        SetMessage("Sorry, this invoice can't be deleted!", Message.Category.Warning);
                        return View(viewmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Warning);
            }

            return View(viewmodel);
        }

        public ActionResult UpdateStudentData()
        {
            viewmodel = new SupportViewModel();
            try
            {
                //ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                //ViewBag.ProgrammeId = Utility.PopulateAllProgrammeSelectListItem();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateStudentData(SupportViewModel viewmodel)
        {
            try
            {
                if (viewmodel.ApplicationForm.Number != null)
                {
                    var studentLevelLogic = new StudentLevelLogic();
                    var studentLogic = new StudentLogic();
                    var applicatinformLogic = new ApplicationFormLogic();
                    var admissionListLogic = new AdmissionListLogic();
                    var admissionList = new AdmissionList();
                    viewmodel.ApplicationForm =
                        applicatinformLogic.GetModelBy(
                            a => a.Application_Form_Number == viewmodel.ApplicationForm.Number);
                    admissionList =
                        admissionListLogic.GetModelBy(p => p.Application_Form_Id == viewmodel.ApplicationForm.Id);
                    using (var scope = new TransactionScope())
                    {
                        viewmodel.StudentLevel =
                            studentLevelLogic.GetModelsBy(s => s.Person_Id == viewmodel.ApplicationForm.Person.Id)
                                .LastOrDefault();

                        // clear course registration data and assign new matric number
                        var currentSessionSemesterLogic = new CurrentSessionSemesterLogic();
                        viewmodel.CurrentSessionSemester = currentSessionSemesterLogic.GetCurrentSessionSemester();

                        var courseRegistrationLogic = new CourseRegistrationLogic();
                        var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                        CourseRegistration courseRegistration =
                            courseRegistrationLogic.GetBy(viewmodel.StudentLevel.Student, viewmodel.StudentLevel.Level,
                                viewmodel.StudentLevel.Programme, viewmodel.StudentLevel.Department,
                                viewmodel.CurrentSessionSemester.SessionSemester.Session);
                        if (courseRegistration != null && courseRegistration.Id > 0)
                        {
                            Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> selector =
                                cr => cr.Student_Course_Registration_Id == courseRegistration.Id;
                            if (courseRegistrationDetailLogic.Delete(selector))
                            {
                                Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> deleteSelector =
                                    cr => cr.Student_Course_Registration_Id == courseRegistration.Id;
                                courseRegistrationLogic.Delete(deleteSelector);
                            }
                            else
                            {
                                Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> deleteSelector =
                                    cr => cr.Student_Course_Registration_Id == courseRegistration.Id;
                                courseRegistrationLogic.Delete(deleteSelector);
                                scope.Complete();
                            }
                        }

                        Expression<Func<STUDENT_LEVEL, bool>> deleteStudentLevelSelector =
                            sl => sl.Person_Id == viewmodel.ApplicationForm.Person.Id;
                        List<StudentLevel> studentItems = studentLevelLogic.GetModelsBy(deleteStudentLevelSelector);

                        if (studentLevelLogic.Delete(deleteStudentLevelSelector))
                        {
                            foreach (StudentLevel studentItem in studentItems)
                            {
                                CheckStudentSponsor(studentItem);
                                CheckStudentFinanceInformation(studentItem);
                                CheckStudentAcademicInformation(studentItem);
                                CheckStudentResultDetails(studentItem);
                                CheckStudentPaymentLog(studentItem);
                            }

                            Expression<Func<STUDENT, bool>> deleteStudentSelector =
                                sl => sl.Person_Id == viewmodel.ApplicationForm.Person.Id;
                            if (studentLogic.Delete(deleteStudentSelector))
                            {
                                var applicantLogic = new ApplicantLogic();
                                ApplicationFormView applicant = applicantLogic.GetBy(viewmodel.ApplicationForm.Id);
                                if (applicant != null)
                                {
                                    bool matricNoAssigned = studentLogic.AssignMatricNumber(applicant);
                                    if (matricNoAssigned)
                                    {
                                        scope.Complete();
                                        SetMessage("Student was successfully reset ", Message.Category.Information);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewmodel);
        }

        private static void CheckStudentSponsor(StudentLevel studentItem)
        {
            var studentSponsorLogic = new StudentSponsorLogic();
            StudentSponsor studentSponsor = studentSponsorLogic.GetModelBy(ss => ss.Person_Id == studentItem.Student.Id);
            if (studentSponsor != null)
            {
                Expression<Func<STUDENT_SPONSOR, bool>> deleteStudentSponsorSelector =
                    ss => ss.Person_Id == studentItem.Student.Id;
                studentSponsorLogic.Delete(deleteStudentSponsorSelector);
            }
        }

        private static void CheckStudentFinanceInformation(StudentLevel studentItem)
        {
            var studentFinanceInformationLogic = new StudentFinanceInformationLogic();
            StudentFinanceInformation studentFinanceInformation =
                studentFinanceInformationLogic.GetModelBy(ss => ss.Person_Id == studentItem.Student.Id);
            if (studentFinanceInformation != null)
            {
                Expression<Func<STUDENT_FINANCE_INFORMATION, bool>> deleteStudentFinanceInfoSelector =
                    sfi => sfi.Person_Id == studentItem.Student.Id;
                studentFinanceInformationLogic.Delete(deleteStudentFinanceInfoSelector);
            }
        }

        private static void CheckStudentAcademicInformation(StudentLevel studentItem)
        {
            var studentAcademicInformationLogic = new StudentAcademicInformationLogic();
            StudentAcademicInformation studentAcademicInformation =
                studentAcademicInformationLogic.GetModelBy(ss => ss.Person_Id == studentItem.Student.Id);
            if (studentAcademicInformation != null)
            {
                Expression<Func<STUDENT_ACADEMIC_INFORMATION, bool>> deleteStudentAcademicInfoSelector =
                    sai => sai.Person_Id == studentItem.Student.Id;
                studentAcademicInformationLogic.Delete(deleteStudentAcademicInfoSelector);
            }
        }

        private static void CheckStudentPaymentLog(StudentLevel studentItem)
        {
            var StudentPaymentLogic = new StudentPaymentLogic();
            List<StudentPayment> studentPayments =
                StudentPaymentLogic.GetModelsBy(ss => ss.Person_Id == studentItem.Student.Id);
            if (studentPayments != null & studentPayments.Count > 0)
            {
                foreach (var studentPayment in studentPayments)
                {
                    Expression<Func<STUDENT_PAYMENT, bool>> deleteStudentPaymentSelector =
                   sai => sai.Person_Id == studentItem.Student.Id;
                    StudentPaymentLogic.Delete(deleteStudentPaymentSelector);
                }

            }
        }


        private static void CheckStudentResultDetails(StudentLevel studentItem)
        {
            var studentResultDetailLogic = new StudentResultDetailLogic();
            StudentResultDetail studentResultDetail =
                studentResultDetailLogic.GetModelBy(srd => srd.Person_Id == studentItem.Student.Id);
            if (studentResultDetail != null)
            {
                Expression<Func<STUDENT_RESULT_DETAIL, bool>> deleteStudentResultDetailSelector =
                    srd => srd.Person_Id == studentItem.Student.Id;
                studentResultDetailLogic.Delete(deleteStudentResultDetailSelector);
            }
        }

        private List<CourseRegistrationDetail> GetRegisteredCourse(CourseRegistration courseRegistration,
            List<Course> courses, Semester semester, List<CourseRegistrationDetail> registeredCourseDetails,
            CourseMode courseMode)
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
                            CourseRegistrationDetail registeredCourseDetail =
                                registeredCourseDetails.Where(
                                    c => c.Course.Id == course.Id && c.Mode.Id == courseMode.Id).SingleOrDefault();
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
                }

                return courseRegistrationDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        public ActionResult CheckMatricNumberDuplicate(string matric)
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckMatricNumberDuplicate(SupportViewModel viewmodel)
        {
            try
            {
                var studentLogic = new StudentLogic();
                // studentLogic.CheckMatricNumberDuplicate(viewmodel.Pin);
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Information);
            }
            return View();
        }

        public ActionResult ResetStep()
        {
            try
            {
                viewmodel = new SupportViewModel();
                ViewBag.StatusId = viewmodel.StatusSelectListItem;
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Warning);
            }
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult ResetStep(SupportViewModel viewmodel)
        {
            try
            {
                if (viewmodel.ApplicationForm.Number != null)
                {
                    var applicantLogic = new ApplicantLogic();
                    viewmodel.Applicants = applicantLogic.GetApplicantsBy(viewmodel.ApplicationForm.Number);

                    if (viewmodel.Applicants.Status != null)
                    {
                        ViewBag.StatusId = new SelectList(viewmodel.StatusSelectListItem, VALUE, TEXT,
                            viewmodel.Applicants.Status.Id);
                    }
                    else
                    {
                        ViewBag.StatusId = viewmodel.StatusSelectListItem;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Warning);
            }
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult UpdateStep(SupportViewModel viewmodel)
        {
            try
            {
                var applicantLogic = new ApplicantLogic();
                var status = (ApplicantStatus.Status)viewmodel.Applicants.Status.Id;
                applicantLogic.UpdateStatus(viewmodel.Applicants.ApplicationForm, status);
            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Warning);
            }
            SetMessage("Updated Successfully", Message.Category.Information);
            return RedirectToAction("ResetStep");
        }

        public ActionResult Correction()
        {
            try
            {
                viewmodel = new SupportViewModel();
                viewmodel.Role = new Role();
                viewmodel.Student = new Model.Model.Student();
                ViewBag.Level = viewmodel.LevelSelectList;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred: " + ex.Message, Message.Category.Error);
            }
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Correction(SupportViewModel viewmodel)
        {
            var person = new Person();
            var studentLogic = new StudentLogic();
            var personLogic = new PersonLogic();
            var studentLevelLogic = new StudentLevelLogic();
            var studentLevel = new StudentLevel();
            var levelLogic = new LevelLogic();
            var departmentLogic = new DepartmentLogic();
            var programmeLogic = new ProgrammeLogic();
            var programme = new Programme();
            var departments = new List<Department>();
            var deparmentLogic = new DepartmentLogic();
            try
            {
                List<Model.Model.Student> studnetList = studentLogic.GetModelsBy(p => p.Matric_Number == viewmodel.Student.MatricNumber);
                int count = studnetList.Count;
                if (count == 1)
                {
                    var studentModelId = (int)studentLogic.GetModelBy(p => p.Matric_Number == viewmodel.Student.MatricNumber).Id;
                    person = personLogic.GetModelBy(p => p.Person_Id == studentModelId);
                    viewmodel.Student.MatricNumber = viewmodel.Student.MatricNumber;
                    viewmodel.Student.LastName = person.LastName;
                    viewmodel.Student.FirstName = person.FirstName;
                    viewmodel.Student.OtherName = person.OtherName;
                    viewmodel.Student.Email = person.Email;
                    viewmodel.Student.MobilePhone = person.MobilePhone;
                    viewmodel.Student.Id = person.Id;
                    studentLevel = studentLevelLogic.GetBy(viewmodel.Student.MatricNumber);
                    programme = programmeLogic.GetModelBy(p => p.Programme_Id == studentLevel.Programme.Id);
                    ViewBag.Programme = new SelectList(programmeLogic.GetAll(), ID, NAME, studentLevel.Programme.Id);
                    departments = deparmentLogic.GetBy(programme);
                    ViewBag.Department = new SelectList(departments, ID, NAME, studentLevel.Department.Id);
                    ViewBag.Level = new SelectList(levelLogic.GetAll(), ID, NAME, studentLevel.Level.Id);
                    viewmodel.DepartmentList = departments;
                    viewmodel.ProgrammeList = programmeLogic.GetAll();
                    studentLevel = studentLevelLogic.GetBy(viewmodel.Student.MatricNumber);
                    viewmodel.Session = studentLevel.Session;
                }
                else if (count < 1)
                {
                    SetMessage("Matric Number does not exist", Message.Category.Error);
                    return View("Correction", new SupportViewModel());
                }
                else if (count > 1)
                {
                    SetMessage("Duplicate Matriculation Number", Message.Category.Error);
                    return View("Correction", new SupportViewModel());
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred: " + ex.Message, Message.Category.Error);
            }

            TempData["studentModel"] = viewmodel;
            return View("Correction", viewmodel);
        }

        [HttpPost]
        public async Task<ActionResult> CorrectionAsync(SupportViewModel viewmodel)
        {
            var person = new Person();
            var studentLogic = new StudentLogic();
            var personLogic = new PersonLogic();
            var studentLevelLogic = new StudentLevelLogic();
            var studentLevel = new StudentLevel();
            var levelLogic = new LevelLogic();
            var departmentLogic = new DepartmentLogic();
            var programmeLogic = new ProgrammeLogic();
            var programme = new Programme();
            var departments = new List<Department>();
            var programmes = new List<Programme>();
            var levels = new List<Level>();
            var deparmentLogic = new DepartmentLogic();
            var loggeduser = new UserLogic();
            viewmodel.Role = new Role();
            try
            {

                List<Model.Model.Student> studnetList = await studentLogic.GetModelsByAsync(p => p.Matric_Number == viewmodel.Student.MatricNumber);

                int count = studnetList.Count;
                if (count == 1)
                {
                    var user = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                    if (user != null && user.Id > 0)
                    {
                        //Ensure no Editing If Faculty officer is accessing the module
                        if (user.Role.Name == "Faculty Officer")
                        {
                            
                            StaffLogic staffLogic = new StaffLogic();
                            Staff staff = new Staff();
                            staff = staffLogic.GetBy(user.Id);
                            if ((bool)staff.isManagement)
                            {
                                var facultyId = staff.Department.Faculty.Id;
                                var stdDept = GetStudentDept(studnetList.FirstOrDefault().Id);
                                if (facultyId != stdDept.Faculty.Id)
                                {
                                    SetMessage("You are not allowed to View this student's Detail", Message.Category.Error);
                                    return View("Correction", new SupportViewModel());
                                }
                            }
                            else if ((bool)staff.isHead)
                            {
                                var department = staff.Department.Id;
                                var stdDept = GetStudentDept(studnetList.FirstOrDefault().Id);
                                if (department != stdDept.Id)
                                {
                                    SetMessage("You are not allowed to View this student's Detail", Message.Category.Error);
                                    return View("Correction", new SupportViewModel());
                                }
                            }
                            viewmodel.Role.Name = user.Role.Name;
                        }
                    }
                        long studentModelId = studnetList.FirstOrDefault().Id;
                        person = personLogic.GetModelBy(p => p.Person_Id == studentModelId);
                        viewmodel.Student.MatricNumber = viewmodel.Student.MatricNumber;
                        viewmodel.Student.LastName = person.LastName;
                        viewmodel.Student.FirstName = person.FirstName;
                        viewmodel.Student.OtherName = person.OtherName;
                        viewmodel.Student.Email = person.Email;
                        viewmodel.Student.MobilePhone = person.MobilePhone;
                        viewmodel.Student.Id = person.Id;

                        studentLevel = await studentLevelLogic.GetByAsync(viewmodel.Student.MatricNumber);

                        programmes = await programmeLogic.GetAllAsync();
                        programme = programmeLogic.GetModelBy(p => p.Programme_Id == studentLevel.Programme.Id);
                        ViewBag.Programme = new SelectList(programmes, ID, NAME, studentLevel.Programme.Id);

                        departments = deparmentLogic.GetBy(programme);
                        ViewBag.Department = new SelectList(departments, ID, NAME, studentLevel.Department.Id);

                        levels = await levelLogic.GetAllAsync();
                        ViewBag.Level = new SelectList(levels, ID, NAME, studentLevel.Level.Id);

                        viewmodel.DepartmentList = departments;
                        viewmodel.ProgrammeList = programmes;

                        viewmodel.Session = studentLevel.Session;
                    }
                    else if (count < 1)
                    {
                        SetMessage("Matric Number does not exist", Message.Category.Error);
                        return View("Correction", new SupportViewModel());
                    }
                    else if (count > 1)
                    {
                        SetMessage("Duplicate Matriculation Number", Message.Category.Error);
                        return View("Correction", new SupportViewModel());
                    }
                }
            
            catch (Exception ex)
            {
                SetMessage("Error Occurred: " + ex.Message, Message.Category.Error);
            }

            TempData["studentModel"] = viewmodel;
            return View("Correction", viewmodel);
        }


        [HttpPost]
        public ActionResult SaveName(SupportViewModel viewmodel)
        {
            var person = new Person();
            var personLogic = new PersonLogic();
            var levelLogic = new LevelLogic();
            try
            {
                var initialstudentModel = (SupportViewModel)TempData["studentModel"];
                person = personLogic.GetModelBy(p => p.Person_Id == initialstudentModel.Student.Id);
                person.FirstName = viewmodel.Student.FirstName;
                person.LastName = viewmodel.Student.LastName;
                person.OtherName = viewmodel.Student.OtherName;
                person.Email = viewmodel.Student.Email;
                person.MobilePhone = viewmodel.Student.MobilePhone;

                bool modified = personLogic.Modify(person);

                var studentLogic = new StudentLogic();
                var student = new Model.Model.Student();
                student = studentLogic.GetBy(initialstudentModel.Student.Id);
                if (student != null)
                {
                    student.MatricNumber = viewmodel.Student.MatricNumber;
                    bool modifiedStudentStatus = studentLogic.Modify(student);
                }

                var studentLevelLogic = new StudentLevelLogic();
                var studentLevel = new StudentLevel();
                studentLevel.Level = viewmodel.Level;
                studentLevel.Department = viewmodel.Department;
                studentLevel.Programme = viewmodel.Programme;
                studentLevel.Session = initialstudentModel.Session;

                bool isStudentLevelModified = studentLevelLogic.Modify(studentLevel, person);
                if (modified || isStudentLevelModified)
                {
                    SetMessage("Correction Successful", Message.Category.Information);
                }
                else
                {
                    SetMessage("Correction Unsuccessful", Message.Category.Information);
                }
                viewmodel.Student = null;
                ViewBag.Programme = new SelectList(initialstudentModel.ProgrammeList, ID, NAME, viewmodel.Programme.Id);
                ViewBag.Department = new SelectList(initialstudentModel.DepartmentList, ID, NAME,
                    viewmodel.Department.Id);
                ViewBag.Level = new SelectList(levelLogic.GetAll(), ID, NAME, studentLevel.Level.Id);
            }

            catch (Exception ex)
            {
                SetMessage("Error: " + ex.Message, Message.Category.Error);
            }

            return View("Correction", viewmodel);
        }

        public ActionResult MatricNumberCorrection()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MatricNumberCorrection(SupportViewModel viewmodel)
        {
            try
            {
                var applicationForm = new ApplicationForm();
                var applicationFormLogic = new ApplicationFormLogic();
                applicationForm =
                    applicationFormLogic.GetModelBy(p => p.Application_Form_Number == viewmodel.ApplicationForm.Number);
                var student = new Model.Model.Student();
                var studentLogic = new StudentLogic();
                var studentLevel = new StudentLevel();
                var studentLevelLogic = new StudentLevelLogic();
                student = studentLogic.GetModelBy(p => p.Person_Id == applicationForm.Person.Id);
                var studentList = new List<Model.Model.Student>();
                studentList = studentLogic.GetModelsBy(p => p.Matric_Number == student.MatricNumber);
                Model.Model.Student studentToUpdate = updateStudentDuplicateMatricNumber(studentList, student);
                studentLevel = studentLevelLogic.GetModelBy(p => p.Person_Id == studentToUpdate.Id);
                bool isMatricNumberUpdated = studentLogic.UpdateMatricNumber(studentLevel, studentToUpdate);
                viewmodel.MatricNumber = studentToUpdate.MatricNumber;
                //if (studentList.Count > 1)
                //{
                //    bool isMatricNumberUpdated = studentLogic.UpdateMatricNumber(studentLevel, student);
                //    viewmodel.MatricNumber = student.MatricNumber;
                //}
            }
            catch (Exception)
            {
                throw;
            }
            return View(viewmodel);
        }

        public ActionResult ResetCourseRegistration()
        {
            var viewModel = new SupportViewModel();
            ViewBag.Session = viewModel.SessionSelectList;
            return View();
        }

        [HttpPost]
        public ActionResult ResetCourseRegistration(SupportViewModel viewModel)
        {
            try
            {
                var studentLogic = new StudentLogic();
                var courseRegistrationLogic = new CourseRegistrationLogic();
                var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                if (viewModel != null)
                {
                    if (viewModel.Student != null && viewModel.Session != null)
                    {
                        string matricNumber = viewModel.Student.MatricNumber;
                        List<Model.Model.Student> students =
                            studentLogic.GetModelsBy(p => p.Matric_Number == matricNumber);
                        if (students.Count == 1)
                        {
                            Model.Model.Student student = students[0];
                            CourseRegistration courseRegistration =
                                courseRegistrationLogic.GetModelBy(
                                    p => p.Person_Id == student.Id && p.Session_Id == viewModel.Session.Id);
                            if (courseRegistration != null)
                            {
                                List<CourseRegistrationDetail> courseRegistrationDetails =
                                    courseRegistrationDetailLogic.GetModelsBy(
                                        p => p.Student_Course_Registration_Id == courseRegistration.Id);
                                if (courseRegistrationDetails != null)
                                {
                                    if (courseRegistrationDetails.Count > 0)
                                    {
                                        foreach (
                                            CourseRegistrationDetail courseRegistrationDetail in
                                                courseRegistrationDetails)
                                        {
                                            if (courseRegistrationDetail.ExamScore == null &&
                                                courseRegistrationDetail.TestScore == null)
                                            {
                                                bool isDeleted =
                                                    courseRegistrationDetailLogic.Delete(
                                                        p =>
                                                            p.Student_Course_Registration_Detail_Id ==
                                                            courseRegistrationDetail.Id);
                                            }
                                        }
                                    }
                                }
                                bool isCourseRegistrationDeleted =
                                    courseRegistrationLogic.Delete(
                                        p => p.Student_Course_Registration_Id == courseRegistration.Id);
                                if (isCourseRegistrationDeleted)
                                {
                                    SetMessage("Reset successful ", Message.Category.Information);
                                }
                                else
                                {
                                    SetMessage("Reset Failed ", Message.Category.Error);
                                }
                            }
                        }
                        else
                        {
                            SetMessage("Reset Failed ", Message.Category.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred: " + ex.Message, Message.Category.Error);
                ViewBag.Session = viewModel.SessionSelectList;
            }
            ViewBag.Session = viewModel.SessionSelectList;
            return View();
        }

        //public ActionResult FixAllMatricNumberDuplicates()
        //{
        //  //  DuplicateMatricNumberFix duplicateMatricNumber = new DuplicateMatricNumberFix ();
        //    List<string> matricNumList = new List<string>();
        //    matricNumList.Add("PN/BF/12/1923");

        //    foreach (string item in matricNumList)
        //    {
        //        StudentLogic studentLogic = new StudentLogic();
        //        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
        //          List<Abundance_Nk.Model.Model.Student> studentList = new List<Model.Model.Student>();
        //          studentList = studentLogic.GetModelsBy(p => p.Matric_Number == item);

        //          foreach (Abundance_Nk.Model.Model.Student studentItem in studentList)
        //          {
        //              StudentLevel studentLevel = new StudentLevel();
        //              Abundance_Nk.Model.Model.Student studentToUpdate = updateStudentDuplicateMatricNumber(studentList, studentItem);
        //              studentLevel = studentLevelLogic.GetModelBy(p => p.Person_Id == studentToUpdate.Id);
        //              bool isMatricNumberUpdated = studentLogic.UpdateMatricNumber(studentLevel, studentToUpdate);
        //          }

        //    }
        //    return View();
        //}

        private Model.Model.Student updateStudentDuplicateMatricNumber(List<Model.Model.Student> studentList,
            Model.Model.Student student)
        {
            try
            {
                Model.Model.Student studentWithStudentNumber = null;
                var studentLogic = new StudentLogic();

                foreach (Model.Model.Student studentItem in studentList)
                {
                    string firstmatricNumber = studentItem.MatricNumber;
                    string[] matricNoArray = firstmatricNumber.Split('/');
                    long studentNumber = Convert.ToInt64(matricNoArray[3]);
                    studentItem.Number = studentNumber;
                    studentLogic.Modify(studentItem);
                    studentWithStudentNumber = studentItem;
                }

                return studentWithStudentNumber;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult UpdatePayment()
        {
            var viewmodel = new SupportViewModel();
            ViewBag.FeeTypeId = viewmodel.FeeTypeSelectList;
            ViewBag.SessionId = viewmodel.SessionSelectList;
            ViewBag.PaymentModeId = viewmodel.PaymentModeSelectListItem;
            viewmodel.Payment = new Payment();
            viewmodel.Payment.FeeType = new FeeType();
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePayment(SupportViewModel viewmodel)
        {
            try
            {
                var paymentEtranzact = new PaymentEtranzact();
                var paymentEtranzactLogic = new PaymentEtranzactLogic();
                var paymentTerminal = new PaymentTerminal();
                var paymentTerminalLogic = new PaymentTerminalLogic();

                paymentEtranzact = paymentEtranzactLogic.GetModelBy(a => a.Confirmation_No == viewmodel.PaymentEtranzact.ConfirmationNo);
                paymentTerminal = paymentTerminalLogic.GetModelsBy(p => p.Fee_Type_Id == viewmodel.FeeType.Id && p.Session_Id == viewmodel.Session.Id).FirstOrDefault();
                if (paymentEtranzact == null)
                {
                    paymentEtranzact = paymentEtranzactLogic.RetrieveEtranzactWebServicePinDetails(viewmodel.PaymentEtranzact.ConfirmationNo, paymentTerminal);
                }
                viewmodel.PaymentEtranzact = paymentEtranzact;
                viewmodel.PaymentEtranzact.Terminal = paymentTerminal;
                if (viewmodel.PaymentEtranzact.Payment.Payment != null)
                {
                    viewmodel.Payment = viewmodel.PaymentEtranzact.Payment.Payment;
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }
            ViewBag.FeeTypeId = viewmodel.FeeTypeSelectList;
            ViewBag.SessionId = viewmodel.SessionSelectList;
            ViewBag.PaymentModeId = viewmodel.PaymentModeSelectListItem;
            return View(viewmodel);
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePaymentAsync(SupportViewModel viewmodel)
        {
            try
            {
                var paymentEtranzact = new PaymentEtranzact();
                var paymentEtranzactLogic = new PaymentEtranzactLogic();
                var paymentTerminal = new PaymentTerminal();
                var paymentTerminalLogic = new PaymentTerminalLogic();

                paymentEtranzact = await paymentEtranzactLogic.GetModelByAsync(a => a.Confirmation_No == viewmodel.PaymentEtranzact.ConfirmationNo);
                paymentTerminal = await paymentTerminalLogic.GetModelByAsync(p => p.Fee_Type_Id == viewmodel.FeeType.Id && p.Session_Id == viewmodel.Session.Id);
                if (paymentEtranzact == null && paymentTerminal != null)
                {
                    paymentEtranzact = paymentEtranzactLogic.RetrieveEtranzactWebServicePinDetails(viewmodel.PaymentEtranzact.ConfirmationNo, paymentTerminal);
                }

                viewmodel.PaymentEtranzact = paymentEtranzact;
                viewmodel.PaymentEtranzact.Terminal = paymentTerminal;

                if (viewmodel.PaymentEtranzact.Payment.Payment != null)
                {
                    viewmodel.Payment = viewmodel.PaymentEtranzact.Payment.Payment;
                }

            }
            catch (Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }
            ViewBag.FeeTypeId = viewmodel.FeeTypeSelectList;
            ViewBag.SessionId = viewmodel.SessionSelectList;
            ViewBag.PaymentModeId = viewmodel.PaymentModeSelectListItem;
            return View("UpdatePayment", viewmodel);
        }


        [HttpPost]
        public ActionResult SavePayment(SupportViewModel viewmodel)
        {
            try
            {
                var paymentEtranzactLogic = new PaymentEtranzactLogic();
                var paymetEtranzact = new PaymentEtranzact();
                paymetEtranzact = viewmodel.PaymentEtranzact;
                var paymentLogic = new PaymentLogic();
                var payment = new Payment();
                payment = paymentLogic.GetModelBy(p => p.Invoice_Number == viewmodel.PaymentEtranzact.CustomerID);
                var paymentTerminalLogic = new PaymentTerminalLogic();
                PaymentTerminal paymentTerminal = paymentTerminalLogic.GetModelsBy(p => p.Fee_Type_Id == viewmodel.PaymentEtranzact.Payment.Payment.FeeType.Id).FirstOrDefault();
                var paymentEtranzactType = new PaymentEtranzactType();
                var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == viewmodel.PaymentEtranzact.Payment.Payment.FeeType.Id).LastOrDefault();
                paymetEtranzact.EtranzactType = paymentEtranzactType;
                paymetEtranzact.Terminal = paymentTerminal;
                PaymentMode paymentMode = viewmodel.Payment.PaymentMode;

                string operation = "UPDATE";
                string action = "UPDATE PIN";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress +
                                ")";

                var paymentEtranzactAudit = new PaymentEtranzactAudit();
                var loggeduser = new UserLogic();
                paymentEtranzactAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                paymentEtranzactAudit.Operation = operation;
                paymentEtranzactAudit.Action = action;
                paymentEtranzactAudit.Time = DateTime.Now;
                paymentEtranzactAudit.Client = client;

                if (payment != null)
                {
                    var onlinePayment = new OnlinePayment();
                    var onlinePaymentLogic = new OnlinePaymentLogic();
                    onlinePayment = onlinePaymentLogic.GetModelBy(c => c.PAYMENT_CHANNEL.Payment_Channnel_Id == (int)PaymentChannel.Channels.Etranzact && c.Payment_Id == payment.Id);
                    paymetEtranzact.Payment = onlinePayment;
                }

                PaymentEtranzact paymentEtranzactCheck = paymentEtranzactLogic.GetModelBy(p => p.Confirmation_No == paymetEtranzact.ConfirmationNo);
                if (paymentEtranzactCheck == null)
                {
                    //payment.PaymentMode = paymentMode;
                    // paymentLogic.Modify(payment);
                    paymetEtranzact = paymentEtranzactLogic.Create(paymetEtranzact, paymentEtranzactAudit);
                    SetMessage("Payment Updated Successfully! ", Message.Category.Information);

                }
                else
                {
                    payment.PaymentMode = paymentMode;
                    payment.FeeType = viewmodel.Payment.FeeType;
                    paymentLogic.Modify(payment);
                    paymentEtranzactLogic.Delete(paymentEtranzactCheck);
                    paymetEtranzact = paymentEtranzactLogic.Create(paymetEtranzact, paymentEtranzactAudit);
                    SetMessage("Payment updated! ", Message.Category.Warning);

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }
            ViewBag.FeeTypeId = viewmodel.FeeTypeSelectList;
            ViewBag.SessionId = viewmodel.SessionSelectList;
            ViewBag.PaymentModeId = viewmodel.PaymentModeSelectListItem;
            return View("UpdatePayment", viewmodel);
        }

        [HttpPost]
        public async Task<ActionResult> SavePaymentAsync(SupportViewModel viewmodel)
        {
            try
            {
                var paymentEtranzactLogic = new PaymentEtranzactLogic();
                var paymetEtranzact = new PaymentEtranzact();
                paymetEtranzact = viewmodel.PaymentEtranzact;
                var paymentLogic = new PaymentLogic();
                var payment = new Payment();
                payment = await paymentLogic.GetModelByAsync(p => p.Invoice_Number == viewmodel.PaymentEtranzact.CustomerID);

                var paymentTerminalLogic = new PaymentTerminalLogic();
                PaymentTerminal paymentTerminal = await paymentTerminalLogic.GetModelByAsync(p => p.Fee_Type_Id == viewmodel.PaymentEtranzact.Payment.Payment.FeeType.Id);

                var paymentEtranzactType = new PaymentEtranzactType();
                var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                paymentEtranzactType = await paymentEtranzactTypeLogic.GetModelByAsync(p => p.Fee_Type_Id == viewmodel.PaymentEtranzact.Payment.Payment.FeeType.Id);

                paymetEtranzact.EtranzactType = paymentEtranzactType;
                paymetEtranzact.Terminal = paymentTerminal;
                PaymentMode paymentMode = viewmodel.Payment.PaymentMode;

                string operation = "UPDATE";
                string action = "UPDATE PIN";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";

                var paymentEtranzactAudit = new PaymentEtranzactAudit();
                var loggeduser = new UserLogic();
                paymentEtranzactAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                paymentEtranzactAudit.Operation = operation;
                paymentEtranzactAudit.Action = action;
                paymentEtranzactAudit.Time = DateTime.Now;
                paymentEtranzactAudit.Client = client;

                if (payment != null)
                {
                    var onlinePayment = new OnlinePayment();
                    var onlinePaymentLogic = new OnlinePaymentLogic();
                    onlinePayment = await onlinePaymentLogic.GetModelByAsync(c => c.PAYMENT_CHANNEL.Payment_Channnel_Id == (int)PaymentChannel.Channels.Etranzact && c.Payment_Id == payment.Id);
                    paymetEtranzact.Payment = onlinePayment;
                }

                PaymentEtranzact paymentEtranzactCheck = await paymentEtranzactLogic.GetModelByAsync(p => p.Confirmation_No == paymetEtranzact.ConfirmationNo);
                if (paymentEtranzactCheck == null)
                {
                    //payment.PaymentMode = paymentMode;
                    // paymentLogic.Modify(payment);
                    paymetEtranzact = paymentEtranzactLogic.Create(paymetEtranzact, paymentEtranzactAudit);
                    SetMessage("Payment Updated Successfully! ", Message.Category.Information);

                }
                else
                {
                    payment.PaymentMode = paymentMode;
                    payment.FeeType = viewmodel.Payment.FeeType;
                    paymentLogic.Modify(payment);
                    paymentEtranzactLogic.Delete(paymentEtranzactCheck);
                    paymetEtranzact = paymentEtranzactLogic.Create(paymetEtranzact, paymentEtranzactAudit);
                    SetMessage("Payment updated! ", Message.Category.Warning);

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }

            ViewBag.FeeTypeId = viewmodel.FeeTypeSelectList;
            ViewBag.SessionId = viewmodel.SessionSelectList;
            ViewBag.PaymentModeId = viewmodel.PaymentModeSelectListItem;
            return View("UpdatePayment", viewmodel);
        }


        public ActionResult UpdatePassport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePassport(SupportViewModel viewmodel)
        {
            try
            {
                var applicationFormLogic = new ApplicationFormLogic();
                var applicationForm = new ApplicationForm();
                applicationForm =
                    applicationFormLogic.GetModelBy(p => p.Application_Form_Number == viewmodel.ApplicationForm.Number);
                viewmodel.Person = applicationForm.Person;
            }
            catch (Exception ex)
            {
                SetMessage("Error occured! " + ex.Message, Message.Category.Error);
            }

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult SaveStudentPassport(SupportViewModel viewmodel)
        {
            try
            {
                var personLogic = new PersonLogic();
                Person person = personLogic.GetModelBy(p => p.Person_Id == viewmodel.Person.Id);
                string extension = Path.GetExtension(viewmodel.File.FileName).ToLower();

                string invalidImage = InvalidFile(viewmodel.File.ContentLength, extension);
                if (string.IsNullOrEmpty(invalidImage))
                {
                    string imageUrl = getImageURL(viewmodel, person);
                    person.ImageFileUrl = imageUrl;
                    personLogic.Modify(person);
                    SetMessage("Correction Successful ", Message.Category.Information);
                }
                else
                {
                    SetMessage("Operation Failed, " + invalidImage, Message.Category.Error);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View("UpdatePassport", viewmodel);
        }

        private string getImageURL(SupportViewModel viewModel, Person person)
        {
            if (viewModel.File != null)
            {
                //Saving image to a folder and saving its url to db
                string[] allowedFIleExtensions = { ".jpg", ".png", ".jpeg", ".Jpeg" };

                string filenameWithExtension = Path.GetFileName(viewModel.File.FileName); // getting filename
                string extension = Path.GetExtension(viewModel.File.FileName).ToLower(); // getting only the extension
                if (allowedFIleExtensions.Contains(extension)) // check extension type
                {
                    string FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filenameWithExtension);
                    //retireves filename without extension
                    FileNameWithoutExtension = person.FullName;
                    string FileNameInServer = FileNameWithoutExtension + DateTime.Now.Year + "_" + DateTime.Now.Month +
                                              DateTime.Now.Day + "_" + extension;
                    // add reg number after underscore to make the filename unique for each person
                    string pathToFileInServer = Path.Combine(Server.MapPath("/Content/Student/"), FileNameInServer);
                    string passportUrl = "/Content/Student/" + FileNameInServer;
                    viewModel.File.SaveAs(pathToFileInServer);
                    return passportUrl;
                }
                return person.ImageFileUrl;
            }
            return person.ImageFileUrl;
        }

        private string InvalidFile(decimal uploadedFileSize, string fileExtension)
        {
            try
            {
                string message = null;
                decimal oneKiloByte = 1024;
                decimal maximumFileSize = 50 * oneKiloByte;

                decimal actualFileSizeToUpload = Math.Round(uploadedFileSize / oneKiloByte, 1);
                if (InvalidFileType(fileExtension))
                {
                    message = "File type '" + fileExtension +
                              "' is invalid! File type must be any of the following: .jpg, .jpeg, .png or .jif ";
                }
                else if (actualFileSizeToUpload > (maximumFileSize / oneKiloByte))
                {
                    message = "Your file size of " + actualFileSizeToUpload.ToString("0.#") +
                              " Kb is too large, maximum allowed size is " + (maximumFileSize / oneKiloByte) + " Kb";
                }

                return message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidFileType(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return false;

                case ".png":
                    return false;

                case ".gif":
                    return false;

                case ".jpeg":
                    return false;

                default:
                    return true;
            }
        }

        public ActionResult UnlockStudentData()
        {
            var viewModel = new SupportViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UnlockStudentData(SupportViewModel viewModel)
        {
            var vModel = new SupportViewModel();
            try
            {
                var studentLogic = new StudentLogic();
                var student = new Model.Model.Student();
                student = studentLogic.GetModelBy(p => p.Matric_Number == viewModel.Student.MatricNumber);
                var categoryLogic = new StudentCategoryLogic();
                vModel.Student = student;
                TempData["student"] = student;
                ViewBag.Category = new SelectList(categoryLogic.GetAll(), "Id", "Name", student.Category.Id);
            }
            catch (Exception ex)
            {
                SetMessage("Error, " + ex.Message, Message.Category.Error);
            }
            return View(vModel);
        }

        [HttpPost]
        public ActionResult SaveUnlockedData(SupportViewModel viewModel)
        {
            try
            {
                var studentLogic = new StudentLogic();
                var student = new Model.Model.Student();
                student = (Model.Model.Student)TempData["student"];
                student.Category = viewModel.Student.Category;
                studentLogic.Modify(student);
                SetMessage("Save Successful", Message.Category.Information);
            }
            catch (Exception ex)
            {
                SetMessage("Error, " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("UnlockStudentData");
        }

        public ActionResult ViewCarryOverCourses()
        {
            try
            {
                var viewModel = new SupportViewModel();
                ViewBag.Session = viewModel.AllSessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }
            return View();
        }

        [HttpPost]
        public ActionResult ViewCarryOverCourses(SupportViewModel viewModel)
        {
            try
            {
                var studentLevel = new StudentLevel();
                var studentLevelLogic = new StudentLevelLogic();
                var studentLogic = new StudentLogic();
                var courseLogic = new CourseLogic();
                if (viewModel != null)
                {
                    Model.Model.Student student = studentLogic.GetModelBy(p => p.Matric_Number == viewModel.MatricNumber);
                    if (student != null)
                    {
                        studentLevel = studentLevelLogic.GetModelsBy(p => p.Person_Id == student.Id).FirstOrDefault();
                        if (studentLevel != null && studentLevel.Department != null && viewModel.Level != null &&
                            viewModel.Semester != null)
                        {
                            viewModel.Courses = courseLogic.GetBy(studentLevel.Department, viewModel.Level,
                                viewModel.Semester, studentLevel.Programme);
                            viewModel.Department = studentLevel.Department;
                            viewModel.Programme = studentLevel.Programme;
                            TempData["supportViewModel"] = viewModel;

                            ViewBag.Session = viewModel.AllSessionSelectList;
                            ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                            ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME, viewModel.Level);

                            return View(viewModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured!" + ex.Message, Message.Category.Error);
            }
            return View();
        }

        // [HttpPost]
        //public ActionResult AddCarryOverCourses(SupportViewModel checkedCourseVM)
        //{
        //    try
        //    {
        //        SupportViewModel viewModel = (SupportViewModel)TempData["supportViewModel"];
        //        CourseRegistration courseRegistration = new CourseRegistration();
        //        CourseRegistration courseRegistrationNew = new CourseRegistration();
        //        CourseRegistrationDetail courseRegistrationDetail = new CourseRegistrationDetail();
        //        CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
        //        CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
        //        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
        //        StudentLogic studentLogic = new StudentLogic();
        //        if (viewModel != null && viewModel.MatricNumber != null && viewModel.Session != null && viewModel.Semester != null && viewModel.Programme != null && viewModel.Department != null && viewModel.Level != null && viewModel.Courses != null)
        //        {
        //            Model.Model.Student student = studentLogic.GetModelBy(p => p.Matric_Number == viewModel.MatricNumber);
        //            CourseMode courseMode = new CourseMode() { Id = 1 };

        //            if (student != null)
        //            {
        //                foreach (Course course in checkedCourseVM.Courses)
        //                {
        //                    if (course.IsRegistered == true)
        //                    {
        //                        courseRegistration = courseRegistrationLogic.GetModelBy(p => p.Department_Id == viewModel.Department.Id && p.Level_Id == viewModel.Level.Id && p.Person_Id == student.Id && p.Programme_Id == viewModel.Programme.Id && p.Session_Id == viewModel.Session.Id);
        //                        if (courseRegistration == null)
        //                        {
        //                            using (TransactionScope trans = new TransactionScope())
        //                            {
        //                                courseRegistration = new CourseRegistration();
        //                                courseRegistration.Student = student;
        //                                courseRegistration.Level = viewModel.Level;
        //                                courseRegistration.Programme = viewModel.Programme;
        //                                courseRegistration.Department = viewModel.Department;
        //                                courseRegistration.Session = viewModel.Session;
        //                                courseRegistrationNew = courseRegistrationLogic.Create(courseRegistration);

        //                                courseRegistrationDetail.CourseRegistration = courseRegistrationNew;
        //                                courseRegistrationDetail.Course = course;
        //                                courseRegistrationDetail.Mode = courseMode;
        //                                courseRegistrationDetail.Semester = viewModel.Semester;
        //                                courseRegistrationDetail.TestScore = 0;
        //                                courseRegistrationDetail.ExamScore = 0;
        //                                courseRegistrationDetailLogic.Create(courseRegistrationDetail);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            courseRegistrationDetail = courseRegistrationDetailLogic.GetModelBy(p => p.Student_Course_Registration_Id == courseRegistration.Id && p.Course_Id == course.Id && p.Semester_Id == viewModel.Semester.Id);
        //                            if (courseRegistrationDetail != null)
        //                            {
        //                                courseRegistrationDetail.TestScore = 0;
        //                                courseRegistrationDetail.ExamScore = 0;
        //                                courseRegistrationDetailLogic.Modify(courseRegistrationDetail);
        //                            }
        //                            else
        //                            {
        //                                courseRegistrationDetail = new CourseRegistrationDetail();
        //                                courseRegistrationDetail.CourseRegistration = courseRegistration;
        //                                courseRegistrationDetail.Course = course;
        //                                courseRegistrationDetail.Mode = courseMode;
        //                                courseRegistrationDetail.Semester = viewModel.Semester;
        //                                courseRegistrationDetail.TestScore = 0;
        //                                courseRegistrationDetail.ExamScore = 0;
        //                                courseRegistrationDetailLogic.Create(courseRegistrationDetail);
        //                            }

        //                        }

        //                        SetMessage("Course Added Successfully", Message.Category.Information);
        //                    }
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SetMessage("Error: " + ex.Message, Message.Category.Error);
        //    }

        //    return RedirectToAction("ViewCarryOverCourses");
        //}
        public ActionResult AddCarryOverCourses(SupportViewModel checkedCourseVM)
        {
            try
            {
                var viewModel = (SupportViewModel)TempData["supportViewModel"];
                var courseRegistration = new CourseRegistration();
                var courseRegistrationNew = new CourseRegistration();
                var courseRegistrationDetail = new CourseRegistrationDetail();
                var courseRegistrationLogic = new CourseRegistrationLogic();
                var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                var studentLevelLogic = new StudentLevelLogic();
                var studentLogic = new StudentLogic();
                if (viewModel != null && viewModel.MatricNumber != null && viewModel.Session != null &&
                    viewModel.Semester != null && viewModel.Programme != null && viewModel.Department != null &&
                    viewModel.Level != null && viewModel.Courses != null)
                {
                    Model.Model.Student student = studentLogic.GetModelBy(p => p.Matric_Number == viewModel.MatricNumber);
                    var courseMode = new CourseMode { Id = 1 };

                    if (student != null)
                    {
                        foreach (Course course in checkedCourseVM.Courses)
                        {
                            if (course.IsRegistered)
                            {
                                List<CourseRegistration> courseRegistrationList =
                                    courseRegistrationLogic.GetModelsBy(
                                        p =>
                                            p.Department_Id == viewModel.Department.Id &&
                                            p.Level_Id == viewModel.Level.Id && p.Person_Id == student.Id &&
                                            p.Programme_Id == viewModel.Programme.Id &&
                                            p.Session_Id == viewModel.Session.Id);
                                if (courseRegistrationList.Count > 1)
                                {
                                    bool isRemoved =
                                        courseRegistrationDetailLogic.RemoveDuplicateCourseRegistration(
                                            courseRegistrationList);
                                }

                                courseRegistration =
                                    courseRegistrationLogic.GetModelBy(
                                        p =>
                                            p.Department_Id == viewModel.Department.Id &&
                                            p.Level_Id == viewModel.Level.Id && p.Person_Id == student.Id &&
                                            p.Programme_Id == viewModel.Programme.Id &&
                                            p.Session_Id == viewModel.Session.Id);
                                if (courseRegistration == null)
                                {
                                    //using (TransactionScope trans = new TransactionScope(TransactionScopeOption.Required))
                                    //{
                                    courseRegistration = new CourseRegistration();
                                    courseRegistration.Student = student;
                                    courseRegistration.Level = viewModel.Level;
                                    courseRegistration.Programme = viewModel.Programme;
                                    courseRegistration.Department = viewModel.Department;
                                    courseRegistration.Session = viewModel.Session;
                                    courseRegistrationNew =
                                        courseRegistrationLogic.CreateCourseRegistration(courseRegistration);

                                    courseRegistrationDetail.CourseRegistration = courseRegistrationNew;
                                    courseRegistrationDetail.Course = course;
                                    courseRegistrationDetail.Mode = courseMode;
                                    courseRegistrationDetail.Semester = viewModel.Semester;
                                    courseRegistrationDetail.TestScore = 0;
                                    courseRegistrationDetail.ExamScore = 0;
                                    courseRegistrationDetailLogic.Create(courseRegistrationDetail);

                                    //trans.Complete();

                                    SetMessage("Course Added Successfully", Message.Category.Information);
                                    //}
                                }
                                else
                                {
                                    courseRegistrationDetail =
                                        courseRegistrationDetailLogic.GetModelBy(
                                            p =>
                                                p.Student_Course_Registration_Id == courseRegistration.Id &&
                                                p.Course_Id == course.Id && p.Semester_Id == viewModel.Semester.Id);
                                    if (courseRegistrationDetail != null)
                                    {
                                        courseRegistrationDetail.TestScore = 0;
                                        courseRegistrationDetail.ExamScore = 0;
                                        courseRegistrationDetailLogic.Modify(courseRegistrationDetail);
                                    }
                                    else
                                    {
                                        courseRegistrationDetail = new CourseRegistrationDetail();
                                        courseRegistrationDetail.CourseRegistration = courseRegistration;
                                        courseRegistrationDetail.Course = course;
                                        courseRegistrationDetail.Mode = courseMode;
                                        courseRegistrationDetail.Semester = viewModel.Semester;
                                        courseRegistrationDetail.TestScore = 0;
                                        courseRegistrationDetail.ExamScore = 0;
                                        courseRegistrationDetailLogic.Create(courseRegistrationDetail);
                                    }

                                    SetMessage("Course Added Successfully", Message.Category.Information);
                                }
                            }
                            //else
                            //{
                            //    SetMessage("No Course Was Added", Message.Category.Information);
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error: " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ViewCarryOverCourses");
        }

        public ActionResult ChangeMatricNumber()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangeMatricNumber(SupportViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    var studentLogic = new StudentLogic();
                    Model.Model.Student currentMatricNumberStudent =
                        studentLogic.GetModelBy(p => p.Matric_Number == viewModel.MatricNumber);
                    if (currentMatricNumberStudent == null)
                    {
                        SetMessage("The Current Matric Number does not exist", Message.Category.Error);
                        return View();
                    }
                    List<Model.Model.Student> studentNewMatricNumberCheck =
                        studentLogic.GetModelsBy(p => p.Matric_Number == viewModel.MatricNumberAlt);
                    if (studentNewMatricNumberCheck.Count == 0)
                    {
                        currentMatricNumberStudent.MatricNumber = viewModel.MatricNumberAlt;
                        studentLogic.Modify(currentMatricNumberStudent);

                        SetMessage("Matric Number Correction Successful", Message.Category.Information);
                        return View();
                    }
                    SetMessage("New Matric Number has already been assigned", Message.Category.Error);
                    return View();
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error: " + ex.Message, Message.Category.Error);
            }
            return View();
        }

        public ActionResult AddStaff()
        {
            var viewmodel = new SupportViewModel();
            return View();
        }

        [HttpPost]
        public ActionResult AddStaff(SupportViewModel viewModel)
        {
            try
            {
                viewModel = new SupportViewModel();
                var users = new List<User>();
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    hpf.SaveAs(savedFileName);
                    DataSet FileSet = ReadExcel(savedFileName);
                    if (FileSet != null && FileSet.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < FileSet.Tables[0].Rows.Count; i++)
                        {
                            var User = new User();
                            User.Username = FileSet.Tables[0].Rows[i][0].ToString().Trim();
                            User.Password = FileSet.Tables[0].Rows[i][1].ToString().Trim();
                            var role = new Role { Id = 10 };
                            User.Role = role;
                            User.LastLoginDate = DateTime.Now;
                            User.SecurityAnswer = "a";
                            var securityQuestion = new SecurityQuestion { Id = 1 };
                            User.SecurityQuestion = securityQuestion;

                            users.Add(User);
                        }
                    }
                }
                viewModel.Users = users;
                TempData["supportViewModel"] = viewModel;
            }
            catch (Exception ex)
            {
                SetMessage("Error" + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }

        public ActionResult SaveAddedStaff()
        {
            try
            {
                var userLogic = new UserLogic();
                var user = new User();
                bool itemSaved = false;
                var viewModel = new SupportViewModel();
                viewModel = (SupportViewModel)TempData["supportViewModel"];

                foreach (User item in viewModel.Users)
                {
                    List<User> users = userLogic.GetModelsBy(u => u.User_Name == item.Username);
                    if (users.Count == 0)
                    {
                        userLogic.Create(item);
                        itemSaved = true;
                    }
                }
                if (itemSaved)
                {
                    SetMessage("Added Successfully", Message.Category.Information);
                }
                else
                {
                    SetMessage("No Staff Added", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error." + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("AddStaff");
        }

        public ActionResult StaffAllocationUpload()
        {
            try
            {
                var viewModel = new SupportViewModel();
                ViewBag.AllSession = viewModel.SessionSelectList;
                ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                ViewBag.Programme = viewModel.ProgrammeSelectListItem;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }

        [HttpPost]
        public ActionResult StaffAllocationUpload(SupportViewModel viewModel)
        {
            try
            {
                var sessionLogic = new SessionLogic();
                Session session = sessionLogic.GetModelBy(p => p.Activated == true);
                var courseAllocations = new List<CourseAllocation>();
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    hpf.SaveAs(savedFileName);
                    DataSet FileSet = ReadExcel(savedFileName);
                    if (FileSet != null && FileSet.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < FileSet.Tables[0].Rows.Count; i++)
                        {
                            string courseCode = FileSet.Tables[0].Rows[i][0].ToString().Trim();
                            string username = FileSet.Tables[0].Rows[i][1].ToString().Trim();
                            var course = new Course { Code = courseCode };
                            var user = new User { Username = username };

                            var courseAllocation = new CourseAllocation();
                            courseAllocation.Department = viewModel.Department;
                            courseAllocation.Level = viewModel.Level;
                            courseAllocation.Programme = viewModel.Programme;
                            courseAllocation.Session = session;
                            courseAllocation.Course = course;
                            courseAllocation.User = user;
                            courseAllocations.Add(courseAllocation);
                        }
                    }
                }
                viewModel.CourseAllocationList = courseAllocations;
                TempData["supportViewModel"] = viewModel;
                RetainDropdownState(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error" + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }

        public ActionResult SaveStaffAllocationUpload()
        {
            var viewModel = (SupportViewModel)TempData["supportViewModel"];
            try
            {
                var sessionLogic = new SessionLogic();
                Session session = sessionLogic.GetModelBy(p => p.Activated == true);
                var courseAllocationLogic = new CourseAllocationLogic();
                var courseLogic = new CourseLogic();
                var userLogic = new UserLogic();
                bool itemSaved = true;

                foreach (CourseAllocation courseAllocationItem in viewModel.CourseAllocationList)
                {
                    User user = userLogic.GetModelBy(u => u.User_Name == courseAllocationItem.User.Username);
                    Course course =
                        courseLogic.GetModelsBy(
                            p =>
                                p.Course_Code == courseAllocationItem.Course.Code &&
                                p.Department_Id == viewModel.Department.Id && p.Level_Id == viewModel.Level.Id)
                            .FirstOrDefault();
                    if (user != null && course != null)
                    {
                        courseAllocationItem.User = user;
                        courseAllocationItem.Course = course;
                        CourseAllocation courseAllocationCheck =
                            courseAllocationLogic.GetModelBy(
                                p =>
                                    p.Course_Id == course.Id && p.User_Id == user.Id && p.Session_Id == session.Id &&
                                    p.Programme_Id == viewModel.Programme.Id);
                        if (courseAllocationCheck == null)
                        {
                            courseAllocationLogic.Create(courseAllocationItem);
                        }
                        itemSaved = true;
                    }
                }
                if (itemSaved)
                {
                    SetMessage("Courses allocated successfully", Message.Category.Information);
                }
                else
                {
                    SetMessage("No Staff Added", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error." + ex.Message, Message.Category.Error);
            }
            RetainDropdownState(viewModel);
            return RedirectToAction("StaffAllocationUpload");
        }

        [HttpPost]
        public ActionResult DownloadStaffUser(SupportViewModel viewModel)
        {
            try
            {
                var gv = new GridView();
                var staffUsers = new List<User>();
                var userLogic = new UserLogic();
                var staffUserList = new List<StaffUser>();
                staffUsers = userLogic.GetModelsBy(x => x.Role_Id == viewModel.Role.Id);
                if (staffUsers.Count > 0)
                {
                    for (int i = 0; i < staffUsers.Count; i++)
                    {
                        var staffUser = new StaffUser();
                        staffUser.USER_NAME = staffUsers[i].Username;
                        staffUser.PASSWORD = staffUsers[i].Password;
                        staffUserList.Add(staffUser);
                    }
                    if (staffUserList.Count > 0)
                    {
                        List<StaffUser> orderedstaffUserList = staffUserList.OrderBy(x => x.USER_NAME).ToList();
                        var finalstaffUserList = new List<StaffUser>();
                        for (int i = 0; i < orderedstaffUserList.Count; i++)
                        {
                            orderedstaffUserList[i].SN = (i + 1);
                            finalstaffUserList.Add(orderedstaffUserList[i]);
                        }
                        gv.DataSource = finalstaffUserList;
                        gv.Caption = "Course Staff User List";
                        gv.DataBind();
                        string filename = "User List";
                        return new DownloadFileActionResult(gv, filename + ".xls");
                    }
                    Response.Write("No data available for download");
                    Response.End();
                    return new JavaScriptResult();
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("EditStaff");
        }

        public ActionResult EditStaff()
        {
            try
            {
                var viewmodel = new SupportViewModel();
                ViewBag.Role = Utility.PopulateStaffRoleSelectListItem();
            }
            catch (Exception ex)
            {
                SetMessage("Error occurred! " + ex.Message, Message.Category.Error);
            }

            return View();
        }

        [HttpPost]
        public ActionResult EditStaff(SupportViewModel viewModel)
        {
            try
            {
                viewModel = new SupportViewModel();
                var users = new List<User>();
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    hpf.SaveAs(savedFileName);
                    DataSet FileSet = ReadExcel(savedFileName);
                    if (FileSet != null && FileSet.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 1; i < FileSet.Tables[0].Rows.Count; i++)
                        {
                            var User = new User();
                            User.Username = FileSet.Tables[0].Rows[i][1].ToString().Trim();
                            User.Password = FileSet.Tables[0].Rows[i][2].ToString().Trim();
                            users.Add(User);
                        }
                    }
                }
                viewModel.Users = users;
                TempData["EditStaff"] = viewModel;
            }
            catch (Exception ex)
            {
                SetMessage("Error" + ex.Message, Message.Category.Error);
            }
            ViewBag.Role = Utility.PopulateStaffRoleSelectListItem();
            return View(viewModel);
        }

        public ActionResult SaveEditedStaff()
        {
            try
            {
                var userLogic = new UserLogic();
                var user = new User();
                bool itemSaved = false;
                var viewModel = new SupportViewModel();
                viewModel = (SupportViewModel)TempData["EditStaff"];
                for (int i = 0; i < viewModel.Users.Count; i++)
                {
                    var editedUser = new User();
                    string username = viewModel.Users[i].Username;
                    editedUser = userLogic.GetModelBy(x => x.User_Name == username);
                    if (editedUser != null)
                    {
                        editedUser.Username = viewModel.Users[i].Username;
                        editedUser.Password = viewModel.Users[i].Password;
                        if (editedUser.Username == "" || editedUser.Password == "")
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    userLogic.Modify(editedUser);
                    itemSaved = true;
                }

                if (itemSaved)
                {
                    SetMessage("Edited Successfully", Message.Category.Information);
                }
                else
                {
                    SetMessage("No Staff Edited", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error." + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("EditStaff");
        }

        private DataSet ReadExcel(string filepath)
        {
            DataSet Result = null;
            try
            {
                string xConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" +
                                  "Extended Properties=Excel 8.0;";
                var connection = new OleDbConnection(xConnStr);

                connection.Open();
                DataTable sheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow dataRow in sheet.Rows)
                {
                    string sheetName = dataRow[2].ToString().Replace("'", "");
                    var command = new OleDbCommand("Select * FROM [" + sheetName + "]", connection);
                    // Create DbDataReader to Data Worksheet

                    var MyData = new OleDbDataAdapter();
                    MyData.SelectCommand = command;
                    var ds = new DataSet();
                    ds.Clear();
                    MyData.Fill(ds);
                    connection.Close();

                    Result = ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
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
                var SemesterLogic = new SemesterLogic();
                semesters = SemesterLogic.GetAll();
                return Json(new SelectList(semesters, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetDepartments(string id)
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

        public JsonResult GetCourses(int[] ids)
        {
            try
            {
                if (ids.Count() == 0)
                {
                    return null;
                }
                var level = new Level { Id = Convert.ToInt32(ids[1]) };
                var department = new Department { Id = Convert.ToInt32(ids[0]) };
                var semester = new Semester { Id = Convert.ToInt32(ids[2]) };
                var programme = new Programme { Id = Convert.ToInt32(ids[3]) };
                List<Course> courseList = Utility.GetCoursesByLevelDepartmentAndSemester(level, department, semester,
                    programme);

                return Json(new SelectList(courseList, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RetainDropdownState(SupportViewModel viewModel)
        {
            try
            {
                var semesterLogic = new SemesterLogic();
                var departmentLogic = new DepartmentLogic();
                var sessionLogic = new SessionLogic();
                var programmeLogic = new ProgrammeLogic();
                var levelLogic = new LevelLogic();
                if (viewModel != null)
                {
                    if (viewModel.Session != null)
                    {
                        ViewBag.Session = new SelectList(sessionLogic.GetModelsBy(p => p.Activated == true), ID, NAME,
                            viewModel.Session.Id);
                        ViewBag.AllSession = new SelectList(sessionLogic.GetAll(), ID, NAME, viewModel.Session.Id);
                    }
                    else
                    {
                        ViewBag.Session = viewModel.SessionSelectList;
                        ViewBag.AllSession = viewModel.SessionSelectList;
                    }
                    if (viewModel.Semester != null)
                    {
                        ViewBag.Semester = new SelectList(semesterLogic.GetAll(), ID, NAME, viewModel.Semester.Id);
                    }
                    else
                    {
                        ViewBag.Semester = new SelectList(new List<Semester>(), ID, NAME);
                    }
                    if (viewModel.Programme != null)
                    {
                        ViewBag.Programme = new SelectList(programmeLogic.GetModelsBy(p => p.Activated == true), ID,
                            NAME, viewModel.Programme.Id);
                    }
                    else
                    {
                        ViewBag.Programme = viewModel.ProgrammeSelectListItem;
                    }
                    if (viewModel.Department != null && viewModel.Programme != null)
                    {
                        ViewBag.Department = new SelectList(departmentLogic.GetBy(viewModel.Programme), ID, NAME,
                            viewModel.Department.Id);
                    }
                    else
                    {
                        ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                    }
                    if (viewModel.Level != null)
                    {
                        ViewBag.Level = new SelectList(levelLogic.GetAll(), ID, NAME, viewModel.Level.Id);
                    }
                    else
                    {
                        ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<Person> CreatePerson(SupportViewModel viewModel)
        {
            try
            {
                var role = new Role { Id = 5 };
                var personType = new PersonType { Id = 3 };
                var nationality = new Nationality { Id = 1 };

                viewModel.Person.Role = role;
                viewModel.Person.Nationality = nationality;
                viewModel.Person.DateEntered = DateTime.Now;
                viewModel.Person.Type = personType;
                var personLogic = new PersonLogic();
                Person person = await personLogic.CreateAsync(viewModel.Person);
                if (person != null && person.Id > 0)
                {
                    viewModel.Person.Id = person.Id;
                }

                return person;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Model.Model.Student> CreateStudent(SupportViewModel viewModel)
        {
            try
            {
                viewModel.Student.Number = 4;
                viewModel.Student.Status = new StudentStatus { Id = 1 };
                viewModel.Student.Type = new StudentType { Id = 1 };
                viewModel.Student.Category = new StudentCategory { Id = viewModel.StudentLevel.Level.Id <= 2 ? 1 : 2 };
                viewModel.Student.Id = viewModel.Person.Id;
                viewModel.Student.PasswordHash = "1234567";
                var studentLogic = new StudentLogic();
                viewModel.Student = await studentLogic.CreateAsync(viewModel.Student);
                return viewModel.Student;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<StudentLevel> CreateStudentLevel(SupportViewModel viewModel)
        {
            try
            {
                var studentLevelLogic = new StudentLevelLogic();
                viewModel.StudentLevel.Session = viewModel.Session;
                viewModel.StudentLevel.Student = viewModel.Student;
                viewModel.StudentLevel = await studentLevelLogic.CreateAsync(viewModel.StudentLevel);
                return viewModel.StudentLevel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult GetDepartmentAndLevelByProgrammeId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                List<Level> levels = null;
                List<Department> departments = null;
                var programme = new Programme { Id = Convert.ToInt32(id) };
                if (programme.Id > 0)
                {
                    var departmentLogic = new DepartmentLogic();
                    departments = departmentLogic.GetBy(programme);

                    var levelLogic = new LevelLogic();
                    levels = levelLogic.GetAll();
                }

                return Json(new { Departments = departments, Levels = levels }, "json", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetDepartmentOptionByDepartment(string id, string programmeid)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var department = new Department { Id = Convert.ToInt32(id) };
                var programme = new Programme { Id = Convert.ToInt32(programmeid) };
                var departmentLogic = new DepartmentOptionLogic();
                List<DepartmentOption> departmentOptions = departmentLogic.GetBy(department, programme);

                return Json(new SelectList(departmentOptions, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult CreateReturningStudent()
        {
            try
            {
                viewmodel = new SupportViewModel();
                viewmodel.FeeType = new FeeType { Id = (int)FeeTypes.SchoolFees };
                ViewBag.Levels = new SelectList(new List<Level>(), Utility.ID, Utility.NAME);
                ViewBag.States = viewmodel.StateSelectListItem;
                ViewBag.Sessions = viewmodel.SessionSelectList;
                ViewBag.Programmes = viewmodel.ProgrammeSelectListItem;
                ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
            }
            catch (Exception)
            {
                throw;
            }
            return View(viewmodel);
        }
        [HttpPost]
        public ActionResult CreateReturningStudent(SupportViewModel viewModel)
        {
            try
            {
                ModelState.Remove("Student.LastName");
                ModelState.Remove("Student.FirstName");
                ModelState.Remove("Person.DateOfBirth");
                ModelState.Remove("Student.MobilePhone");
                ModelState.Remove("Student.SchoolContactAddress");

                if (ModelState.IsValid)
                {
                    using (var transaction = new TransactionScope())
                    {
                        CreatePerson(viewModel);
                        if (viewModel.Person.Id > 0)
                        {
                            CreateStudent(viewModel);
                            if (viewModel.Student.Id > 0)
                            {
                                CreateStudentLevel(viewModel);
                                if (viewModel.StudentLevel.Id > 0)
                                {
                                    transaction.Complete();
                                    SetMessage("Student added succesfully", Message.Category.Information);
                                    return RedirectToAction("CreateReturningStudent");
                                }
                            }
                            else
                            {
                                SetMessage("Student was not added!", Message.Category.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error occurred: " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("CreateReturningStudent");
        }

        [HttpPost]
        public async Task<ActionResult> CreateReturningStudentAsync(SupportViewModel viewModel)
        {
            try
            {
                ModelState.Remove("Student.LastName");
                ModelState.Remove("Student.FirstName");
                ModelState.Remove("Person.DateOfBirth");
                ModelState.Remove("Student.MobilePhone");
                ModelState.Remove("Student.SchoolContactAddress");

                if (ModelState.IsValid)
                {
                    using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await Task.WhenAll(CreatePerson(viewModel));
                        if (viewModel.Person.Id > 0)
                        {
                            await Task.WhenAll(CreateStudent(viewModel));
                            if (viewModel.Student.Id > 0)
                            {
                                await Task.WhenAll(CreateStudentLevel(viewModel));
                                if (viewModel.StudentLevel.Id > 0)
                                {
                                    transaction.Complete();
                                    SetMessage("Student added succesfully", Message.Category.Information);
                                    return RedirectToAction("CreateReturningStudent");
                                }
                            }
                            else
                            {
                                SetMessage("Student was not added!", Message.Category.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error occurred: " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("CreateReturningStudent");
        }

        public ActionResult CorrectJambDetail()
        {
            try
            {
                viewmodel = new SupportViewModel();
                viewmodel.Person = new Person();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred: " + ex.Message, Message.Category.Error);
            }
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult CorrectJambDetail(SupportViewModel viewModel)
        {
            try
            {
                var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                var admissionListLogic = new AdmissionListLogic();
                AdmissionList admissionList =
                    admissionListLogic.GetBy(viewModel.ApplicantJambDetail.JambRegistrationNumber);
                if (admissionList != null && admissionList.Form.Id > 0)
                {
                    viewModel.ApplicantJambDetail = applicantJambDetailLogic.GetBy(admissionList.Form);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred: " + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UpdateJambDetail(SupportViewModel viewmodel)
        {
            var person = new Person();
            var personLogic = new PersonLogic();
            var applicantJambDetailLogic = new ApplicantJambDetailLogic();
            try
            {
                string operation = "UPDATE";
                string action = "MODIFY APPLICANT PERSON AND APPLIED COURSE DETAILS";
                string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress +
                                ")";

                var personAudit = new PersonAudit();
                var loggeduser = new UserLogic();
                personAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                personAudit.Operation = operation;
                personAudit.Action = action;
                personAudit.Time = DateTime.Now;
                personAudit.Client = client;

                var jambAudit = new ApplicantJambDetailAudit();
                jambAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                jambAudit.Operation = operation;
                jambAudit.Action = action;
                jambAudit.Time = DateTime.Now;
                jambAudit.Client = client;

                var oldData = personLogic.GetModelBy(a => a.Person_Id == viewmodel.ApplicantJambDetail.Person.Id);
                if (oldData != null)
                {
                    oldData.LastName = viewmodel.ApplicantJambDetail.Person.LastName;
                    oldData.FirstName = viewmodel.ApplicantJambDetail.Person.FirstName;
                    oldData.OtherName = viewmodel.ApplicantJambDetail.Person.OtherName;
                    bool personChanged = personLogic.Modify(oldData, personAudit);
                    var oldJambData = applicantJambDetailLogic.GetModelBy(a => a.Person_Id == viewmodel.ApplicantJambDetail.Person.Id);
                    if (oldJambData != null)
                    {
                        oldJambData.JambRegistrationNumber = viewmodel.ApplicantJambDetail.JambRegistrationNumber;
                        applicantJambDetailLogic.Modify(oldJambData, jambAudit);
                        SetMessage("Student updated successfully", Message.Category.Information);
                    }


                }

            }

            catch (Exception ex)
            {
                SetMessage("Error: " + ex.Message, Message.Category.Error);
            }

            return View("CorrectJambDetail", viewmodel);
        }

        public ActionResult DownloadBiodata()
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                var gv = new GridView();
                var resultFormatList = new List<StudentBiodataIDCardReport>();
                resultFormatList = studentLogic.GetBiodataReport();
                gv.DataSource = resultFormatList; // resultFormatList.OrderBy(p => p.MATRICNO);
                gv.DataBind();

                string filename = "Biodata.xls";
                return new DownloadFileActionResult(gv, filename);
            }
            catch (Exception ex)
            {
                SetMessage("Error occured! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index");
        }

        private void RetainPaymentDropDownState(StudentPayment payment)
        {
            try
            {
                if (payment != null)
                {
                    if (payment.Session != null)
                    {
                        ViewBag.Session = new SelectList(Utility.GetAllSessions(), "Id", "Name", payment.Session.Id);
                    }
                    if (payment.Level != null)
                    {
                        ViewBag.Level = new SelectList(Utility.GetAllLevels(), "Id", "Name", payment.Level.Id);
                    }
                    ViewBag.PaymentMode = new SelectList(Utility.PopulatePaymentModeSelectListItem(), "Value", "Text", payment.PaymentMode.Id);
                    ViewBag.FeeType = new SelectList(Utility.PopulateFeeTypeSelectListItem(), "Value", "Text", payment.FeeType.Id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult ViewStudentPayments()
        {
            SupportViewModel viewModel = null;
            try
            {
                viewModel = new SupportViewModel();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewStudentPayments(SupportViewModel viewModel)
        {
            try
            {
                if (viewModel.Student.MatricNumber != null)
                {
                    PaymentLogic paymentLogic = new PaymentLogic();
                    StudentLogic studentLogic = new StudentLogic();
                    List<Payment> paymentList = new List<Payment>();

                    List<Model.Model.Student> students = studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber);
                    if (students.Count != 1)
                    {
                        SetMessage("No Student with this Matric Number OR Matric Number is Duplicate", Message.Category.Error);
                        return View(viewModel);
                    }

                    long studentId = students[0].Id;
                    paymentList = paymentLogic.GetModelsBy(p => p.Person_Id == studentId);
                    if (paymentList.Count == 0)
                    {
                        SetMessage("No Payment Record for this Student", Message.Category.Error);
                        return View(viewModel);
                    }

                    viewModel.Payments = paymentList;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public async Task<ActionResult> EditPayment(long pmid)
        {
            SupportViewModel viewModel = new SupportViewModel();
            try
            {
                if (pmid > 0)
                {
                    PaymentLogic paymentLogic = new PaymentLogic();
                    StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                    Payment payment = await paymentLogic.GetModelByAsync(p => p.Payment_Id == pmid);
                    viewModel.Payment = payment;
                    var paymentHistory = studentPaymentLogic.GetBy(payment);
                    if (paymentHistory != null)
                    {
                        paymentHistory.FeeType = payment.FeeType;
                        paymentHistory.PaymentMode = payment.PaymentMode;
                        viewModel.StudentPayment = paymentHistory;
                    }
                    else
                    {
                        paymentHistory = new StudentPayment
                        {
                            Level = new Level() { Id = 1 },
                            PaymentMode = payment.PaymentMode,
                            FeeType = payment.FeeType,
                            Session = payment.Session
                        };
                    }
                    viewModel.StudentPayment = paymentHistory;
                    RetainPaymentDropDownState(paymentHistory);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditPayment(SupportViewModel viewModel)
        {
            try
            {
                if (viewModel.Payment != null)
                {
                    PaymentLogic paymentLogic = new PaymentLogic();
                    StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                    await paymentLogic.ModifyAsync(viewModel.Payment);
                    viewModel.StudentPayment.Id = viewModel.Payment.Id;
                    viewModel.StudentPayment.PaymentMode = viewModel.Payment.PaymentMode;
                    viewModel.StudentPayment.FeeType = viewModel.Payment.FeeType;
                    viewModel.StudentPayment.Session = viewModel.Payment.Session;
                    studentPaymentLogic.Modify(viewModel.StudentPayment);

                    SetMessage("Operation Successful!", Message.Category.Information);
                    return RedirectToAction("ViewStudentPayments");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainPaymentDropDownState(viewModel.StudentPayment);
            return View(viewModel);
        }

        public ActionResult ViewPaymentDetails()
        {
            SupportViewModel viewModel = null;
            try
            {
                viewModel = new SupportViewModel();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewPaymentDetails(SupportViewModel viewModel)
        {
            try
            {
                if (viewModel.Student.MatricNumber != null)
                {
                    PaymentLogic paymentLogic = new PaymentLogic();

                    var payment = paymentLogic.GetBy(viewModel.Student.MatricNumber);
                    if (payment != null)
                    {
                        return RedirectToAction("EditPayment", "Support", new { pmid = payment.Id });

                    }

                    SetMessage("No Payment Record with the number supplied", Message.Category.Error);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult DeleteDuplicateMatricNumber()
        {
            try
            {
                viewmodel = new SupportViewModel();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View();
        }

        [HttpPost]
        public ActionResult DeleteDuplicateMatricNumber(SupportViewModel viewModel)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                List<Model.Model.Student> students = studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber);
                if (students.Count == 0)
                {
                    SetMessage("Matric Number does not exist", Message.Category.Error);
                    return View();
                }
                viewModel.StudentList = students;
                TempData["ViewModel"] = viewModel;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        public ActionResult FixDuplicateMatricNumber()
        {
            try
            {
                long legitPersonId = 0;
                SupportViewModel viewModel = (SupportViewModel)TempData["ViewModel"];
                StudentLogic studentLogic = new StudentLogic();
                List<StudentLevel> studentList = new List<StudentLevel>();
                StudentLevel studentLevel = new StudentLevel();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                List<Model.Model.Student> students = studentLogic.GetModelsBy(s => s.Matric_Number == viewModel.Student.MatricNumber);
                if (students.Count == 1)
                {
                    SetMessage("Matric Number is not Duplicate", Message.Category.Information);
                }
                else
                {
                    studentList = studentLevelLogic.GetModelsBy(x => x.STUDENT.Matric_Number == viewModel.Student.MatricNumber);
                    legitPersonId = GetLegitimateStudent(studentList);
                    if (studentList != null && studentList.Count > 0)
                    {
                        foreach (StudentLevel studentItem in studentList)
                        {

                            using (TransactionScope scope = new TransactionScope())
                            {
                                CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                                List<CourseRegistration> courseRegistrations = courseRegistrationLogic.GetListBy(studentItem.Student, studentItem.Programme, studentItem.Department);

                                if (courseRegistrations.Count == 0 && studentItem.Student.Id != legitPersonId)
                                {
                                    if (courseRegistrations.Count != 0)
                                    {
                                        List<CourseRegistrationDetail> courseRegistrationDetails = new List<CourseRegistrationDetail>();
                                        CourseRegistrationDetailLogic courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
                                        CourseRegistration courseReg = courseRegistrationLogic.GetModelBy(p => p.Person_Id == studentItem.Student.Id && p.Session_Id == studentItem.Session.Id);
                                        courseRegistrationDetails = courseRegistrationDetailLogic.GetModelsBy(crd => crd.Student_Course_Registration_Id == courseReg.Id);
                                        if (courseRegistrationDetails.Count > 0)
                                        {
                                            foreach (CourseRegistrationDetail courseRegistrationDetail in courseRegistrationDetails)
                                            {
                                                Expression<Func<STUDENT_COURSE_REGISTRATION_DETAIL, bool>> deleteCourseRegistrationDetailSelector = crd => crd.Student_Course_Registration_Id == courseReg.Id;
                                                courseRegistrationDetailLogic.Delete(deleteCourseRegistrationDetailSelector);
                                            }
                                        }

                                        Expression<Func<STUDENT_COURSE_REGISTRATION, bool>> deleteCourseRegistrationSelector = cr => cr.Student_Course_Registration_Id == courseReg.Id;
                                        courseRegistrationLogic.Delete(deleteCourseRegistrationSelector);
                                    }

                                    Expression<Func<STUDENT_LEVEL, bool>> deleteStudentLevelSelector = sl => sl.Student_Level_Id == studentItem.Id;

                                    if (studentLevelLogic.Delete(deleteStudentLevelSelector))
                                    {
                                        CheckStudentSponsor(studentItem);
                                        CheckStudentFinanceInformation(studentItem);
                                        CheckStudentAcademicInformation(studentItem);
                                        CheckStudentResultDetails(studentItem);
                                        CheckStudentPaymentLog(studentItem);

                                        Expression<Func<STUDENT, bool>> deleteStudentSelector = s => s.Person_Id == studentItem.Student.Id;
                                        if (studentLogic.Delete(deleteStudentSelector))
                                        {
                                            PaymentLogic paymentLogic = new PaymentLogic();
                                            OnlinePaymentLogic onlinePyamentLogic = new OnlinePaymentLogic();
                                            PaymentEtranzactLogic paymentEtransactLogic = new PaymentEtranzactLogic();

                                            List<Payment> currentPayments = new List<Payment>();
                                            List<Payment> legitPayments = new List<Payment>();

                                            currentPayments = paymentLogic.GetModelsBy(p => p.Person_Id == studentItem.Student.Id);
                                            legitPayments = paymentLogic.GetModelsBy(p => p.Person_Id == legitPersonId);

                                            foreach (Payment currentPayment in currentPayments)
                                            {
                                                PaymentEtranzact checkEtranzact = paymentEtransactLogic.GetModelBy(p => p.Payment_Id == currentPayment.Id);

                                                if (checkEtranzact != null)
                                                {
                                                    Payment payment = paymentLogic.GetModelBy(p => p.Payment_Id == currentPayment.Id);
                                                    Person person = new Person() { Id = legitPersonId };
                                                    payment.Person = person;
                                                    paymentLogic.Modify(payment);

                                                    List<Payment> checkSamePayments = paymentLogic.GetModelsBy(p => p.Session_Id == payment.Session.Id && p.Fee_Type_Id == payment.FeeType.Id && p.Payment_Mode_Id == payment.PaymentMode.Id && p.Payment_Type_Id == payment.PaymentType.Id && p.Person_Id == legitPersonId);
                                                    foreach (Payment samePayment in checkSamePayments)
                                                    {
                                                        PaymentEtranzact etranzactRecordExists = paymentEtransactLogic.GetModelBy(p => p.Payment_Id == samePayment.Id);
                                                        if (etranzactRecordExists == null)
                                                        {
                                                            OnlinePayment onlinePayment = onlinePyamentLogic.GetModelBy(op => op.Payment_Id == samePayment.Id);

                                                            Expression<Func<ONLINE_PAYMENT, bool>> deleteOnlinePaymentSelector = op => op.Payment_Id == onlinePayment.Payment.Id;
                                                            if (onlinePyamentLogic.Delete(deleteOnlinePaymentSelector))
                                                            {
                                                                Expression<Func<PAYMENT, bool>> deletePaymentSelector = op => op.Payment_Id == onlinePayment.Payment.Id;
                                                                paymentLogic.Delete(deletePaymentSelector);
                                                            }
                                                        }
                                                    }

                                                    continue;
                                                }

                                                Payment checkPayment = legitPayments.Where(p => p.FeeType.Id == currentPayment.FeeType.Id && p.PaymentMode.Id == currentPayment.PaymentMode.Id && p.PaymentType.Id == currentPayment.PaymentType.Id && p.Session.Id == currentPayment.Session.Id).FirstOrDefault();
                                                if (checkPayment == null)
                                                {
                                                    Payment payment = paymentLogic.GetModelBy(p => p.Payment_Id == currentPayment.Id);
                                                    Person person = new Person() { Id = legitPersonId };
                                                    payment.Person = person;
                                                    paymentLogic.Modify(payment);
                                                }
                                                else
                                                {
                                                    OnlinePayment thisOnlinePayment = onlinePyamentLogic.GetModelBy(op => op.Payment_Id == currentPayment.Id);

                                                    if (thisOnlinePayment != null)
                                                    {
                                                        Expression<Func<ONLINE_PAYMENT, bool>> deleteOnlinePaymentSelector = op => op.PAYMENT.Payment_Id == currentPayment.Id;
                                                        if (onlinePyamentLogic.Delete(deleteOnlinePaymentSelector))
                                                        {
                                                            Expression<Func<PAYMENT, bool>> deletePaymentSelector = p => p.Payment_Id == currentPayment.Id;

                                                            paymentLogic.Delete(deletePaymentSelector);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Expression<Func<PAYMENT, bool>> deletePaymentSelector = p => p.Payment_Id == currentPayment.Id;

                                                        paymentLogic.Delete(deletePaymentSelector);
                                                    }
                                                }
                                            }
                                            //else
                                            //{
                                            //    List<Payment> payments = new List<Payment>();
                                            //    payments = paymentLogic.GetModelsBy(p => p.Person_Id == studentItem.Student.Id);
                                            //    Person person = new Person() { Id = legitPersonId };
                                            //    foreach (Payment payment in payments)
                                            //    {
                                            //        payment.Person = person;
                                            //        paymentLogic.Modify(payment);
                                            //    }    
                                            //}

                                            scope.Complete();
                                            SetMessage("Operation Successful", Message.Category.Information);

                                        }
                                    }
                                }
                                else
                                {
                                    legitPersonId = studentItem.Student.Id;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View("DeleteDuplicateMatricNumber");
        }
        private long GetLegitimateStudent(List<StudentLevel> studentList)
        {
            long personId = 0;
            try
            {
                foreach (StudentLevel studentItem in studentList)
                {
                    CourseRegistrationLogic courseRegistrationLogic = new CourseRegistrationLogic();
                    List<CourseRegistration> courseRegistrations = courseRegistrationLogic.GetListBy(studentItem.Student, studentItem.Programme, studentItem.Department);
                    foreach (CourseRegistration courseregistration in courseRegistrations)
                    {
                        if (courseRegistrations.Count >= 1 && courseregistration.Session.Id != 1)
                        {
                            personId = courseregistration.Student.Id;
                        }
                        else if (courseRegistrations.Count == 1 && personId == 0)
                        {
                            personId = courseregistration.Student.Id;
                        }
                    }
                }
                if (personId == 0)
                {
                    SessionLogic sessionLogic = new SessionLogic();
                    Session currentSession = sessionLogic.GetModelBy(s => s.Activated == true);
                    List<StudentLevel> studentLevels = studentList.Where(s => s.Session.Id == currentSession.Id).ToList();
                    if (studentLevels.Count >= 1)
                    {
                        personId = studentLevels.FirstOrDefault().Student.Id;
                    }
                    else
                    {
                        if (studentList.Count >= 1)
                        {
                            personId = studentList.FirstOrDefault().Student.Id;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return personId;
        }
        public ActionResult EditMatricNumber(string sid)
        {
            try
            {
                viewmodel = new SupportViewModel();
                long studentId = Convert.ToInt64(sid);
                StudentLogic studentLogic = new StudentLogic();
                Model.Model.Student student = studentLogic.GetModelBy(s => s.Person_Id == studentId);
                viewmodel.Student = student;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewmodel);
        }
        public ActionResult SaveEditedMatricNumber(SupportViewModel viewModel)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                Model.Model.Student student = new Model.Model.Student();
                student = studentLogic.GetModelBy(s => s.Person_Id == viewModel.Student.Id);
                student.MatricNumber = viewModel.Student.MatricNumber;
                studentLogic.Modify(student);
                SetMessage("Operation Successful", Message.Category.Information);
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("DeleteDuplicateMatricNumber");
        }

        public ActionResult ResetStudentPassword()
        {
            try
            {
                SupportViewModel viewModel = new SupportViewModel();
                return View(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<ActionResult> ResetStudentPassword(SupportViewModel viewModel)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                Model.Model.Student student = await studentLogic.GetByAsync(viewModel.Student.MatricNumber);
                if (student != null && student.Id > 0)
                {

                    student.PasswordHash = "1234567";
                    await studentLogic.ChangeUserPasswordAsync(student);
                    SetMessage("Password has been reset!", Message.Category.Information);

                }
                else
                {
                    SetMessage("User was not found!", Message.Category.Information);
                }

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult ResetPin()
        {
            viewmodel = new SupportViewModel();
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult ResetPin(SupportViewModel model)
        {
            try
            {
                if (model != null)
                {
                    ScratchCardLogic scratchCardLogic = new ScratchCardLogic();
                    ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();

                    var scratchCard = scratchCardLogic.GetModelBy(s => s.Pin == model.ScratchCard.Pin);
                    var applicationForm = applicationFormLogic.GetModelBy(af => af.Application_Form_Number == model.ApplicationForm.Number);
                    if (scratchCard == null && applicationForm == null)
                    {
                        SetMessage("Error Occured: Application Number or Pin does not exist kindly try again", Message.Category.Error);
                    }
                    model = new SupportViewModel
                    {
                        ScratchCard = scratchCard,
                        ApplicationForm = applicationForm
                    };

                    TempData["PIN"] = model;

                }

            }
            catch (Exception)
            {

                throw;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SavePinReset()
        {
            try
            {
                SupportViewModel model = (SupportViewModel)TempData["PIN"];
                ScratchCardLogic scratchCardLogic = new ScratchCardLogic();
                ScratchCard scratchCard = new ScratchCard();

                if (model != null)
                {
                    ScratchCardAudit audit = new ScratchCardAudit();
                    ScratchCardAuditLogic auditLogic = new ScratchCardAuditLogic();
                    UserLogic userLogic = new UserLogic();


                    string pin = model.ScratchCard.Pin;

                    scratchCard.person = model.ApplicationForm.Person;

                    bool updated = scratchCardLogic.Modify(pin, scratchCard.person);
                    if (updated)
                    {


                        audit.OldPerson = model.ScratchCard.person;
                        audit.NewPerson = model.ApplicationForm.Person;
                        audit.ScratchCard = model.ScratchCard;
                        audit.Action = "Modified Scratch card pin for {0} in support controller" + model.ScratchCard.Id;
                        audit.Operation = "Modified";
                        audit.Pin = model.ScratchCard.Pin;
                        audit.Client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress + ")";
                        audit.Time = DateTime.UtcNow;
                        audit.User = userLogic.GetModelBy(u => u.User_Name == User.Identity.Name);
                        audit.SerialNumber = model.ScratchCard.SerialNumber;


                        auditLogic.Create(audit);

                        SetMessage("Operation Successful", Message.Category.Information);
                    }
                }
                else
                {
                    SetMessage("Error Occured while Processing your request.Please Try Again", Message.Category.Error);
                }


            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("ResetPin");
        }

        public ActionResult DownloadPassport()
        {
            SupportViewModel viewModel = new SupportViewModel();
            try
            {
                ViewBag.Programme = Utility.PopulateProgrammeSelectListItem();
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Level = new SelectList(Utility.PopulateLevelSelectListItem(), VALUE, TEXT);
                ViewBag.Session = new SelectList(Utility.PopulateSessionSelectListItem(), VALUE, TEXT);
            }
            catch (Exception)
            {

                throw;
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DownloadPassport(SupportViewModel viewModel)
        {
            try
            {

                if (viewModel != null && viewModel.Programme != null && viewModel.Department != null && viewModel.Level != null)
                {
                    string savedFile = null;
                    string zipFileName = null;
                    int count = 0;
                    StudentLevelLogic studnetLogic = new StudentLevelLogic();
                    List<StudentLevel> studendtLevels = studnetLogic.GetModelsBy(s => s.Department_Id == viewModel.Department.Id && s.Programme_Id == viewModel.Programme.Id && s.Level_Id == viewModel.Level.Id && s.Session_Id == viewModel.Session.Id);
                    if (studendtLevels.Count > 0)
                    {
                        string path = Server.MapPath("~/Content/Junk/TempPassportFolder/");

                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);

                        }
                        Directory.CreateDirectory(path);
                        for (int i = 0; i < studendtLevels.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(studendtLevels[i].Student.ImageFileUrl))
                            {
                                if (System.IO.File.Exists(Server.MapPath(studendtLevels[i].Student.ImageFileUrl)))
                                {
                                    FileInfo file = new FileInfo(studendtLevels[i].Student.ImageFileUrl);
                                    string fileInfo = path + file.Name;
                                    file.CopyTo(fileInfo);
                                    count += 1;
                                }
                            }
                        }
                        if (count > 0)
                        {

                            using (ZipFile zip = new ZipFile())
                            {
                                string file = path;
                                zip.AddDirectory(file, "");
                                zipFileName = studendtLevels.FirstOrDefault().Department.Name + "_" + studendtLevels.FirstOrDefault().Programme.Name + "_" + studendtLevels.FirstOrDefault().Level.Name + "Level";
                                zip.Save(file + zipFileName + ".zip");
                                savedFile = file + zipFileName + ".zip";
                            }
                            return File(savedFile, "application/zip", zipFileName + ".zip");
                        }
                        else
                        {
                            SetMessage("No Passport found", Message.Category.Error);
                        }

                        RetainDropdownState(viewModel);

                    }
                    else
                    {
                        SetMessage("Students not found for the select Parameters", Message.Category.Error);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }
        public ActionResult DownloadApplicantPassport()
        {
            SupportViewModel viewModel = new SupportViewModel();
            try
            {
                ApplicationFormSettingLogic applicationFormSettingLogic = new ApplicationFormSettingLogic();
                List<Session> sessions = new List<Session>();
                var settingList = applicationFormSettingLogic.GetAll().LastOrDefault();
                if (settingList != null) sessions.Add(settingList.Session);

                ViewBag.Programme = Utility.PopulateProgrammeSelectListItem();
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                ViewBag.Session = new SelectList(sessions, "Id", "Name");
            }
            catch (Exception)
            {

                throw;
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DownloadApplicantPassport(SupportViewModel viewModel)
        {
            try
            {

                if (viewModel != null && viewModel.Programme != null && viewModel.Department != null)
                {
                    string savedFile = null;
                    string zipFileName = null;
                    int count = 0;
                    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    List<AppliedCourse> appliedCourses = appliedCourseLogic.GetModelsBy(s => s.Department_Id == viewModel.Department.Id && s.Programme_Id == viewModel.Programme.Id && s.APPLICATION_FORM_SETTING.Session_Id == viewModel.Session.Id);
                    if (appliedCourses.Count > 0)
                    {
                        string path = Server.MapPath("~/Content/Junk/TempPassportFolder/");

                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);

                        }
                        Directory.CreateDirectory(path);
                        for (int i = 0; i < appliedCourses.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(appliedCourses[i].Person.ImageFileUrl))
                            {
                                if (System.IO.File.Exists(Server.MapPath(appliedCourses[i].Person.ImageFileUrl)))
                                {
                                    FileInfo file = new FileInfo(appliedCourses[i].Person.ImageFileUrl);
                                    string fileInfo = path + file.Name;
                                    file.CopyTo(fileInfo);
                                    count += 1;
                                }
                            }
                        }
                        if (count > 0)
                        {

                            using (ZipFile zip = new ZipFile())
                            {
                                string file = path;
                                zip.AddDirectory(file, "");
                                zipFileName = appliedCourses.FirstOrDefault().Department.Name + "_" + appliedCourses.FirstOrDefault().Programme.Name;
                                zip.Save(file + zipFileName + ".zip");
                                savedFile = file + zipFileName + ".zip";
                            }
                            return File(savedFile, "application/zip", zipFileName + ".zip");
                        }
                        else
                        {
                            SetMessage("No Passport found", Message.Category.Error);
                        }

                        RetainDropdownState(viewModel);

                    }
                    else
                    {
                        SetMessage("Applicant not found for the select Parameters", Message.Category.Error);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }

        public ActionResult RetrievePinWithSerial()
        {
            viewmodel = new SupportViewModel();
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult RetrievePinWithSerial(SupportViewModel model)
        {
            try
            {
                if (model != null)
                {
                    ScratchCardLogic scratchCardLogic = new ScratchCardLogic();
                    ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();

                    var scratchCard = scratchCardLogic.GetModelsBy(s => s.Serial_Number.Contains(model.ScratchCard.Pin));
                    if (scratchCard == null)
                    {
                        SetMessage("Error Occured:Pin does not exist kindly try again", Message.Category.Error);
                    }
                    model = new SupportViewModel
                    {
                        ScratchCards = scratchCard,
                    };

                    TempData["PIN"] = model;

                }

            }
            catch (Exception ex)
            {

                SetMessage("Error Occured while Processing your request.Please Try Again", Message.Category.Error);
            }
            return View(model);
        }

        public ActionResult InterSwitchPayment()
        {
            return View();
        }

        public ContentResult GetInterSwitchPayment(string dateFrom, string dateTo, string sortOption)
        {
            PaymentReportArray reportArray = new PaymentReportArray();
            List<PaymentReportArray> paymentReportArrayList = new List<PaymentReportArray>();
            try
            {
                var result = new ContentResult();


                if (string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo) && string.IsNullOrEmpty(sortOption))
                {
                    return null;
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };

                PaymentInterswitchLogic paymentInterswitchLogic = new PaymentInterswitchLogic();
                List<PaymentEtranzactView> paymentEtranzactView = paymentInterswitchLogic.GetPaymentBy(dateFrom, dateTo, sortOption);
                if (paymentEtranzactView != null && paymentEtranzactView.Count > 0)
                {
                    for (int i = 0; i < paymentEtranzactView.Count; i++)
                    {
                        PaymentReportArray paymentReport = new PaymentReportArray();
                        paymentReport.FullName = paymentEtranzactView[i].FullName;
                        paymentReport.MatricNumber = paymentEtranzactView[i].MatricNumber ?? "-";
                        paymentReport.JambNumber = paymentEtranzactView[i].JambNumber;
                        paymentReport.Amount = String.Format("{0:N}", paymentEtranzactView[i].TransactionAmount);
                        paymentReport.LevelName = paymentEtranzactView[i].LevelName ?? "-";
                        paymentReport.DepartmentName = paymentEtranzactView[i].DepartmentName ?? "-";
                        paymentReport.FacultyName = paymentEtranzactView[i].FacultyName ?? "-";
                        paymentReport.ProgrammeName = paymentEtranzactView[i].ProgrammeName ?? "-";
                        paymentReport.SessionName = paymentEtranzactView[i].SessionName;
                        paymentReport.TransactionDate = paymentEtranzactView[i].TransactionDate.ToString();
                        paymentReport.ConfirmationNo = paymentEtranzactView[i].ConfirmationNo ?? "-";
                        paymentReport.InvoiceNumber = paymentEtranzactView[i].InvoiceNumber;
                        paymentReport.ResponseDecription = paymentEtranzactView[i].ResponseDecription + " (" + paymentEtranzactView[i].ResponseCode + ")";
                        paymentReport.Issuccessful = paymentEtranzactView[i].Issuccessful;
                        paymentReport.MearchantReference = paymentEtranzactView[i].MearchantReference;
                        paymentReportArrayList.Add(paymentReport);

                    }

                }
                else
                {
                    reportArray.IsError = true;
                    reportArray.ErrorMessage = "No Record found for the selected date range";
                    paymentReportArrayList.Add(reportArray);
                }

                var serializedList = serializer.Serialize(paymentReportArrayList);
                result = new ContentResult
                {
                    Content = serializedList,
                    ContentType = "application/json"
                };

                return result;
            }
            catch (Exception ex)
            {
                reportArray.IsError = true;
                reportArray.ErrorMessage = "Error Occured" + ex;
                paymentReportArrayList.Add(reportArray);

            }
            return null;
        }

        public JsonResult QueryInterSwitchTransaction(string invoiceNumber)
        {
            PaymentInterswitch paymentInterSwitch = new PaymentInterswitch();
            try
            {
                PaymentInterswitchLogic paymentInterswitchLogic = new PaymentInterswitchLogic();
                paymentInterSwitch = paymentInterswitchLogic.TransactionStatus(invoiceNumber);

            }
            catch (Exception ex)
            {

                throw;
            }
            return Json(paymentInterSwitch, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StudentRecord()
        {
            SupportViewModel viewModel = new SupportViewModel();

            return View(viewModel);

        }
        [HttpPost]
        public ActionResult StudentRecord(SupportViewModel viewModel)
        {
            try
            {
                if (viewModel.Student != null && !string.IsNullOrEmpty(viewModel.Student.MatricNumber))
                {
                    PersonLogic personLogic = new PersonLogic(); viewModel.Person = personLogic.GetPersonByMatricNumber(viewModel.Student.MatricNumber);

                    if (viewModel.Person != null)
                    {
                        StudentLogic studentLogic = new StudentLogic();
                        viewModel.Student = studentLogic.GetModelBy(s => s.Person_Id == viewModel.Person.Id);

                        AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                        viewModel.AppliedCourse = appliedCourseLogic.GetModelBy(s => s.Person_Id == viewModel.Person.Id);
                        if (viewModel.AppliedCourse != null)
                            viewModel.ApplicationForm = viewModel.AppliedCourse.ApplicationForm;

                        ApplicantJambDetailLogic jambDetailLogic = new ApplicantJambDetailLogic();
                        if (viewModel.ApplicationForm != null)
                            viewModel.ApplicantJambDetail = jambDetailLogic.GetModelsBy(j => j.Application_Form_Id == viewModel.ApplicationForm.Id).LastOrDefault();
                        else
                            viewModel.ApplicantJambDetail = jambDetailLogic.GetModelsBy(j => j.Person_Id == viewModel.Person.Id).LastOrDefault();

                        AdmissionListLogic listLogic = new AdmissionListLogic();
                        viewModel.AdmissionList = viewModel.ApplicationForm != null ? listLogic.GetModelsBy(a => a.Application_Form_Id == viewModel.ApplicationForm.Id).LastOrDefault() : null;

                        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                        viewModel.StudentLevels = viewModel.Student != null ? studentLevelLogic.GetModelsBy(s => s.Person_Id == viewModel.Student.Id) : null;

                        PaymentLogic paymentLogic = new PaymentLogic();
                        viewModel.Payments = paymentLogic.GetModelsBy(p => p.Person_Id == viewModel.Person.Id);

                        ScratchCardLogic scratchCardLogic = new ScratchCardLogic();
                        viewModel.ScratchCards = scratchCardLogic.GetModelsBy(s => s.Person_Id == viewModel.Person.Id);

                        viewModel.PaymentHistory = new PaymentHistory();
                        viewModel.PaymentHistory.Payments = paymentLogic.GetBy(viewModel.Person);
                        List<PaymentView> paystackPayments = paymentLogic.GetPaystackPaymentBy(viewModel.Person);
                        viewModel.PaymentHistory.Payments = viewModel.PaymentHistory.Payments != null ? viewModel.PaymentHistory.Payments.Where(p => p.ConfirmationOrderNumber != null).ToList() : null;
                        viewModel.PaymentHistory.Payments.AddRange(paystackPayments);

                    }
                    else
                        SetMessage("Student not found.", Message.Category.Error);
                }
                else
                    SetMessage("Invalid search parameter.", Message.Category.Error);
            }
            catch (Exception ex)
            {

                SetMessage("Error Occured while Processing your request.Please Try Again.", Message.Category.Error);
            }

            return View(viewModel);
        }
        public Department GetStudentDept(long sId)
        {
            Department dept;
            try
            {

                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                var studentLevel = studentLevelLogic.GetModelBy(x => x.Person_Id == sId);
                dept = studentLevel.Department;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dept;
        }
        public ActionResult MakeManualPayment()
        {
            try
            {
                SupportViewModel viewModel = new SupportViewModel();
                viewModel.Payment = new Payment();
                viewModel.StudentLevel = new StudentLevel();
                viewModel.Student = new Model.Model.Student();
                viewModel.ManualPayments = new List<ManualPayment>();
                return View(viewModel);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult MakeManualPayment(SupportViewModel viewModel)
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = new StudentLevel();
                StudentLogic studentLogic = new StudentLogic();
                viewModel.Student = new Model.Model.Student();
                viewModel.ManualPayments = new List<ManualPayment>();
                StudentPayment studentPayment = new StudentPayment();
                StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                Payment payment = new Payment();
                viewModel.Payment = new Payment();
                viewModel.ApplicationForm = new ApplicationForm();
                viewModel.AdmissionList = new AdmissionList();
                
                if (viewModel.InvoiceNumber != null)
                {
                    string invNumber = viewModel.InvoiceNumber;
                    viewModel.Payment = new Payment();
                    viewModel.StudentLevel = new StudentLevel();
                    payment = paymentLogic.GetModelsBy(x => x.Invoice_Number == invNumber).LastOrDefault();
                    if (payment != null)
                    {
                        studentPayment=studentPaymentLogic.GetModelsBy(x => x.Payment_Id == payment.Id).LastOrDefault();
                        studentLevel =studentLevelLogic.GetModelsBy(x => x.Person_Id == payment.Person.Id).FirstOrDefault();
                        viewModel.ApplicationForm=applicationFormLogic.GetModelsBy(x => x.Person_Id == payment.Person.Id).LastOrDefault();

                        if (studentPayment != null && studentLevel!=null)
                        {
                            viewModel.StudentLevel = studentLevel;
                            viewModel.StudentPayment = studentPayment;
                            viewModel.StudentLevel = studentLevel;
                            viewModel.Payment.InvoiceNumber = invNumber;
                            viewModel.Student=studentLogic.GetModelBy(x => x.Person_Id == payment.Person.Id);
                        }
                        else if(viewModel.ApplicationForm!=null)
                        {
                            viewModel.Payment.InvoiceNumber = invNumber;
                            viewModel.AdmissionList= admissionListLogic.GetModelBy(x => x.Application_Form_Id == viewModel.ApplicationForm.Id);
                            var level = new Level();
                           
                            if (viewModel.AdmissionList != null)
                            {
                                level=Utility.SetLevel(viewModel.AdmissionList.Programme);
                                var amount=feeDetailLogic.GetModelsBy(x => x.Fee_Type_Id == payment.FeeType.Id && x.Session_Id == payment.Session.Id && 
                                x.Department_Id == viewModel.AdmissionList.Deprtment.Id && x.Programme_Id == viewModel.AdmissionList.Programme.Id &&
                                x.Payment_Mode_Id == payment.PaymentMode.Id && x.Level_Id == level.Id).Sum(x=>x.Fee.Amount);
                                viewModel.Payment.Amount = amount.ToString();
                            }
                        }
                    }
                    else
                    {
                        SetMessage("Payment Not Found", Message.Category.Error);
                        return View();
                    }

                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ApproveManualPayment (string code, string ivn,decimal amount)
        {
            JsonResultView result = new JsonResultView();
            try
            {
                if (!String.IsNullOrEmpty(code) && !String.IsNullOrEmpty(ivn))
                {
                    if (code != "1234lloy")
                    {
                        result.IsError = true;
                        result.Message = "You are not Authorized for this Action";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    PaymentLogic paymentLogic = new PaymentLogic();
                    ManualPayment manualPayment = new ManualPayment();
                    ManualPaymentLogic manualPaymentLogic = new ManualPaymentLogic();
                    PaymentEtranzactTypeLogic paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                    Payment payment = new Payment();
                    UserLogic loggeduser = new UserLogic();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    StudentPayment studentPayment = new StudentPayment();
                    StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                    PaymentEtranzactType etranzactType = new PaymentEtranzactType();
                    var existingPayment=manualPaymentLogic.GetModelBy(x => x.Invoice_Number == ivn);
                    AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                    ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                    ApplicationForm applicationForm = new ApplicationForm();
                    AdmissionList admissionList = new AdmissionList();
                    var level = new Level();
                    if (existingPayment != null)
                    {
                        result.IsError = true;
                        result.Message = "The Payment of this invoice has already been Approved";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    var user = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                    payment = paymentLogic.GetModelsBy(x => x.Invoice_Number == ivn).LastOrDefault();
                    if (payment != null)
                    {
                        studentPayment = studentPaymentLogic.GetModelsBy(x => x.Payment_Id == payment.Id).LastOrDefault();
                        StudentLevel studentLevel = studentLevelLogic.GetModelBy(x=>x.Person_Id==payment.Person.Id);
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            manualPayment.Amount = amount;
                            manualPayment.FeeType = payment.FeeType;
                            manualPayment.InvoiceNumber = ivn;
                            manualPayment.Person = payment.Person;
                            manualPayment.Session = payment.Session;
                            manualPayment.User = user;
                            var isCreated = manualPaymentLogic.Create(manualPayment);
                            if(studentPayment!=null && studentLevel != null)
                            {
                                etranzactType =
                                        paymentEtranzactTypeLogic.GetModelBy(
                                            s =>
                                                s.Fee_Type_Id == payment.FeeType.Id &&
                                                s.Programme_Id == studentLevel.Programme.Id &&
                                                s.Level_Id == studentPayment.Level.Id && s.Payment_Mode_Id == payment.PaymentMode.Id && s.Session_Id == payment.Session.Id);
                            }
                            else
                            {
                                applicationForm = applicationFormLogic.GetModelsBy(x => x.Person_Id == payment.Person.Id).LastOrDefault();
                                if (applicationForm != null)
                                {
                                    admissionList = admissionListLogic.GetModelBy(x => x.Application_Form_Id == applicationForm.Id);
                                    level = Utility.SetLevel(admissionList.Programme);
                                    etranzactType =
                                        paymentEtranzactTypeLogic.GetModelsBy(
                                            s =>
                                                s.Fee_Type_Id == payment.FeeType.Id &&
                                                s.Programme_Id == admissionList.Programme.Id &&
                                                s.Level_Id == level.Id && s.Payment_Mode_Id == payment.PaymentMode.Id && s.Session_Id == payment.Session.Id).LastOrDefault();
                                }
                                
                            }
                             
                            PaymentEtranzact paymentEtranzact = new PaymentEtranzact();

                            paymentEtranzact.Payment = new OnlinePayment()
                            {
                                Payment = payment
                            };
                            paymentEtranzact.PaymentCode = payment.InvoiceNumber;
                            paymentEtranzact.EtranzactType = etranzactType;
                            paymentEtranzact.ReceiptNo = payment.Id.ToString();
                            paymentEtranzact.ConfirmationNo = payment.InvoiceNumber;
                            paymentEtranzact.CustomerName = payment.Person.FullName;
                            paymentEtranzact.TransactionAmount = amount;
                            paymentEtranzact.TransactionDescription = "Manual Payment";
                            paymentEtranzact.TransactionDate = DateTime.Today;
                            paymentEtranzact.CustomerID = payment.InvoiceNumber;
                            paymentEtranzact.Terminal = new PaymentTerminal() { Id = 5 };
                            var existingPaymetEtranzact=paymentEtranzactLogic.GetBy(payment);
                            if (existingPaymetEtranzact == null)
                            {
                                paymentEtranzactLogic.Create(paymentEtranzact);
                            }
                            transaction.Complete();
                            result.IsError = false;
                            result.Message = "Payment Approval was Successful";
                        }
                    }

                }

            }
            catch(Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewManualPayment()
        {
            try
            {
                SupportViewModel viewModel = new SupportViewModel();
                ManualPaymentLogic manualPaymentLogic = new ManualPaymentLogic();
                viewModel.Student = new Model.Model.Student();
                viewModel.ManualPayments = new List<ManualPayment>();
                viewModel.ManualPayments=manualPaymentLogic.GetAll();
                return View("MakeManualPayment", viewModel);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult LecturerUserManual()
        {
            try
            {
                return File(Server.MapPath("~/Content/Manual/") + "ABSU_Userguide_manual_Lecturer.pdf", "application/pdf", "User_Manual.pdf");
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured. " + ex.Message, Message.Category.Error);
                return RedirectToAction("Home", "Account", new { Area = "Security"});
            }
        }
        public ActionResult HODUSerManual()
        {
            try
            {
                return File(Server.MapPath("~/Content/Manual/") + "ABSU_HOD_Manual.pdf", "application/pdf", "User_Manual.pdf");
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured. " + ex.Message, Message.Category.Error);
                return RedirectToAction("Home", "Account", new { Area = "Security" });
            }
        }
        
        public ActionResult BursaryUserManual()
        {
            try
            {
                return File(Server.MapPath("~/Content/Manual/") + "ABSU_Bursary_manual.pdf", "application/pdf", "User_Manual.pdf");
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured. " + ex.Message, Message.Category.Error);
                return RedirectToAction("Home", "Account", new { Area = "Security" });
            }
        }
        public ActionResult SAOUserManual()
        {
            try
            {
                return File(Server.MapPath("~/Content/Manual/") + "ABSU_Students Account Officer_manual.pdf", "application/pdf", "User_Manual.pdf");
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured. " + ex.Message, Message.Category.Error);
                return RedirectToAction("Home", "Account", new { Area = "Security" });
            }
        }
        public ActionResult VOUSermanual()
        {
            try
            {
                return File(Server.MapPath("~/Content/Manual/") + "ABSU_Verification Officer_manual.pdf", "application/pdf", "User_Manual.pdf");
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured. " + ex.Message, Message.Category.Error);
                return RedirectToAction("Home", "Account", new { Area = "Security" });
            }
        }
        
        public ActionResult FacultyOfficerUsermanual()
        {
            try
            {
                return File(Server.MapPath("~/Content/Manual/") + "Faculty_Officer_User_Manual.pdf", "application/pdf", "User_Manual.pdf");
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured. " + ex.Message, Message.Category.Error);
                return RedirectToAction("Home", "Account", new { Area = "Security" });
            }
        }
        public ActionResult DeanUserManual()
        {
            try
            {
                return File(Server.MapPath("~/Content/Manual/") + "ABSU_Dean_Manual.pdf", "application/pdf", "User_Manual.pdf");
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured. " + ex.Message, Message.Category.Error);
                return RedirectToAction("Home", "Account", new { Area = "Security" });
            }
        }
        public class JsonResultView
        {
            public bool IsError { get; set; }
            public string Message { get; set; }
        }
        [HttpGet]
        public ActionResult ManageDepartment()
        {

            SupportViewModel supportViewModel = new SupportViewModel();
            supportViewModel = (SupportViewModel)TempData["ProgrammeDepartment"];
            if (supportViewModel == null)
            {
                supportViewModel = new SupportViewModel();
                supportViewModel.ShowTable = false;
            }
            
            ViewBag.ProgrammeId = Utility.PopulateAllProgrammeSelectListItem();
            return View(supportViewModel);
        }
        [HttpPost]
        public ActionResult ManageDepartment(SupportViewModel supportViewModel)
        {
            try
            {
                supportViewModel.ShowTable = true;
                ViewBag.ProgrammeId = Utility.PopulateAllProgrammeSelectListItem();
                if (supportViewModel.Programme.Id <= 0)
                {
                    SetMessage("Please, Select Programme to Continue", Message.Category.Error);
                    return View();
                }
                ProgrammeDepartmentLogic programmeDepartmentLogic = new ProgrammeDepartmentLogic();
                supportViewModel.ProgrammeDepartments= programmeDepartmentLogic.GetModelsBy(d => d.Programme_Id == supportViewModel.Programme.Id).OrderBy(d=>d.Department.Name).ToList();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
            return View(supportViewModel);
        }
        public ActionResult DeactivateActivateProgrammeDepartment(SupportViewModel supportViewModel)
        {
            try
            {
                ProgrammeDepartmentLogic programmeDepartmentLogic = new ProgrammeDepartmentLogic();
                List<ProgrammeDepartment> activateProgrammeDept = supportViewModel.ProgrammeDepartments.Where(r => r.Activate).ToList();
                List<ProgrammeDepartment> activateProgrammeDeptForPUTME = supportViewModel.ProgrammeDepartments.Where(r => r.ActivePUTMEApplication).ToList();
                List<ProgrammeDepartment> deActivateProgrammeDept = supportViewModel.ProgrammeDepartments.Where(r => r.Activate==false).ToList();
                List<ProgrammeDepartment> deActivateProgrammeDeptForPUTME = supportViewModel.ProgrammeDepartments.Where(r => r.ActivePUTMEApplication==false).ToList();
                supportViewModel.Programme = new Programme { Id = supportViewModel.ProgrammeDepartments.FirstOrDefault().Programme.Id };
                if (activateProgrammeDept.Count > 0)
                {
                    for(int i=0; i<activateProgrammeDept.Count; i++)
                    {
                        var id = activateProgrammeDept[i].Id;
                        var programmeDept=programmeDepartmentLogic.GetModelBy(d => d.Programme_Department_Id == id);
                        if (programmeDept.Activate != activateProgrammeDept[i].Activate)
                        {
                            programmeDept.Activate = activateProgrammeDept[i].Activate;
                            programmeDepartmentLogic.Modify(programmeDept);
                        }
                    }
                }
                if (activateProgrammeDeptForPUTME.Count > 0)
                {
                    for (int i = 0; i < activateProgrammeDeptForPUTME.Count; i++)
                    {
                        var id = activateProgrammeDeptForPUTME[i].Id;
                        var programmeDept = programmeDepartmentLogic.GetModelBy(d => d.Programme_Department_Id == id);
                        if (programmeDept.ActivePUTMEApplication != activateProgrammeDeptForPUTME[i].ActivePUTMEApplication)
                        {
                            programmeDept.ActivePUTMEApplication = activateProgrammeDept[i].ActivePUTMEApplication;
                            programmeDepartmentLogic.Modify(programmeDept);
                        }
                    }
                }
                if (deActivateProgrammeDept.Count > 0)
                {
                    for (int i = 0; i < deActivateProgrammeDept.Count; i++)
                    {
                        var id = deActivateProgrammeDept[i].Id;
                        var programmeDept = programmeDepartmentLogic.GetModelBy(d => d.Programme_Department_Id == id);
                        if (programmeDept.ActivePUTMEApplication != deActivateProgrammeDept[i].Activate)
                        {
                            programmeDept.Activate = deActivateProgrammeDept[i].Activate;
                            programmeDepartmentLogic.Modify(programmeDept);
                        }
                    }
                }
                if (deActivateProgrammeDeptForPUTME.Count > 0)
                {
                    for (int i = 0; i < deActivateProgrammeDeptForPUTME.Count; i++)
                    {
                        var id = deActivateProgrammeDeptForPUTME[i].Id;
                        var programmeDept = programmeDepartmentLogic.GetModelBy(d => d.Programme_Department_Id == id);
                        if (programmeDept.ActivePUTMEApplication != deActivateProgrammeDeptForPUTME[i].ActivePUTMEApplication)
                        {
                            programmeDept.ActivePUTMEApplication = deActivateProgrammeDeptForPUTME[i].ActivePUTMEApplication;
                            programmeDepartmentLogic.Modify(programmeDept);
                        }
                    }
                }
                supportViewModel.ProgrammeDepartments = programmeDepartmentLogic.GetModelsBy(d => d.Programme_Id == supportViewModel.Programme.Id).OrderBy(d => d.Department.Name).ToList();
                SetMessage("Operation was Successfull", Message.Category.Information);

            }
            catch(Exception ex)
            {
                throw ex;
            }
            supportViewModel.ShowTable = true;
            TempData["ProgrammeDepartment"] = supportViewModel;
            TempData.Keep();
            return RedirectToAction("ManageDepartment");
        }
        public ActionResult UploadStudentUsingExcel()
        {
            try
            {
                var viewModel = new SupportViewModel();
                ViewBag.AllSession = viewModel.SessionSelectList;
                ViewBag.Programme = viewModel.ProgrammeSelectListItem;
                ViewBag.Department = new SelectList(new List<Department>(), ID, NAME);
                //ViewBag.Level = new SelectList(viewModel.LevelList, ID, NAME);
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }
        [HttpPost]
        public ActionResult UploadStudentUsingExcel(SupportViewModel viewModel)
        {
            try
            {
                viewModel = new SupportViewModel();
                List<UploadStudentData> uploadStudentDataList = new List<UploadStudentData>();
                viewModel.UploadStudentDataList = new List<UploadStudentData>();
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file];
                    string pathForSaving = Server.MapPath("~/Content/ExcelUploads");
                    string savedFileName = Path.Combine(pathForSaving, hpf.FileName);
                    hpf.SaveAs(savedFileName);
                    DataSet FileSet = ReadExcel(savedFileName);
                    if (FileSet != null && FileSet.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < FileSet.Tables[0].Rows.Count; i++)
                        {
                            var uploadStudentData = new UploadStudentData();
                            uploadStudentData.SN = FileSet.Tables[0].Rows[i][0]!=null? Convert.ToInt32(FileSet.Tables[0].Rows[i][0]):0;
                            uploadStudentData.LastName = FileSet.Tables[0].Rows[i][1] != null ? FileSet.Tables[0].Rows[i][1].ToString().Trim() : null;
                            uploadStudentData.FirstName = FileSet.Tables[0].Rows[i][2] != null ? FileSet.Tables[0].Rows[i][2].ToString().Trim() : null;
                            uploadStudentData.OtherName = FileSet.Tables[0].Rows[i][3] != null ? FileSet.Tables[0].Rows[i][3].ToString().Trim() : null;
                            uploadStudentData.JambNo = FileSet.Tables[0].Rows[i][4] != null ? FileSet.Tables[0].Rows[i][4].ToString().Trim() : null;
                            uploadStudentData.EmailAddress = FileSet.Tables[0].Rows[i][5] != null ? FileSet.Tables[0].Rows[i][5].ToString().Trim() : null;
                            if (!string.IsNullOrEmpty(uploadStudentData.JambNo))
                            {
                                uploadStudentDataList.Add(uploadStudentData);
                                viewModel.UploadStudentDataList.Add(uploadStudentData);
                            }
                            else
                            {
                                uploadStudentData.Remarks = "No Jamb No.";
                                viewModel.UploadStudentDataList.Add(uploadStudentData);
                            }
                            
                        }
                    }
                }
                TempData["studentData"] = uploadStudentDataList;
            }
            catch (Exception ex)
            {
                SetMessage("Error" + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }
        public ActionResult SaveStudentUpload()
        {
            var viewModel = new SupportViewModel();
            try
            {
                int count = 0;
                 
                viewModel.UploadStudentDataList = new List<UploadStudentData>();
                List<UploadStudentData> uploadStudentDataList = (List<UploadStudentData>)TempData["studentData"];
                StudentLogic studentLogic = new StudentLogic();
                Programme programme = new Programme() { Id = 1 };
                //create default department
                Department department = new Department() { Id = 83 };
                Level level = new Level() { Id = 1 };
                Model.Model.Session session = new Session() { Id = 1 };
                if (uploadStudentDataList?.Count > 0)
                {
                    foreach (var item in uploadStudentDataList)
                    {
                        var existStudent = studentLogic.GetModelsBy(f => f.Matric_Number == item.JambNo);

                        if (existStudent?.Count > 0)
                        {
                            item.Remarks = "Jamb No already exist";
                            viewModel.UploadStudentDataList.Add(item);

                        }
                        else
                        {
                            var person = CreatePerson(item);
                            if (person?.Id > 0)
                            {
                                var studentCreated = CreateStuden(person, item);
                                if (studentCreated != null)
                                {

                                    var studentLevelCreated=CreateStudentLevel(studentCreated, department, programme, level, session);
                                    if(studentLevelCreated?.Id>0)
                                    item.Remarks = "Created Successfully";
                                    viewModel.UploadStudentDataList.Add(item);
                                    count += 1;
                                }
                            }
                        }


                    }
                }
                    SetMessage($"{ count } Added Successfully", Message.Category.Information);

            }
            catch (Exception ex)
            {
                SetMessage("Error." + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }
        private Person CreatePerson(UploadStudentData uploadStudentData)
        {
            try
            {
                Person person = new Person();
                var role = new Role { Id = 5 };
                var personType = new PersonType { Id = 3 };
                var nationality = new Nationality { Id = 1 };
                var state = new State { Id = "AN" };
                person.DateEntered = DateTime.Now;
                person.InstitutionalEmail = uploadStudentData.EmailAddress;
                person.FirstName = uploadStudentData.FirstName;
                person.LastName = uploadStudentData.LastName;
                person.Nationality = nationality;
                person.OtherName = uploadStudentData.OtherName;
                person.Role = role;
                person.Type = personType;
                person.State = state;
                var personLogic = new PersonLogic();
                Person createdPerson = personLogic.Create(person);
                if (createdPerson != null && createdPerson.Id > 0)
                {
                   return  createdPerson;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private Model.Model.Student CreateStuden(Person person,UploadStudentData uploadStudentData)
        {
            StudentLogic studentLogic = new StudentLogic();
            Model.Model.Student student = new Model.Model.Student();
            student.Id = person.Id;
            student.Type = new StudentType() { Id = 1 };
            student.Category = new StudentCategory() { Id = 1 };
            student.Status = new StudentStatus() { Id = 1 };
            student.Number = null;
            student.MatricNumber = uploadStudentData.JambNo;
            student.PasswordHash = "12345678";
            return studentLogic.Create(student);
        }
        private StudentLevel CreateStudentLevel( Model.Model.Student student,Department department,Programme programme,Level level,Session session)
        {
            StudentLevel studentLevel = new StudentLevel()
            {
                Department = department,
                Level = level,
                Session = session,
                Student = student,
                Programme = programme,

            };
            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
            return studentLevelLogic.Create(studentLevel);
        }
    }
}