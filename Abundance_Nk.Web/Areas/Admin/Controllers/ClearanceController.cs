﻿using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class ClearanceController :BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private Abundance_NkEntities db = new Abundance_NkEntities();
        private ClearanceViewModel viewmodel;

        public ActionResult Index(string sortOrder)
        {
            viewmodel = new ClearanceViewModel();
            if(TempData["ClearanceViewModel"] != null)
            {
                viewmodel = (ClearanceViewModel)TempData["ClearanceViewModel"];
            }
            string sortDesc = sortOrder;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.FullName = String.IsNullOrEmpty(sortDesc) ? "name_desc" : "";
            ViewBag.Number = String.IsNullOrEmpty(sortDesc) ? "Number_desc" : "";
            ViewBag.Programme = String.IsNullOrEmpty(sortDesc) ? "Programme_desc" : "";
            ViewBag.Department = String.IsNullOrEmpty(sortDesc) ? "Department_desc" : "";

            try
            {
                switch(sortDesc)
                {
                    case "name_desc":
                    viewmodel.appliedCourseList =
                        viewmodel.appliedCourseList.OrderByDescending(s => s.Person.FullName).ToList();
                    break;

                    case "Number_desc":
                    viewmodel.appliedCourseList =
                        viewmodel.appliedCourseList.OrderByDescending(s => s.ApplicationForm.Number).ToList();
                    break;

                    case "Programme_desc":
                    viewmodel.appliedCourseList =
                        viewmodel.appliedCourseList.OrderByDescending(s => s.Programme.Name).ToList();
                    break;

                    case "Department_desc":
                    viewmodel.appliedCourseList =
                        viewmodel.appliedCourseList.OrderByDescending(s => s.Department.Name).ToList();
                    break;

                    default:
                    viewmodel.appliedCourseList =
                        viewmodel.appliedCourseList.OrderByDescending(s => s.ApplicationForm.Id).ToList();
                    break;
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            TempData["ClearanceViewModel"] = viewmodel;
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Index(ClearanceViewModel vModel)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    if(vModel.ApplicationNumber != null)
                    {
                        var viewModel = new ClearanceViewModel();

                        var applicationForm = new List<ApplicationForm>();
                        var applicationFormLogic = new ApplicationFormLogic();

                        var personList = new List<Person>();
                        var personLogic = new PersonLogic();

                        personList = personLogic.GetModelsBy(n => n.Last_Name == vModel.ApplicationNumber || n.First_Name == vModel.ApplicationNumber || n.Other_Name == vModel.ApplicationNumber || n.Mobile_Phone == vModel.ApplicationNumber);
                        if(personList != null && personList.Count > 0)
                        {
                            foreach(Person applicant in personList)
                            {
                                ApplicationForm appForm =
                                    applicationFormLogic.GetModelBy(m => m.Person_Id == applicant.Id);
                                applicationForm.Add(appForm);
                            }
                        }
                        else
                        {
                            ApplicationForm appForm =
                                applicationFormLogic.GetModelBy(
                                    m =>
                                        m.Application_Form_Number == vModel.ApplicationNumber ||
                                        m.Application_Exam_Number == vModel.ApplicationNumber);
                            applicationForm.Add(appForm);
                        }

                        if(applicationForm != null && applicationForm.Count > 0)
                        {
                            var appCourse = new List<AppliedCourse>();
                            var result = new List<OLevelResult>();
                            var resultdetail = new List<OLevelResultDetail>();
                            var applicantForm = new List<ApplicationForm>();

                            var appliedcourse = new AppliedCourse();
                            var appliedCourseLogic = new AppliedCourseLogic();
                            foreach(ApplicationForm applicant in applicationForm)
                            {
                                if(applicant != null && applicant.Person.Id > 0)
                                {
                                    appliedcourse =
                                        appliedCourseLogic.GetModelBy(n => n.Person_Id == applicant.Person.Id);
                                    if(appliedcourse != null && appliedcourse.Department.Id > 0)
                                    {
                                        applicantForm.Add(applicant);
                                        appCourse.Add(appliedcourse);
                                    }
                                }
                            }

                            viewModel.applicationFormList = applicantForm;
                            viewModel.appliedCourseList = appCourse;
                            TempData["ClearanceViewModel"] = viewModel;

                            return View(viewModel);
                        }
                        SetMessage("Record does not exist! ",Message.Category.Error);
                        return View(vModel);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            return View(vModel);
        }

        public ActionResult View(long Id)
        {
            viewmodel = new ClearanceViewModel();
            try
            {
                var appliedCourse = new AppliedCourse();
                var appliedCourseLogic = new AppliedCourseLogic();

                long personid = Id;
                if(personid > 0)
                {
                    appliedCourse = appliedCourseLogic.GetModelBy(x => x.Person_Id == personid);
                    if(appliedCourse != null && appliedCourse.Person.Id > 0)
                    {
                        viewmodel.appliedCourse = appliedCourse;
                        viewmodel.person = appliedCourse.Person;
                        viewmodel.LoadApplicantResult(appliedCourse.Person);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            TempData["ClearanceViewModel"] = viewmodel;
            return View(viewmodel);
        }

        public ActionResult AdmissionCriteria(string sortOrder)
        {
            viewmodel = new ClearanceViewModel();
            if(TempData["ClearanceViewModel"] != null)
            {
                viewmodel = (ClearanceViewModel)TempData["ClearanceViewModel"];
            }
            try
            {
                string sortDesc = sortOrder;
                ViewBag.CurrentSort = sortOrder;
                ViewBag.Department = String.IsNullOrEmpty(sortDesc) ? "Department_desc" : "";
                ViewBag.Programme = String.IsNullOrEmpty(sortDesc) ? "Programme_desc" : "";
                ViewBag.Minimum = String.IsNullOrEmpty(sortDesc) ? "Minimum_desc" : "";
                ViewBag.Date = String.IsNullOrEmpty(sortDesc) ? "Date_desc" : "";

                switch(sortDesc)
                {
                    case "Department_desc":
                    viewmodel.admissionCriteriaList =
                        viewmodel.admissionCriteriaList.OrderByDescending(s => s.Department.Name).ToList();
                    break;

                    case "Programme_desc":
                    viewmodel.admissionCriteriaList =
                        viewmodel.admissionCriteriaList.OrderByDescending(s => s.Programme.Name).ToList();
                    break;

                    case "Minimum_desc":
                    viewmodel.admissionCriteriaList =
                        viewmodel.admissionCriteriaList.OrderByDescending(s => s.MinimumRequiredNumberOfSubject)
                            .ToList();
                    break;

                    case "Date_desc":
                    viewmodel.admissionCriteriaList =
                        viewmodel.admissionCriteriaList.OrderByDescending(s => s.DateEntered).ToList();
                    break;

                    default:
                    viewmodel.admissionCriteriaList =
                        viewmodel.admissionCriteriaList.OrderByDescending(s => s.Department.Name).ToList();
                    break;
                }
                TempData["ClearanceViewModel"] = viewmodel;
                return View(viewmodel);
            }

            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            return View();
        }

        public ActionResult ViewCriteria(long Id)
        {
            viewmodel = new ClearanceViewModel();
            try
            {
                var OlevelLogic = new AdmissionCriteriaForOLevelSubjectLogic();
                var olevelAltList = new List<AdmissionCriteriaForOLevelSubjectAlternative>();
                var olevelAlt = new AdmissionCriteriaForOLevelSubjectAlternative();
                var olevelAltLogic = new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
                var olevelTypeLogic = new AdmissionCriteriaForOLevelTypeLogic();

                viewmodel.admissionCriteriaForOLevelSubject = OlevelLogic.GetModelsBy(a => a.Admission_Criteria_Id == Id);
                foreach(AdmissionCriteriaForOLevelSubject subject in viewmodel.admissionCriteriaForOLevelSubject)
                {
                    subject.Alternatives =
                        olevelAltLogic.GetModelsBy(o => o.Admission_Criteria_For_O_Level_Subject_Id == subject.Id);
                }
                viewmodel.admissionCriteriaForOLevelType =
                    olevelTypeLogic.GetModelsBy(n => n.Admission_Criteria_Id == Id);

                TempData["ClearanceViewModel"] = viewmodel;
                return View(viewmodel);
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            return View();
        }

        public ActionResult AddAdmissionCriteria()
        {
            var Viewmodel = new ClearanceViewModel();

            try
            {
                ViewBag.ProgrammeId = Viewmodel.ProgrammeSelectListItem;
                ViewBag.DepartmentId = new SelectList(new List<Department>(),ID,NAME);

                for(int i = 0;i < 15;i++)
                {
                    ViewData["FirstSittingOLevelSubjectId" + i] = Viewmodel.OLevelSubjectSelectList;
                    ViewData["SecondSittingOLevelSubjectId" + i] = Viewmodel.OLevelSubjectSelectList;
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            TempData["viewModel"] = Viewmodel;
            return View(Viewmodel);
        }

        [HttpPost]
        public ActionResult AddAdmissionCriteria(ClearanceViewModel criteriaModel)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    //Add team and redirect
                }

                using(
                    var transaction = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                {
                    if(criteriaModel.admissionCriteria.Department != null &&
                        criteriaModel.admissionCriteria.Programme != null && criteriaModel.OLevelSubjects != null &&
                        criteriaModel.OLevelSubjects.Count > 0)
                    {
                        var criteria = new AdmissionCriteria();
                        var criteria2 = new AdmissionCriteria();
                        var criteriaLogic = new AdmissionCriteriaLogic();
                        criteria =
                            criteriaLogic.GetModelBy(
                                c =>
                                    c.Department_Id == criteriaModel.admissionCriteria.Department.Id &&
                                    c.Programme_Id == criteriaModel.admissionCriteria.Programme.Id);
                        if(criteria == null)
                        {
                            criteria2.DateEntered = DateTime.Now;
                            criteria2.Department = criteriaModel.admissionCriteria.Department;
                            criteria2.Programme = criteriaModel.admissionCriteria.Programme;
                            criteria2.MinimumRequiredNumberOfSubject = 5;
                            criteriaLogic.Create(criteria2);
                            criteria =
                                criteriaLogic.GetModelBy(
                                    c =>
                                        c.Department_Id == criteriaModel.admissionCriteria.Department.Id &&
                                        c.Programme_Id == criteriaModel.admissionCriteria.Programme.Id);

                            //Add subjects
                            var criteriaSubjectLogic = new AdmissionCriteriaForOLevelSubjectLogic();
                            var criteriaSubject = new AdmissionCriteriaForOLevelSubject();
                            var criteriaSubjectAlternativeLogic =
                                new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
                            var criteriaSubjectAlternative = new AdmissionCriteriaForOLevelSubjectAlternative();
                            var grade = new OLevelGrade();
                            var gradeLogic = new OLevelGradeLogic();
                            grade = gradeLogic.GetModelBy(g => g.O_Level_Grade_Id == 6);

                            int count = 0;
                            foreach(OLevelSubject subject in criteriaModel.OLevelSubjects)
                            {
                                if(subject != null && subject.Id > 0)
                                {
                                    criteriaSubject.MainCriteria = criteria;
                                    criteriaSubject.Subject = subject;
                                    criteriaSubject.IsCompulsory = subject.IsChecked;
                                    criteriaSubject.MinimumGrade = grade;
                                    criteriaSubject = criteriaSubjectLogic.Create(criteriaSubject);
                                    if(criteriaModel.OLevelSubjectsAlternatives[count].Id > 0)
                                    {
                                        criteriaSubjectAlternative.OLevelSubject =
                                            criteriaModel.OLevelSubjectsAlternatives[count];
                                        criteriaSubjectAlternative.Alternative = criteriaSubject;
                                        criteriaSubjectAlternativeLogic.Create(criteriaSubjectAlternative);
                                    }
                                }
                                count++;
                            }
                        }
                    }
                    transaction.Complete();

                    ViewBag.ProgrammeId = criteriaModel.ProgrammeSelectListItem;
                    ViewBag.DepartmentId = new SelectList(new List<Department>(),ID,NAME);

                    for(int i = 0;i < 15;i++)
                    {
                        ViewData["FirstSittingOLevelSubjectId" + i] = criteriaModel.OLevelSubjectSelectList;
                        ViewData["SecondSittingOLevelSubjectId" + i] = criteriaModel.OLevelSubjectSelectList;
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            TempData["Message"] = "Successfully Added criteria";
            return View(criteriaModel);
        }

        public ActionResult EditCriteria(long Id)
        {
            viewmodel = new ClearanceViewModel();
            try
            {
                var OlevelLogic = new AdmissionCriteriaForOLevelSubjectLogic();
                var olevelAltList = new List<AdmissionCriteriaForOLevelSubjectAlternative>();
                var olevelAlt = new AdmissionCriteriaForOLevelSubjectAlternative();
                var olevelAltLogic = new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
                var olevelTypeLogic = new AdmissionCriteriaForOLevelTypeLogic();

                for(int i = 0;i < 15;i++)
                {
                    ViewData["FirstSittingOLevelSubjectId" + i] = viewmodel.OLevelSubjectSelectList;
                    ViewData["SecondSittingOLevelSubjectId" + i] = viewmodel.OLevelSubjectSelectList;
                    ViewData["FirstSittingOLevelGradeId" + i] = viewmodel.OLevelGradeSelectList;
                }

                viewmodel.admissionCriteriaForOLevelSubject = OlevelLogic.GetModelsBy(a => a.Admission_Criteria_Id == Id);
                foreach(AdmissionCriteriaForOLevelSubject subject in viewmodel.admissionCriteriaForOLevelSubject)
                {
                    subject.Alternatives =
                        olevelAltLogic.GetModelsBy(o => o.Admission_Criteria_For_O_Level_Subject_Id == subject.Id);
                }

                viewmodel.admissionCriteriaForOLevelType =
                    olevelTypeLogic.GetModelsBy(n => n.Admission_Criteria_Id == Id);
                SetSelectedSittingSubjectAndGrade(viewmodel);

                TempData["ClearanceViewModel"] = viewmodel;
                return View(viewmodel);
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            return View();
        }

        private void SetSelectedSittingSubjectAndGrade(ClearanceViewModel existingViewModel)
        {
            try
            {
                if(existingViewModel != null && existingViewModel.admissionCriteriaForOLevelSubject != null &&
                    existingViewModel.admissionCriteriaForOLevelSubject.Count > 0)
                {
                    int i = 0;
                    foreach(
                        AdmissionCriteriaForOLevelSubject subject in existingViewModel.admissionCriteriaForOLevelSubject
                        )
                    {
                        if(subject.Subject.Name != null)
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList,VALUE,TEXT,
                                    subject.Subject.Id);
                            if(subject.Alternatives.Count > 0)
                            {
                                ViewData["SecondSittingOLevelSubjectId" + i] =
                                    new SelectList(existingViewModel.OLevelSubjectSelectList,VALUE,TEXT,
                                        subject.Alternatives[0].OLevelSubject.Id);
                            }
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList,VALUE,TEXT,
                                    subject.MinimumGrade.Id);
                        }
                        else
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(viewmodel.OLevelSubjectSelectList,VALUE,TEXT,0);
                            ViewData["SecondSittingOLevelSubjectId" + i] =
                                new SelectList(viewmodel.OLevelSubjectSelectList,VALUE,TEXT,0);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList,VALUE,TEXT,0);
                        }

                        i++;
                    }

                    var sub = new AdmissionCriteriaForOLevelSubject();
                    sub.Id = -1;
                    sub.MainCriteria = existingViewModel.admissionCriteriaForOLevelSubject[0].MainCriteria;
                    sub.Alternatives[0].OLevelSubject.Id = -1;

                    for(int u = 0;u < 5;u++)
                    {
                        existingViewModel.admissionCriteriaForOLevelSubject.Add(sub);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        [HttpPost]
        public ActionResult EditCriteria(ClearanceViewModel criteriaModel)
        {
            //Add subjects
            var criteriaSubjectLogic = new AdmissionCriteriaForOLevelSubjectLogic();
            var criteriaSubjectAlternativeLogic = new AdmissionCriteriaForOLevelSubjectAlternativeLogic();
            criteriaSubjectLogic.Modify(criteriaModel.admissionCriteriaForOLevelSubject);

            SetMessage("Criteria was updated successfully",Message.Category.Information);
            return RedirectToAction("AdmissionCriteria");
        }

        public JsonResult GetDepartmentByProgrammeId(string id)
        {
            try
            {
                if(string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var programme = new Programme { Id = Convert.ToInt32(id) };

                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetBy(programme);

                return Json(new SelectList(departments,ID,NAME),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private Payment InvalidConfirmationOrderNumber(string confirmationOrderNumber)
        {
            var payment = new Payment();
            var etranzactLogic = new PaymentEtranzactLogic();
            PaymentEtranzact etranzactDetails =
                etranzactLogic.GetModelBy(m => m.Confirmation_No == confirmationOrderNumber);
            if(etranzactDetails == null || etranzactDetails.ReceiptNo == null)
            {
                var paymentTerminal = new PaymentTerminal();
                var paymentTerminalLogic = new PaymentTerminalLogic();
                paymentTerminal = paymentTerminalLogic.GetModelBy(p => p.Fee_Type_Id == 1 && p.Session_Id == 1);

                etranzactDetails = etranzactLogic.RetrievePinAlternative(confirmationOrderNumber,paymentTerminal);
                if(etranzactDetails != null && etranzactDetails.ReceiptNo != null)
                {
                    var paymentLogic = new PaymentLogic();
                    payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                    if(payment != null && payment.Id > 0)
                    {
                        var feeDetail = new FeeDetail();
                        var feeDetailLogic = new FeeDetailLogic();
                        feeDetail = feeDetailLogic.GetModelBy(a => a.Fee_Type_Id == payment.FeeType.Id);
                        //if (!etranzactLogic.ValidatePin(etranzactDetails, payment, feeDetail.Fee.Amount))
                        //{
                        //    SetMessage("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.", Message.Category.Error);
                        //    payment = null;
                        //    return payment;
                        //}
                    }
                    else
                    {
                        SetMessage(
                            "The invoice number attached to the pin doesn't belong to you! Please cross check and try again.",
                            Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage(
                        "Confirmation Order Number entered seems not to be valid! Please cross check and try again.",
                        Message.Category.Error);
                }
            }
            else
            {
                var paymentLogic = new PaymentLogic();
                payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                if(payment != null && payment.Id > 0)
                {
                    var feeDetail = new FeeDetail();
                    var feeDetailLogic = new FeeDetailLogic();
                    feeDetail = feeDetailLogic.GetModelBy(a => a.Fee_Type_Id == payment.FeeType.Id);

                    if(!etranzactLogic.ValidatePin(etranzactDetails,payment,feeDetail.Fee.Amount))
                    {
                        SetMessage(
                            "The pin amount tied to the pin is not correct. Please contact support@lloydant.com.",
                            Message.Category.Error);
                        payment = null;
                        return payment;
                    }
                }
                else
                {
                    SetMessage(
                        "The invoice number attached to the pin doesn't belong to you! Please cross check and try again.",
                        Message.Category.Error);
                    //return View(viewModel);
                }
            }

            return payment;
        }

        public ActionResult CheckApplicants()
        {
            try
            {
                var Applicants = new List<ApplicationForm>();
                var CheckApplicants = new ApplicationFormLogic();
                Applicants = CheckApplicants.GetAllHndApplicants();
                foreach(ApplicationForm candidate in Applicants)
                {
                    ApplicationForm candidate2 = CheckApplicants.GetModelBy(m => m.Application_Form_Id == candidate.Id);
                    ApplicationForm audit = candidate2;

                    var CandidateAppliedCourse = new AppliedCourse();
                    var appliedCourseLogic = new AppliedCourseLogic();
                    CandidateAppliedCourse = appliedCourseLogic.GetModelBy(m => m.Person_Id == candidate2.Person.Id);
                    candidate2.Release = false;

                    var admissionCriteriaLogic = new AdmissionCriteriaLogic();
                    string rejectReason = admissionCriteriaLogic.EvaluateApplication(CandidateAppliedCourse);
                    if(string.IsNullOrEmpty(rejectReason))
                    {
                        candidate2.Rejected = false;
                        candidate2.RejectReason = "";
                    }
                    else
                    {
                        candidate2.Rejected = true;
                        candidate2.RejectReason = rejectReason;
                    }
                    if(!CheckApplicants.SetRejectReason(candidate2))
                    {
                        candidate2.Remarks = "Reject Reason not set";
                        //throw new Exception("Reject Reason not set! Please try again.");
                    }
                    using(var writer = new StreamWriter("C:\\inetpub\\wwwroot\\log.txt",true))
                    {
                        writer.WriteLine(audit.Number + " " + audit.Rejected + " " + audit.RejectReason + " -  " +
                                         candidate2.Rejected + " " + candidate2.RejectReason + " " + candidate2.Remarks);
                    }
                }
            }
            catch(Exception ex)
            {
                throw;
            }
            return View();
        }
    }
}