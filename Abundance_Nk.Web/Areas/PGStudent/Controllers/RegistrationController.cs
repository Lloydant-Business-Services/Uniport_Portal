using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.PGApplicant.ViewModels;
using Abundance_Nk.Web.Areas.PGStudent.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.PGStudent.Controllers
{
    public class RegistrationController : BaseController
    {
        private readonly string appRoot = ConfigurationManager.AppSettings["AppRoot"];
        private readonly PGRegistrationIndexViewModel indexViewModel;
        private readonly PGRegistrationLogonViewModel logonViewModel;

        private readonly PaymentLogic paymentLogic;
        private PGRegistrationViewModel viewModel;
        // GET: PGStudent/Registration

        public ActionResult Logon()
        {
            ViewBag.Sessions = logonViewModel.SessionSelectListItem;
            return View(logonViewModel);
        }

        [HttpPost]
        public ActionResult Logon(PGRegistrationLogonViewModel viewModel)
        {
            var payment = new Payment();

            try
            {
                var session = new Session { Id = viewModel.Session.Id };
                var feetype = new FeeType { Id = (int)FeeTypes.SchoolFees };
                var paymentLogic = new PaymentLogic();
                payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.ConfirmationOrderNumber, session,
                    feetype);
                if (payment == null)
                {
                    ViewBag.Sessions = logonViewModel.SessionSelectListItem;
                    SetMessage(
                        "Confirmation Order Number (" + viewModel.ConfirmationOrderNumber +
                        ") entered is not for School Fees payment! Please enter your School Fees Confirmation Order Number.",
                        Message.Category.Error);
                    return View(logonViewModel);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Sessions = logonViewModel.SessionSelectListItem;
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                return View(logonViewModel);
            }

            TempData["Payment"] = payment;
            Session["StudentData"] = Utility.Encrypt(payment.Person.Id.ToString());
            return RedirectToAction("Index", "Registration",
                new { Area = "PGStudent", sid = Utility.Encrypt(payment.Person.Id.ToString()), });
        }

        public ActionResult Form(string sid, string pid)
        {
            var existingViewModel = (PGRegistrationViewModel)TempData["RegistrationViewModel"];

            try
            {
                long studentId = Convert.ToInt64(Utility.Decrypt(sid));
                int programmeId = Convert.ToInt32(Utility.Decrypt(pid));

                PopulateAllDropDowns(programmeId);
                if (existingViewModel != null)
                {
                    viewModel = existingViewModel;
                    SetStudentUploadedPassport(viewModel);
                }

                viewModel.LoadStudentInformationFormBy(studentId);
                if (viewModel.Student != null && viewModel.Student.Id > 0)
                {
                    if (viewModel.Payment == null)
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
                    if (viewModel.StudentLevel.Programme.Id == 3 || viewModel.StudentLevel.Programme.Id == 4)
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
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            TempData["RegistrationViewModel"] = viewModel;
            TempData["imageUrl"] = viewModel.Person.ImageFileUrl;

            return View(viewModel);
        }
        private void PopulateAllDropDowns(int programmeId)
        {
            var existingViewModel = (PGRegistrationViewModel)TempData["RegistrationViewModel"];

            try
            {
                if (existingViewModel == null)
                {
                    viewModel = new PGRegistrationViewModel();

                    ViewBag.States = viewModel.StateSelectList;
                    ViewBag.Sexes = viewModel.SexSelectList;
                    ViewBag.FirstChoiceFaculties = viewModel.FacultySelectList;
                    ViewBag.SecondChoiceFaculties = viewModel.FacultySelectList;
                    ViewBag.Lgas = new SelectList(new List<LocalGovernment>(), Utility.ID, Utility.NAME);
                    ViewBag.Relationships = viewModel.RelationshipSelectList;
                    ViewBag.FirstSittingOLevelTypes = viewModel.OLevelTypeSelectList;
                    ViewBag.SecondSittingOLevelTypes = viewModel.OLevelTypeSelectList;
                    ViewBag.FirstSittingExamYears = viewModel.ExamYearSelectList;
                    ViewBag.SecondSittingExamYears = viewModel.ExamYearSelectList;
                    ViewBag.Religions = viewModel.ReligionSelectList;
                    ViewBag.Abilities = viewModel.AbilitySelectList;
                    ViewBag.DayOfBirths = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
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

                    if (viewModel.DepartmentSelectList != null)
                    {
                        ViewBag.Departments = viewModel.DepartmentSelectList;
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                    }

                    if (programmeId == 3 || programmeId == 4)
                    {
                        ViewBag.StudentNdResultDayAwardeds = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                        ViewBag.StudentNdResultMonthAwardeds = viewModel.StudentNdResultMonthAwardedSelectList;
                        ViewBag.StudentNdResultYearAwardeds = viewModel.StudentNdResultYearAwardedSelectList;

                        ViewBag.StudentLastEmploymentStartDays = new SelectList(new List<Value>(), Utility.ID,
                            Utility.NAME);
                        ViewBag.StudentLastEmploymentStartMonths = viewModel.StudentLastEmploymentStartMonthSelectList;
                        ViewBag.StudentLastEmploymentStartYears = viewModel.StudentLastEmploymentStartYearSelectList;

                        ViewBag.StudentLastEmploymentEndDays = new SelectList(new List<Value>(), Utility.ID,
                            Utility.NAME);
                        ViewBag.StudentLastEmploymentEndMonths = viewModel.StudentLastEmploymentEndMonthSelectList;
                        ViewBag.StudentLastEmploymentEndYears = viewModel.StudentLastEmploymentEndYearSelectList;

                        ViewBag.PreviousEducationStartDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                        ViewBag.PreviousEducationStartMonths = viewModel.PreviousEducationStartMonthSelectList;
                        ViewBag.PreviousEducationStartYears = viewModel.PreviousEducationStartYearSelectList;

                        ViewBag.PreviousEducationEndDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                        ViewBag.PreviousEducationEndMonths = viewModel.PreviousEducationEndMonthSelectList;
                        ViewBag.PreviousEducationEndYears = viewModel.PreviousEducationEndYearSelectList;

                        ViewBag.ResultGrades = viewModel.ResultGradeSelectList;
                    }

                    SetDefaultSelectedSittingSubjectAndGrade(viewModel);
                }
                else
                {
                    if (existingViewModel.Student.Title == null)
                    {
                        existingViewModel.Student.Title = new Title();
                    }
                    if (existingViewModel.Person.Sex == null)
                    {
                        existingViewModel.Person.Sex = new Sex();
                    }
                    if (existingViewModel.Student.MaritalStatus == null)
                    {
                        existingViewModel.Student.MaritalStatus = new MaritalStatus();
                    }
                    if (existingViewModel.Person.Religion == null)
                    {
                        existingViewModel.Person.Religion = new Religion();
                    }
                    if (existingViewModel.Person.State == null)
                    {
                        existingViewModel.Person.State = new State();
                    }
                    if (existingViewModel.StudentLevel.Programme == null)
                    {
                        existingViewModel.StudentLevel.Programme = new Programme();
                    }
                    if (existingViewModel.NextOfKin.Relationship == null)
                    {
                        existingViewModel.NextOfKin.Relationship = new Relationship();
                    }
                    if (existingViewModel.StudentSponsor.Relationship == null)
                    {
                        existingViewModel.StudentSponsor.Relationship = new Relationship();
                    }
                    if (existingViewModel.FirstSittingOLevelResult.Type == null)
                    {
                        existingViewModel.FirstSittingOLevelResult.Type = new OLevelType();
                    }
                    if (existingViewModel.Person.YearOfBirth == null)
                    {
                        existingViewModel.Person.YearOfBirth = new Value();
                    }
                    if (existingViewModel.Person.MonthOfBirth == null)
                    {
                        existingViewModel.Person.MonthOfBirth = new Value();
                    }
                    if (existingViewModel.Person.DayOfBirth == null)
                    {
                        existingViewModel.Person.DayOfBirth = new Value();
                    }
                    if (existingViewModel.StudentLevel.Department == null)
                    {
                        existingViewModel.StudentLevel.Department = new Department();
                    }
                    if (existingViewModel.Student.BloodGroup == null)
                    {
                        existingViewModel.Student.BloodGroup = new BloodGroup();
                    }
                    if (existingViewModel.Student.Genotype == null)
                    {
                        existingViewModel.Student.Genotype = new Genotype();
                    }
                    if (existingViewModel.StudentAcademicInformation.Level == null)
                    {
                        existingViewModel.StudentAcademicInformation.Level = new Level();
                    }
                    if (existingViewModel.StudentFinanceInformation.Mode == null)
                    {
                        existingViewModel.StudentFinanceInformation.Mode = new ModeOfFinance();
                    }

                    // PERSONAL INFORMATION
                    ViewBag.Titles = new SelectList(existingViewModel.TitleSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Student.Title.Id);
                    ViewBag.Sexes = new SelectList(existingViewModel.SexSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Person.Sex.Id);
                    ViewBag.MaritalStatuses = new SelectList(existingViewModel.MaritalStatusSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Student.MaritalStatus.Id);
                    SetDateOfBirthDropDown(existingViewModel);
                    ViewBag.Religions = new SelectList(existingViewModel.ReligionSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Person.Religion.Id);
                    ViewBag.States = new SelectList(existingViewModel.StateSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Person.State.Id);

                    if (existingViewModel.Person.LocalGovernment != null &&
                        existingViewModel.Person.LocalGovernment.Id > 0)
                    {
                        ViewBag.Lgas = new SelectList(existingViewModel.LocalGovernmentSelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.Person.LocalGovernment.Id);
                    }
                    else
                    {
                        ViewBag.Lgas = new SelectList(new List<LocalGovernment>(), Utility.VALUE, Utility.TEXT);
                    }
                    ViewBag.BloodGroups = new SelectList(existingViewModel.BloodGroupSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Student.BloodGroup.Id);
                    ViewBag.Genotypes = new SelectList(existingViewModel.GenotypeSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Student.Genotype.Id);

                    // ACADEMIC DETAILS
                    ViewBag.ModeOfEntries = new SelectList(existingViewModel.ModeOfEntrySelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentAcademicInformation.ModeOfEntry.Id);
                    ViewBag.ModeOfStudies = new SelectList(existingViewModel.ModeOfStudySelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentAcademicInformation.ModeOfStudy.Id);
                    ViewBag.Programmes = new SelectList(existingViewModel.ProgrammeSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentLevel.Programme.Id);
                    ViewBag.Faculties = new SelectList(existingViewModel.FacultySelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.StudentLevel.Department.Faculty.Id);

                    SetDepartmentIfExist(existingViewModel);
                    SetDepartmentOptionIfExist(existingViewModel);

                    ViewBag.StudentTypes = new SelectList(existingViewModel.StudentTypeSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Student.Type.Id);
                    ViewBag.StudentCategories = new SelectList(existingViewModel.StudentCategorySelectList,
                        Utility.VALUE, Utility.TEXT, existingViewModel.Student.Category.Id);
                    ViewBag.AdmissionYears = new SelectList(existingViewModel.AdmissionYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentAcademicInformation.YearOfAdmission);
                    ViewBag.GraduationYears = new SelectList(existingViewModel.GraduationYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentAcademicInformation.YearOfGraduation);

                    if (existingViewModel.StudentAcademicInformation.Level != null &&
                        existingViewModel.StudentAcademicInformation.Level.Id > 0)
                    {
                        ViewBag.Levels = new SelectList(existingViewModel.LevelSelectList, Utility.VALUE, Utility.TEXT,
                            existingViewModel.StudentAcademicInformation.Level.Id);
                    }
                    else if (existingViewModel.StudentLevel.Level != null && existingViewModel.StudentLevel.Level.Id > 0)
                    {
                        ViewBag.Levels = new SelectList(existingViewModel.LevelSelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.StudentLevel.Level.Id);
                    }

                    // FINANCE DETAILS
                    ViewBag.ModeOfFinances = new SelectList(existingViewModel.ModeOfFinanceSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentFinanceInformation.Mode.Id);

                    // NEXT OF KIN
                    ViewBag.Relationships = new SelectList(existingViewModel.RelationshipSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentSponsor.Relationship.Id);

                    //SPONSOR
                    ViewBag.Relationships = new SelectList(existingViewModel.RelationshipSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.NextOfKin.Relationship.Id);

                    ViewBag.FirstSittingOLevelTypes = new SelectList(existingViewModel.OLevelTypeSelectList,
                        Utility.VALUE, Utility.TEXT, existingViewModel.FirstSittingOLevelResult.Type.Id);
                    ViewBag.FirstSittingExamYears = new SelectList(existingViewModel.ExamYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.FirstSittingOLevelResult.ExamYear);
                    ViewBag.SecondSittingExamYears = new SelectList(existingViewModel.ExamYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.SecondSittingOLevelResult.ExamYear);

                    if (programmeId == 3 || programmeId == 4)
                    {
                        SetStudentNdResultDateAwardedDropDown(existingViewModel);
                        SetStudentLastEmploymentEndDateDropDown(existingViewModel);
                        SetStudentLastEmploymentStartDateDropDown(existingViewModel);
                        SetPreviousEducationEndDateDropDowns(existingViewModel);
                        SetPreviousEducationStartDateDropDowns(existingViewModel);

                        ViewBag.ResultGrades = new SelectList(existingViewModel.ResultGradeSelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.PreviousEducation.ResultGrade.Id);
                    }

                    if (existingViewModel.SecondSittingOLevelResult.Type != null)
                    {
                        ViewBag.SecondSittingOLevelTypes = new SelectList(existingViewModel.OLevelTypeSelectList,
                            Utility.VALUE, Utility.TEXT, existingViewModel.SecondSittingOLevelResult.Type.Id);
                    }
                    else
                    {
                        existingViewModel.SecondSittingOLevelResult.Type = new OLevelType { Id = 0 };
                        ViewBag.SecondSittingOLevelTypes = new SelectList(existingViewModel.OLevelTypeSelectList,
                            Utility.VALUE, Utility.TEXT, 0);
                    }

                    SetSelectedSittingSubjectAndGrade(existingViewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }
        private void SetStudentUploadedPassport(PGRegistrationViewModel viewModel)
        {
            if (viewModel != null && viewModel.Person != null && !string.IsNullOrEmpty((string)TempData["imageUrl"]))
            {
                viewModel.Person.ImageFileUrl = (string)TempData["imageUrl"];
            }
            else
            {
                viewModel.Person.ImageFileUrl = Utility.DEFAULT_AVATAR;
            }
        }
        private void SetStudentLastEmploymentStartDateDropDown(PGRegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.StudentLastEmploymentStartMonths =
                    new SelectList(existingViewModel.StudentLastEmploymentStartMonthSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentEmploymentInformation.StartMonth.Id);
                ViewBag.StudentLastEmploymentStartYears =
                    new SelectList(existingViewModel.StudentLastEmploymentStartYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentEmploymentInformation.StartYear.Id);

                if ((existingViewModel.StudentLastEmploymentStartDaySelectList == null ||
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
                    if (existingViewModel.StudentLastEmploymentStartDaySelectList != null &&
                        existingViewModel.StudentEmploymentInformation.StartDay.Id > 0)
                    {
                        ViewBag.StudentLastEmploymentStartDays =
                            new SelectList(existingViewModel.StudentLastEmploymentStartDaySelectList, Utility.VALUE,
                                Utility.TEXT, existingViewModel.StudentEmploymentInformation.StartDay.Id);
                    }
                    else if (existingViewModel.StudentLastEmploymentStartDaySelectList != null &&
                             existingViewModel.StudentEmploymentInformation.StartDay.Id <= 0)
                    {
                        ViewBag.StudentLastEmploymentStartDays =
                            existingViewModel.StudentLastEmploymentStartDaySelectList;
                    }
                    else if (existingViewModel.StudentLastEmploymentStartDaySelectList == null)
                    {
                        existingViewModel.StudentLastEmploymentStartDaySelectList = new List<SelectListItem>();
                        ViewBag.StudentLastEmploymentStartDays = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.StudentEmploymentInformation.StartDay != null &&
                    existingViewModel.StudentEmploymentInformation.StartDay.Id > 0)
                {
                    ViewBag.StudentLastEmploymentStartDays =
                        new SelectList(existingViewModel.StudentLastEmploymentStartDaySelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.StudentEmploymentInformation.StartDay.Id);
                }
                else
                {
                    ViewBag.StudentLastEmploymentStartDays = existingViewModel.StudentLastEmploymentStartDaySelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetStudentLastEmploymentEndDateDropDown(PGRegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.StudentLastEmploymentEndMonths =
                    new SelectList(existingViewModel.StudentLastEmploymentEndMonthSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentEmploymentInformation.EndMonth.Id);
                ViewBag.StudentLastEmploymentEndYears =
                    new SelectList(existingViewModel.StudentLastEmploymentEndYearSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.StudentEmploymentInformation.EndYear.Id);

                if ((existingViewModel.StudentLastEmploymentEndDaySelectList == null ||
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
                    if (existingViewModel.StudentLastEmploymentEndDaySelectList != null &&
                        existingViewModel.StudentEmploymentInformation.EndDay.Id > 0)
                    {
                        ViewBag.StudentLastEmploymentEndDays =
                            new SelectList(existingViewModel.StudentLastEmploymentEndDaySelectList, Utility.VALUE,
                                Utility.TEXT, existingViewModel.StudentEmploymentInformation.EndDay.Id);
                    }
                    else if (existingViewModel.StudentLastEmploymentEndDaySelectList != null &&
                             existingViewModel.StudentEmploymentInformation.EndDay.Id <= 0)
                    {
                        ViewBag.StudentLastEmploymentEndDays =
                            existingViewModel.StudentLastEmploymentEndDaySelectList;
                    }
                    else if (existingViewModel.StudentLastEmploymentEndDaySelectList == null)
                    {
                        existingViewModel.StudentLastEmploymentEndDaySelectList = new List<SelectListItem>();
                        ViewBag.StudentLastEmploymentEndDays = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.StudentEmploymentInformation.EndDay != null &&
                    existingViewModel.StudentEmploymentInformation.EndDay.Id > 0)
                {
                    ViewBag.StudentLastEmploymentEndDays =
                        new SelectList(existingViewModel.StudentLastEmploymentEndDaySelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.StudentEmploymentInformation.EndDay.Id);
                }
                else
                {
                    ViewBag.StudentLastEmploymentEndDays = existingViewModel.StudentLastEmploymentEndDaySelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetStudentNdResultDateAwardedDropDown(PGRegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.StudentNdResultMonthAwardeds =
                    new SelectList(existingViewModel.StudentNdResultMonthAwardedSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.StudentNdResult.MonthAwarded.Id);
                ViewBag.StudentNdResultYearAwardeds =
                    new SelectList(existingViewModel.StudentNdResultYearAwardedSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.StudentNdResult.YearAwarded.Id);
                if ((existingViewModel.StudentNdResultDayAwardedSelectList == null ||
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
                    if (existingViewModel.StudentNdResultDayAwardedSelectList != null &&
                        existingViewModel.StudentNdResult.DayAwarded.Id > 0)
                    {
                        ViewBag.StudentNdResultDayAwardeds =
                            new SelectList(existingViewModel.StudentNdResultDayAwardedSelectList, Utility.VALUE,
                                Utility.TEXT, existingViewModel.StudentNdResult.DayAwarded.Id);
                    }
                    else if (existingViewModel.StudentNdResultDayAwardedSelectList != null &&
                             existingViewModel.StudentNdResult.DayAwarded.Id <= 0)
                    {
                        ViewBag.StudentNdResultDayAwardeds = existingViewModel.StudentNdResultDayAwardedSelectList;
                    }
                    else if (existingViewModel.StudentNdResultDayAwardedSelectList == null)
                    {
                        existingViewModel.StudentNdResultDayAwardedSelectList = new List<SelectListItem>();
                        ViewBag.StudentNdResultDayAwardeds = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.StudentNdResult.DayAwarded != null &&
                    existingViewModel.StudentNdResult.DayAwarded.Id > 0)
                {
                    ViewBag.StudentNdResultDayAwardeds =
                        new SelectList(existingViewModel.StudentNdResultDayAwardedSelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.StudentNdResult.DayAwarded.Id);
                }
                else
                {
                    ViewBag.StudentNdResultDayAwardeds = existingViewModel.StudentNdResultDayAwardedSelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetDateOfBirthDropDown(PGRegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.MonthOfBirths = new SelectList(existingViewModel.MonthOfBirthSelectList, Utility.VALUE,
                    Utility.TEXT, existingViewModel.Person.MonthOfBirth.Id);
                ViewBag.YearOfBirths = new SelectList(existingViewModel.YearOfBirthSelectList, Utility.VALUE,
                    Utility.TEXT, existingViewModel.Person.YearOfBirth.Id);
                if ((existingViewModel.DayOfBirthSelectList == null || existingViewModel.DayOfBirthSelectList.Count == 0) &&
                    (existingViewModel.Person.MonthOfBirth.Id > 0 && existingViewModel.Person.YearOfBirth.Id > 0))
                {
                    existingViewModel.DayOfBirthSelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.Person.MonthOfBirth,
                            existingViewModel.Person.YearOfBirth);
                }
                else
                {
                    if (existingViewModel.DayOfBirthSelectList != null && existingViewModel.Person.DayOfBirth.Id > 0)
                    {
                        ViewBag.DayOfBirths = new SelectList(existingViewModel.DayOfBirthSelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.Person.DayOfBirth.Id);
                    }
                    else if (existingViewModel.DayOfBirthSelectList != null &&
                             existingViewModel.Person.DayOfBirth.Id <= 0)
                    {
                        ViewBag.DayOfBirths = existingViewModel.DayOfBirthSelectList;
                    }
                    else if (existingViewModel.DayOfBirthSelectList == null)
                    {
                        existingViewModel.DayOfBirthSelectList = new List<SelectListItem>();
                        ViewBag.DayOfBirths = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.Person.DayOfBirth != null && existingViewModel.Person.DayOfBirth.Id > 0)
                {
                    ViewBag.DayOfBirths = new SelectList(existingViewModel.DayOfBirthSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Person.DayOfBirth.Id);
                }
                else
                {
                    ViewBag.DayOfBirths = existingViewModel.DayOfBirthSelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetPreviousEducationStartDateDropDowns(PGRegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.PreviousEducationStartMonths =
                    new SelectList(existingViewModel.PreviousEducationStartMonthSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.PreviousEducation.StartMonth.Id);
                ViewBag.PreviousEducationStartYears =
                    new SelectList(existingViewModel.PreviousEducationStartYearSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.PreviousEducation.StartYear.Id);
                if ((existingViewModel.PreviousEducationStartDaySelectList == null ||
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
                    if (existingViewModel.PreviousEducationStartDaySelectList != null &&
                        existingViewModel.PreviousEducation.StartDay.Id > 0)
                    {
                        ViewBag.PreviousEducationStartDays =
                            new SelectList(existingViewModel.PreviousEducationStartDaySelectList, Utility.VALUE,
                                Utility.TEXT, existingViewModel.PreviousEducation.StartDay.Id);
                    }
                    else if (existingViewModel.PreviousEducationStartDaySelectList != null &&
                             existingViewModel.PreviousEducation.StartDay.Id <= 0)
                    {
                        ViewBag.PreviousEducationStartDays = existingViewModel.PreviousEducationStartDaySelectList;
                    }
                    else if (existingViewModel.PreviousEducationStartDaySelectList == null)
                    {
                        existingViewModel.PreviousEducationStartDaySelectList = new List<SelectListItem>();
                        ViewBag.PreviousEducationStartDays = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.PreviousEducation.StartDay != null &&
                    existingViewModel.PreviousEducation.StartDay.Id > 0)
                {
                    ViewBag.PreviousEducationStartDays =
                        new SelectList(existingViewModel.PreviousEducationStartDaySelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.PreviousEducation.StartDay.Id);
                }
                else
                {
                    ViewBag.PreviousEducationStartDays = existingViewModel.PreviousEducationStartDaySelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetPreviousEducationEndDateDropDowns(PGRegistrationViewModel existingViewModel)
        {
            try
            {
                ViewBag.PreviousEducationEndMonths =
                    new SelectList(existingViewModel.PreviousEducationEndMonthSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.PreviousEducation.EndMonth.Id);
                ViewBag.PreviousEducationEndYears = new SelectList(
                    existingViewModel.PreviousEducationEndYearSelectList, Utility.VALUE, Utility.TEXT,
                    existingViewModel.PreviousEducation.EndYear.Id);
                if ((existingViewModel.PreviousEducationEndDaySelectList == null ||
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
                    if (existingViewModel.PreviousEducationEndDaySelectList != null &&
                        existingViewModel.PreviousEducation.EndDay.Id > 0)
                    {
                        ViewBag.PreviousEducationEndDays =
                            new SelectList(existingViewModel.PreviousEducationEndDaySelectList, Utility.VALUE,
                                Utility.TEXT, existingViewModel.PreviousEducation.EndDay.Id);
                    }
                    else if (existingViewModel.PreviousEducationEndDaySelectList != null &&
                             existingViewModel.PreviousEducation.EndDay.Id <= 0)
                    {
                        ViewBag.PreviousEducationEndDays = existingViewModel.PreviousEducationEndDaySelectList;
                    }
                    else if (existingViewModel.PreviousEducationEndDaySelectList == null)
                    {
                        existingViewModel.PreviousEducationEndDaySelectList = new List<SelectListItem>();
                        ViewBag.PreviousEducationEndDays = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.PreviousEducation.EndDay != null &&
                    existingViewModel.PreviousEducation.EndDay.Id > 0)
                {
                    ViewBag.PreviousEducationEndDays =
                        new SelectList(existingViewModel.PreviousEducationEndDaySelectList, Utility.VALUE, Utility.TEXT,
                            existingViewModel.PreviousEducation.EndDay.Id);
                }
                else
                {
                    ViewBag.PreviousEducationEndDays = existingViewModel.PreviousEducationEndDaySelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetDefaultSelectedSittingSubjectAndGrade(PGRegistrationViewModel viewModel)
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

                if (viewModel != null && viewModel.SecondSittingOLevelResultDetails != null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        ViewData["SecondSittingOLevelSubjectId" + i] = viewModel.OLevelSubjectSelectList;
                        ViewData["SecondSittingOLevelGradeId" + i] = viewModel.OLevelGradeSelectList;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetLgaIfExist(PGRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.Person.State != null && !string.IsNullOrEmpty(viewModel.Person.State.Id))
                {
                    var localGovernmentLogic = new LocalGovernmentLogic();
                    List<LocalGovernment> lgas =
                        localGovernmentLogic.GetModelsBy(l => l.State_Id == viewModel.Person.State.Id);
                    if (viewModel.Person.LocalGovernment != null && viewModel.Person.LocalGovernment.Id > 0)
                    {
                        ViewBag.Lgas = new SelectList(lgas, Utility.ID, Utility.NAME,
                            viewModel.Person.LocalGovernment.Id);
                    }
                    else
                    {
                        ViewBag.Lgas = new SelectList(lgas, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.Lgas = new SelectList(new List<LocalGovernment>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetDepartmentIfExist(PGRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentLevel.Programme != null && viewModel.StudentLevel.Programme.Id > 0)
                {
                    var departmentLogic = new ProgrammeDepartmentLogic();
                    List<Department> departments = departmentLogic.GetBy(viewModel.StudentLevel.Programme);
                    if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                    {
                        ViewBag.Departments = new SelectList(departments, Utility.ID, Utility.NAME,
                            viewModel.StudentLevel.Department.Id);
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(departments, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetDepartmentOptionIfExist(PGRegistrationViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                {
                    var departmentOptionLogic = new DepartmentOptionLogic();
                    List<DepartmentOption> departmentOptions =
                        departmentOptionLogic.GetModelsBy(l => l.Department_Id == viewModel.StudentLevel.Department.Id);
                    if (viewModel.StudentLevel.DepartmentOption != null &&
                        viewModel.StudentLevel.DepartmentOption.Id > 0)
                    {
                        ViewBag.DepartmentOptions = new SelectList(departmentOptions, Utility.ID, Utility.NAME,
                            viewModel.StudentLevel.DepartmentOption.Id);
                    }
                    else
                    {
                        var options = new List<DepartmentOption>();
                        var option = new DepartmentOption { Id = 0, Name = viewModel.StudentLevel.Department.Name };
                        options.Add(option);

                        ViewBag.DepartmentOptions = new SelectList(options, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetPreviousEducationStartDate()
        {
            try
            {
                if (viewModel.PreviousEducation != null && viewModel.PreviousEducation.StartDate != DateTime.MinValue)
                {
                    int noOfDays = DateTime.DaysInMonth(viewModel.PreviousEducation.StartYear.Id,
                        viewModel.PreviousEducation.StartMonth.Id);
                    List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                    if (days != null && days.Count > 0)
                    {
                        days.Insert(0, new Value { Name = "--DD--" });
                    }

                    if (viewModel.PreviousEducation.StartDay != null && viewModel.PreviousEducation.StartDay.Id > 0)
                    {
                        ViewBag.PreviousEducationStartDays = new SelectList(days, Utility.ID, Utility.NAME,
                            viewModel.PreviousEducation.StartDay.Id);
                    }
                    else
                    {
                        ViewBag.PreviousEducationStartDays = new SelectList(days, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.PreviousEducationStartDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetPreviousEducationEndDate()
        {
            try
            {
                if (viewModel.PreviousEducation != null && viewModel.PreviousEducation.EndDate != DateTime.MinValue)
                {
                    int noOfDays = DateTime.DaysInMonth(viewModel.PreviousEducation.EndYear.Id,
                        viewModel.PreviousEducation.EndMonth.Id);
                    List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                    if (days != null && days.Count > 0)
                    {
                        days.Insert(0, new Value { Name = "--DD--" });
                    }

                    if (viewModel.PreviousEducation.EndDay != null && viewModel.PreviousEducation.EndDay.Id > 0)
                    {
                        ViewBag.PreviousEducationEndDays = new SelectList(days, Utility.ID, Utility.NAME,
                            viewModel.PreviousEducation.EndDay.Id);
                    }
                    else
                    {
                        ViewBag.PreviousEducationEndDays = new SelectList(days, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.PreviousEducationEndDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetDateOfBirth()
        {
            try
            {
                if (viewModel.Person.DateOfBirth.HasValue)
                {
                    if (viewModel.Person.YearOfBirth.Id > 0 && viewModel.Person.MonthOfBirth.Id > 0)
                    {
                        int noOfDays = DateTime.DaysInMonth(viewModel.Person.YearOfBirth.Id,
                            viewModel.Person.MonthOfBirth.Id);
                        List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                        if (days != null && days.Count > 0)
                        {
                            days.Insert(0, new Value { Name = "--DD--" });
                        }

                        if (viewModel.Person.DayOfBirth != null && viewModel.Person.DayOfBirth.Id > 0)
                        {
                            ViewBag.DayOfBirths = new SelectList(days, Utility.ID, Utility.NAME,
                                viewModel.Person.DayOfBirth.Id);
                        }
                        else
                        {
                            ViewBag.DayOfBirths = new SelectList(days, Utility.ID, Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.DayOfBirths = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetSelectedSittingSubjectAndGrade(PGRegistrationViewModel existingViewModel)
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
                                new SelectList(existingViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT,
                                    firstSittingOLevelResultDetail.Subject.Id);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT,
                                    firstSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            firstSittingOLevelResultDetail.Subject = new OLevelSubject { Id = 0 };
                            firstSittingOLevelResultDetail.Grade = new OLevelGrade { Id = 0 };

                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, 0);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, 0);
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
                                new SelectList(existingViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT,
                                    secondSittingOLevelResultDetail.Subject.Id);
                            ViewData["SecondSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT,
                                    secondSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            secondSittingOLevelResultDetail.Subject = new OLevelSubject { Id = 0 };
                            secondSittingOLevelResultDetail.Grade = new OLevelGrade { Id = 0 };

                            ViewData["SecondSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, Utility.VALUE, Utility.TEXT, 0);
                            ViewData["SecondSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, Utility.VALUE, Utility.TEXT, 0);
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
        private void SetEntryAndStudyMode(PGRegistrationViewModel vModel)
        {
            try
            {
                //set mode of entry

                switch (vModel.StudentLevel.Programme.Id)
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
                switch (vModel.StudentLevel.Programme.Id)
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
            catch (Exception)
            {
                throw;
            }
        }
        private void SetLastEmploymentEndDate()
        {
            try
            {
                //if (viewModel.StudentEmploymentInformation != null && viewModel.StudentEmploymentInformation.EndDate != null)
                if (viewModel.StudentEmploymentInformation != null &&
                    viewModel.StudentEmploymentInformation.EndDate != DateTime.MinValue)
                {
                    if (viewModel.StudentEmploymentInformation.EndYear.Id > 0 &&
                        viewModel.StudentEmploymentInformation.EndMonth.Id > 0)
                    {
                        int noOfDays = DateTime.DaysInMonth(viewModel.StudentEmploymentInformation.EndYear.Id,
                            viewModel.StudentEmploymentInformation.EndMonth.Id);
                        List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                        if (days != null && days.Count > 0)
                        {
                            days.Insert(0, new Value { Name = "--DD--" });
                        }

                        if (viewModel.StudentEmploymentInformation.EndDay != null &&
                            viewModel.StudentEmploymentInformation.EndDay.Id > 0)
                        {
                            ViewBag.StudentLastEmploymentEndDays = new SelectList(days, Utility.ID, Utility.NAME,
                                viewModel.StudentEmploymentInformation.EndDay.Id);
                        }
                        else
                        {
                            ViewBag.StudentLastEmploymentEndDays = new SelectList(days, Utility.ID, Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.StudentLastEmploymentEndDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }
        private void SetLastEmploymentStartDate()
        {
            try
            {
                //if (viewModel.StudentEmploymentInformation != null && viewModel.StudentEmploymentInformation.StartDate != null)
                if (viewModel.StudentEmploymentInformation != null &&
                    viewModel.StudentEmploymentInformation.StartDate != DateTime.MinValue)
                {
                    if (viewModel.StudentEmploymentInformation.StartYear.Id > 0 &&
                        viewModel.StudentEmploymentInformation.StartMonth.Id > 0)
                    {
                        int noOfDays = DateTime.DaysInMonth(viewModel.StudentEmploymentInformation.StartYear.Id,
                            viewModel.StudentEmploymentInformation.StartMonth.Id);
                        List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                        if (days != null && days.Count > 0)
                        {
                            days.Insert(0, new Value { Name = "--DD--" });
                        }

                        if (viewModel.StudentEmploymentInformation.StartDay != null &&
                            viewModel.StudentEmploymentInformation.StartDay.Id > 0)
                        {
                            ViewBag.StudentLastEmploymentStartDays = new SelectList(days, Utility.ID, Utility.NAME,
                                viewModel.StudentEmploymentInformation.StartDay.Id);
                        }
                        else
                        {
                            ViewBag.StudentLastEmploymentStartDays = new SelectList(days, Utility.ID, Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.StudentLastEmploymentStartDays = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }
        private void SetStudentAcademicInformationLevel()
        {
            try
            {
                if (viewModel.StudentAcademicInformation.Level == null ||
                    viewModel.StudentAcademicInformation.Level.Id <= 0)
                {
                    if (viewModel.StudentLevel.Level != null && viewModel.StudentLevel.Level.Id > 0)
                    {
                        viewModel.StudentAcademicInformation.Level = viewModel.StudentLevel.Level;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetNdResultDateAwarded()
        {
            try
            {
                //if (viewModel.StudentNdResult != null && viewModel.StudentNdResult.DateAwarded != null)
                if (viewModel.StudentNdResult != null && viewModel.StudentNdResult.DateAwarded != DateTime.MinValue)
                {
                    if (viewModel.StudentNdResult.YearAwarded.Id > 0 && viewModel.StudentNdResult.MonthAwarded.Id > 0)
                    {
                        int noOfDays = DateTime.DaysInMonth(viewModel.StudentNdResult.YearAwarded.Id,
                            viewModel.StudentNdResult.MonthAwarded.Id);
                        List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                        if (days != null && days.Count > 0)
                        {
                            days.Insert(0, new Value { Name = "--DD--" });
                        }

                        if (viewModel.StudentNdResult.DayAwarded != null && viewModel.StudentNdResult.DayAwarded.Id > 0)
                        {
                            ViewBag.StudentNdResultDayAwardeds = new SelectList(days, Utility.ID, Utility.NAME,
                                viewModel.StudentNdResult.DayAwarded.Id);
                        }
                        else
                        {
                            ViewBag.StudentNdResultDayAwardeds = new SelectList(days, Utility.ID, Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.StudentNdResultDayAwardeds = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }
        public ActionResult CourseRegistration(string sid)
        {
            Int64 studentId = Convert.ToInt64(Utility.Decrypt(sid));
            if (studentId > 0)
            {
                var studentLogic = new StudentLogic();
                var studentLevelLogic = new StudentLevelLogic();
                var viewModel = new PGSemesterViewModel();
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
    }
}