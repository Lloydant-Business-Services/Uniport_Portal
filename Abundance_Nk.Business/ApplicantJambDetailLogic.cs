using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ApplicantJambDetailLogic : BusinessBaseLogic<ApplicantJambDetail, APPLICANT_JAMB_DETAIL>
    {
        public ApplicantJambDetailLogic()
        {
            translator = new ApplicantJambDetailTranslator();
        }

        public ApplicantJambDetail GetBy(string JambNumber)
        {
            try
            {
                return GetModelsBy(a => a.Applicant_Jamb_Registration_Number == JambNumber && a.Application_Form_Id != null).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async  Task<ApplicantJambDetail> GetBy(long PersonId)
        {
            try
            {
               return  await GetModelsByFODAsync(a => a.Person_Id == PersonId && a.Application_Form_Id != null);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<ApplicantJambDetail> GetAllBy(string JambNumber)
        {
            try
            {
                return GetModelsBy(a => a.Applicant_Jamb_Registration_Number == JambNumber && a.Application_Form_Id != null);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ApplicantJambDetail GetBy(ApplicationForm applicationForm)
        {
            try
            {
                return GetModelsBy(a => a.Application_Form_Id == applicationForm.Id).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

       
        public bool Modify(ApplicantJambDetail jambDetail)
        {
            try
            {
                Expression<Func<APPLICANT_JAMB_DETAIL, bool>> selector = p => p.Person_Id == jambDetail.Person.Id;
                APPLICANT_JAMB_DETAIL entity = GetEntityBy(selector);

                if (entity == null || entity.Person_Id <= 0)
                {
                   return false;
                }

                entity.Person_Id = jambDetail.Person.Id;
                entity.Applicant_Jamb_Registration_Number = jambDetail.JambRegistrationNumber;
                entity.Applicant_Jamb_Score = jambDetail.JambScore;

                if (jambDetail.InstitutionChoice != null)
                {
                    entity.Institution_Choice_Id = jambDetail.InstitutionChoice.Id;
                }

                if (jambDetail.ApplicationForm != null)
                {
                    entity.Application_Form_Id = jambDetail.ApplicationForm.Id;
                }

                if (jambDetail.Subject1 != null)
                {
                    entity.Subject1 = jambDetail.Subject1.Id;
                }
                if (jambDetail.Subject2 != null)
                {
                    entity.Subject2 = jambDetail.Subject2.Id;
                }
                if (jambDetail.Subject3 != null)
                {
                    entity.Subject3 = jambDetail.Subject3.Id;
                }
                if (jambDetail.Subject4 != null)
                {
                    entity.Subject4 = jambDetail.Subject4.Id;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
  
        public bool Modify(ApplicantJambDetail jambDetail, ApplicantJambDetailAudit audit)
        {
            try
            {
                Expression<Func<APPLICANT_JAMB_DETAIL, bool>> selector = p => p.Person_Id == jambDetail.Person.Id;
                APPLICANT_JAMB_DETAIL entity = GetEntityBy(selector);

                if (entity == null || entity.Person_Id <= 0)
                {
                    return false;
                }
                bool audited = CreateAudit(jambDetail, audit, entity);
                if (audited)
                {
                    entity.Person_Id = jambDetail.Person.Id;
                    entity.Applicant_Jamb_Registration_Number = jambDetail.JambRegistrationNumber;
                    entity.Applicant_Jamb_Score = jambDetail.JambScore;

                    if (jambDetail.InstitutionChoice != null)
                    {
                        entity.Institution_Choice_Id = jambDetail.InstitutionChoice.Id;
                    }

                    if (jambDetail.ApplicationForm != null)
                    {
                        entity.Application_Form_Id = jambDetail.ApplicationForm.Id;
                    }

                    if (jambDetail.Subject1 != null)
                    {
                        entity.Subject1 = jambDetail.Subject1.Id;
                    }
                    if (jambDetail.Subject2 != null)
                    {
                        entity.Subject2 = jambDetail.Subject2.Id;
                    }
                    if (jambDetail.Subject3 != null)
                    {
                        entity.Subject3 = jambDetail.Subject3.Id;
                    }
                    if (jambDetail.Subject4 != null)
                    {
                        entity.Subject4 = jambDetail.Subject4.Id;
                    }

                    int modifiedRecordCount = Save();
                    if (modifiedRecordCount <= 0)
                    {
                        return false;
                    }

                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CreateAudit(ApplicantJambDetail jambDetail, ApplicantJambDetailAudit audit, APPLICANT_JAMB_DETAIL personEntity)
        {
            try
            {
                audit.JambRegistrationNumber = jambDetail.JambRegistrationNumber;
                audit.JambScore = jambDetail.JambScore;
                if (jambDetail.Person != null && jambDetail.Person.Id > 0)
                {
                    audit.Person = new Person { Id = jambDetail.Person.Id };
                }
                if(jambDetail.InstitutionChoice != null && jambDetail.InstitutionChoice.Id > 0)
                {
                    audit.InstitutionChoice = new InstitutionChoice { Id = jambDetail.InstitutionChoice.Id };

                }
                if (jambDetail.ApplicationForm != null && jambDetail.ApplicationForm.Id > 0)
                {
                    audit.ApplicationForm = new ApplicationForm { Id = jambDetail.ApplicationForm.Id };

                }
                if (jambDetail.Subject1 != null && jambDetail.Subject1.Id > 0)
                {
                    audit.Subject1 = new OLevelSubject { Id = jambDetail.Subject1.Id };
                }
                if (jambDetail.Subject2 != null && jambDetail.Subject2.Id > 0)
                {
                    audit.Subject2 = new OLevelSubject { Id = jambDetail.Subject2.Id };
                }
                if (jambDetail.Subject3 != null && jambDetail.Subject3.Id > 0)
                {
                    audit.Subject3 = new OLevelSubject { Id = jambDetail.Subject3.Id };
                }
                if (jambDetail.Subject4 != null && jambDetail.Subject4.Id > 0)
                {
                    audit.Subject4 = new OLevelSubject { Id = jambDetail.Subject4.Id };
                }

                var applicantJambDetailAuditLogic = new ApplicantJambDetailAuditLogic();
                var jambAudit = applicantJambDetailAuditLogic.Create(audit);
                if (jambAudit == null || jambAudit.Id <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }




        public List<ApplicantResult> GetApplicantResults(Department department, Session session)
        {
            try
            {
                List<ApplicantResult> applicantResults =
                    (from sr in
                        repository.GetBy<VW_APPLICANT_RESULT>(
                            x => x.Programme_Id == 1 && x.Department_Id == department.Id && x.Session_Id == session.Id)
                        select new ApplicantResult
                        {
                            JambRegNumber = sr.JambRegNumber,
                            JambScore = sr.Applicant_Jamb_Score,
                            JambSubjects = sr.JambSubjects,
                            OLevelType = sr.O_Level_Type_Name,
                            OLevelYear = sr.Exam_Year.ToString(),
                            NumberOfSittings = sr.O_Level_Exam_Sitting_Id,
                            SubjectName = sr.O_Level_Subject_Name,
                            Grade = sr.O_Level_Grade_Name,
                            Name = sr.Name,
                            Programme = sr.Programme_Name,
                            Department = sr.Department_Name,
                            Session = sr.Session_Name,
                            ImageUrl = sr.Image_File_Url,
                            ApplicationFormNumber = sr.Application_Form_Number
                        }).ToList();
                var masterList = new List<ApplicantResult>();
                List<string> regNumbers = applicantResults.Select(r => r.JambRegNumber).Distinct().ToList();
                for (int i = 0; i < regNumbers.Count; i++)
                {
                    List<ApplicantResult> results =
                        applicantResults.Where(r => r.JambRegNumber == regNumbers[i]).ToList();
                    string firstOLevelType = results[0].OLevelType;
                    string firstOLevelYear = results[0].OLevelYear;
                    int firstOlevelSitting = results[0].NumberOfSittings;
                    string OLevelResults = "";
                    int checkSittingChange = 0;
                    string oLevelType = "";
                    string oLevelYear = "";
                    string sitting = "";

                    for (int j = 0; j < results.Count; j++)
                    {
                        if (results[j].NumberOfSittings != firstOlevelSitting)
                        {
                            checkSittingChange += 1;
                            results[j].Sitting = "Two Sittings";
                            sitting = results[j].Sitting;
                            results[j].OLevelType = firstOLevelType + " | " + results[j].OLevelType;
                            oLevelType = results[j].OLevelType;
                            results[j].OLevelYear = firstOLevelYear + " | " + results[j].OLevelYear;
                            oLevelYear = results[j].OLevelYear;
                        }
                        else
                        {
                            if (oLevelType != "" && oLevelYear != "" && sitting != "")
                            {
                                results[j].Sitting = sitting;
                                results[j].OLevelYear = oLevelYear;
                                results[j].OLevelType = oLevelType;
                            }
                            else
                            {
                                results[j].Sitting = "One Sitting";
                            }
                        }

                        if (checkSittingChange == 1)
                        {
                            OLevelResults += "==== 2ND RESULT ====> ";
                        }
                        OLevelResults += results[j].SubjectName + " : " + results[j].Grade + " | ";
                        results[j].OLevelResults = OLevelResults;
                    }

                    results.LastOrDefault().OLevelResults = OLevelResults;
                }

                masterList = applicantResults.GroupBy(a => a.JambRegNumber).Select(a => a.Last()).ToList();

                return masterList.OrderBy(m => m.JambRegNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ApplicantResult> GetApplicantsByChoice(Department department, Session session,InstitutionChoice institutionChoice,ApplicationFormSetting applicationFormSetting)
        {
            try
            {
                List<ApplicantResult> applicantResults =
                    (from sr in
                        repository.GetBy<VW_APPLICANT_RESULT>(
                            x =>
                                x.Programme_Id == 1 && x.Department_Id == department.Id && x.Session_Id == session.Id &&
                                x.Institution_Choice_Id == institutionChoice.Id && x.Application_Form_Setting_Id == applicationFormSetting.Id )
                        select new ApplicantResult
                        {
                            JambRegNumber = sr.JambRegNumber,
                            JambScore = sr.Applicant_Jamb_Score,
                            JambSubjects = sr.JambSubjects,
                            OLevelType = sr.O_Level_Type_Name,
                            OLevelYear = sr.Exam_Year.ToString(),
                            NumberOfSittings = sr.O_Level_Exam_Sitting_Id,
                            SubjectName = sr.O_Level_Subject_Description,
                            Grade = sr.O_Level_Grade_Name,
                            Name = sr.Name,
                            Programme = sr.Programme_Name,
                            Department = sr.Department_Name,
                            Session = sr.Session_Name,
                            ImageUrl = sr.Image_File_Url,
                            ApplicationFormNumber = sr.Application_Form_Number,
                            InstitutionChoice = sr.Institution_Choice_Name,
                            State = sr.State_Name,
                            LocalGovernmentArea = sr.Local_Government_Name,
                            Sex = sr.Sex_Name
                        }).ToList();
                var masterList = new List<ApplicantResult>();
                List<string> regNumbers = applicantResults.Select(r => r.JambRegNumber).Distinct().ToList();
                for (int i = 0; i < regNumbers.Count; i++)
                {
                    List<ApplicantResult> results = applicantResults.Where(r => r.JambRegNumber == regNumbers[i]).ToList();
                    string FirstSitting = "";
                    string SecondSitting = "";

                    var firstSittingResult = results.Where(r => r.NumberOfSittings == 1).ToList();
                    var secondSittingResult = results.Where(r => r.NumberOfSittings == 2).ToList();
                    if (firstSittingResult.Count > 0)
                    {
                        FirstSitting = firstSittingResult[0].OLevelType;
                        for (int j = 0; j < firstSittingResult.Count;j++)
                        {
                            FirstSitting +=  firstSittingResult[j].SubjectName + " : " + firstSittingResult[j].Grade + ",";
                        } 
                    }

                   if (secondSittingResult.Count > 0)
                    {
                        SecondSitting = secondSittingResult[0].OLevelType;
                        for (int k =0; k < secondSittingResult.Count;k++)
                        {
                            SecondSitting +=  secondSittingResult[k].SubjectName + " : " + secondSittingResult[k].Grade + ",";
                        } 
                    }

                    results.LastOrDefault().Qualification1 = FirstSitting;
                    results.LastOrDefault().Qualification2 = SecondSitting;
                }

                masterList = applicantResults.GroupBy(a => a.JambRegNumber).Select(a => a.Last()).ToList();

                return masterList.OrderBy(m => m.JambRegNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ProcessedSupplementaryReport> GetSupApplicantsByChoice(Department department, Session session, InstitutionChoice institutionChoice, ApplicationFormSetting applicationFormSetting)
        {
            try
            {
                List<ProcessedSupplementaryReport> supApplicantResults =
                    (from sr in
                         repository.GetBy<VW_PROCESSED_SUPPLEMENTARY_REPORT>(
                             x =>
                                  x.SupplementaryCourseId == department.Id && x.Session_Id == session.Id &&
                                 x.Institution_Choice_Id == institutionChoice.Id && x.Application_Form_Setting_Id == applicationFormSetting.Id)
                     select new ProcessedSupplementaryReport
                     {
                         JambRegNumber = sr.JambRegNumber,
                         Applicant_Jamb_Score = sr.Applicant_Jamb_Score,

                         Name = sr.Name,
                         RAW_SCORE = sr.RAW_SCORE,
                         DEPARTMENT = sr.SupplementaryCourse,
                         Session_Name = sr.Session_Name,
                         Institution_Choice_Name = sr.Institution_Choice_Name,
                         State_Name = sr.State_Name,
                         Local_Government_Name = sr.Local_Government_Name,
                         Sex_Name = sr.Sex_Name,
                         COMPUTED_JAMB_SCORE = sr.COMPUTED_JAMB_SCORE,
                         COMPUTED_PUTME_SCORE = sr.COMPUTED_PUTME_SCORE,
                         SUM = sr.SUM,
                         Qualification1 = sr.Qualification1,
                         Qualification2 = sr.Qualification2


                     }).ToList();


                return supApplicantResults.OrderByDescending(m => m.SUM).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

       
    }
}