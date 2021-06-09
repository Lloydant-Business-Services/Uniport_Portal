using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.PGStudent.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.PGStudent.Controllers
{
    [AllowAnonymous]
    public class InformationController : BaseController
    {
        // GET: PGStudent/Information
        private const string DEFAULT_PASSPORT = "/Content/Images/default_avatar.png";
        private readonly string appRoot = ConfigurationManager.AppSettings["AppRoot"];

        private PGFormViewModel viewModel;

        public InformationController()
        {
            viewModel = new PGFormViewModel();
        }

        public ActionResult Form(long? fid, int? pid)
        {
            var existingViewModel = (PGFormViewModel)TempData["FormViewModel"];

            try
            {
                PopulateAllDropDowns((int)pid);

                if (existingViewModel != null)
                {
                    viewModel = existingViewModel;
                    SetStudentUploadedPassport(viewModel);
                }
                SetDateOfBirth();
                SetLgaIfExist(viewModel);

                ApplicationForm applicationform = viewModel.GetApplicationFormBy((long)fid);
                if ((applicationform != null && applicationform.Id > 0) && viewModel.ApplicationAlreadyExist == false)
                {
                    viewModel.ApplicationAlreadyExist = true;
                    viewModel.LoadApplicantionFormBy((long)fid);

                    SetSelectedSittingSubjectAndGrade(viewModel);
                    SetLgaIfExist(viewModel);
                    SetDepartmentIfExist(viewModel);
                    SetDepartmentOptionIfExist(viewModel);

                    SetDateOfBirth();

                    SetLevel(viewModel);
                    SetEntryAndStudyMode(viewModel);

                    viewModel.Student.Type = new StudentType { Id = (int)StudentType.EnumName.New };
                    if (viewModel.AppliedCourse.Programme.Id == 2 || viewModel.AppliedCourse.Programme.Id == 5 || viewModel.AppliedCourse.Programme.Id == 6 || viewModel.AppliedCourse.Programme.Id == 8 || viewModel.AppliedCourse.Programme.Id == 9 || viewModel.AppliedCourse.Programme.Id == 10)
                    {
                        SetPreviousEducationStartDate();
                        SetPreviousEducationEndDate();
                        viewModel.Student.Category = new StudentCategory { Id = 2 };
                    }
                    else
                    {
                        viewModel.Student.Category = new StudentCategory { Id = 1 };
                    }

                    Model.Model.Student student = viewModel.GetStudentBy(applicationform.Person.Id);
                    if (student != null && student.Id > 0)
                    {
                        viewModel.StudentAlreadyExist = true;
                        viewModel.LoadStudentInformationFormBy(applicationform.Person.Id);
                        SetLastEmploymentStartDate();
                        SetLastEmploymentEndDate();
                        SetNdResultDateAwarded();
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            TempData["FormViewModel"] = viewModel;
            TempData["imageUrl"] = viewModel.Person.ImageFileUrl;

            return View(viewModel);
        }
        private void PopulateAllDropDowns(int programmeId)
        {
            var existingViewModel = (PGFormViewModel)TempData["FormViewModel"];

            try
            {
                if (existingViewModel == null)
                {
                    viewModel = new PGFormViewModel();

                    ViewBag.StateId = viewModel.StateSelectList;
                    ViewBag.SexId = viewModel.SexSelectList;
                    ViewBag.FirstChoiceFacultyId = viewModel.FacultySelectList;
                    ViewBag.SecondChoiceFacultyId = viewModel.FacultySelectList;
                    ViewBag.LgaId = new SelectList(new List<LocalGovernment>(), Utility.ID, Utility.NAME);
                    ViewBag.RelationshipId = viewModel.RelationshipSelectList;
                    ViewBag.FirstSittingOLevelTypeId = viewModel.OLevelTypeSelectList;
                    ViewBag.SecondSittingOLevelTypeId = viewModel.OLevelTypeSelectList;
                    ViewBag.FirstSittingExamYearId = viewModel.ExamYearSelectList;
                    ViewBag.SecondSittingExamYearId = viewModel.ExamYearSelectList;
                    ViewBag.ReligionId = viewModel.ReligionSelectList;
                    ViewBag.AbilityId = viewModel.AbilitySelectList;
                    ViewBag.DayOfBirthId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                    ViewBag.MonthOfBirthId = viewModel.MonthOfBirthSelectList;
                    ViewBag.YearOfBirthId = viewModel.YearOfBirthSelectList;
                    ViewBag.TitleId = viewModel.TitleSelectList;
                    ViewBag.MaritalStatusId = viewModel.MaritalStatusSelectList;
                    ViewBag.BloodGroupId = viewModel.BloodGroupSelectList;
                    ViewBag.GenotypeId = viewModel.GenotypeSelectList;
                    ViewBag.ModeOfEntryId = viewModel.ModeOfEntrySelectList;
                    ViewBag.ModeOfStudyId = viewModel.ModeOfStudySelectList;

                    //ViewBag.StudentTypeId = viewModel.StudentTypeSelectList;
                    //ViewBag.StudentStatusId = viewModel.StudentStatusSelectList;

                    ViewBag.StudentCategoryId = viewModel.StudentCategorySelectList;
                    ViewBag.StudentTypeId = viewModel.StudentTypeSelectList;

                    ViewBag.LevelId = viewModel.LevelSelectList;
                    ViewBag.ModeOfFinanceId = viewModel.ModeOfFinanceSelectList;
                    ViewBag.RelationshipId = viewModel.RelationshipSelectList;
                    ViewBag.FacultyId = viewModel.FacultySelectList;
                    ViewBag.AdmissionYearId = viewModel.AdmissionYearSelectList;
                    ViewBag.GraduationYearId = viewModel.GraduationYearSelectList;
                    ViewBag.ProgrammeId = viewModel.ProgrammeSelectList;

                    if (viewModel.DepartmentSelectList != null)
                    {
                        ViewBag.DepartmentId = viewModel.DepartmentSelectList;
                    }
                    else
                    {
                        ViewBag.DepartmentId = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
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
                    if (existingViewModel.AppliedCourse.Programme == null)
                    {
                        existingViewModel.AppliedCourse.Programme = new Programme();
                    }
                    if (existingViewModel.Sponsor.Relationship == null)
                    {
                        existingViewModel.Sponsor.Relationship = new Relationship();
                    }
                    if (existingViewModel.FirstSittingOLevelResult.Type == null)
                    {
                        existingViewModel.FirstSittingOLevelResult.Type = new OLevelType();
                    }
                    if (existingViewModel.Applicant.Ability == null)
                    {
                        existingViewModel.Applicant.Ability = new Ability();
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
                    if (existingViewModel.AppliedCourse.Department == null)
                    {
                        existingViewModel.AppliedCourse.Department = new Department();
                    }
                    if (existingViewModel.Student.BloodGroup == null)
                    {
                        existingViewModel.Student.BloodGroup = new BloodGroup();
                    }
                    if (existingViewModel.Student.Genotype == null)
                    {
                        existingViewModel.Student.Genotype = new Genotype();
                    }

                    // PERSONAL INFORMATION
                    ViewBag.TitleId = new SelectList(existingViewModel.TitleSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Student.Title.Id);
                    ViewBag.SexId = new SelectList(existingViewModel.SexSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Person.Sex.Id);
                    ViewBag.MaritalStatusId = new SelectList(existingViewModel.MaritalStatusSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Student.MaritalStatus.Id);
                    SetDateOfBirthDropDown(existingViewModel);
                    ViewBag.ReligionId = new SelectList(existingViewModel.ReligionSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Person.Religion.Id);
                    ViewBag.StateId = new SelectList(existingViewModel.StateSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Person.State.Id);

                    if (existingViewModel.Person.LocalGovernment != null &&
                        existingViewModel.Person.LocalGovernment.Id > 0)
                    {
                        ViewBag.LgaId = new SelectList(existingViewModel.LocalGovernmentSelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.Person.LocalGovernment.Id);
                    }
                    else
                    {
                        ViewBag.LgaId = new SelectList(new List<LocalGovernment>(), Utility.VALUE, Utility.TEXT);
                    }
                    ViewBag.BloodGroupId = new SelectList(existingViewModel.BloodGroupSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Student.BloodGroup.Id);
                    ViewBag.GenotypeId = new SelectList(existingViewModel.GenotypeSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Student.Genotype.Id);

                    // ACADEMIC DETAILS
                    ViewBag.ModeOfEntryId = new SelectList(existingViewModel.ModeOfEntrySelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentAcademicInformation.ModeOfEntry.Id);
                    ViewBag.ModeOfStudyId = new SelectList(existingViewModel.ModeOfStudySelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentAcademicInformation.ModeOfStudy.Id);
                    ViewBag.ProgrammeId = new SelectList(existingViewModel.ProgrammeSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.AppliedCourse.Programme.Id);
                    ViewBag.FacultyId = new SelectList(existingViewModel.FacultySelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.AppliedCourse.Department.Faculty.Id);
                    //ViewBag.ProgrammeId = new SelectList(existingViewModel.FacultySelectList, VALUE, TEXT, existingViewModel.AppliedCourse.Programme.Id);

                    SetDepartmentIfExist(existingViewModel);
                    SetDepartmentOptionIfExist(existingViewModel);

                    ViewBag.StudentTypeId = new SelectList(viewModel.StudentTypeSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.Student.Type.Id);
                    ViewBag.StudentCategoryId = new SelectList(viewModel.StudentCategorySelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Student.Category.Id);
                    ViewBag.AdmissionYearId = new SelectList(existingViewModel.AdmissionYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentAcademicInformation.YearOfAdmission);
                    ViewBag.GraduationYearId = new SelectList(existingViewModel.GraduationYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentAcademicInformation.YearOfGraduation);
                    ViewBag.LevelId = new SelectList(existingViewModel.LevelSelectList, Utility.VALUE, Utility.TEXT,
                        existingViewModel.StudentAcademicInformation.Level.Id);

                    // FINANCE DETAILS
                    ViewBag.ModeOfFinanceId = new SelectList(existingViewModel.ModeOfFinanceSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentFinanceInformation.Mode.Id);

                    // NEXT OF KIN
                    ViewBag.RelationshipId = new SelectList(existingViewModel.RelationshipSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.StudentSponsor.Relationship.Id);

                    //SPONSOR
                    ViewBag.RelationshipId = new SelectList(existingViewModel.RelationshipSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Sponsor.Relationship.Id);

                    ViewBag.FirstSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList,
                        Utility.VALUE, Utility.TEXT, existingViewModel.FirstSittingOLevelResult.Type.Id);
                    ViewBag.FirstSittingExamYearId = new SelectList(existingViewModel.ExamYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.FirstSittingOLevelResult.ExamYear);
                    ViewBag.SecondSittingExamYearId = new SelectList(existingViewModel.ExamYearSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.SecondSittingOLevelResult.ExamYear);
                    //ViewBag.AbilityId = new SelectList(existingViewModel.AbilitySelectList, VALUE, TEXT, existingViewModel.Applicant.Ability.Id);

                    if (existingViewModel.SecondSittingOLevelResult.Type != null)
                    {
                        ViewBag.SecondSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList,
                            Utility.VALUE, Utility.TEXT, existingViewModel.SecondSittingOLevelResult.Type.Id);
                    }
                    else
                    {
                        existingViewModel.SecondSittingOLevelResult.Type = new OLevelType { Id = 0 };
                        ViewBag.SecondSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList,
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
        private void SetDefaultSelectedSittingSubjectAndGrade(PGFormViewModel viewModel)
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
        private void SetDateOfBirthDropDown(PGFormViewModel existingViewModel)
        {
            try
            {
                ViewBag.MonthOfBirthId = new SelectList(existingViewModel.MonthOfBirthSelectList, Utility.VALUE,
                    Utility.TEXT, existingViewModel.Person.MonthOfBirth.Id);
                ViewBag.YearOfBirthId = new SelectList(existingViewModel.YearOfBirthSelectList, Utility.VALUE,
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
                        ViewBag.DayOfBirthId = new SelectList(existingViewModel.DayOfBirthSelectList, Utility.VALUE,
                            Utility.TEXT, existingViewModel.Person.DayOfBirth.Id);
                    }
                    else if (existingViewModel.DayOfBirthSelectList != null &&
                             existingViewModel.Person.DayOfBirth.Id <= 0)
                    {
                        ViewBag.DayOfBirthId = existingViewModel.DayOfBirthSelectList;
                    }
                    else if (existingViewModel.DayOfBirthSelectList == null)
                    {
                        existingViewModel.DayOfBirthSelectList = new List<SelectListItem>();
                        ViewBag.DayOfBirthId = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.Person.DayOfBirth != null && existingViewModel.Person.DayOfBirth.Id > 0)
                {
                    ViewBag.DayOfBirthId = new SelectList(existingViewModel.DayOfBirthSelectList, Utility.VALUE,
                        Utility.TEXT, existingViewModel.Person.DayOfBirth.Id);
                }
                else
                {
                    ViewBag.DayOfBirthId = existingViewModel.DayOfBirthSelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void SetDepartmentIfExist(PGFormViewModel viewModel)
        {
            try
            {
                if (viewModel.AppliedCourse.Programme != null && viewModel.AppliedCourse.Programme.Id > 0)
                {
                    var applicationFormLogic = new ApplicationFormLogic();
                    var departmentLogic = new ProgrammeDepartmentLogic();
                    var admissionList = new AdmissionList(); //Ugo 2/4/2016
                    var admissionListLogic = new AdmissionListLogic();
                    if (viewModel.AppliedCourse.ApplicationForm != null)
                    {
                        admissionList =
                            admissionListLogic.GetModelBy(
                                p => p.Application_Form_Id == viewModel.AppliedCourse.ApplicationForm.Id);
                    }
                    else
                    {
                        var applicationForm = new ApplicationForm();
                        applicationForm =
                            applicationFormLogic.GetModelsBy(p => p.Person_Id == viewModel.Person.Id).FirstOrDefault();
                        admissionList = admissionListLogic.GetModelBy(p => p.Application_Form_Id == applicationForm.Id);
                    }

                    List<Department> departments = departmentLogic.GetBy(viewModel.AppliedCourse.Programme);
                    if (viewModel.AppliedCourse.Department != null && viewModel.AppliedCourse.Department.Id > 0)
                    {
                        //ViewBag.DepartmentId = new SelectList(departments, Utility.ID, Utility.NAME, viewModel.AppliedCourse.Department.Id);
                        ViewBag.DepartmentId = new SelectList(departments, Utility.ID, Utility.NAME,
                            admissionList.Deprtment.Id);
                        viewModel.AdmissionList = admissionList;
                    }
                    else
                    {
                        ViewBag.DepartmentId = new SelectList(departments, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.DepartmentId = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetDepartmentOptionIfExist(PGFormViewModel viewModel)
        {
            try
            {
                if (viewModel.AppliedCourse.Department != null && viewModel.AppliedCourse.Department.Id > 0)
                {
                    var applicationFormLogic = new ApplicationFormLogic();
                    var departmentOptionLogic = new DepartmentOptionLogic();
                    var admissionList = new AdmissionList(); //Ugo 2/4/2016
                    var admissionListLogic = new AdmissionListLogic();
                    if (viewModel.AppliedCourse.ApplicationForm != null)
                    {
                        admissionList =
                            admissionListLogic.GetModelBy(
                                p => p.Application_Form_Id == viewModel.AppliedCourse.ApplicationForm.Id);
                    }
                    else
                    {
                        var applicationForm = new ApplicationForm();
                        applicationForm =
                            applicationFormLogic.GetModelsBy(p => p.Person_Id == viewModel.Person.Id).FirstOrDefault();
                        admissionList = admissionListLogic.GetModelBy(p => p.Application_Form_Id == applicationForm.Id);
                    }

                    List<DepartmentOption> departmentOptions =
                        departmentOptionLogic.GetModelsBy(l => l.Department_Id == viewModel.AppliedCourse.Department.Id);
                    if (viewModel.AppliedCourse.Option != null && viewModel.AppliedCourse.Option.Id > 0)
                    {
                        ViewBag.DepartmentOptionId = new SelectList(departmentOptions, Utility.ID, Utility.NAME,
                            viewModel.AppliedCourse.Option.Id);
                    }
                    else
                    {
                        var options = new List<DepartmentOption>();
                        //DepartmentOption option = new DepartmentOption() { Id = 0, Name = viewModel.AppliedCourse.Department.Name };
                        var option = new DepartmentOption { Id = 0, Name = admissionList.Deprtment.Name };
                        options.Add(option);

                        ViewBag.DepartmentOptionId = new SelectList(options, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }
        private void SetSelectedSittingSubjectAndGrade(PGFormViewModel existingViewModel)
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
        private void SetStudentUploadedPassport(PGFormViewModel viewModel)
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
                            ViewBag.DayOfBirthId = new SelectList(days, Utility.ID, Utility.NAME,
                                viewModel.Person.DayOfBirth.Id);
                        }
                        else
                        {
                            ViewBag.DayOfBirthId = new SelectList(days, Utility.ID, Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.DayOfBirthId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }
        private void SetLgaIfExist(PGFormViewModel viewModel)
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
                        ViewBag.LgaId = new SelectList(lgas, Utility.ID, Utility.NAME,
                            viewModel.Person.LocalGovernment.Id);
                    }
                    else
                    {
                        ViewBag.LgaId = new SelectList(lgas, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.LgaId = new SelectList(new List<LocalGovernment>(), Utility.ID, Utility.NAME);
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
                if (viewModel.ApplicantPreviousEducation != null &&
                    viewModel.ApplicantPreviousEducation.StartDate != null)
                {
                    int noOfDays = DateTime.DaysInMonth(viewModel.ApplicantPreviousEducation.StartYear.Id,
                        viewModel.ApplicantPreviousEducation.StartMonth.Id);
                    List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                    if (days != null && days.Count > 0)
                    {
                        days.Insert(0, new Value { Name = "--DD--" });
                    }

                    if (viewModel.ApplicantPreviousEducation.StartDay != null &&
                        viewModel.ApplicantPreviousEducation.StartDay.Id > 0)
                    {
                        ViewBag.PreviousEducationStartDayId = new SelectList(days, Utility.ID, Utility.NAME,
                            viewModel.ApplicantPreviousEducation.StartDay.Id);
                    }
                    else
                    {
                        ViewBag.PreviousEducationStartDayId = new SelectList(days, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.PreviousEducationStartDayId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
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
                if (viewModel.ApplicantPreviousEducation != null && viewModel.ApplicantPreviousEducation.EndDate != null)
                {
                    int noOfDays = DateTime.DaysInMonth(viewModel.ApplicantPreviousEducation.EndYear.Id,
                        viewModel.ApplicantPreviousEducation.EndMonth.Id);
                    List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                    if (days != null && days.Count > 0)
                    {
                        days.Insert(0, new Value { Name = "--DD--" });
                    }

                    if (viewModel.ApplicantPreviousEducation.EndDay != null &&
                        viewModel.ApplicantPreviousEducation.EndDay.Id > 0)
                    {
                        ViewBag.PreviousEducationEndDayId = new SelectList(days, Utility.ID, Utility.NAME,
                            viewModel.ApplicantPreviousEducation.EndDay.Id);
                    }
                    else
                    {
                        ViewBag.PreviousEducationEndDayId = new SelectList(days, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.PreviousEducationEndDayId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
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
                if (viewModel.StudentNdResult != null && viewModel.StudentNdResult.DateAwarded != null)
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
                            ViewBag.StudentNdResultDayAwardedId = new SelectList(days, Utility.ID, Utility.NAME,
                                viewModel.StudentNdResult.DayAwarded.Id);
                        }
                        else
                        {
                            ViewBag.StudentNdResultDayAwardedId = new SelectList(days, Utility.ID, Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.StudentNdResultDayAwardedId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
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
                if (viewModel.StudentEmploymentInformation != null &&
                    viewModel.StudentEmploymentInformation.StartDate != null)
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
                            ViewBag.StudentLastEmploymentStartDayId = new SelectList(days, Utility.ID, Utility.NAME,
                                viewModel.StudentEmploymentInformation.StartDay.Id);
                        }
                        else
                        {
                            ViewBag.StudentLastEmploymentStartDayId = new SelectList(days, Utility.ID, Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.StudentLastEmploymentStartDayId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetLastEmploymentEndDate()
        {
            try
            {
                if (viewModel.StudentEmploymentInformation != null &&
                    viewModel.StudentEmploymentInformation.EndDate != null)
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
                            ViewBag.StudentLastEmploymentEndDayId = new SelectList(days, Utility.ID, Utility.NAME,
                                viewModel.StudentEmploymentInformation.EndDay.Id);
                        }
                        else
                        {
                            ViewBag.StudentLastEmploymentEndDayId = new SelectList(days, Utility.ID, Utility.NAME);
                        }
                    }
                }
                else
                {
                    ViewBag.StudentLastEmploymentEndDayId = new SelectList(new List<Value>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetLevel(PGFormViewModel vModel)
        {
            try
            {
                //set mode of entry
                vModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry { Id = vModel.AppliedCourse.Programme.Id };

                //set mode of study
                switch (vModel.AppliedCourse.Programme.Id)
                {
                    case 1:
                        {
                            vModel.StudentAcademicInformation.Level = new Level { Id = 1 };
                            break;
                        }
                    case 2:
                        {
                            vModel.StudentAcademicInformation.Level = new Level { Id = 1 };
                            break;
                        }
                    case 3:
                        {
                            vModel.StudentAcademicInformation.Level = new Level { Id = 1 };
                            break;
                        }
                    case 4:
                        {
                            vModel.StudentAcademicInformation.Level = new Level { Id = 1 };
                            break;
                        }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void SetEntryAndStudyMode(PGFormViewModel vModel)
        {
            try
            {
                //set mode of entry
                vModel.StudentAcademicInformation.ModeOfEntry = new ModeOfEntry { Id = 1 };

                //set mode of study
                switch (vModel.AppliedCourse.Programme.Id)
                {
                    case 1:
                        {
                            vModel.StudentAcademicInformation.ModeOfStudy = new ModeOfStudy { Id = 1 };

                            break;
                        }
                    case 3:
                        {
                            vModel.StudentAcademicInformation.ModeOfStudy = new ModeOfStudy { Id = 1 };

                            break;
                        }
                    case 2:
                        {
                            vModel.StudentAcademicInformation.ModeOfStudy = new ModeOfStudy { Id = 1 };

                            break;
                        }
                    case 4:
                        {
                            vModel.StudentAcademicInformation.ModeOfStudy = new ModeOfStudy { Id = 1 };
                            break;
                        }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetLocalGovernmentsByState(string id)
        {
            try
            {
                var lgaLogic = new LocalGovernmentLogic();
                List<LocalGovernment> lgas = lgaLogic.GetModelsBy(l => l.State_Id == id);

                return Json(new SelectList(lgas, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetDayOfBirthBy(string monthId, string yearId)
        {
            try
            {
                if (string.IsNullOrEmpty(monthId) || string.IsNullOrEmpty(yearId))
                {
                    return null;
                }

                var month = new Value { Id = Convert.ToInt32(monthId) };
                var year = new Value { Id = Convert.ToInt32(yearId) };
                List<Value> days = Utility.GetNumberOfDaysInMonth(month, year);

                return Json(new SelectList(days, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

                return Json(new SelectList(departments, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public virtual ActionResult UploadFile(FormCollection form)
        {
            HttpPostedFileBase file = Request.Files["MyFile"];

            bool isUploaded = false;
            string personId = form["Person.Id"];
            string passportUrl = form["Person.ImageFileUrl"];
            string message = "File upload failed";

            string path = null;
            string imageUrl = null;
            string imageUrlDisplay = null;

            try
            {
                if (file != null && file.ContentLength != 0)
                {
                    var fileInfo = new FileInfo(file.FileName);
                    string fileExtension = fileInfo.Extension;
                    string newFile = personId + "__";
                    string newFileName = newFile +
                                         DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
                                         fileExtension;

                    string invalidFileMessage = InvalidFile(file.ContentLength, fileExtension);
                    if (!string.IsNullOrEmpty(invalidFileMessage))
                    {
                        isUploaded = false;
                        TempData["imageUrl"] = null;
                        return Json(new { isUploaded, message = invalidFileMessage, imageUrl = passportUrl }, "text/html",
                            JsonRequestBehavior.AllowGet);
                    }

                    string pathForSaving = Server.MapPath("~/Content/Junk");
                    if (CreateFolderIfNeeded(pathForSaving))
                    {
                        DeleteFileIfExist(pathForSaving, personId);

                        file.SaveAs(Path.Combine(pathForSaving, newFileName));

                        isUploaded = true;
                        message = "File uploaded successfully!";

                        path = Path.Combine(pathForSaving, newFileName);
                        if (path != null)
                        {
                            imageUrl = "/Content/Junk/" + newFileName;
                            imageUrlDisplay = appRoot + imageUrl + "?t=" + DateTime.Now;

                            TempData["imageUrl"] = imageUrl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = string.Format("File upload failed: {0}", ex.Message);
            }

            return Json(new { isUploaded, message, imageUrl = imageUrlDisplay }, "text/html", JsonRequestBehavior.AllowGet);
        }
        private string InvalidFile(decimal uploadedFileSize, string fileExtension)
        {
            try
            {
                string message = null;
                decimal oneKiloByte = 1024;
                decimal maximumFileSize = 20 * oneKiloByte;

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

        private void DeleteFileIfExist(string folderPath, string fileName)
        {
            try
            {
                string wildCard = fileName + "*.*";
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath, wildCard, SearchOption.TopDirectoryOnly);

                if (files != null && files.Count() > 0)
                {
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CreateFolderIfNeeded(string path)
        {
            try
            {
                bool result = true;
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}