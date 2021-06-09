using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    public class RegistrationController :BaseController
    {
        private readonly string appRoot = ConfigurationManager.AppSettings["AppRoot"];
        private readonly RegistrationIndexViewModel indexViewModel;
        private readonly RegistrationLogonViewModel logonViewModel;

        private readonly PaymentLogic paymentLogic;
        private RegistrationViewModel viewModel;

        public RegistrationController()
        {
            paymentLogic = new PaymentLogic();
            indexViewModel = new RegistrationIndexViewModel();
            logonViewModel = new RegistrationLogonViewModel();
        }

        public ActionResult Logon()
        {
            ViewBag.Sessions = logonViewModel.SessionSelectListItem;
            return View(logonViewModel);
        }

        [HttpPost]
        public ActionResult Logon(RegistrationLogonViewModel viewModel)
        {
            var payment = new Payment();

            try
            {
                var session = new Session { Id = viewModel.Session.Id };
                var feetype = new FeeType { Id = (int)FeeTypes.SchoolFees };
                var paymentLogic = new PaymentLogic();
                payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.ConfirmationOrderNumber,session,
                    feetype);
                if(payment == null)
                {
                    ViewBag.Sessions = logonViewModel.SessionSelectListItem;
                    SetMessage(
                        "Confirmation Order Number (" + viewModel.ConfirmationOrderNumber +
                        ") entered is not for School Fees payment! Please enter your School Fees Confirmation Order Number.",
                        Message.Category.Error);
                    return View(logonViewModel);
                }
            }
            catch(Exception ex)
            {
                ViewBag.Sessions = logonViewModel.SessionSelectListItem;
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
                return View(logonViewModel);
            }

            TempData["Payment"] = payment;
            Session["StudentData"] = Utility.Encrypt(payment.Person.Id.ToString());
            return RedirectToAction("Index","Registration",
                new { Area = "Student",sid = Utility.Encrypt(payment.Person.Id.ToString()), });
        }

        public ActionResult LogonCarryOver()
        {
            return View(logonViewModel);
        }

        public ActionResult Index(string sid)
        {
            try
            {
                string sessionData = Session["StudentData"].ToString();
                if(sid != null)
                {
                    long stId = Convert.ToInt64(Utility.Decrypt(sessionData));
                    var studentLogic = new StudentLogic();
                    indexViewModel.Student = studentLogic.GetBy(stId);
                    indexViewModel.Payment = (Payment)TempData.Peek("Payment");

                    if(indexViewModel.Student != null && indexViewModel.Student.Id > 0)
                    {
                        indexViewModel.isExtraYearStudent = false;
                        var studentLevelLogic = new StudentLevelLogic();
                        indexViewModel.StudentLevel = studentLevelLogic.GetBy(indexViewModel.Student.Id);
                        var paymentHistory = new PaymentHistory();
                        paymentHistory.Payments = paymentLogic.GetBy(indexViewModel.Payment.Person);
                        paymentHistory.Student = indexViewModel.Student;

                        indexViewModel.PaymentHistory = paymentHistory;
                        if(paymentHistory.Payments == null || paymentHistory.Payments.Count <= 0)
                        {
                            SetMessage("No payment made yet! Kindly generate invoice, go to bank and make your payments",Message.Category.Error);
                        }
                    }
                    else
                    {
                        SetMessage("Student details not found in the system! Please contact your system administrator.",Message.Category.Error);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            return View(indexViewModel);
        }

        public ActionResult Form(string sid,string pid)
        {
            var existingViewModel = (RegistrationViewModel)TempData["RegistrationViewModel"];

            try
            {
                long studentId = Convert.ToInt64(Utility.Decrypt(sid));
                int programmeId = Convert.ToInt32(Utility.Decrypt(pid));

                PopulateAllDropDowns(programmeId);
                if(existingViewModel != null)
                {
                    viewModel = existingViewModel;
                    SetStudentUploadedPassport(viewModel);
                }

                viewModel.LoadStudentInformationFormBy(studentId);
                if(viewModel.Student != null && viewModel.Student.Id > 0)
                {
                    if(viewModel.Payment == null)
                    {
                        viewModel.Payment = (Payment)TempData.Peek("Payment");
                    }

                    SetSelectedSittingSubjectAndGrade(viewModel);
                    SetLgaIfExist(viewModel);
                    SetDepartmentIfExist(viewModel);
                    SetDepartmentOptionIfExist(viewModel);
                    SetEntryAndStudyMode(viewModel);
                    SetDateOfBirth();

                    viewModel.Student.Type = new StudentType { Id = (int)StudentType.EnumName.Returning };
                    if(viewModel.StudentLevel.Programme.Id == 3 || viewModel.StudentLevel.Programme.Id == 4)
                    {
                        SetPreviousEducationStartDate();
                        SetPreviousEducationEndDate();

                        viewModel.Student.Category = viewModel.StudentLevel.Student.Category;
                        viewModel.Student.Type = viewModel.StudentLevel.Student.Type;
                    }
                    else
                    {
                        viewModel.Student.Category = viewModel.StudentLevel.Student.Category;
                        viewModel.Student.Type = viewModel.StudentLevel.Student.Type;
                    }

                    SetLastEmploymentStartDate();
                    SetLastEmploymentEndDate();
                    SetNdResultDateAwarded();
                    SetStudentAcademicInformationLevel();
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            TempData["RegistrationViewModel"] = viewModel;
            TempData["imageUrl"] = viewModel.Person.ImageFileUrl;

            return View(viewModel);
        }

        private void SetStudentAcademicInformationLevel()
        {
            try
            {
                if(viewModel.StudentAcademicInformation.Level == null ||
                    viewModel.StudentAcademicInformation.Level.Id <= 0)
                {
                    if(viewModel.StudentLevel.Level != null && viewModel.StudentLevel.Level.Id > 0)
                    {
                        viewModel.StudentAcademicInformation.Level = viewModel.StudentLevel.Level;
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetNdResultDateAwarded()
        {
            try
            {
                //if (viewModel.StudentNdResult != null && viewModel.StudentNdResult.DateAwarded != null)
                if(viewModel.StudentNdResult != null && viewModel.StudentNdResult.DateAwarded != DateTime.MinValue)
                {
                    if(viewModel.StudentNdResult.YearAwarded.Id > 0 && viewModel.StudentNdResult.MonthAwarded.Id > 0)
                    {
                        int noOfDays = DateTime.DaysInMonth(viewModel.StudentNdResult.YearAwarded.Id,
                            viewModel.StudentNdResult.MonthAwarded.Id);
                        List<Value> days = Utility.CreateNumberListFrom(1,noOfDays);
                        if(days != null && days.Count > 0)
                        {
                            days.Insert(0,new Value { Name = "--DD--" });
                        }

                        if(viewModel.StudentNdResult.DayAwarded != null && viewModel.StudentNdResult.DayAwarded.Id > 0)
                        {
                            ViewBag.StudentNdResultDayAwardeds = new SelectList(days,Utility.ID,Utility.NAME,
                                viewModel.StudentNdResult.DayAwarded.Id);
                        }
                        else
                        {
                            ViewBag.StudentNdResultDayAwardeds = new SelectList(days,Utility.ID,Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.StudentNdResultDayAwardeds = new SelectList(new List<Value>(),Utility.ID,Utility.NAME);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetLastEmploymentStartDate()
        {
            try
            {
                //if (viewModel.StudentEmploymentInformation != null && viewModel.StudentEmploymentInformation.StartDate != null)
                if(viewModel.StudentEmploymentInformation != null &&
                    viewModel.StudentEmploymentInformation.StartDate != DateTime.MinValue)
                {
                    if(viewModel.StudentEmploymentInformation.StartYear.Id > 0 &&
                        viewModel.StudentEmploymentInformation.StartMonth.Id > 0)
                    {
                        int noOfDays = DateTime.DaysInMonth(viewModel.StudentEmploymentInformation.StartYear.Id,
                            viewModel.StudentEmploymentInformation.StartMonth.Id);
                        List<Value> days = Utility.CreateNumberListFrom(1,noOfDays);
                        if(days != null && days.Count > 0)
                        {
                            days.Insert(0,new Value { Name = "--DD--" });
                        }

                        if(viewModel.StudentEmploymentInformation.StartDay != null &&
                            viewModel.StudentEmploymentInformation.StartDay.Id > 0)
                        {
                            ViewBag.StudentLastEmploymentStartDays = new SelectList(days,Utility.ID,Utility.NAME,
                                viewModel.StudentEmploymentInformation.StartDay.Id);
                        }
                        else
                        {
                            ViewBag.StudentLastEmploymentStartDays = new SelectList(days,Utility.ID,Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.StudentLastEmploymentStartDays = new SelectList(new List<Value>(),Utility.ID,Utility.NAME);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetLastEmploymentEndDate()
        {
            try
            {
                //if (viewModel.StudentEmploymentInformation != null && viewModel.StudentEmploymentInformation.EndDate != null)
                if(viewModel.StudentEmploymentInformation != null &&
                    viewModel.StudentEmploymentInformation.EndDate != DateTime.MinValue)
                {
                    if(viewModel.StudentEmploymentInformation.EndYear.Id > 0 &&
                        viewModel.StudentEmploymentInformation.EndMonth.Id > 0)
                    {
                        int noOfDays = DateTime.DaysInMonth(viewModel.StudentEmploymentInformation.EndYear.Id,
                            viewModel.StudentEmploymentInformation.EndMonth.Id);
                        List<Value> days = Utility.CreateNumberListFrom(1,noOfDays);
                        if(days != null && days.Count > 0)
                        {
                            days.Insert(0,new Value { Name = "--DD--" });
                        }

                        if(viewModel.StudentEmploymentInformation.EndDay != null &&
                            viewModel.StudentEmploymentInformation.EndDay.Id > 0)
                        {
                            ViewBag.StudentLastEmploymentEndDays = new SelectList(days,Utility.ID,Utility.NAME,
                                viewModel.StudentEmploymentInformation.EndDay.Id);
                        }
                        else
                        {
                            ViewBag.StudentLastEmploymentEndDays = new SelectList(days,Utility.ID,Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.StudentLastEmploymentEndDays = new SelectList(new List<Value>(),Utility.ID,Utility.NAME);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetEntryAndStudyMode(RegistrationViewModel vModel)
        {
            try
            {
                //set mode of entry

                switch(vModel.StudentLevel.Programme.Id)
                {
                    case 1:
                    {
                        vModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry { Id = 3 };
                        break;
                    }
                    case 2:
                    {
                        vModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry { Id = 2 };
                        break;
                    }
                    case 3:
                    {
                        vModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry { Id = 4 };
                        break;
                    }
                    case 4:
                    {
                        vModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry { Id = 1 };

                        break;
                    }
                }

                //set mode of study
                switch(vModel.StudentLevel.Programme.Id)
                {
                    case 1:
                    case 3:
                    {
                        vModel.StudentAcademicInformation.ModeOfStudy = new ModeOfStudy { Id = 1 };
                        break;
                    }
                    case 2:
                    case 4:
                    {
                        vModel.StudentAcademicInformation.ModeOfStudy = new ModeOfStudy { Id = 2 };
                        break;
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Form(RegistrationViewModel viewModel)
        {
            try
            {
                SetStudentUploadedPassport(viewModel);
                ModelState.Remove("Student.FirstName");
                ModelState.Remove("Student.LastName");
                ModelState.Remove("Student.MobilePhone");
                ModelState.Remove("Payment.Id");

                if(ModelState.IsValid)
                {
                    if(string.IsNullOrEmpty(viewModel.Person.ImageFileUrl) ||
                        viewModel.Person.ImageFileUrl == Utility.DEFAULT_AVATAR)
                    {
                        SetMessage("No Passport uploaded! Please upload your passport to continue.",
                            Message.Category.Error);
                        SetStateVariables(viewModel);
                        return View(viewModel);
                    }

                    TempData["RegistrationViewModel"] = viewModel;
                    return RedirectToAction("FormPreview","Registration");
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            SetStateVariables(viewModel);
            return View(viewModel);
        }

        private void SetStateVariables(RegistrationViewModel viewModel)
        {
            try
            {
                TempData["RegistrationViewModel"] = viewModel;
                TempData["imageUrl"] = viewModel.Person.ImageFileUrl;

                PopulateAllDropDowns(viewModel.StudentLevel.Programme.Id);
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        public ActionResult FormPreview()
        {
            var viewModel = (RegistrationViewModel)TempData["RegistrationViewModel"];

            try
            {
                if(viewModel != null)
                {
                    viewModel.Person.DateOfBirth = new DateTime(viewModel.Person.YearOfBirth.Id,
                        viewModel.Person.MonthOfBirth.Id,viewModel.Person.DayOfBirth.Id);
                    viewModel.Person.State =
                        viewModel.States.Where(m => m.Id == viewModel.Person.State.Id).SingleOrDefault();
                    viewModel.Person.LocalGovernment =
                        viewModel.Lgas.Where(m => m.Id == viewModel.Person.LocalGovernment.Id).SingleOrDefault();
                    viewModel.Person.Sex =
                        viewModel.Genders.Where(m => m.Id == viewModel.Person.Sex.Id).SingleOrDefault();
                    viewModel.NextOfKin.Relationship =
                        viewModel.Relationships.Where(m => m.Id == viewModel.NextOfKin.Relationship.Id)
                            .SingleOrDefault();
                    viewModel.Person.Religion =
                        viewModel.Religions.Where(m => m.Id == viewModel.Person.Religion.Id).SingleOrDefault();
                    viewModel.Student.Title =
                        viewModel.Titles.Where(m => m.Id == viewModel.Student.Title.Id).SingleOrDefault();
                    viewModel.Student.MaritalStatus =
                        viewModel.MaritalStatuses.Where(m => m.Id == viewModel.Student.MaritalStatus.Id)
                            .SingleOrDefault();

                    if(viewModel.Student.BloodGroup != null && viewModel.Student.BloodGroup.Id > 0)
                    {
                        viewModel.Student.BloodGroup =
                            viewModel.BloodGroups.Where(m => m.Id == viewModel.Student.BloodGroup.Id).SingleOrDefault();
                    }
                    if(viewModel.Student.Genotype != null && viewModel.Student.Genotype.Id > 0)
                    {
                        viewModel.Student.Genotype =
                            viewModel.Genotypes.Where(m => m.Id == viewModel.Student.Genotype.Id).SingleOrDefault();
                    }

                    viewModel.StudentAcademicInformation.ModeOfEntry =
                        viewModel.ModeOfEntries.Where(m => m.Id == viewModel.StudentAcademicInformation.ModeOfEntry.Id)
                            .SingleOrDefault();
                    viewModel.StudentAcademicInformation.ModeOfStudy =
                        viewModel.ModeOfStudies.Where(m => m.Id == viewModel.StudentAcademicInformation.ModeOfStudy.Id)
                            .SingleOrDefault();
                    viewModel.Student.Category =
                        viewModel.StudentCategories.Where(m => m.Id == viewModel.Student.Category.Id).SingleOrDefault();
                    viewModel.Student.Type =
                        viewModel.StudentTypes.Where(m => m.Id == viewModel.Student.Type.Id).SingleOrDefault();
                    viewModel.StudentAcademicInformation.Level =
                        viewModel.Levels.Where(m => m.Id == viewModel.StudentAcademicInformation.Level.Id)
                            .SingleOrDefault();
                    viewModel.StudentFinanceInformation.Mode =
                        viewModel.ModeOfFinances.Where(m => m.Id == viewModel.StudentFinanceInformation.Mode.Id)
                            .SingleOrDefault();
                    viewModel.StudentSponsor.Relationship =
                        viewModel.Relationships.Where(m => m.Id == viewModel.StudentSponsor.Relationship.Id)
                            .SingleOrDefault();

                    viewModel.FirstSittingOLevelResult.Type =
                        viewModel.OLevelTypes.Where(m => m.Id == viewModel.FirstSittingOLevelResult.Type.Id)
                            .SingleOrDefault();
                    if(viewModel.SecondSittingOLevelResult.Type != null)
                    {
                        viewModel.SecondSittingOLevelResult.Type =
                            viewModel.OLevelTypes.Where(m => m.Id == viewModel.SecondSittingOLevelResult.Type.Id)
                                .SingleOrDefault();
                    }

                    if(viewModel.StudentLevel.DepartmentOption == null ||
                        viewModel.StudentLevel.DepartmentOption.Id <= 0)
                    {
                        viewModel.StudentLevel.DepartmentOption = new DepartmentOption { Id = 1 };
                        viewModel.StudentLevel.DepartmentOption.Name = viewModel.StudentLevel.Department.Name;
                    }

                    if(viewModel.StudentLevel.Programme.Id == 3 || viewModel.StudentLevel.Programme.Id == 4)
                    {
                        viewModel.PreviousEducation.StartDate = new DateTime(viewModel.PreviousEducation.StartYear.Id,
                            viewModel.PreviousEducation.StartMonth.Id,viewModel.PreviousEducation.StartDay.Id);
                        viewModel.PreviousEducation.EndDate = new DateTime(viewModel.PreviousEducation.EndYear.Id,
                            viewModel.PreviousEducation.EndMonth.Id,viewModel.PreviousEducation.EndDay.Id);
                        viewModel.StudentEmploymentInformation.StartDate =
                            new DateTime(viewModel.StudentEmploymentInformation.StartYear.Id,
                                viewModel.StudentEmploymentInformation.StartMonth.Id,
                                viewModel.StudentEmploymentInformation.StartDay.Id);
                        viewModel.StudentEmploymentInformation.EndDate =
                            new DateTime(viewModel.StudentEmploymentInformation.EndYear.Id,
                                viewModel.StudentEmploymentInformation.EndMonth.Id,
                                viewModel.StudentEmploymentInformation.EndDay.Id);
                        viewModel.StudentNdResult.DateAwarded = new DateTime(viewModel.StudentNdResult.YearAwarded.Id,
                            viewModel.StudentNdResult.MonthAwarded.Id,viewModel.StudentNdResult.DayAwarded.Id);
                        viewModel.PreviousEducation.ResultGrade =
                            viewModel.ResultGrades.Where(m => m.Id == viewModel.PreviousEducation.ResultGrade.Id)
                                .SingleOrDefault();
                    }

                    UpdateOLevelResultDetail(viewModel);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            TempData["RegistrationViewModel"] = viewModel;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult FormPreview(RegistrationViewModel vm)
        {
            Model.Model.Student newStudent = null;
            var viewModel = (RegistrationViewModel)TempData["RegistrationViewModel"];
            var personType = new PersonType { Id = (int)PersonType.EnumName.Student };

            try
            {
                if(viewModel.StudentAlreadyExist == false)
                {
                    using(var transaction = new TransactionScope())
                    {
                        viewModel.Student.Id = viewModel.Person.Id;
                        viewModel.Student.Status = new StudentStatus { Id = 1 };
                        var studentLogic = new StudentLogic();

                        newStudent = viewModel.Student;
                        studentLogic.Modify(viewModel.Student);

                        viewModel.StudentSponsor.Student = newStudent;
                        var sponsorLogic = new StudentSponsorLogic();
                        if(sponsorLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
                        {
                            sponsorLogic.Modify(viewModel.StudentSponsor);
                        }
                        else
                        {
                            sponsorLogic.Create(viewModel.StudentSponsor);
                        }

                        viewModel.NextOfKin.Person = newStudent;
                        viewModel.NextOfKin.PersonType = new PersonType { Id = (int)PersonType.EnumName.Student };
                        var nextOfKinLogic = new NextOfKinLogic();
                        if(nextOfKinLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
                        {
                            nextOfKinLogic.Modify(viewModel.NextOfKin);
                        }
                        else
                        {
                            nextOfKinLogic.Create(viewModel.NextOfKin);
                        }

                        if(viewModel.FirstSittingOLevelResult == null || viewModel.FirstSittingOLevelResult.Id <= 0)
                        {
                            viewModel.FirstSittingOLevelResult.Person = viewModel.Person;
                            viewModel.FirstSittingOLevelResult.PersonType = personType;
                            viewModel.FirstSittingOLevelResult.Sitting = new OLevelExamSitting { Id = 1 };
                        }

                        if(viewModel.SecondSittingOLevelResult == null || viewModel.SecondSittingOLevelResult.Id <= 0)
                        {
                            viewModel.SecondSittingOLevelResult.Person = viewModel.Person;
                            viewModel.SecondSittingOLevelResult.PersonType = personType;
                            viewModel.SecondSittingOLevelResult.Sitting = new OLevelExamSitting { Id = 2 };
                        }

                        ModifyOlevelResult(viewModel.FirstSittingOLevelResult,viewModel.FirstSittingOLevelResultDetails);
                        ModifyOlevelResult(viewModel.SecondSittingOLevelResult,
                            viewModel.SecondSittingOLevelResultDetails);

                        viewModel.StudentAcademicInformation.Student = newStudent;
                        var academicInformationLogic = new StudentAcademicInformationLogic();
                        if(academicInformationLogic.GetModelBy(a => a.Person_Id == newStudent.Id) != null)
                        {
                            academicInformationLogic.Modify(viewModel.StudentAcademicInformation);
                        }
                        else
                        {
                            academicInformationLogic.Create(viewModel.StudentAcademicInformation);
                        }

                        viewModel.StudentFinanceInformation.Student = newStudent;
                        var financeInformationLogic = new StudentFinanceInformationLogic();
                        if(financeInformationLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() !=
                            null)
                        {
                            financeInformationLogic.Modify(viewModel.StudentFinanceInformation);
                        }
                        else
                        {
                            financeInformationLogic.Create(viewModel.StudentFinanceInformation);
                        }

                        if(viewModel.StudentLevel.Programme.Id == 3 || viewModel.StudentLevel.Programme.Id == 4)
                        {
                            var duration = new ITDuration { Id = 1 };
                            var qualification = new EducationalQualification { Id = 45 };
                            viewModel.PreviousEducation.Person = newStudent;
                            viewModel.PreviousEducation.PersonType = personType;
                            viewModel.PreviousEducation.ITDuration = duration;
                            viewModel.PreviousEducation.Qualification = qualification;
                            var previousEducationLogic = new PreviousEducationLogic();
                            if(
                                previousEducationLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() !=
                                null)
                            {
                                previousEducationLogic.Modify(viewModel.PreviousEducation);
                            }
                            else
                            {
                                previousEducationLogic.Create(viewModel.PreviousEducation);
                            }

                            viewModel.StudentEmploymentInformation.Student = newStudent;
                            var employmentInformationLogic = new StudentEmploymentInformationLogic();
                            if(
                                employmentInformationLogic.GetModelsBy(a => a.Person_Id == newStudent.Id)
                                    .FirstOrDefault() != null)
                            {
                                employmentInformationLogic.Modify(viewModel.StudentEmploymentInformation);
                            }
                            else
                            {
                                employmentInformationLogic.Create(viewModel.StudentEmploymentInformation);
                            }

                            viewModel.StudentNdResult.Student = newStudent;
                            var ndResultLogic = new StudentNdResultLogic();
                            if(ndResultLogic.GetModelsBy(a => a.Person_Id == newStudent.Id).FirstOrDefault() != null)
                            {
                                ndResultLogic.Modify(viewModel.StudentNdResult);
                            }
                            else
                            {
                                ndResultLogic.Create(viewModel.StudentNdResult);
                            }
                        }

                        string junkFilePath;
                        string destinationFilePath;
                        SetPersonPassportDestination(viewModel,out junkFilePath,out destinationFilePath);

                        var personLogic = new PersonLogic();
                        bool personModified = personLogic.Modify(viewModel.Person);
                        if(personModified)
                        {
                            SavePersonPassport(junkFilePath,destinationFilePath,viewModel.Person);
                            transaction.Complete();
                        }
                        else
                        {
                            throw new Exception("Passport save operation failed! Please try again.");
                        }

                        //transaction.Complete();
                    }
                }
                else
                {
                    using(var transaction = new TransactionScope())
                    {
                        viewModel.Student.Id = viewModel.Person.Id;
                        viewModel.Student.Status = new StudentStatus { Id = 1 };
                        var studentLogic = new StudentLogic();

                        newStudent = viewModel.Student;
                        studentLogic.Modify(viewModel.Student);

                        viewModel.StudentSponsor.Student = newStudent;
                        var sponsorLogic = new StudentSponsorLogic();
                        sponsorLogic.Modify(viewModel.StudentSponsor);

                        viewModel.NextOfKin.Person = newStudent;
                        viewModel.NextOfKin.PersonType = new PersonType { Id = (int)PersonType.EnumName.Student };
                        var nextOfKinLogic = new NextOfKinLogic();
                        nextOfKinLogic.Modify(viewModel.NextOfKin);

                        if(viewModel.FirstSittingOLevelResult == null || viewModel.FirstSittingOLevelResult.Id <= 0)
                        {
                            viewModel.FirstSittingOLevelResult.Person = viewModel.NextOfKin.Person;
                            viewModel.FirstSittingOLevelResult.Sitting = new OLevelExamSitting { Id = 1 };
                        }

                        if(viewModel.SecondSittingOLevelResult == null || viewModel.SecondSittingOLevelResult.Id <= 0)
                        {
                            viewModel.SecondSittingOLevelResult.Person = viewModel.NextOfKin.Person;
                            viewModel.SecondSittingOLevelResult.Sitting = new OLevelExamSitting { Id = 2 };
                        }
                        ModifyOlevelResult(viewModel.FirstSittingOLevelResult,viewModel.FirstSittingOLevelResultDetails);
                        ModifyOlevelResult(viewModel.SecondSittingOLevelResult,
                            viewModel.SecondSittingOLevelResultDetails);

                        viewModel.StudentAcademicInformation.Student = newStudent;
                        var academicInformationLogic = new StudentAcademicInformationLogic();
                        academicInformationLogic.Modify(viewModel.StudentAcademicInformation);

                        viewModel.StudentFinanceInformation.Student = newStudent;
                        var financeInformationLogic = new StudentFinanceInformationLogic();
                        financeInformationLogic.Modify(viewModel.StudentFinanceInformation);

                        if(viewModel.StudentLevel.Programme.Id == 3 || viewModel.StudentLevel.Programme.Id == 4)
                        {
                            viewModel.PreviousEducation.Person = newStudent;
                            viewModel.PreviousEducation.PersonType = personType;
                            var previousEducationLogic = new PreviousEducationLogic();
                            previousEducationLogic.Modify(viewModel.PreviousEducation);

                            viewModel.StudentEmploymentInformation.Student = newStudent;
                            var employmentInformationLogic = new StudentEmploymentInformationLogic();
                            employmentInformationLogic.Modify(viewModel.StudentEmploymentInformation);
                        }

                        var personLogic = new PersonLogic();
                        bool personModified = personLogic.Modify(viewModel.Person);
                        transaction.Complete();
                    }
                }
                TempData["RegistrationViewModel"] = viewModel;
                return RedirectToAction("AcknowledgementSlip","Registration");
            }
            catch(Exception ex)
            {
                newStudent = null;
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            TempData["RegistrationViewModel"] = viewModel;
            return View(viewModel);
        }

        private void SaveOLevelResult(Person person,OLevelResult oLevelResult,
            List<OLevelResultDetail> oLevelResultDetails,PersonType personType)
        {
            try
            {
                var oLevelResultLogic = new OLevelResultLogic();
                var oLevelResultDetailLogic = new OLevelResultDetailLogic();
                if(oLevelResult != null && oLevelResult.ExamNumber != null && oLevelResult.Type != null &&
                    oLevelResult.ExamYear > 0)
                {
                    //oLevelResult.ApplicationForm = applicationForm;
                    oLevelResult.Person = person;
                    oLevelResult.PersonType = personType;
                    //oLevelResult.Sitting = new OLevelExamSitting() { Id = 1 };

                    OLevelResult firstSittingOLevelResult = oLevelResultLogic.Create(oLevelResult);

                    if(oLevelResultDetails != null && oLevelResultDetails.Count > 0 && firstSittingOLevelResult != null)
                    {
                        List<OLevelResultDetail> olevelResultDetails =
                            oLevelResultDetails.Where(m => m.Grade != null && m.Subject != null).ToList();
                        foreach(OLevelResultDetail oLevelResultDetail in olevelResultDetails)
                        {
                            oLevelResultDetail.Header = firstSittingOLevelResult;
                        }

                        oLevelResultDetailLogic.Create(olevelResultDetails);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SavePersonPassport(string junkFilePath,string pathForSaving,Person person)
        {
            try
            {
                string folderPath = Path.GetDirectoryName(pathForSaving);
                string mainFileName = person.Id + "__";

                DeleteFileIfExist(folderPath,mainFileName);

                System.IO.File.Move(junkFilePath,pathForSaving);
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void SetPersonPassportDestination(RegistrationViewModel viewModel,out string junkFilePath,
            out string destinationFilePath)
        {
            const string TILDA = "~";

            try
            {
                string passportUrl = viewModel.Person.ImageFileUrl;
                junkFilePath = Server.MapPath(TILDA + viewModel.Person.ImageFileUrl);
                destinationFilePath = junkFilePath.Replace("Junk","Student");
                viewModel.Person.ImageFileUrl = passportUrl.Replace("Junk","Student");
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ActionResult AcknowledgementSlip()
        {
            var existingViewModel = (RegistrationViewModel)TempData["RegistrationViewModel"];

            TempData["RegistrationViewModel"] = existingViewModel;
            return View(existingViewModel);
        }

        private void UpdateOLevelResultDetail(RegistrationViewModel viewModel)
        {
            try
            {
                if(viewModel != null && viewModel.FirstSittingOLevelResultDetails != null &&
                    viewModel.FirstSittingOLevelResultDetails.Count > 0)
                {
                    foreach(
                        OLevelResultDetail firstSittingOLevelResultDetail in viewModel.FirstSittingOLevelResultDetails)
                    {
                        if(firstSittingOLevelResultDetail.Subject != null)
                        {
                            firstSittingOLevelResultDetail.Subject =
                                viewModel.OLevelSubjects.Where(m => m.Id == firstSittingOLevelResultDetail.Subject.Id)
                                    .SingleOrDefault();
                        }
                        if(firstSittingOLevelResultDetail.Grade != null)
                        {
                            firstSittingOLevelResultDetail.Grade =
                                viewModel.OLevelGrades.Where(m => m.Id == firstSittingOLevelResultDetail.Grade.Id)
                                    .SingleOrDefault();
                        }
                    }
                }

                if(viewModel != null && viewModel.SecondSittingOLevelResultDetails != null &&
                    viewModel.SecondSittingOLevelResultDetails.Count > 0)
                {
                    foreach(
                        OLevelResultDetail secondSittingOLevelResultDetail in viewModel.SecondSittingOLevelResultDetails
                        )
                    {
                        if(secondSittingOLevelResultDetail.Subject != null)
                        {
                            secondSittingOLevelResultDetail.Subject =
                                viewModel.OLevelSubjects.Where(m => m.Id == secondSittingOLevelResultDetail.Subject.Id)
                                    .SingleOrDefault();
                        }
                        if(secondSittingOLevelResultDetail.Grade != null)
                        {
                            secondSittingOLevelResultDetail.Grade =
                                viewModel.OLevelGrades.Where(m => m.Id == secondSittingOLevelResultDetail.Grade.Id)
                                    .SingleOrDefault();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetStudentUploadedPassport(RegistrationViewModel viewModel)
        {
            if(viewModel != null && viewModel.Person != null && !string.IsNullOrEmpty((string)TempData["imageUrl"]))
            {
                viewModel.Person.ImageFileUrl = (string)TempData["imageUrl"];
            }
            else
            {
                viewModel.Person.ImageFileUrl = Utility.DEFAULT_AVATAR;
            }
        }

        private void PopulateAllDropDowns(int programmeId)
        {
            var existingViewModel = (RegistrationViewModel)TempData["RegistrationViewModel"];

            try
            {
                if(existingViewModel == null)
                {
                    viewModel = new RegistrationViewModel();

                    ViewBag.States = viewModel.StateSelectList;
                    ViewBag.Sexes = viewModel.SexSelectList;
                    ViewBag.FirstChoiceFaculties = viewModel.FacultySelectList;
                    ViewBag.SecondChoiceFaculties = viewModel.FacultySelectList;
                    ViewBag.Lgas = new SelectList(new List<LocalGovernment>(),Utility.ID,Utility.NAME);
                    ViewBag.Relationships = viewModel.RelationshipSelectList;
                    ViewBag.FirstSittingOLevelTypes = viewModel.OLevelTypeSelectList;
                    ViewBag.SecondSittingOLevelTypes = viewModel.OLevelTypeSelectList;
                    ViewBag.FirstSittingExamYears = viewModel.ExamYearSelectList;
                    ViewBag.SecondSittingExamYears = viewModel.ExamYearSelectList;
                    ViewBag.Religions = viewModel.ReligionSelectList;
                    ViewBag.Abilities = viewModel.AbilitySelectList;
                    ViewBag.DayOfBirths = new SelectList(new List<Value>(),Utility.ID,Utility.NAME);
                    ViewBag.MonthOfBirths = viewModel.MonthOfBirthSelectList;
                    ViewBag.YearOfBirths = viewModel.YearOfBirthSelectList;
                    ViewBag.Titles = viewModel.TitleSelectList;
                    ViewBag.MaritalStatuses = viewModel.MaritalStatusSelectList;
                    ViewBag.BloodGroups = viewModel.BloodGroupSelectList;
                    ViewBag.Genotypes = viewModel.GenotypeSelectList;
                    ViewBag.ModeOfEntries = viewModel.ModeOfEntrySelectList;
                    ViewBag.ModeOfStudies = viewModel.ModeOfStudySelectList;
                    ViewBag.StudentCategories = viewModel.StudentCategorySelectList;
                    ViewBag.StudentTypes = viewModel.StudentTypeSelectList;
                    ViewBag.Levels = viewModel.LevelSelectList;
                    ViewBag.ModeOfFinances = viewModel.ModeOfFinanceSelectList;
                    ViewBag.Relationships = viewModel.RelationshipSelectList;
                    ViewBag.Faculties = viewModel.FacultySelectList;
                    ViewBag.AdmissionYears = viewModel.AdmissionYearSelectList;
                    ViewBag.GraduationYears = viewModel.GraduationYearSelectList;
                    ViewBag.Programmes = viewModel.ProgrammeSelectList;

                    if(viewModel.DepartmentSelectList != null)
                    {
                        ViewBag.Departments = viewModel.DepartmentSelectList;
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(new List<Department>(),Utility.ID,Utility.NAME);
                    }

                    if(programmeId == 3 || programmeId == 4)
                    {
                        ViewBag.StudentNdResultDayAwardeds = new SelectList(new List<Value>(),Utility.ID,Utility.NAME);
                        ViewBag.StudentNdResultMonthAwardeds = viewModel.StudentNdResultMonthAwardedSelectList;
                        ViewBag.StudentNdResultYearAwardeds = viewModel.StudentNdResultYearAwardedSelectList;

                        ViewBag.StudentLastEmploymentStartDays = new SelectList(new List<Value>(),Utility.ID,
                            Utility.NAME);
                        ViewBag.StudentLastEmploymentStartMonths = viewModel.StudentLastEmploymentStartMonthSelectList;
                        ViewBag.StudentLastEmploymentStartYears = viewModel.StudentLastEmploymentStartYearSelectList;

                        ViewBag.StudentLastEmploymentEndDays = new SelectList(new List<Value>(),Utility.ID,
                            Utility.NAME);
                        ViewBag.StudentLastEmploymentEndMonths = viewModel.StudentLastEmploymentEndMonthSelectList;
                        ViewBag.StudentLastEmploymentEndYears = viewModel.StudentLastEmploymentEndYearSelectList;

                        ViewBag.PreviousEducationStartDays = new SelectList(new List<Value>(),Utility.ID,Utility.NAME);
                        ViewBag.PreviousEducationStartMonths = viewModel.PreviousEducationStartMonthSelectList;
                        ViewBag.PreviousEducationStartYears = viewModel.PreviousEducationStartYearSelectList;

                        ViewBag.PreviousEducationEndDays = new SelectList(new List<Value>(),Utility.ID,Utility.NAME);
                        ViewBag.PreviousEducationEndMonths = viewModel.PreviousEducationEndMonthSelectList;
                        ViewBag.PreviousEducationEndYears = viewModel.PreviousEducationEndYearSelectList;

                        ViewBag.ResultGrades = viewModel.ResultGradeSelectList;
                    }

                    SetDefaultSelectedSittingSubjectAndGrade(viewModel);
                }
                else
                {
                    if(existingViewModel.Student.Title == null)
                    {
                        existingViewModel.Student.Title = new Title();
                    }
                    if(existingViewModel.Person.Sex == null)
                    {
                        existingViewModel.Person.Sex = new Sex();
                    }
                    if(existingViewModel.Student.MaritalStatus == null)
                    {
                        existingViewModel.Student.MaritalStatus = new MaritalStatus();
                    }
                    if(existingViewModel.Person.Religion == null)
                    {
                        existingViewModel.Person.Religion = new Religion();
                    }
                    if(existingViewModel.Person.State == null)
                    {
                        existingViewModel.Person.State = new State();
                    }
                    if(existingViewModel.StudentLevel.Programme == null)
                    {
                        existingViewModel.StudentLevel.Programme = new Programme();
                    }
                    if(existingViewModel.NextOfKin.Relationship == null)
                    {
                        existingViewModel.NextOfKin.Relationship = new Relationship();
                    }
                    if(existingViewModel.StudentSponsor.Relationship == null)
                    {
                        existingViewModel.StudentSponsor.Relationship = new Relationship();
                    }
                    if(existingViewModel.FirstSittingOLevelResult.Type == null)
                    {
                        existingViewModel.FirstSittingOLevelResult.Type = new OLevelType();
                    }
                    if(existingViewModel.Person.YearOfBirth == null)
                    {
                        existingViewModel.Person.YearOfBirth = new Value();
                    }
                    if(existingViewModel.Person.MonthOfBirth == null)
                    {
                        existingViewModel.Person.MonthOfBirth = new Value();
                    }
                    if(existingViewModel.Person.DayOfBirth == null)
                    {
                        existingViewModel.Person.DayOfBirth = new Value();
                    }
                    if(existingViewModel.StudentLevel.Department == null)
                    {
                        existingViewModel.StudentLevel.Department = new Department();
                    }
                    if(existingViewModel.Student.BloodGroup == null)
                    {
                        existingViewModel.Student.BloodGroup = new BloodGroup();
                    }
                    if(existingViewModel.Student.Genotype == null)
                    {
                        existingViewModel.Student.Genotype = new Genotype();
                    }
                    if(existingViewModel.StudentAcademicInformation.Level == null)
                    {
                        existingViewModel.StudentAcademicInformation.Level = new Level();
                    }
                    if(existingViewModel.StudentFinanceInformation.Mode == null)
                    {
                        existingViewModel.StudentFinanceInformation.Mode = new ModeOfFinance();
                    }

                    // PERSONAL INFORMATION
                    ViewBag.Titles = new SelectList(existingViewModel.TitleSelectList,Utility.VALUE,Utility.TEXT,
                        existingViewModel.Student.Title.Id);
                    ViewBag.Sexes = new SelectList(existingViewModel.SexSelectList,Utility.VALUE,Utility.TEXT,
                        existingViewModel.Person.Sex.Id);
                    ViewBag.MaritalStatuses = new SelectList(existingViewModel.MaritalStatusSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.Student.MaritalStatus.Id);
                    SetDateOfBirthDropDown(existingViewModel);
                    ViewBag.Religions = new SelectList(existingViewModel.ReligionSelectList,Utility.VALUE,Utility.TEXT,
                        existingViewModel.Person.Religion.Id);
                    ViewBag.States = new SelectList(existingViewModel.StateSelectList,Utility.VALUE,Utility.TEXT,
                        existingViewModel.Person.State.Id);

                    if(existingViewModel.Person.LocalGovernment != null &&
                        existingViewModel.Person.LocalGovernment.Id > 0)
                    {
                        ViewBag.Lgas = new SelectList(existingViewModel.LocalGovernmentSelectList,Utility.VALUE,
                            Utility.TEXT,existingViewModel.Person.LocalGovernment.Id);
                    }
                    else
                    {
                        ViewBag.Lgas = new SelectList(new List<LocalGovernment>(),Utility.VALUE,Utility.TEXT);
                    }
                    ViewBag.BloodGroups = new SelectList(existingViewModel.BloodGroupSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.Student.BloodGroup.Id);
                    ViewBag.Genotypes = new SelectList(existingViewModel.GenotypeSelectList,Utility.VALUE,Utility.TEXT,
                        existingViewModel.Student.Genotype.Id);

                    // ACADEMIC DETAILS
                    ViewBag.ModeOfEntries = new SelectList(existingViewModel.ModeOfEntrySelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.StudentAcademicInformation.ModeOfEntry.Id);
                    ViewBag.ModeOfStudies = new SelectList(existingViewModel.ModeOfStudySelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.StudentAcademicInformation.ModeOfStudy.Id);
                    ViewBag.Programmes = new SelectList(existingViewModel.ProgrammeSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.StudentLevel.Programme.Id);
                    ViewBag.Faculties = new SelectList(existingViewModel.FacultySelectList,Utility.VALUE,Utility.TEXT,
                        existingViewModel.StudentLevel.Department.Faculty.Id);

                    SetDepartmentIfExist(existingViewModel);
                    SetDepartmentOptionIfExist(existingViewModel);

                    ViewBag.StudentTypes = new SelectList(existingViewModel.StudentTypeSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.Student.Type.Id);
                    ViewBag.StudentCategories = new SelectList(existingViewModel.StudentCategorySelectList,
                        Utility.VALUE,Utility.TEXT,existingViewModel.Student.Category.Id);
                    ViewBag.AdmissionYears = new SelectList(existingViewModel.AdmissionYearSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.StudentAcademicInformation.YearOfAdmission);
                    ViewBag.GraduationYears = new SelectList(existingViewModel.GraduationYearSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.StudentAcademicInformation.YearOfGraduation);

                    if(existingViewModel.StudentAcademicInformation.Level != null &&
                        existingViewModel.StudentAcademicInformation.Level.Id > 0)
                    {
                        ViewBag.Levels = new SelectList(existingViewModel.LevelSelectList,Utility.VALUE,Utility.TEXT,
                            existingViewModel.StudentAcademicInformation.Level.Id);
                    }
                    else if(existingViewModel.StudentLevel.Level != null && existingViewModel.StudentLevel.Level.Id > 0)
                    {
                        ViewBag.Levels = new SelectList(existingViewModel.LevelSelectList,Utility.VALUE,
                            Utility.TEXT,existingViewModel.StudentLevel.Level.Id);
                    }

                    // FINANCE DETAILS
                    ViewBag.ModeOfFinances = new SelectList(existingViewModel.ModeOfFinanceSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.StudentFinanceInformation.Mode.Id);

                    // NEXT OF KIN
                    ViewBag.Relationships = new SelectList(existingViewModel.RelationshipSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.StudentSponsor.Relationship.Id);

                    //SPONSOR
                    ViewBag.Relationships = new SelectList(existingViewModel.RelationshipSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.NextOfKin.Relationship.Id);

                    ViewBag.FirstSittingOLevelTypes = new SelectList(existingViewModel.OLevelTypeSelectList,
                        Utility.VALUE,Utility.TEXT,existingViewModel.FirstSittingOLevelResult.Type.Id);
                    ViewBag.FirstSittingExamYears = new SelectList(existingViewModel.ExamYearSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.FirstSittingOLevelResult.ExamYear);
                    ViewBag.SecondSittingExamYears = new SelectList(existingViewModel.ExamYearSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.SecondSittingOLevelResult.ExamYear);

                    if(programmeId == 3 || programmeId == 4)
                    {
                        SetStudentNdResultDateAwardedDropDown(existingViewModel);
                        SetStudentLastEmploymentEndDateDropDown(existingViewModel);
                        SetStudentLastEmploymentStartDateDropDown(existingViewModel);
                        SetPreviousEducationEndDateDropDowns(existingViewModel);
                        SetPreviousEducationStartDateDropDowns(existingViewModel);

                        ViewBag.ResultGrades = new SelectList(existingViewModel.ResultGradeSelectList,Utility.VALUE,
                            Utility.TEXT,existingViewModel.PreviousEducation.ResultGrade.Id);
                    }

                    if(existingViewModel.SecondSittingOLevelResult.Type != null)
                    {
                        ViewBag.SecondSittingOLevelTypes = new SelectList(existingViewModel.OLevelTypeSelectList,
                            Utility.VALUE,Utility.TEXT,existingViewModel.SecondSittingOLevelResult.Type.Id);
                    }
                    else
                    {
                        existingViewModel.SecondSittingOLevelResult.Type = new OLevelType { Id = 0 };
                        ViewBag.SecondSittingOLevelTypes = new SelectList(existingViewModel.OLevelTypeSelectList,
                            Utility.VALUE,Utility.TEXT,0);
                    }

                    SetSelectedSittingSubjectAndGrade(existingViewModel);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetStudentLastEmploymentStartDateDropDown(RegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.StudentLastEmploymentStartMonths =
                    new SelectList(existingViewModel.StudentLastEmploymentStartMonthSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.StudentEmploymentInformation.StartMonth.Id);
                ViewBag.StudentLastEmploymentStartYears =
                    new SelectList(existingViewModel.StudentLastEmploymentStartYearSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.StudentEmploymentInformation.StartYear.Id);

                if((existingViewModel.StudentLastEmploymentStartDaySelectList == null ||
                     existingViewModel.StudentLastEmploymentStartDaySelectList.Count == 0) &&
                    (existingViewModel.StudentEmploymentInformation.StartMonth.Id > 0 &&
                     existingViewModel.StudentEmploymentInformation.StartYear.Id > 0))
                {
                    existingViewModel.StudentLastEmploymentStartDaySelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.StudentEmploymentInformation.StartMonth,
                            existingViewModel.StudentEmploymentInformation.StartYear);
                }
                else
                {
                    if(existingViewModel.StudentLastEmploymentStartDaySelectList != null &&
                        existingViewModel.StudentEmploymentInformation.StartDay.Id > 0)
                    {
                        ViewBag.StudentLastEmploymentStartDays =
                            new SelectList(existingViewModel.StudentLastEmploymentStartDaySelectList,Utility.VALUE,
                                Utility.TEXT,existingViewModel.StudentEmploymentInformation.StartDay.Id);
                    }
                    else if(existingViewModel.StudentLastEmploymentStartDaySelectList != null &&
                             existingViewModel.StudentEmploymentInformation.StartDay.Id <= 0)
                    {
                        ViewBag.StudentLastEmploymentStartDays =
                            existingViewModel.StudentLastEmploymentStartDaySelectList;
                    }
                    else if(existingViewModel.StudentLastEmploymentStartDaySelectList == null)
                    {
                        existingViewModel.StudentLastEmploymentStartDaySelectList = new List<SelectListItem>();
                        ViewBag.StudentLastEmploymentStartDays = new List<SelectListItem>();
                    }
                }

                if(existingViewModel.StudentEmploymentInformation.StartDay != null &&
                    existingViewModel.StudentEmploymentInformation.StartDay.Id > 0)
                {
                    ViewBag.StudentLastEmploymentStartDays =
                        new SelectList(existingViewModel.StudentLastEmploymentStartDaySelectList,Utility.VALUE,
                            Utility.TEXT,existingViewModel.StudentEmploymentInformation.StartDay.Id);
                }
                else
                {
                    ViewBag.StudentLastEmploymentStartDays = existingViewModel.StudentLastEmploymentStartDaySelectList;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void SetStudentLastEmploymentEndDateDropDown(RegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.StudentLastEmploymentEndMonths =
                    new SelectList(existingViewModel.StudentLastEmploymentEndMonthSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.StudentEmploymentInformation.EndMonth.Id);
                ViewBag.StudentLastEmploymentEndYears =
                    new SelectList(existingViewModel.StudentLastEmploymentEndYearSelectList,Utility.VALUE,Utility.TEXT,
                        existingViewModel.StudentEmploymentInformation.EndYear.Id);

                if((existingViewModel.StudentLastEmploymentEndDaySelectList == null ||
                     existingViewModel.StudentLastEmploymentEndDaySelectList.Count == 0) &&
                    (existingViewModel.StudentEmploymentInformation.EndMonth.Id > 0 &&
                     existingViewModel.StudentEmploymentInformation.EndYear.Id > 0))
                {
                    existingViewModel.StudentLastEmploymentEndDaySelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.StudentEmploymentInformation.EndMonth,
                            existingViewModel.StudentEmploymentInformation.EndYear);
                }
                else
                {
                    if(existingViewModel.StudentLastEmploymentEndDaySelectList != null &&
                        existingViewModel.StudentEmploymentInformation.EndDay.Id > 0)
                    {
                        ViewBag.StudentLastEmploymentEndDays =
                            new SelectList(existingViewModel.StudentLastEmploymentEndDaySelectList,Utility.VALUE,
                                Utility.TEXT,existingViewModel.StudentEmploymentInformation.EndDay.Id);
                    }
                    else if(existingViewModel.StudentLastEmploymentEndDaySelectList != null &&
                             existingViewModel.StudentEmploymentInformation.EndDay.Id <= 0)
                    {
                        ViewBag.StudentLastEmploymentEndDays =
                            existingViewModel.StudentLastEmploymentEndDaySelectList;
                    }
                    else if(existingViewModel.StudentLastEmploymentEndDaySelectList == null)
                    {
                        existingViewModel.StudentLastEmploymentEndDaySelectList = new List<SelectListItem>();
                        ViewBag.StudentLastEmploymentEndDays = new List<SelectListItem>();
                    }
                }

                if(existingViewModel.StudentEmploymentInformation.EndDay != null &&
                    existingViewModel.StudentEmploymentInformation.EndDay.Id > 0)
                {
                    ViewBag.StudentLastEmploymentEndDays =
                        new SelectList(existingViewModel.StudentLastEmploymentEndDaySelectList,Utility.VALUE,
                            Utility.TEXT,existingViewModel.StudentEmploymentInformation.EndDay.Id);
                }
                else
                {
                    ViewBag.StudentLastEmploymentEndDays = existingViewModel.StudentLastEmploymentEndDaySelectList;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void SetStudentNdResultDateAwardedDropDown(RegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.StudentNdResultMonthAwardeds =
                    new SelectList(existingViewModel.StudentNdResultMonthAwardedSelectList,Utility.VALUE,Utility.TEXT,
                        existingViewModel.StudentNdResult.MonthAwarded.Id);
                ViewBag.StudentNdResultYearAwardeds =
                    new SelectList(existingViewModel.StudentNdResultYearAwardedSelectList,Utility.VALUE,Utility.TEXT,
                        existingViewModel.StudentNdResult.YearAwarded.Id);
                if((existingViewModel.StudentNdResultDayAwardedSelectList == null ||
                     existingViewModel.StudentNdResultDayAwardedSelectList.Count == 0) &&
                    (existingViewModel.StudentNdResult.MonthAwarded.Id > 0 &&
                     existingViewModel.StudentNdResult.YearAwarded.Id > 0))
                {
                    existingViewModel.StudentNdResultDayAwardedSelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.StudentNdResult.MonthAwarded,
                            existingViewModel.StudentNdResult.YearAwarded);
                }
                else
                {
                    if(existingViewModel.StudentNdResultDayAwardedSelectList != null &&
                        existingViewModel.StudentNdResult.DayAwarded.Id > 0)
                    {
                        ViewBag.StudentNdResultDayAwardeds =
                            new SelectList(existingViewModel.StudentNdResultDayAwardedSelectList,Utility.VALUE,
                                Utility.TEXT,existingViewModel.StudentNdResult.DayAwarded.Id);
                    }
                    else if(existingViewModel.StudentNdResultDayAwardedSelectList != null &&
                             existingViewModel.StudentNdResult.DayAwarded.Id <= 0)
                    {
                        ViewBag.StudentNdResultDayAwardeds = existingViewModel.StudentNdResultDayAwardedSelectList;
                    }
                    else if(existingViewModel.StudentNdResultDayAwardedSelectList == null)
                    {
                        existingViewModel.StudentNdResultDayAwardedSelectList = new List<SelectListItem>();
                        ViewBag.StudentNdResultDayAwardeds = new List<SelectListItem>();
                    }
                }

                if(existingViewModel.StudentNdResult.DayAwarded != null &&
                    existingViewModel.StudentNdResult.DayAwarded.Id > 0)
                {
                    ViewBag.StudentNdResultDayAwardeds =
                        new SelectList(existingViewModel.StudentNdResultDayAwardedSelectList,Utility.VALUE,
                            Utility.TEXT,existingViewModel.StudentNdResult.DayAwarded.Id);
                }
                else
                {
                    ViewBag.StudentNdResultDayAwardeds = existingViewModel.StudentNdResultDayAwardedSelectList;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void SetDateOfBirthDropDown(RegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.MonthOfBirths = new SelectList(existingViewModel.MonthOfBirthSelectList,Utility.VALUE,
                    Utility.TEXT,existingViewModel.Person.MonthOfBirth.Id);
                ViewBag.YearOfBirths = new SelectList(existingViewModel.YearOfBirthSelectList,Utility.VALUE,
                    Utility.TEXT,existingViewModel.Person.YearOfBirth.Id);
                if((existingViewModel.DayOfBirthSelectList == null || existingViewModel.DayOfBirthSelectList.Count == 0) &&
                    (existingViewModel.Person.MonthOfBirth.Id > 0 && existingViewModel.Person.YearOfBirth.Id > 0))
                {
                    existingViewModel.DayOfBirthSelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.Person.MonthOfBirth,
                            existingViewModel.Person.YearOfBirth);
                }
                else
                {
                    if(existingViewModel.DayOfBirthSelectList != null && existingViewModel.Person.DayOfBirth.Id > 0)
                    {
                        ViewBag.DayOfBirths = new SelectList(existingViewModel.DayOfBirthSelectList,Utility.VALUE,
                            Utility.TEXT,existingViewModel.Person.DayOfBirth.Id);
                    }
                    else if(existingViewModel.DayOfBirthSelectList != null &&
                             existingViewModel.Person.DayOfBirth.Id <= 0)
                    {
                        ViewBag.DayOfBirths = existingViewModel.DayOfBirthSelectList;
                    }
                    else if(existingViewModel.DayOfBirthSelectList == null)
                    {
                        existingViewModel.DayOfBirthSelectList = new List<SelectListItem>();
                        ViewBag.DayOfBirths = new List<SelectListItem>();
                    }
                }

                if(existingViewModel.Person.DayOfBirth != null && existingViewModel.Person.DayOfBirth.Id > 0)
                {
                    ViewBag.DayOfBirths = new SelectList(existingViewModel.DayOfBirthSelectList,Utility.VALUE,
                        Utility.TEXT,existingViewModel.Person.DayOfBirth.Id);
                }
                else
                {
                    ViewBag.DayOfBirths = existingViewModel.DayOfBirthSelectList;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void SetPreviousEducationStartDateDropDowns(RegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.PreviousEducationStartMonths =
                    new SelectList(existingViewModel.PreviousEducationStartMonthSelectList,Utility.VALUE,Utility.TEXT,
                        existingViewModel.PreviousEducation.StartMonth.Id);
                ViewBag.PreviousEducationStartYears =
                    new SelectList(existingViewModel.PreviousEducationStartYearSelectList,Utility.VALUE,Utility.TEXT,
                        existingViewModel.PreviousEducation.StartYear.Id);
                if((existingViewModel.PreviousEducationStartDaySelectList == null ||
                     existingViewModel.PreviousEducationStartDaySelectList.Count == 0) &&
                    (existingViewModel.PreviousEducation.StartMonth.Id > 0 &&
                     existingViewModel.PreviousEducation.StartYear.Id > 0))
                {
                    existingViewModel.PreviousEducationStartDaySelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.PreviousEducation.StartMonth,
                            existingViewModel.PreviousEducation.StartYear);
                }
                else
                {
                    if(existingViewModel.PreviousEducationStartDaySelectList != null &&
                        existingViewModel.PreviousEducation.StartDay.Id > 0)
                    {
                        ViewBag.PreviousEducationStartDays =
                            new SelectList(existingViewModel.PreviousEducationStartDaySelectList,Utility.VALUE,
                                Utility.TEXT,existingViewModel.PreviousEducation.StartDay.Id);
                    }
                    else if(existingViewModel.PreviousEducationStartDaySelectList != null &&
                             existingViewModel.PreviousEducation.StartDay.Id <= 0)
                    {
                        ViewBag.PreviousEducationStartDays = existingViewModel.PreviousEducationStartDaySelectList;
                    }
                    else if(existingViewModel.PreviousEducationStartDaySelectList == null)
                    {
                        existingViewModel.PreviousEducationStartDaySelectList = new List<SelectListItem>();
                        ViewBag.PreviousEducationStartDays = new List<SelectListItem>();
                    }
                }

                if(existingViewModel.PreviousEducation.StartDay != null &&
                    existingViewModel.PreviousEducation.StartDay.Id > 0)
                {
                    ViewBag.PreviousEducationStartDays =
                        new SelectList(existingViewModel.PreviousEducationStartDaySelectList,Utility.VALUE,
                            Utility.TEXT,existingViewModel.PreviousEducation.StartDay.Id);
                }
                else
                {
                    ViewBag.PreviousEducationStartDays = existingViewModel.PreviousEducationStartDaySelectList;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void SetPreviousEducationEndDateDropDowns(RegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.PreviousEducationEndMonths =
                    new SelectList(existingViewModel.PreviousEducationEndMonthSelectList,Utility.VALUE,Utility.TEXT,
                        existingViewModel.PreviousEducation.EndMonth.Id);
                ViewBag.PreviousEducationEndYears = new SelectList(
                    existingViewModel.PreviousEducationEndYearSelectList,Utility.VALUE,Utility.TEXT,
                    existingViewModel.PreviousEducation.EndYear.Id);
                if((existingViewModel.PreviousEducationEndDaySelectList == null ||
                     existingViewModel.PreviousEducationEndDaySelectList.Count == 0) &&
                    (existingViewModel.PreviousEducation.EndMonth.Id > 0 &&
                     existingViewModel.PreviousEducation.EndYear.Id > 0))
                {
                    existingViewModel.PreviousEducationEndDaySelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.PreviousEducation.EndMonth,
                            existingViewModel.PreviousEducation.EndYear);
                }
                else
                {
                    if(existingViewModel.PreviousEducationEndDaySelectList != null &&
                        existingViewModel.PreviousEducation.EndDay.Id > 0)
                    {
                        ViewBag.PreviousEducationEndDays =
                            new SelectList(existingViewModel.PreviousEducationEndDaySelectList,Utility.VALUE,
                                Utility.TEXT,existingViewModel.PreviousEducation.EndDay.Id);
                    }
                    else if(existingViewModel.PreviousEducationEndDaySelectList != null &&
                             existingViewModel.PreviousEducation.EndDay.Id <= 0)
                    {
                        ViewBag.PreviousEducationEndDays = existingViewModel.PreviousEducationEndDaySelectList;
                    }
                    else if(existingViewModel.PreviousEducationEndDaySelectList == null)
                    {
                        existingViewModel.PreviousEducationEndDaySelectList = new List<SelectListItem>();
                        ViewBag.PreviousEducationEndDays = new List<SelectListItem>();
                    }
                }

                if(existingViewModel.PreviousEducation.EndDay != null &&
                    existingViewModel.PreviousEducation.EndDay.Id > 0)
                {
                    ViewBag.PreviousEducationEndDays =
                        new SelectList(existingViewModel.PreviousEducationEndDaySelectList,Utility.VALUE,Utility.TEXT,
                            existingViewModel.PreviousEducation.EndDay.Id);
                }
                else
                {
                    ViewBag.PreviousEducationEndDays = existingViewModel.PreviousEducationEndDaySelectList;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void SetDefaultSelectedSittingSubjectAndGrade(RegistrationViewModel viewModel)
        {
            try
            {
                if(viewModel != null && viewModel.FirstSittingOLevelResultDetails != null)
                {
                    for(int i = 0;i < 9;i++)
                    {
                        ViewData["FirstSittingOLevelSubjectId" + i] = viewModel.OLevelSubjectSelectList;
                        ViewData["FirstSittingOLevelGradeId" + i] = viewModel.OLevelGradeSelectList;
                    }
                }

                if(viewModel != null && viewModel.SecondSittingOLevelResultDetails != null)
                {
                    for(int i = 0;i < 9;i++)
                    {
                        ViewData["SecondSittingOLevelSubjectId" + i] = viewModel.OLevelSubjectSelectList;
                        ViewData["SecondSittingOLevelGradeId" + i] = viewModel.OLevelGradeSelectList;
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetLgaIfExist(RegistrationViewModel viewModel)
        {
            try
            {
                if(viewModel.Person.State != null && !string.IsNullOrEmpty(viewModel.Person.State.Id))
                {
                    var localGovernmentLogic = new LocalGovernmentLogic();
                    List<LocalGovernment> lgas =
                        localGovernmentLogic.GetModelsBy(l => l.State_Id == viewModel.Person.State.Id);
                    if(viewModel.Person.LocalGovernment != null && viewModel.Person.LocalGovernment.Id > 0)
                    {
                        ViewBag.Lgas = new SelectList(lgas,Utility.ID,Utility.NAME,
                            viewModel.Person.LocalGovernment.Id);
                    }
                    else
                    {
                        ViewBag.Lgas = new SelectList(lgas,Utility.ID,Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.Lgas = new SelectList(new List<LocalGovernment>(),Utility.ID,Utility.NAME);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetDepartmentIfExist(RegistrationViewModel viewModel)
        {
            try
            {
                if(viewModel.StudentLevel.Programme != null && viewModel.StudentLevel.Programme.Id > 0)
                {
                    var departmentLogic = new ProgrammeDepartmentLogic();
                    List<Department> departments = departmentLogic.GetBy(viewModel.StudentLevel.Programme);
                    if(viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                    {
                        ViewBag.Departments = new SelectList(departments,Utility.ID,Utility.NAME,
                            viewModel.StudentLevel.Department.Id);
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(departments,Utility.ID,Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.Departments = new SelectList(new List<Department>(),Utility.ID,Utility.NAME);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetDepartmentOptionIfExist(RegistrationViewModel viewModel)
        {
            try
            {
                if(viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                {
                    var departmentOptionLogic = new DepartmentOptionLogic();
                    List<DepartmentOption> departmentOptions =
                        departmentOptionLogic.GetModelsBy(l => l.Department_Id == viewModel.StudentLevel.Department.Id);
                    if(viewModel.StudentLevel.DepartmentOption != null &&
                        viewModel.StudentLevel.DepartmentOption.Id > 0)
                    {
                        ViewBag.DepartmentOptions = new SelectList(departmentOptions,Utility.ID,Utility.NAME,
                            viewModel.StudentLevel.DepartmentOption.Id);
                    }
                    else
                    {
                        var options = new List<DepartmentOption>();
                        var option = new DepartmentOption { Id = 0,Name = viewModel.StudentLevel.Department.Name };
                        options.Add(option);

                        ViewBag.DepartmentOptions = new SelectList(options,Utility.ID,Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(),Utility.ID,Utility.NAME);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetPreviousEducationStartDate()
        {
            try
            {
                if(viewModel.PreviousEducation != null && viewModel.PreviousEducation.StartDate != DateTime.MinValue)
                {
                    int noOfDays = DateTime.DaysInMonth(viewModel.PreviousEducation.StartYear.Id,
                        viewModel.PreviousEducation.StartMonth.Id);
                    List<Value> days = Utility.CreateNumberListFrom(1,noOfDays);
                    if(days != null && days.Count > 0)
                    {
                        days.Insert(0,new Value { Name = "--DD--" });
                    }

                    if(viewModel.PreviousEducation.StartDay != null && viewModel.PreviousEducation.StartDay.Id > 0)
                    {
                        ViewBag.PreviousEducationStartDays = new SelectList(days,Utility.ID,Utility.NAME,
                            viewModel.PreviousEducation.StartDay.Id);
                    }
                    else
                    {
                        ViewBag.PreviousEducationStartDays = new SelectList(days,Utility.ID,Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.PreviousEducationStartDays = new SelectList(new List<Value>(),Utility.ID,Utility.NAME);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetPreviousEducationEndDate()
        {
            try
            {
                if(viewModel.PreviousEducation != null && viewModel.PreviousEducation.EndDate != DateTime.MinValue)
                {
                    int noOfDays = DateTime.DaysInMonth(viewModel.PreviousEducation.EndYear.Id,
                        viewModel.PreviousEducation.EndMonth.Id);
                    List<Value> days = Utility.CreateNumberListFrom(1,noOfDays);
                    if(days != null && days.Count > 0)
                    {
                        days.Insert(0,new Value { Name = "--DD--" });
                    }

                    if(viewModel.PreviousEducation.EndDay != null && viewModel.PreviousEducation.EndDay.Id > 0)
                    {
                        ViewBag.PreviousEducationEndDays = new SelectList(days,Utility.ID,Utility.NAME,
                            viewModel.PreviousEducation.EndDay.Id);
                    }
                    else
                    {
                        ViewBag.PreviousEducationEndDays = new SelectList(days,Utility.ID,Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.PreviousEducationEndDays = new SelectList(new List<Value>(),Utility.ID,Utility.NAME);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetDateOfBirth()
        {
            try
            {
                if(viewModel.Person.DateOfBirth.HasValue)
                {
                    if(viewModel.Person.YearOfBirth.Id > 0 && viewModel.Person.MonthOfBirth.Id > 0)
                    {
                        int noOfDays = DateTime.DaysInMonth(viewModel.Person.YearOfBirth.Id,
                            viewModel.Person.MonthOfBirth.Id);
                        List<Value> days = Utility.CreateNumberListFrom(1,noOfDays);
                        if(days != null && days.Count > 0)
                        {
                            days.Insert(0,new Value { Name = "--DD--" });
                        }

                        if(viewModel.Person.DayOfBirth != null && viewModel.Person.DayOfBirth.Id > 0)
                        {
                            ViewBag.DayOfBirths = new SelectList(days,Utility.ID,Utility.NAME,
                                viewModel.Person.DayOfBirth.Id);
                        }
                        else
                        {
                            ViewBag.DayOfBirths = new SelectList(days,Utility.ID,Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.DayOfBirths = new SelectList(new List<Value>(),Utility.ID,Utility.NAME);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        private void SetSelectedSittingSubjectAndGrade(RegistrationViewModel existingViewModel)
        {
            try
            {
                if(existingViewModel != null && existingViewModel.FirstSittingOLevelResultDetails != null &&
                    existingViewModel.FirstSittingOLevelResultDetails.Count > 0)
                {
                    int i = 0;
                    foreach(
                        OLevelResultDetail firstSittingOLevelResultDetail in
                            existingViewModel.FirstSittingOLevelResultDetails)
                    {
                        if(firstSittingOLevelResultDetail.Subject != null &&
                            firstSittingOLevelResultDetail.Grade != null)
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList,Utility.VALUE,Utility.TEXT,
                                    firstSittingOLevelResultDetail.Subject.Id);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList,Utility.VALUE,Utility.TEXT,
                                    firstSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            firstSittingOLevelResultDetail.Subject = new OLevelSubject { Id = 0 };
                            firstSittingOLevelResultDetail.Grade = new OLevelGrade { Id = 0 };

                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList,Utility.VALUE,Utility.TEXT,0);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList,Utility.VALUE,Utility.TEXT,0);
                        }

                        i++;
                    }
                }

                if(existingViewModel != null && existingViewModel.SecondSittingOLevelResultDetails != null &&
                    existingViewModel.SecondSittingOLevelResultDetails.Count > 0)
                {
                    int i = 0;
                    foreach(
                        OLevelResultDetail secondSittingOLevelResultDetail in
                            existingViewModel.SecondSittingOLevelResultDetails)
                    {
                        if(secondSittingOLevelResultDetail.Subject != null &&
                            secondSittingOLevelResultDetail.Grade != null)
                        {
                            ViewData["SecondSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList,Utility.VALUE,Utility.TEXT,
                                    secondSittingOLevelResultDetail.Subject.Id);
                            ViewData["SecondSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList,Utility.VALUE,Utility.TEXT,
                                    secondSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            secondSittingOLevelResultDetail.Subject = new OLevelSubject { Id = 0 };
                            secondSittingOLevelResultDetail.Grade = new OLevelGrade { Id = 0 };

                            ViewData["SecondSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList,Utility.VALUE,Utility.TEXT,0);
                            ViewData["SecondSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList,Utility.VALUE,Utility.TEXT,0);
                        }

                        i++;
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
        }

        public JsonResult GetLocalGovernmentsByState(string id)
        {
            try
            {
                var lgaLogic = new LocalGovernmentLogic();
                List<LocalGovernment> lgas = lgaLogic.GetModelsBy(l => l.State_Id == id);

                return Json(new SelectList(lgas,Utility.ID,Utility.NAME),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetDayOfBirthBy(string monthId,string yearId)
        {
            try
            {
                if(string.IsNullOrEmpty(monthId) || string.IsNullOrEmpty(yearId))
                {
                    return null;
                }

                var month = new Value { Id = Convert.ToInt32(monthId) };
                var year = new Value { Id = Convert.ToInt32(yearId) };
                List<Value> days = Utility.GetNumberOfDaysInMonth(month,year);

                return Json(new SelectList(days,Utility.ID,Utility.NAME),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
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

                return Json(new SelectList(departments,Utility.ID,Utility.NAME),JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public virtual ActionResult UploadFile(FormCollection form)
        {
            HttpPostedFileBase file = Request.Files["PassportFile"];

            bool isUploaded = false;
            string personId = form["Person.Id"];
            string passportUrl = form["Person.ImageFileUrl"];
            string message = "File upload failed";

            string path = null;
            string imageUrl = null;
            string imageUrlDisplay = null;

            try
            {
                if(file != null && file.ContentLength != 0)
                {
                    var fileInfo = new FileInfo(file.FileName);
                    string fileExtension = fileInfo.Extension;
                    string newFile = personId + "__";
                    string newFileName = newFile +
                                         DateTime.Now.ToString().Replace("/","").Replace(":","").Replace(" ","") +
                                         fileExtension;

                    string invalidFileMessage = InvalidFile(file.ContentLength,fileExtension);
                    if(!string.IsNullOrEmpty(invalidFileMessage))
                    {
                        isUploaded = false;
                        TempData["imageUrl"] = null;
                        return Json(new { isUploaded,message = invalidFileMessage,imageUrl = passportUrl },"text/html",
                            JsonRequestBehavior.AllowGet);
                    }

                    string pathForSaving = Server.MapPath("~/Content/Junk");
                    if(CreateFolderIfNeeded(pathForSaving))
                    {
                        DeleteFileIfExist(pathForSaving,personId);

                        file.SaveAs(Path.Combine(pathForSaving,newFileName));

                        isUploaded = true;
                        message = "File uploaded successfully!";

                        path = Path.Combine(pathForSaving,newFileName);
                        if(path != null)
                        {
                            imageUrl = "/Content/Junk/" + newFileName;
                            imageUrlDisplay = appRoot + imageUrl + "?t=" + DateTime.Now;

                            //imageUrlDisplay = "/ilaropoly" + imageUrl + "?t=" + DateTime.Now;
                            TempData["imageUrl"] = imageUrl;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                message = string.Format("File upload failed: {0}",ex.Message);
            }

            return Json(new { isUploaded,message,imageUrl = imageUrlDisplay },"text/html",JsonRequestBehavior.AllowGet);
        }

        private string InvalidFile(decimal uploadedFileSize,string fileExtension)
        {
            try
            {
                string message = null;
                decimal oneKiloByte = 1024;
                decimal maximumFileSize = 20 * oneKiloByte;

                decimal actualFileSizeToUpload = Math.Round(uploadedFileSize / oneKiloByte,1);
                if(InvalidFileType(fileExtension))
                {
                    message = "File type '" + fileExtension +
                              "' is invalid! File type must be any of the following: .jpg, .jpeg, .png or .jif ";
                }
                else if(actualFileSizeToUpload > (maximumFileSize / oneKiloByte))
                {
                    message = "Your file size of " + actualFileSizeToUpload.ToString("0.#") +
                              " Kb is too large, maximum allowed size is " + (maximumFileSize / oneKiloByte) + " Kb";
                }

                return message;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private bool InvalidFileType(string extension)
        {
            extension = extension.ToLower();
            switch(extension)
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

        private void DeleteFileIfExist(string folderPath,string fileName)
        {
            try
            {
                string wildCard = fileName + "*.*";
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath,wildCard,SearchOption.TopDirectoryOnly);

                if(files != null && files.Count() > 0)
                {
                    foreach(string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        private bool CreateFolderIfNeeded(string path)
        {
            try
            {
                bool result = true;
                if(!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch(Exception)
                    {
                        result = false;
                    }
                }

                return result;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void ModifyOlevelResult(OLevelResult oLevelResult,List<OLevelResultDetail> oLevelResultDetails)
        {
            try
            {
                var oLevelResultLogic = new OLevelResultLogic();
                var oLevelResultDetailLogic = new OLevelResultDetailLogic();
                if(oLevelResult != null && oLevelResult.ExamNumber != null && oLevelResult.Type != null &&
                    oLevelResult.ExamYear > 0)
                {
                    OLevelResult oLevel =
                        oLevelResultLogic.GetModelsBy(
                            p =>
                                p.Person_Id == oLevelResult.Person.Id && p.Exam_Number == oLevelResult.ExamNumber &&
                                p.Exam_Year == oLevelResult.ExamYear &&
                                p.O_Level_Exam_Sitting_Id == oLevelResult.Sitting.Id).FirstOrDefault();
                    if(oLevel != null)
                    {
                        oLevelResult.Id = oLevel.Id;
                    }

                    if(oLevelResult != null && oLevelResult.Id > 0)
                    {
                        oLevelResultDetailLogic.DeleteBy(oLevelResult);
                        oLevelResultLogic.Modify(oLevelResult);
                    }
                    else
                    {
                        OLevelResult newOLevelResult = oLevelResultLogic.Create(oLevelResult);
                        oLevelResult.Id = newOLevelResult.Id;
                    }

                    if(oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        List<OLevelResultDetail> olevelResultDetails =
                            oLevelResultDetails.Where(
                                m => m.Grade != null && m.Grade.Id > 0 && m.Subject != null && m.Subject.Id > 0)
                                .ToList();
                        foreach(OLevelResultDetail oLevelResultDetail in olevelResultDetails)
                        {
                            oLevelResultDetail.Header = oLevelResult;
                            oLevelResultDetailLogic.Create(oLevelResultDetail);
                        }

                        //oLevelResultDetailLogic.Create(olevelResultDetails);
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ActionResult CourseRegistration(string sid)
        {
            Int64 studentId = Convert.ToInt64(Utility.Decrypt(sid));
            if(studentId > 0)
            {
                var studentLogic = new StudentLogic();
                var studentLevelLogic = new StudentLevelLogic();
                var viewModel = new SemesterViewModel();
                Model.Model.Student student = studentLogic.GetBy(studentId);
                StudentLevel studentLevel = studentLevelLogic.GetBy(student.Id);
                viewModel.Student = student;
                viewModel.StudentLevel = studentLevel;
                ViewBag.semesterId = viewModel.SemesterSelectList;
                ViewBag.sessionId = viewModel.SessionSelectList;
                ViewBag.levelId = viewModel.levelSelectList;
                return View(viewModel);
            }
            return RedirectToAction("logon");
        }
   
        public ActionResult PrintReceipt()
        {
            ViewBag.Sessions = logonViewModel.SessionSelectListItem;
            ViewBag.FeeTypes = logonViewModel.FeeTypeSelectListItem;
            return View(logonViewModel);
        }

        [HttpPost]
        public ActionResult PrintReceipt(RegistrationLogonViewModel viewModel)
        {
            var payment = new Payment();

            try
            {
                var session = new Session { Id = viewModel.Session.Id };
                var feetype = new FeeType { Id = viewModel.FeeType.Id };

                var paymentLogic = new PaymentLogic();

                payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.ConfirmationOrderNumber,session,feetype);

                if (payment == null || payment.Id <= 0)
                {
                    PaystackLogic paystackLogic = new PaystackLogic();
                    string PaystackSecrect = ConfigurationManager.AppSettings["PaystackSecrect"].ToString();
                    paystackLogic.VerifyPayment(new Payment() { InvoiceNumber = viewModel.ConfirmationOrderNumber }, PaystackSecrect);

                    payment = paystackLogic.ValidatePayment(viewModel.ConfirmationOrderNumber);
                    if (payment != null && payment.Id > 0)
                    {
                        decimal amount = GetAmountToBePaid(payment);

                        if (!paystackLogic.ValidateAmountPaid(viewModel.ConfirmationOrderNumber, amount))
                        {
                            SetMessage("Amount paid is not correct!", Message.Category.Error);
                            ViewBag.Sessions = logonViewModel.SessionSelectListItem;
                            return RedirectToAction("PrintReceipt");
                        }
                                
                    }
                    else
                    {

                        if (viewModel.ConfirmationOrderNumber.StartsWith("AB") && feetype.Id==81)
                        {
                            string MonnifyURL = ConfigurationManager.AppSettings["MonnifyUrl"].ToString();
                            string MonnifyUser = ConfigurationManager.AppSettings["MonnifyApiKey"].ToString();
                            string MonnifySecrect = ConfigurationManager.AppSettings["MonnifyContractCode"].ToString();
                            string MonnifyCode = ConfigurationManager.AppSettings["MonnifyCode"].ToString();
                            PaymentMonnifyLogic paymentMonnifyLogic = new PaymentMonnifyLogic(MonnifyURL, MonnifyUser, MonnifySecrect, MonnifyCode);
                            var PaymentMonnify = paymentMonnifyLogic.GetInvoiceStatus(viewModel.ConfirmationOrderNumber);
                            if (PaymentMonnify != null && PaymentMonnify.Completed && PaymentMonnify.Payment != null && PaymentMonnify.Amount > 2000 && (PaymentMonnify.Payment.FeeType.Id == 1 || PaymentMonnify.Payment.FeeType.Id == 6 || PaymentMonnify.Payment.FeeType.Id == 18 || PaymentMonnify.Payment.FeeType.Id == 81))
                            {
                                payment = PaymentMonnify.Payment;
                            }
                            else
                            {
                                SetMessage("Confirmation Order Number or Invoice Number entered was not found or isn't valid for this payment!",
                                Message.Category.Error);
                                return View(viewModel);
                            }
                        }
                        else
                        {
                            SetMessage("Confirmation Order Number or Invoice Number entered was not found or isn't valid for this payment!",
                            Message.Category.Error);
                            return View(viewModel);
                        }

                    }
                    //else
                    //{
                    //    SetMessage("Confirmation Order Number or Invoice Number entered was not found or isn't valid for this payment!", Message.Category.Error);
                    //    ViewBag.Sessions = logonViewModel.SessionSelectListItem;
                    //    return RedirectToAction("PrintReceipt");
                    //}

                }

                //if(payment == null)
                //{
                //    ViewBag.Sessions = logonViewModel.SessionSelectListItem;
                //    SetMessage("Confirmation Order Number (" + viewModel.ConfirmationOrderNumber +") entered is not for the payment! Please ensure that you entered the correct Confirmation Order Number.",Message.Category.Error);
                //    return RedirectToAction("PrintReceipt");
                //}
            }
            catch(Exception ex)
            {
                ViewBag.Sessions = logonViewModel.SessionSelectListItem;
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
                return RedirectToAction("PrintReceipt");
            }

            TempData["Payment"] = payment;
            Session["StudentData"] = Utility.Encrypt(payment.Person.Id.ToString());
            return RedirectToAction("Index","Registration",new { Area = "Student",sid = Utility.Encrypt(payment.Person.Id.ToString()), });
        }

        private decimal GetAmountToBePaid(Payment payment)
        {
            try
            {
                decimal amount = 0M;
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                PaymentLogic paymentLogic = new PaymentLogic();

                StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id).LastOrDefault();
                if (studentLevel == null)
                {
                    throw new Exception("Student level not found, contact system administrator.");
                }

                StudentPayment studentPayment = studentPaymentLogic.GetModelBy(p => p.Payment_Id == payment.Id);
                if (studentPayment == null)
                {
                    throw new Exception("Payment not found.");
                }

                payment.FeeDetails = paymentLogic.SetFeeDetails(payment, studentLevel.Programme.Id, studentPayment.Level.Id, payment.PaymentMode.Id, studentLevel.Department.Id, payment.Session.Id);

                amount = payment.FeeDetails.Sum(p => p.Fee.Amount);

                return amount;
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}