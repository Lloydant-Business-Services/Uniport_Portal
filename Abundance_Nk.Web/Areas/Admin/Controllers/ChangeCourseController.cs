using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [AllowAnonymous]
    public class ChangeCourseController :BaseController
    {
        public enum FeeStatus
        {
            OneFee = 1,
            TwoFees = 2
        }

        private ChangeCourseViewModel viewModel;

        // GET: Student/ChangeCourse
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchResult(ChangeCourseViewModel model)
        {
            viewModel = new ChangeCourseViewModel();
            try
            {
                var applicationFormLogic = new ApplicationFormLogic();
                var appliedCourseLogic = new AppliedCourseLogic();
                var personLogic = new PersonLogic();
                var programmeLogic = new ProgrammeLogic();
                var departmentLogic = new DepartmentLogic();
                var applicantJambDetailLogic = new ApplicantJambDetailLogic();
               
                ApplicationForm applicationForm = applicationFormLogic.GetModelBy(p => p.Application_Form_Number == model.ApplicationFormNumber);
                
                ApplicantJambDetail applicantJambDetail =
                    applicantJambDetailLogic.GetModelsBy(
                        s => s.Applicant_Jamb_Registration_Number == model.ApplicationFormNumber).LastOrDefault();

                if(applicationForm != null)
                {
                    Person person = personLogic.GetModelBy(p => p.Person_Id == applicationForm.Person.Id);
                    List<Person> persons =
                        personLogic.GetModelsBy(
                            p =>
                                p.First_Name == person.FirstName && p.Last_Name == person.LastName &&
                                p.Date_Of_Birth == person.DateOfBirth && p.Date_Entered == person.DateEntered);
                    viewModel.Payment = isStudentAlreadyExist(persons);
                    AppliedCourse studentAppliedCourse =
                        appliedCourseLogic.GetModelBy(p => p.Application_Form_Id == applicationForm.Id);
                    model.AppliedCourse = studentAppliedCourse;

                    getOldSchoolFees(model);

                    AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(p => p.Person_Id == person.Id);
                    viewModel.ApplicationForm = applicationForm;
                    viewModel.Person = person;
                    viewModel.AppliedCourse = appliedCourse;
                    ViewBag.ProgrammeId = viewModel.ProgrammeSelectList;
                    ViewBag.Departments = new SelectList(new List<Department>(),Utility.ID,Utility.NAME);

                    return View(viewModel);
                }
                if (applicantJambDetail != null)
                {
                    Person person = personLogic.GetModelBy(p => p.Person_Id == applicantJambDetail.Person.Id);
                    List<Person> persons =
                        personLogic.GetModelsBy(
                            p =>
                                p.First_Name == person.FirstName && p.Last_Name == person.LastName &&
                                p.Date_Of_Birth == person.DateOfBirth && p.Date_Entered == person.DateEntered);
                    viewModel.Payment = isStudentAlreadyExist(persons);
                    AppliedCourse studentAppliedCourse =
                        appliedCourseLogic.GetModelBy(p => p.Application_Form_Id == applicantJambDetail.ApplicationForm.Id);
                    model.AppliedCourse = studentAppliedCourse;

                    getOldSchoolFees(model);

                    AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(p => p.Person_Id == person.Id);
                    viewModel.ApplicationForm = applicantJambDetail.ApplicationForm;
                    viewModel.Person = person;
                    viewModel.AppliedCourse = appliedCourse;
                    ViewBag.ProgrammeId = viewModel.ProgrammeSelectList;
                    ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);

                    return View(viewModel);
                }
              
                SetMessage("Invalid ApplicationFormNumber Form Number!",Message.Category.Error);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                SetMessage("Error Occured !" + ex.Message,Message.Category.Error);
                return RedirectToAction("Index");
            }
        }

        private void getOldSchoolFees(ChangeCourseViewModel model)
        {
            try
            {
                int levelId = GetLevel(model.AppliedCourse.Programme.Id);
                var oldSchoolFees = new List<FeeDetail>();
                var feeDetailLogic = new FeeDetailLogic();
                var sessionLogic = new SessionLogic();
                Session session = sessionLogic.GetModelBy(p => p.Activated == true);
                oldSchoolFees =
                    feeDetailLogic.GetModelsBy(
                        p =>
                            p.Department_Id == model.AppliedCourse.Department.Id &&
                            p.Programme_Id == model.AppliedCourse.Programme.Id && p.Level_Id == levelId &&
                            p.Fee_Type_Id == (int)FeeTypes.SchoolFees && p.Session_Id == session.Id);
                decimal amount = oldSchoolFees.Sum(p => p.Fee.Amount);
                model.OldSchoolFees = amount;
                TempData["SchoolFees"] = amount;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ActionResult GetDepartmentAndLevelByProgrammeId(string id)
        {
            try
            {
                if(string.IsNullOrEmpty(id))
                {
                    return null;
                }

                // List<Level> levels = null;
                List<Department> departments = null;
                var programme = new Programme { Id = Convert.ToInt32(id) };
                if(programme.Id > 0)
                {
                    var departmentLogic = new DepartmentLogic();
                    departments = departmentLogic.GetBy(programme);

                    //LevelLogic levelLogic = new LevelLogic();
                    //if (programme.Id <= 2)
                    //{
                    //    levels = levelLogic.GetONDs();
                    //}
                    //else if (programme.Id == 3 || programme.Id == 4)
                    //{
                    //    levels = levelLogic.GetHNDs();
                    //}
                    //else if (programme.Id == 5)
                    //{
                    //    levels = levelLogic.GetCertificateCourse();
                    //}
                }
                return Json(new { Departments = departments },"json",JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        //[HttpPost]
        //public ActionResult GenerateInvoice(ChangeCourseViewModel model)
        //{
        //    try
        //    {
        //        model.OldSchoolFees = (decimal) TempData["SchoolFees"];
        //        var departmentLogic = new DepartmentLogic();
        //        var personLogic = new PersonLogic();
        //        var applicationFormLogic = new ApplicationFormLogic();
        //        var programmeLogic = new ProgrammeLogic();
        //        var olevelResultLogic = new OLevelResultLogic();
        //        var olevelResultDetailLogic = new OLevelResultDetailLogic();
        //        //OLevelResult olevelResult = olevelResultLogic.GetModelsBy(p => p.Person_Id == model.Person.Id).LastOrDefault();
        //        //List<OLevelResultDetail> olevelResultDetail = olevelResultDetailLogic.GetModelsBy(p => p.Applicant_O_Level_Result_Id == olevelResult.Id);
        //        List<OLevelResult> olevelResult = olevelResultLogic.GetModelsBy(p => p.Person_Id == model.Person.Id);
        //        Department department =
        //            departmentLogic.GetModelBy(p => p.Department_Id == model.AppliedCourse.Department.Id);
        //        Programme programme = programmeLogic.GetModelBy(p => p.Programme_Id == model.AppliedCourse.Programme.Id);

        //        var paymentLogic = new PaymentLogic();
        //        List<Payment> oldPayments = paymentLogic.GetModelsBy(p => p.Person_Id == model.Person.Id);
        //        int feeTypeID = getFeeTypeId(oldPayments);

        //        if (model != null)
        //        {
        //            using (var scope = new TransactionScope())
        //            {
        //                model.AppliedCourse.Programme = programme;
        //                model.AppliedCourse.Department = department;
        //                CreatePerson(model);
        //                model.Payment = CreatePayment(model, feeTypeID);
        //                CreateAppliedCourse(model);
        //                model.ApplicationForm = CreateApplicationForm(model);
        //                foreach (OLevelResult item in olevelResult)
        //                {
        //                    item.Person = model.Person;
        //                    item.ApplicationForm = model.ApplicationForm;
        //                    var olevelResultNew = new OLevelResult();
        //                    olevelResultNew = olevelResultLogic.Create(item);
        //                    olevelResultNew.Person = model.Person;
        //                    olevelResultNew.ApplicationForm = model.ApplicationForm;
        //                    List<OLevelResultDetail> olevelResultDetail =
        //                        olevelResultDetailLogic.GetModelsBy(p => p.Applicant_O_Level_Result_Id == item.Id);
        //                    CreateOLevelResultDetail(olevelResultNew, olevelResultDetail);
        //                }
        //                //olevelResult.Person = model.Person;
        //                //olevelResult.ApplicationForm = model.ApplicationForm;
        //                //olevelResult = olevelResultLogic.Create(olevelResult);
        //                //olevelResult.Person = model.Person;
        //                //olevelResult.ApplicationForm = model.ApplicationForm;
        //                //CreateOLevelResultDetail(olevelResult, olevelResultDetail);
        //                scope.Complete();
        //            }

        //            decimal FeeStatus = checkFeesPaid(oldPayments, model);

        //            if (FeeStatus == 1)
        //            {
        //                var newPaymentFeeDetail = new List<FeeDetail>();

        //                var feeDetailLogic = new FeeDetailLogic();
        //                int LevelId = GetLevel(model.AppliedCourse.Programme.Id);
        //                var sessionLogic = new SessionLogic();
        //                Session session = sessionLogic.GetModelBy(p => p.Activated == true);
        //                newPaymentFeeDetail =
        //                    feeDetailLogic.GetModelsBy(
        //                        p =>
        //                            p.Department_Id == model.AppliedCourse.Department.Id &&
        //                            p.Programme_Id == model.AppliedCourse.Programme.Id && p.Level_Id == LevelId &&
        //                            p.Session_Id == session.Id);
        //                TempData["FeeDetail"] = newPaymentFeeDetail;
        //                return RedirectToAction("ShortFallInvoice", "Credential",
        //                    new {Area = "Common", pmid = model.Payment.Id,});
        //            }
        //            if (FeeStatus > 0)
        //            {
        //                var shortFall = new ShortFall();
        //                var shortFallLogic = new ShortFallLogic();
        //                shortFall.Payment = model.Payment;
        //                shortFall.Amount = (double) FeeStatus;
        //                shortFall = shortFallLogic.Create(shortFall);
        //                return RedirectToAction("ShortFallInvoice", "Credential",
        //                    new {Area = "Common", pmid = model.Payment.Id, amount = FeeStatus});
        //            }
        //            var newPaymentFeeDetail = new List<FeeDetail>();

        //            var feeDetailLogic = new FeeDetailLogic();
        //            int LevelId = GetLevel(model.AppliedCourse.Programme.Id);
        //            var sessionLogic = new SessionLogic();
        //            Session session = sessionLogic.GetModelBy(p => p.Activated == true);
        //            newPaymentFeeDetail =
        //                feeDetailLogic.GetModelsBy(
        //                    p =>
        //                        p.Department_Id == model.AppliedCourse.Department.Id &&
        //                        p.Programme_Id == model.AppliedCourse.Programme.Id && p.Level_Id == LevelId &&
        //                        p.Session_Id == session.Id);
        //            TempData["FeeDetail"] = newPaymentFeeDetail;
        //            return RedirectToAction("ShortFallInvoice", "Credential",
        //                new {Area = "Common", pmid = model.Payment.Id,});
        //            // TempData["Action"] = "Change of course successful, student has not short fall";
        //            // return RedirectToAction("Index", new { controller = "ChangeCourse", area = "Student" });
        //            //No need for invoice

        //            //.Person = personLogic.GetModelBy(p => p.Person_Id == model.Person.Id);
        //            //olevelResult.ApplicationForm = applicationFormLogic.GetModelBy(p => p.Application_Form_Id == model.ApplicationForm.Id);
        //            // return RedirectToAction("ChangeCourse", "Credential", new { Area = "Common", pmid = Utility.Encrypt(model.Payment.Id.ToString()), });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SetMessage("Error Occured !" + ex.Message, Message.Category.Error);
        //    }
        //    return RedirectToAction("Index");
        //}

        private int getFeeTypeId(List<Payment> oldPayments)
        {
            try
            {
                int schoolFeesStatus = 0;
                int acceptanceFeeStatus = 0;
                int otherFeeStatus = 0;

                foreach(Payment oldPayment in oldPayments)
                {
                    if(oldPayment.FeeType.Id == (int)FeeTypes.AcceptanceFee)
                    {
                        acceptanceFeeStatus += 3; //use schoolFeeType
                    }
                    else if(oldPayment.FeeType.Id == (int)FeeTypes.SchoolFees)
                    {
                        schoolFeesStatus += 12; //use shortFallFeeType
                    }
                    else
                    {
                        otherFeeStatus = oldPayment.FeeType.Id;
                    }
                }
                if(schoolFeesStatus > 0 && acceptanceFeeStatus > 0)
                {
                    return 12;
                }
                if(acceptanceFeeStatus > 0)
                {
                    return 3;
                }
                return otherFeeStatus;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private decimal checkFeesPaid(List<Payment> oldPayments,ChangeCourseViewModel model)
        {
            try
            {
                int schoolFeesStatus = 0;
                int acceptanceFeeStatus = 0;
                Person person = null;

                // Payment payment = oldPayments.Where(p => p.FeeType.Id == (int)FeeTypes.SchoolFees).SingleOrDefault();

                foreach(Payment oldPayment in oldPayments)
                {
                    person = oldPayment.Person;

                    if(oldPayment.FeeType.Id == (int)FeeTypes.AcceptanceFee)
                    {
                        acceptanceFeeStatus += 1;
                    }
                    if(oldPayment.FeeType.Id == (int)FeeTypes.SchoolFees)
                    {
                        schoolFeesStatus += 1;
                    }
                }
                TempData["OldPerson"] = person;
                TempData.Keep("OldPerson");
                if(schoolFeesStatus > 0 && acceptanceFeeStatus > 0)
                {
                    // Payment newPayment = model.Payment;
                    var newPaymentFeeDetail = new List<FeeDetail>();
                    var feeDetailLogic = new FeeDetailLogic();
                    int LevelId = GetLevel(model.AppliedCourse.Programme.Id);
                    var sessionLogic = new SessionLogic();
                    Session session = sessionLogic.GetModelBy(p => p.Activated == true);
                    newPaymentFeeDetail =
                        feeDetailLogic.GetModelsBy(
                            p =>
                                p.Department_Id == model.AppliedCourse.Department.Id &&
                                p.Programme_Id == model.AppliedCourse.Programme.Id && p.Level_Id == LevelId &&
                                p.Session_Id == session.Id);

                    var courseRegistration = new CourseRegistration();
                    var courseRegistrationLogic = new CourseRegistrationLogic();
                    var courseRegistrationDetailList = new List<CourseRegistrationDetail>();
                    var courseRegistrationDetaillogic = new CourseRegistrationDetailLogic();
                    courseRegistration = courseRegistrationLogic.GetModelBy(p => p.Person_Id == person.Id);
                    if(courseRegistration != null)
                    {
                        using(var scope = new TransactionScope())
                        {
                            bool courseRegistrationDetailDeleteStatus =
                                courseRegistrationDetaillogic.Delete(
                                    p => p.Student_Course_Registration_Id == courseRegistration.Id);
                            bool courseRegistrationDeleteStatus =
                                courseRegistrationLogic.Delete(p => p.Person_Id == person.Id);
                            scope.Complete();
                        }
                    }

                    decimal newSchoolFees = newPaymentFeeDetail.Sum(p => p.Fee.Amount);
                    decimal shortFall = newSchoolFees - model.OldSchoolFees;
                    return shortFall;
                }
                if(acceptanceFeeStatus > 0)
                {
                    return 1;
                }
                return 0;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private Person CreatePerson(ChangeCourseViewModel viewModel)
        {
            try
            {
                var personLogic = new PersonLogic();
                Person person = personLogic.GetModelBy(p => p.Person_Id == viewModel.Person.Id);
                person = personLogic.Create(person);
                if(person != null && person.Id > 0)
                {
                    viewModel.Person = person;
                }

                return person;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public Payment CreatePayment(ChangeCourseViewModel viewModel,int feeTypeID)
        {
            try
            {
                var paymentMode = new PaymentMode { Id = 1 };
                var paymentType = new PaymentType { Id = 2 };
                var feeType = new FeeType { Id = feeTypeID };
                var session = viewModel.Session;
                decimal amount = Convert.ToDecimal(viewModel.ShortFallAmount);

                var payment = new Payment();
                StudentPayment studentPayment = new StudentPayment();
                
                var paymentLogic = new PaymentLogic();
                var studentPaymentLogic = new StudentPaymentLogic();

                payment.Person = viewModel.Person;
                payment.PaymentMode = paymentMode;
                payment.PaymentType = paymentType;
                payment.PersonType = viewModel.Person.Type;
                payment.FeeType = feeType;
                payment.Session = session;
                payment.DatePaid = DateTime.Now;
                payment = paymentLogic.Create(payment);
                OnlinePayment newOnlinePayment = null;
                if(payment != null)
                {
                    var channel = new PaymentChannel { Id = (int)PaymentChannel.Channels.Etranzact };
                    var onlinePaymentLogic = new OnlinePaymentLogic();
                    var onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = channel;
                    onlinePayment.Payment = payment;

                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);

                    
                    studentPayment.Amount = amount;
                    studentPayment.Level = viewModel.Level;
                    studentPayment.Student = new Model.Model.Student() {Id = viewModel.Person.Id};
                    studentPayment.Session = session;
                    studentPayment.Status = false;
                    studentPayment.Id = payment.Id;

                    studentPaymentLogic.Create(studentPayment);
                    
                }

                return payment;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ApplicationForm CreateApplicationForm(ChangeCourseViewModel viewModel)
        {
            try
            {
                var applicationFormLogic = new ApplicationFormLogic();
                var applicationForm = new ApplicationForm();
                var appProgrammeFeeLogic = new ApplicationProgrammeFeeLogic();
                var applicationProgrammeFee = new ApplicationProgrammeFee();
                applicationProgrammeFee =
                    appProgrammeFeeLogic.GetModelBy(
                        p =>
                            p.Programme_Id == viewModel.AppliedCourse.Programme.Id &&
                            p.Fee_Type_Id == (int)FeeTypes.ApplicationForm);
                var appFormSetting = new ApplicationFormSetting { Id = 1 };
                applicationForm.Person = viewModel.Person;
                applicationForm.Payment = viewModel.Payment;
                applicationForm.DateSubmitted = DateTime.Now;
                applicationForm.Release = false;
                applicationForm.Rejected = false;
                applicationForm.Setting = appFormSetting;
                applicationForm.ProgrammeFee = applicationProgrammeFee;
                applicationForm = applicationFormLogic.Create(applicationForm,viewModel.AppliedCourse);
                return applicationForm;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public AppliedCourse CreateAppliedCourse(ChangeCourseViewModel viewModel)
        {
            try
            {
                var appliedCourseLogic = new AppliedCourseLogic();
                var appliedCourse = new AppliedCourse();
                var newAppliedCourse = new AppliedCourse();
                appliedCourse.Person = viewModel.Person;
                appliedCourse.Programme = viewModel.AppliedCourse.Programme;
                appliedCourse.Department = viewModel.AppliedCourse.Department;
                return appliedCourseLogic.Create(appliedCourse);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public OLevelResultDetail CreateOLevelResultDetail(OLevelResult model,List<OLevelResultDetail> models)
        {
            try
            {
                var olevelResultDetailLogic = new OLevelResultDetailLogic();
                OLevelResultDetail olevelResultDetail = null;
                for(int i = 0;i < models.Count;i++)
                {
                    models[i].Header = model;
                    olevelResultDetail = olevelResultDetailLogic.Create(models[i]);
                }
                return olevelResultDetail;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public Payment isStudentAlreadyExist(List<Person> persons)
        {
            try
            {
                var paymentLogic = new PaymentLogic();
                Payment payment = null;
                foreach(Person p in persons)
                {
                    payment = paymentLogic.GetModelBy(pl => pl.Person_Id == p.Id && pl.Fee_Type_Id == (int)FeeTypes.ApplicationForm);
                    if(payment != null)
                    {
                        return payment;
                    }
                }
                return payment;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private Int32 GetLevel(int ProgrammeId)
        {
            try
            {
                //set mode of study
                switch(ProgrammeId)
                {
                    case 1:
                    {
                        return 1;
                    }
                    case 2:
                    {
                        return 1;
                    }
                    case 3:
                    {
                        return 3;
                    }
                    case 4:
                    {
                        return 3;
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
            return 0;
        }

        public ActionResult CreateShortFall()
        {
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateShortFall(ChangeCourseViewModel model)
        {
            viewModel = new ChangeCourseViewModel();
            try
            {
                var applicationFormLogic = new ApplicationFormLogic();
                var appliedCourseLogic = new AppliedCourseLogic();
                var studentLogic = new StudentLogic();
                var personLogic = new PersonLogic();
                var programmeLogic = new ProgrammeLogic();
                var departmentLogic = new DepartmentLogic();
                ApplicationForm applicationForm =
                    applicationFormLogic.GetModelBy(p => p.Application_Form_Number == model.ApplicationFormNumber);
                List<Model.Model.Student> student =
                    studentLogic.GetModelsBy(p => p.Matric_Number == model.ApplicationFormNumber);
                if(applicationForm != null)
                {
                    Person person = personLogic.GetModelBy(p => p.Person_Id == applicationForm.Person.Id);
                    AppliedCourse studentAppliedCourse = appliedCourseLogic.GetModelBy(p => p.Application_Form_Id == applicationForm.Id);
                    model.AppliedCourse = studentAppliedCourse;
                    viewModel.ApplicationForm = applicationForm;
                    viewModel.Person = person;
                    viewModel.ApplicationFormNumber = model.ApplicationFormNumber;
                    viewModel.AppliedCourse = studentAppliedCourse;
                    ViewBag.ProgrammeId = viewModel.ProgrammeSelectList;
                    ViewBag.Departments = new SelectList(new List<Department>(),Utility.ID,Utility.NAME);

                    ViewBag.Levels = viewModel.LevelSelectList;
                    ViewBag.Sessions = viewModel.SessionSelectList;

                    return View(viewModel);
                }
                if(student.Count == 1)
                {
                    Model.Model.Student studnetItem = student[0];
                    if(studnetItem != null)
                    {
                        Person person = personLogic.GetModelBy(p => p.Person_Id == studnetItem.Id);
                        TempData["person"] = person;
                        viewModel.Person = person;
                        // model.AppliedCourse = studentAppliedCourse;
                        // viewModel.ApplicationForm = applicationForm;
                        // viewModel.Person = person;
                        // viewModel.ApplicationFormNumber = model.ApplicationFormNumber;
                        // viewModel.AppliedCourse = studentAppliedCourse;
                        ViewBag.ProgrammeId = viewModel.ProgrammeSelectList;
                        ViewBag.Departments = new SelectList(new List<Department>(),Utility.ID,Utility.NAME);

                        ViewBag.Levels = viewModel.LevelSelectList;
                        ViewBag.Sessions = viewModel.SessionSelectList;

                        return View(viewModel);
                    }
                }
                else
                {
                    SetMessage("Invalid ApplicationFormNumber Form Number!",Message.Category.Error);
                }
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                SetMessage("Error Occured !" + ex.Message,Message.Category.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult CreateShortFallInvoice(ChangeCourseViewModel model)
        {
            try
            {
                var applicationFormLogic = new ApplicationFormLogic();
                var applicationForm = new ApplicationForm();
                var shortFall = new ShortFall();
                var shortFallLogic = new ShortFallLogic();
                applicationForm = applicationFormLogic.GetModelBy(p => p.Application_Form_Number == model.ApplicationFormNumber);
                if(applicationForm != null)
                {
                    model.Person = applicationForm.Person;
                }
                else
                {
                    model.Person = (Person)TempData["person"];
                }

                var paymentLogic = new PaymentLogic();
                Payment payment = CreatePayment(model,(int)FeeTypes.ShortFall);
                shortFall.Payment = payment;
                shortFall.Amount = (double)model.ShortFallAmount;
                shortFall.Description = model.ShortFallDescription;
                shortFall = shortFallLogic.Create(shortFall);
                return RedirectToAction("ShortFallInvoice","Credential",new { Area = "Common",pmid = payment.Id,amount = model.ShortFallAmount });
            }
            catch(Exception ex)
            {
                SetMessage("Error Occured !" + ex.Message,Message.Category.Error);
                return RedirectToAction("CreateShortFall");
            }
        }
    }
}