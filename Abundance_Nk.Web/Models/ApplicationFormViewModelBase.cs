using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;

namespace Abundance_Nk.Web.Models
{
    public abstract class ApplicationFormViewModelBase
    {
        public ApplicationFormViewModelBase()
        {
            Sponsor = new Sponsor();
            Sponsor.Relationship = new Relationship();

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

            Applicant = new Applicant();
            Applicant.Person = new Person();
            Applicant.ApplicationForm = new ApplicationForm();
            Applicant.Ability = new Ability();

            InitialiseOLevelResult();
        }

        public Person Person { get; set; }
        public Sponsor Sponsor { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public Applicant Applicant { get; set; }
        public Sponsor ApplicantSponsor { get; set; }
        public OLevelResult ApplicantFirstSittingOLevelResult { get; set; }
        public OLevelResult ApplicantSecondSittingOLevelResult { get; set; }
        public List<OLevelResultDetail> ApplicantFirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> ApplicantSecondSittingOLevelResultDetails { get; set; }
        public PreviousEducation ApplicantPreviousEducation { get; set; }
        public ApplicantJambDetail ApplicantJambDetail { get; set; }
        public OLevelResult FirstSittingOLevelResult { get; set; }
        public OLevelResult SecondSittingOLevelResult { get; set; }
        public OLevelResultDetail FirstSittingOLevelResultDetail { get; set; }
        public OLevelResultDetail SecondSittingOLevelResultDetail { get; set; }
        public List<OLevelResultDetail> FirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> SecondSittingOLevelResultDetails { get; set; }
        public PreviousEducation PreviousEducation { get; set; }

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

        public ApplicationForm GetApplicationFormBy(long applicationFormId)
        {
            try
            {
                var applicationFormLogic = new ApplicationFormLogic();
                ApplicationForm = applicationFormLogic.GetBy(applicationFormId);

                return ApplicationForm;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void GetApplicationFormBy(ApplicationForm applicationForm)
        {
            try
            {
                ApplicationForm = GetApplicationFormBy(applicationForm.Id);

                if(ApplicationForm != null && ApplicationForm.Id > 0)
                {
                    var personLogic = new PersonLogic();
                    var sponsorLogic = new SponsorLogic();
                    var oLevelResultLogic = new OLevelResultLogic();
                    var oLevelResultDetailLogic = new OLevelResultDetailLogic();
                    var previousEducationLogic = new PreviousEducationLogic();
                    var studentJambDetailLogic = new ApplicantJambDetailLogic();
                    var applicantLogic = new ApplicantLogic();

                    Person = personLogic.GetModelBy(p => p.Person_Id == ApplicationForm.Person.Id);
                    Applicant = applicantLogic.GetModelBy(p => p.Person_Id == ApplicationForm.Person.Id);
                    ApplicantSponsor = sponsorLogic.GetModelBy(p => p.Person_Id == ApplicationForm.Person.Id);
                    ApplicantFirstSittingOLevelResult =
                        oLevelResultLogic.GetModelBy(
                            p => p.Person_Id == ApplicationForm.Person.Id && p.O_Level_Exam_Sitting_Id == 1);
                    ApplicantSecondSittingOLevelResult =
                        oLevelResultLogic.GetModelBy(
                            p => p.Person_Id == ApplicationForm.Person.Id && p.O_Level_Exam_Sitting_Id == 2);
                    ApplicantPreviousEducation =
                        previousEducationLogic.GetModelBy(p => p.Person_Id == ApplicationForm.Person.Id);
                    ApplicantJambDetail =
                        studentJambDetailLogic.GetModelBy(p => p.Person_Id == ApplicationForm.Person.Id);

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

        public void SetApplicant(Applicant applicant)
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

                    if(sponsor.Relationship != null)
                    {
                        Sponsor.Relationship = sponsor.Relationship;
                    }
                    else
                    {
                        Sponsor.Relationship = new Relationship();
                    }
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

                    FirstSittingOLevelResult.ExamNumber = oLevelResult.ExamNumber;
                    FirstSittingOLevelResult.ExamYear = oLevelResult.ExamYear;

                    if(oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        for(int i = 0;i < oLevelResultDetails.Count;i++)
                        {
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

                    SecondSittingOLevelResult.ExamNumber = oLevelResult.ExamNumber;
                    SecondSittingOLevelResult.ExamYear = oLevelResult.ExamYear;

                    if(oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        for(int i = 0;i < oLevelResultDetails.Count;i++)
                        {
                            if(oLevelResultDetails[i].Subject != null)
                            {
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