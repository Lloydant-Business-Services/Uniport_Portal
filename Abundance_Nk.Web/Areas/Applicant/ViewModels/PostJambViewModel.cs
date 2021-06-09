﻿using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Applicant.ViewModels
{
    public class PostJambViewModel
    {
        //private const string DEFAULT_AVATAR = "/Content/Images/default_avatar.png";

        private readonly AbilityLogic abilityLogic;
        private readonly DepartmentLogic departmentLogic;
        private readonly EducationalQualificationLogic educationalQualificationLogic;
        private readonly ITDurationLogic iTDurationLogic;
        private readonly InstitutionChoiceLogic institutionChoiceLogic;
        private readonly LocalGovernmentLogic lgaLogic;
        private readonly OLevelGradeLogic oLevelGradeLogic;
        private readonly OLevelSubjectLogic oLevelSubjectLogic;
        private readonly OLevelTypeLogic oLevelTypeLogic;
        private readonly ProgrammeLogic programmeLogic;
        private readonly RelationshipLogic relationshipLogic;
        private readonly ReligionLogic religionLogic;
        private readonly ResultGradeLogic resultGradeLogic;
        private readonly SexLogic sexLogic;
        private readonly StateLogic stateLogic;

        public PostJambViewModel()
        {
            sexLogic = new SexLogic();
            stateLogic = new StateLogic();
            lgaLogic = new LocalGovernmentLogic();
            relationshipLogic = new RelationshipLogic();
            programmeLogic = new ProgrammeLogic();
            departmentLogic = new DepartmentLogic();
            oLevelTypeLogic = new OLevelTypeLogic();
            oLevelGradeLogic = new OLevelGradeLogic();
            oLevelSubjectLogic = new OLevelSubjectLogic();
            resultGradeLogic = new ResultGradeLogic();
            educationalQualificationLogic = new EducationalQualificationLogic();
            iTDurationLogic = new ITDurationLogic();
            institutionChoiceLogic = new InstitutionChoiceLogic();
            abilityLogic = new AbilityLogic();
            religionLogic = new ReligionLogic();

            Sponsor = new Sponsor();
            Sponsor.Relationship = new Relationship();

            AppliedCourse = new AppliedCourse();
            AppliedCourse.Programme = new Programme();
            AppliedCourse.Department = new Department();

            Programme = new Programme();

            FirstSittingOLevelResult = new OLevelResult();
            FirstSittingOLevelResult.Type = new OLevelType();

            SecondSittingOLevelResult = new OLevelResult();
            SecondSittingOLevelResult.Type = new OLevelType();

            PreviousEducation = new PreviousEducation();
            PreviousEducation.ResultGrade = new ResultGrade();
            PreviousEducation.Qualification = new EducationalQualification();
            PreviousEducation.ITDuration = new ITDuration();
            PreviousEducation.StartYear = new Value();
            PreviousEducation.StartMonth = new Value();
            PreviousEducation.StartDay = new Value();
            PreviousEducation.EndYear = new Value();
            PreviousEducation.EndMonth = new Value();
            PreviousEducation.EndDay = new Value();

            ApplicantJambDetail = new ApplicantJambDetail();
            ApplicantJambDetail.InstitutionChoice = new InstitutionChoice();

            Person = new Person();
            Person.LocalGovernment = new LocalGovernment();
            Person.Religion = new Religion();
            Person.State = new State();
            Person.Sex = new Sex();
            Person.YearOfBirth = new Value();
            Person.MonthOfBirth = new Value();
            Person.DayOfBirth = new Value();

            Applicant = new Model.Model.Applicant();
            Applicant.Person = new Person();
            Applicant.ApplicationForm = new ApplicationForm();
            Applicant.Ability = new Ability();

            Payment = new Payment();
            ApplicationProgrammeFee = new ApplicationProgrammeFee();
            ApplicantReferees = new List<ApplicantReferee>();
            PopulateAllDropDowns();
            InitialiseOLevelResult();
        }

        public List<ApplicantReferee> ApplicantReferees { get; set; }
        public SupplementaryCourse SupplementaryCourse { get; set; }
        public RemitaPayment remitaPyament { get; set; }
        public string PassportUrl { get; set; }
        public string CredentialUrl { get; set; }
        public bool ApplicationAlreadyExist { get; set; }
        public HttpPostedFileBase MyFile { get; set; }
        public HttpPostedFileBase MyCredentialFile { get; set; }
        public Person Person { get; set; }
        public List<Sex> Genders { get; set; }
        public List<State> States { get; set; }
        public List<LocalGovernment> Lgas { get; set; }
        public List<Religion> Religions { get; set; }
        public Model.Model.Applicant Applicant { get; set; }
        public Payment Payment { get; set; }
        public Sponsor Sponsor { get; set; }
        public List<Relationship> Relationships { get; set; }
        public List<ITDuration> ITDurations { get; set; }
        public List<InstitutionChoice> InstitutionChoices { get; set; }
        public AppliedCourse AppliedCourse { get; set; }
        public List<Programme> Programmes { get; set; }
        public List<Department> Departments { get; set; }
        public List<Value> ExamYears { get; set; }
        public List<OLevelType> OLevelTypes { get; set; }
        public List<OLevelGrade> OLevelGrades { get; set; }
        public List<OLevelSubject> OLevelSubjects { get; set; }
        public OLevelResult FirstSittingOLevelResult { get; set; }
        public OLevelResult SecondSittingOLevelResult { get; set; }
        public OLevelResultDetail FirstSittingOLevelResultDetail { get; set; }
        public OLevelResultDetail SecondSittingOLevelResultDetail { get; set; }
        public List<OLevelResultDetail> FirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> SecondSittingOLevelResultDetails { get; set; }

        public ApplicantJambDetail ApplicantJambDetail { get; set; }
        public PreviousEducation PreviousEducation { get; set; }
        public List<Value> GraduationYears { get; set; }
        public List<ResultGrade> ResultGrades { get; set; }
        public List<EducationalQualification> EducationalQualifications { get; set; }
        public List<Ability> Abilities { get; set; }

        public List<SelectListItem> SupplementaryDepartmentSelectList { get; set; }
        public List<SelectListItem> StateSelectList { get; set; }
        public List<SelectListItem> SexSelectList { get; set; }
        public List<SelectListItem> DepartmentOptionSelectList { get; set; }
        public List<SelectListItem> FacultySelectList { get; set; }
        public List<SelectListItem> RelationshipSelectList { get; set; }
        public List<SelectListItem> GraduationYearSelectList { get; set; }
        public List<SelectListItem> GraduationMonthSelectList { get; set; }
        public List<SelectListItem> ExamYearSelectList { get; set; }
        public List<SelectListItem> OLevelTypeSelectList { get; set; }
        public List<SelectListItem> OLevelGradeSelectList { get; set; }
        public List<SelectListItem> OLevelSubjectSelectList { get; set; }
        public List<SelectListItem> ResultGradeSelectList { get; set; }
        public List<SelectListItem> LocalGovernmentSelectList { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }
        public List<SelectListItem> ReligionSelectList { get; set; }
        public List<SelectListItem> AbilitySelectList { get; set; }
        public List<SelectListItem> EducationalQualificationSelectList { get; set; }
        public List<SelectListItem> ITDurationSelectList { get; set; }
        public List<SelectListItem> JambScoreSelectList { get; set; }
        public List<SelectListItem> InstitutionChoiceSelectList { get; set; }

        public List<SelectListItem> DayOfBirthSelectList { get; set; }
        public List<SelectListItem> MonthOfBirthSelectList { get; set; }
        public List<SelectListItem> YearOfBirthSelectList { get; set; }

        public List<SelectListItem> PreviousEducationStartDaySelectList { get; set; }
        public List<SelectListItem> PreviousEducationStartMonthSelectList { get; set; }
        public List<SelectListItem> PreviousEducationStartYearSelectList { get; set; }
        public List<SelectListItem> PreviousEducationEndDaySelectList { get; set; }
        public List<SelectListItem> PreviousEducationEndMonthSelectList { get; set; }
        public List<SelectListItem> PreviousEducationEndYearSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> LevelSelectListItem { get; set; }
        

        public ApplicationFormSetting ApplicationFormSetting { get; set; }
        public ApplicationProgrammeFee ApplicationProgrammeFee { get; set; }
        public Session Session { get; set; }
        public Programme Programme { get; set; }
        public string ApplicationFormNumber { get; set; }

        //public Person Applicant { get; set; }
        public Sponsor ApplicantSponsor { get; set; }

        public OLevelResult ApplicantFirstSittingOLevelResult { get; set; }
        public OLevelResult ApplicantSecondSittingOLevelResult { get; set; }
        public List<OLevelResultDetail> ApplicantFirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> ApplicantSecondSittingOLevelResultDetails { get; set; }
        public PreviousEducation ApplicantPreviousEducation { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public string QRVerification { get; set; }
        public int HasJambPassport { get; set; }
        public Model.Model.Student Student { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public long StudentId { get; set; }
        public void InitialiseOLevelResult()
        {
            try
            {
                var oLevelResultDetails = new List<OLevelResultDetail>();
                var oLevelResultDetail1 = new OLevelResultDetail();
                var oLevelResultDetail2 = new OLevelResultDetail();
                var oLevelResultDetail3 = new OLevelResultDetail();
                var oLevelResultDetail4 = new OLevelResultDetail();
                var oLevelResultDetail5 = new OLevelResultDetail();
                var oLevelResultDetail6 = new OLevelResultDetail();
                var oLevelResultDetail7 = new OLevelResultDetail();
                var oLevelResultDetail8 = new OLevelResultDetail();
                var oLevelResultDetail9 = new OLevelResultDetail();

                var oLevelResultDetail11 = new OLevelResultDetail();
                var oLevelResultDetail22 = new OLevelResultDetail();
                var oLevelResultDetail33 = new OLevelResultDetail();
                var oLevelResultDetail44 = new OLevelResultDetail();
                var oLevelResultDetail55 = new OLevelResultDetail();
                var oLevelResultDetail66 = new OLevelResultDetail();
                var oLevelResultDetail77 = new OLevelResultDetail();
                var oLevelResultDetail88 = new OLevelResultDetail();
                var oLevelResultDetail99 = new OLevelResultDetail();

                FirstSittingOLevelResultDetails = new List<OLevelResultDetail>();
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail1);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail2);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail3);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail4);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail5);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail6);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail7);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail8);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail9);

                SecondSittingOLevelResultDetails = new List<OLevelResultDetail>();
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail11);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail22);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail33);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail44);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail55);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail66);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail77);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail88);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail99);
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void PopulateAllDropDowns()
        {
            const int ONE = 1;
            const int FOUR_HUNDRED = 400;
            const int START_YEAR = 1920;
            //const int START_YEAR = 1970;

            try
            {
                Genders = sexLogic.GetAll();
                States = stateLogic.GetAll();
                Lgas = lgaLogic.GetAll();
                Relationships = relationshipLogic.GetAll();
                Departments = departmentLogic.GetAll();
                Programmes = programmeLogic.GetAll();
                ITDurations = iTDurationLogic.GetAll();
                InstitutionChoices = institutionChoiceLogic.GetAll();
                Abilities = abilityLogic.GetAll();
                Religions = religionLogic.GetAll();
                GraduationYears = Utility.CreateYearListFrom(START_YEAR);
                ExamYears = GraduationYears;
                OLevelTypes = oLevelTypeLogic.GetAll();
                OLevelGrades = oLevelGradeLogic.GetAll();
                OLevelSubjects = oLevelSubjectLogic.GetAll();
                ResultGrades = resultGradeLogic.GetAll();
                EducationalQualifications = educationalQualificationLogic.GetAll();

                SupplementaryDepartmentSelectList = Utility.PopulateDepartmentSelectListItemBy(new Programme { Id = 1 });
                ReligionSelectList = Utility.PopulateReligionSelectListItem();
                SexSelectList = Utility.PopulateSexSelectListItem();
                StateSelectList = Utility.PopulateStateSelectListItem();
                FacultySelectList = Utility.PopulateFacultySelectListItem();
                RelationshipSelectList = Utility.PopulateRelationshipSelectListItem();
                GraduationYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR,true);
                GraduationMonthSelectList = Utility.PopulateGraduationMonthSelectListItem();
                ExamYearSelectList = Utility.PopulateExamYearSelectListItem(START_YEAR);
                OLevelTypeSelectList = Utility.PopulateOLevelTypeSelectListItem();
                OLevelGradeSelectList = Utility.PopulateOLevelGradeSelectListItem();
                OLevelSubjectSelectList = Utility.PopulateOLevelSubjectSelectListItem();
                ResultGradeSelectList = Utility.PopulateResultGradeSelectListItem();
                LocalGovernmentSelectList = Utility.PopulateLocalGovernmentSelectListItem();
                AbilitySelectList = Utility.PopulateAbilitySelectListItem();
                EducationalQualificationSelectList = Utility.PopulateEducationalQualificationSelectListItem();
                ITDurationSelectList = Utility.PopulateITDurationSelectListItem();
                JambScoreSelectList = Utility.PopulateJambScoreSelectListItem(ONE,FOUR_HUNDRED);
                InstitutionChoiceSelectList = Utility.PopulateInstitutionChoiceSelectListItem();

                MonthOfBirthSelectList = Utility.PopulateMonthSelectListItem();
                YearOfBirthSelectList = Utility.PopulateYearSelectListItem(START_YEAR,false);

                PreviousEducationStartMonthSelectList = Utility.PopulateMonthSelectListItem();
                PreviousEducationStartYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR,false);
                PreviousEducationEndMonthSelectList = Utility.PopulateMonthSelectListItem();
                PreviousEducationEndYearSelectList = Utility.PopulateYearSelectListItem(START_YEAR,false);
                ProgrammeSelectListItem = Utility.PopulateProgrammeSelectListItem();
                LevelSelectListItem= Utility.PopulateLevelSelectListItem();
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void LoadApplicantionFormBy(Person person,Payment payment)
        {
            try
            {
                ApplicationForm = GetApplicationFormBy(person,payment);
                if(ApplicationForm != null && ApplicationForm.Id > 0)
                {
                    var personLogic = new PersonLogic();
                    var sponsorLogic = new SponsorLogic();
                    var oLevelResultLogic = new OLevelResultLogic();
                    var oLevelResultDetailLogic = new OLevelResultDetailLogic();
                    var previousEducationLogic = new PreviousEducationLogic();
                    var studentJambDetailLogic = new ApplicantJambDetailLogic();
                    var applicantLogic = new ApplicantLogic();
                    var applicantRefereeLogic = new ApplicantRefereeLogic();

                    Person = personLogic.GetModelBy(p => p.Person_Id == person.Id);
                    Applicant = applicantLogic.GetModelBy(p => p.Person_Id == person.Id);
                    ApplicantSponsor = sponsorLogic.GetModelBy(p => p.Person_Id == person.Id);
                    ApplicantFirstSittingOLevelResult =
                        oLevelResultLogic.GetModelBy(p => p.Person_Id == person.Id && p.O_Level_Exam_Sitting_Id == 1);
                    ApplicantSecondSittingOLevelResult =
                        oLevelResultLogic.GetModelBy(p => p.Person_Id == person.Id && p.O_Level_Exam_Sitting_Id == 2);
                    ApplicantPreviousEducation = previousEducationLogic.GetModelBy(p => p.Person_Id == person.Id);
                    ApplicantJambDetail = studentJambDetailLogic.GetModelBy(p => p.Person_Id == person.Id);
                    ApplicantReferees =
                        applicantRefereeLogic.GetModelsBy(p => p.Application_Form_Id == ApplicationForm.Id);
                    if(ApplicantFirstSittingOLevelResult != null && ApplicantFirstSittingOLevelResult.Id > 0)
                    {
                        ApplicantFirstSittingOLevelResultDetails =
                            oLevelResultDetailLogic.GetModelsBy(
                                p => p.Applicant_O_Level_Result_Id == ApplicantFirstSittingOLevelResult.Id);
                    }

                    if(ApplicantSecondSittingOLevelResult != null && ApplicantSecondSittingOLevelResult.Id > 0)
                    {
                        ApplicantSecondSittingOLevelResultDetails =
                            oLevelResultDetailLogic.GetModelsBy(
                                p => p.Applicant_O_Level_Result_Id == ApplicantSecondSittingOLevelResult.Id);
                    }

                    SetApplicant(Applicant);
                    SetApplicantBioData(Person);
                    SetApplicantSponsor(ApplicantSponsor);
                    SetApplicantFirstSittingOLevelResult(ApplicantFirstSittingOLevelResult,
                    ApplicantFirstSittingOLevelResultDetails);
                    SetApplicantSecondSittingOLevelResult(ApplicantSecondSittingOLevelResult,
                    ApplicantSecondSittingOLevelResultDetails);
                    SetApplicantPreviousEducation(ApplicantPreviousEducation);
                    SetApplicantJambDetail(ApplicantJambDetail);
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ApplicationForm GetApplicationFormBy(Person person,Payment payment)
        {
            try
            {
                var applicationFormLogic = new ApplicationFormLogic();
                return applicationFormLogic.GetModelBy(a => a.Person_Id == person.Id && a.Payment_Id == payment.Id);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void SetApplicantBioData(Person person)
        {
            try
            {
                if(person != null && person.Id > 0)
                {
                    Person.DateOfBirth = person.DateOfBirth;
                    Person.HomeTown = person.HomeTown;
                    Person.HomeAddress = person.HomeAddress;
                    Person.ImageFileUrl = person.ImageFileUrl;

                    if(person.State != null && !string.IsNullOrEmpty(person.State.Id))
                    {
                        Person.State = person.State;
                    }
                    else
                    {
                        Person.State = new State();
                    }

                    if(person.Sex != null && person.Sex.Id > 0)
                    {
                        Person.Sex = person.Sex;
                    }
                    else
                    {
                        Person.Sex = new Sex();
                    }

                    if(person.LocalGovernment != null && person.LocalGovernment.Id > 0)
                    {
                        Person.LocalGovernment = person.LocalGovernment;
                    }
                    else
                    {
                        Person.LocalGovernment = new LocalGovernment();
                    }

                    if(person.Religion != null && person.Religion.Id > 0)
                    {
                        Person.Religion = person.Religion;
                    }
                    else
                    {
                        Person.Religion = new Religion();
                    }

                    if(person.DateOfBirth.HasValue)
                    {
                        Person.DayOfBirth = person.DayOfBirth;
                        Person.MonthOfBirth = person.MonthOfBirth;
                        Person.YearOfBirth = person.YearOfBirth;
                    }
                    else
                    {
                        Person.DayOfBirth = new Value();
                        Person.MonthOfBirth = new Value();
                        Person.YearOfBirth = new Value();
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void SetApplicant(Model.Model.Applicant applicant)
        {
            try
            {
                if(applicant != null)
                {
                    Applicant.ExtraCurricullarActivities = applicant.ExtraCurricullarActivities;
                    Applicant.OtherAbility = applicant.OtherAbility;

                    if(applicant.Ability != null)
                    {
                        Applicant.Ability = applicant.Ability;
                    }
                    else
                    {
                        Applicant.Ability = new Ability();
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void SetApplicantSponsor(Sponsor sponsor)
        {
            try
            {
                if(sponsor != null)
                {
                    Sponsor.Name = sponsor.Name;
                    Sponsor.ContactAddress = sponsor.ContactAddress;
                    Sponsor.MobilePhone = sponsor.MobilePhone;
                    //Sponsor.ExtraCurricullarActivities = sponsor.ExtraCurricullarActivities;
                    //Sponsor.OtherAbility = sponsor.OtherAbility;

                    if(sponsor.Relationship != null)
                    {
                        Sponsor.Relationship = sponsor.Relationship;
                    }
                    else
                    {
                        Sponsor.Relationship = new Relationship();
                    }

                    //if (sponsor.Ability != null)
                    //{
                    //    Sponsor.Ability = sponsor.Ability;
                    //}
                    //else
                    //{
                    //    Sponsor.Ability = new Ability();
                    //}
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void SetApplicantFirstSittingOLevelResult(OLevelResult oLevelResult,
            List<OLevelResultDetail> oLevelResultDetails)
        {
            try
            {
                if(oLevelResult != null && oLevelResult.Id > 0)
                {
                    if(oLevelResult.Type != null)
                    {
                        FirstSittingOLevelResult.Type = oLevelResult.Type;
                    }
                    else
                    {
                        FirstSittingOLevelResult.Type = new OLevelType();
                    }

                    FirstSittingOLevelResult.Id = oLevelResult.Id;
                    FirstSittingOLevelResult.ExamNumber = oLevelResult.ExamNumber;
                    FirstSittingOLevelResult.ExamYear = oLevelResult.ExamYear;
                    FirstSittingOLevelResult.ScannedCopyUrl = oLevelResult.ScannedCopyUrl;

                    if(oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        for(int i = 0;i < oLevelResultDetails.Count;i++)
                        {
                            FirstSittingOLevelResultDetails[i].Id = oLevelResultDetails[i].Id;
                            FirstSittingOLevelResultDetails[i].Subject = oLevelResultDetails[i].Subject;
                            FirstSittingOLevelResultDetails[i].Grade = oLevelResultDetails[i].Grade;
                        }
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void SetApplicantSecondSittingOLevelResult(OLevelResult oLevelResult,
            List<OLevelResultDetail> oLevelResultDetails)
        {
            try
            {
                if(oLevelResult != null && oLevelResult.Id > 0)
                {
                    if(oLevelResult.Type != null)
                    {
                        SecondSittingOLevelResult.Type = oLevelResult.Type;
                    }
                    else
                    {
                        SecondSittingOLevelResult.Type = new OLevelType();
                    }

                    SecondSittingOLevelResult.Id = oLevelResult.Id;
                    SecondSittingOLevelResult.ExamNumber = oLevelResult.ExamNumber;
                    SecondSittingOLevelResult.ExamYear = oLevelResult.ExamYear;

                    if(oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        for(int i = 0;i < oLevelResultDetails.Count;i++)
                        {
                            if(oLevelResultDetails[i].Subject != null)
                            {
                                SecondSittingOLevelResultDetails[i].Id = oLevelResultDetails[i].Id;
                                SecondSittingOLevelResultDetails[i].Subject = oLevelResultDetails[i].Subject;
                            }
                            else
                            {
                                SecondSittingOLevelResultDetails[i].Subject = new OLevelSubject();
                            }
                            if(oLevelResultDetails[i].Grade != null)
                            {
                                SecondSittingOLevelResultDetails[i].Grade = oLevelResultDetails[i].Grade;
                            }
                            else
                            {
                                SecondSittingOLevelResultDetails[i].Grade = new OLevelGrade();
                            }
                        }
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void SetApplicantPreviousEducation(PreviousEducation previousEducation)
        {
            try
            {
                if(previousEducation != null && previousEducation.Id > 0)
                {
                    PreviousEducation.SchoolName = previousEducation.SchoolName;
                    PreviousEducation.Course = previousEducation.Course;
                    PreviousEducation.StartDate = previousEducation.StartDate;
                    PreviousEducation.EndDate = previousEducation.EndDate;

                    //PreviousEducation.PreND = previousEducation.PreND;
                    //PreviousEducation.PreNDYearFrom = previousEducation.PreNDYearFrom;
                    //PreviousEducation.PreNDYearTo = previousEducation.PreNDYearTo;

                    if(previousEducation.ITDuration != null)
                    {
                        PreviousEducation.ITDuration = previousEducation.ITDuration;
                    }
                    else
                    {
                        PreviousEducation.ITDuration = new ITDuration();
                    }

                    if(previousEducation.ResultGrade != null)
                    {
                        PreviousEducation.ResultGrade = previousEducation.ResultGrade;
                    }
                    else
                    {
                        PreviousEducation.ResultGrade = new ResultGrade();
                    }

                    if(previousEducation.Qualification != null)
                    {
                        PreviousEducation.Qualification = previousEducation.Qualification;
                    }
                    else
                    {
                        PreviousEducation.Qualification = new EducationalQualification();
                    }

                    if(previousEducation.StartDate != null)
                    {
                        PreviousEducation.StartDay = previousEducation.StartDay;
                        PreviousEducation.StartMonth = previousEducation.StartMonth;
                        PreviousEducation.StartYear = previousEducation.StartYear;
                    }
                    else
                    {
                        PreviousEducation.StartDay = new Value();
                        PreviousEducation.StartMonth = new Value();
                        PreviousEducation.StartYear = new Value();
                    }

                    if(previousEducation.EndDate != null)
                    {
                        PreviousEducation.EndDay = previousEducation.EndDay;
                        PreviousEducation.EndMonth = previousEducation.EndMonth;
                        PreviousEducation.EndYear = previousEducation.EndYear;
                    }
                    else
                    {
                        PreviousEducation.EndDay = new Value();
                        PreviousEducation.EndMonth = new Value();
                        PreviousEducation.EndYear = new Value();
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void SetApplicantJambDetail(ApplicantJambDetail jambDetail)
        {
            try
            {
                if(jambDetail != null)
                {
                    ApplicantJambDetail.JambScore = jambDetail.JambScore;

                    if(jambDetail.InstitutionChoice != null)
                    {
                        ApplicantJambDetail.InstitutionChoice = jambDetail.InstitutionChoice;
                    }
                    else
                    {
                        ApplicantJambDetail.InstitutionChoice = new InstitutionChoice();
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        
    }
}