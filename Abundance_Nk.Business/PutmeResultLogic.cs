using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Linq;
using System.Collections.Generic;

namespace Abundance_Nk.Business
{
    public class PutmeResultLogic : BusinessBaseLogic<PutmeResult, PUTME_RESULT>
    {
        public PutmeResultLogic()
        {
            translator = new PutmeResultTranslator();
        }

       public PutmeSlip GetResult(PutmeResult result,int ExamType)
        {
            try
            {
                PutmeSlip Result;
                if (ExamType == 19)
                {
                    Result = (from a in
                        repository.GetBy<VW_PUTME_RESULT>(
                            a => a.ID == result.Id
                                )
                              select new PutmeSlip
                              {
                                  ApplicationNumber = a.Application_Form_Number,
                                  JambNumber = a.Applicant_Jamb_Registration_Number,
                                  Fullname = a.Last_Name + " " + a.First_Name + " " + a.Other_Name ?? a.FULLNAME,
                                  JambScore = a.Applicant_Jamb_Score,
                                  ExamScore = a.TOTAL,
                                  AverageScore = GetAverage(a.TOTAL, a.Applicant_Jamb_Score),
                                  passportUrl = a.Image_File_Url,
                                  Department = a.Department_Name,
                                  ExamNumber = a.Application_Exam_Number,
                                  PersonId = a.Person_Id,

                              }).FirstOrDefault();
                }
                else
                {
                    Result = (from a in
                        repository.GetBy<VW_PUTME_RESULT_SUPPLEMENTARY>(
                            a => a.ID == result.Id
                                )
                              select new PutmeSlip
                              {
                                  ApplicationNumber = a.Application_Form_Number,
                                  JambNumber = a.Applicant_Jamb_Registration_Number,
                                  Fullname = a.Last_Name + " " + a.First_Name + " " + a.Other_Name ?? a.FULLNAME,
                                  JambScore = a.Applicant_Jamb_Score,
                                  ExamScore = a.TOTAL,
                                  AverageScore = GetAverage(a.TOTAL, a.Applicant_Jamb_Score),
                                  passportUrl = a.Image_File_Url,
                                  Department = a.Department_Name,
                                  ExamNumber = a.Application_Exam_Number,
                                  PersonId = a.Person_Id
                              }).LastOrDefault();
                }
                

               
                return Result;

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

       private double? GetAverage(double? exam, short? jambscore)
       {
           try
           {
               var score1 = (exam / 100) * 40;
               double jamb = Convert.ToDouble(jambscore);
               var score2 = (jamb / 400) * 60;
               return score1 + score2;
           }
           catch (Exception)
           {
               
               throw;
           }
       }

       public List<PutmeSlip> GetPUTMEReport(Department Department, int FormSetting,int InstitutionChoice)
       {
           try
           {
                List<PutmeSlip> Result = new List<PutmeSlip>();
                if (FormSetting == 17 || FormSetting == 18)
                {
                    Result = (from a in
                                repository.GetBy<VW_PUTME_RESULT>(
                                    a => a.Department_Name == Department.Name && a.Application_Form_Setting_Id == FormSetting && a.Institution_Choice_Id == InstitutionChoice
                                        )
                              select new PutmeSlip
                              {
                                  ApplicationNumber = a.Application_Form_Number,
                                  JambNumber = a.Applicant_Jamb_Registration_Number,
                                  Fullname = a.Last_Name + " " + a.First_Name + " " + a.Other_Name ?? a.FULLNAME,
                                  JambScore = a.Applicant_Jamb_Score,
                                  ExamScore = a.RAW_SCORE,
                                  AverageScore = ((a.RAW_SCORE + a.Applicant_Jamb_Score) / 2),
                                  passportUrl = a.Image_File_Url,
                                  Department = a.Department_Name,
                                  ExamNumber = a.Application_Exam_Number,
                                  PersonId = a.Person_Id,
                                  Sex = a.Sex_Name,
                                  State = a.State_Name,
                                  LocalGoverment = a.Local_Government_Name,
                                  SchoolChoice = a.Institution_Choice_Name
                              }).ToList();
                }
                else
                {
                    Result = (from a in
                                repository.GetBy<VW_PUTME_RESULT_SUPPLEMENTARY>(
                                    a => a.Department_Name == Department.Name && a.Application_Form_Setting_Id == FormSetting
                                        )
                              select new PutmeSlip
                              {
                                  ApplicationNumber = a.Application_Form_Number,
                                  JambNumber = a.Applicant_Jamb_Registration_Number,
                                  Fullname = a.Last_Name + " " + a.First_Name + " " + a.Other_Name ?? a.FULLNAME,
                                  JambScore = a.Applicant_Jamb_Score,
                                  ExamScore = a.RAW_SCORE,
                                  AverageScore = ((a.RAW_SCORE + a.Applicant_Jamb_Score) / 2),
                                  passportUrl = a.Image_File_Url,
                                  Department = a.Department_Name,
                                  ExamNumber = a.Application_Exam_Number,
                                  PersonId = a.Person_Id,
                                  Sex = a.Sex_Name,
                                  State = a.State_Name,
                                  LocalGoverment = a.Local_Government_Name
                              }).ToList();
                }

               return Result;

           }
           catch (Exception ex)
           {

               throw ex;
           }
       }

       public List<PutmeSlip> GetPUTMEReportOlevelByChoice(Department department, int FormSetting, InstitutionChoice institutionChoice)
        {
            try
            {
                List<PutmeSlip> applicantResults = new List<PutmeSlip>();
                if (FormSetting == 17 || FormSetting == 18)
                {
                    applicantResults =
                (from sr in
                    repository.GetBy<VW_PUTME_RESULT_O_LEVEL>(
                        a => a.Department_Name == department.Name && a.Application_Form_Setting_Id == FormSetting && a.Institution_Choice_Id == institutionChoice.Id)
                 select new PutmeSlip
                 {
                     ApplicationNumber = sr.Application_Form_Number,
                     JambNumber = sr.Applicant_Jamb_Registration_Number,
                     Fullname = sr.Last_Name + " " + sr.First_Name + " " + sr.Other_Name,
                     JambScore = sr.Applicant_Jamb_Score,
                     ExamScore = sr.RAW_SCORE,
                     AverageScore = ((sr.RAW_SCORE + sr.Applicant_Jamb_Score) / 2),
                     passportUrl = sr.Image_File_Url,
                     Department = sr.Department_Name,
                     ExamNumber = sr.Application_Exam_Number,
                     PersonId = sr.Person_Id,
                     Sex = sr.Sex_Name,
                     State = sr.State_Name,
                     LocalGoverment = sr.Local_Government_Name,
                     OLevelExamType = sr.O_Level_Type_Name,
                     OLevelExamYear = sr.Exam_Year.ToString(),
                     OLevelSitting = sr.O_Level_Exam_Sitting_Id,
                     OLevelSubject = sr.O_Level_Subject_Name,
                     OLevelGrade = sr.O_Level_Grade_Name,
                     SchoolChoice = sr.Institution_Choice_Name

                 }).ToList();
                    var masterList = new List<PutmeSlip>();
                    List<string> regNumbers = applicantResults.Select(r => r.JambNumber).Distinct().ToList();
                    for (int i = 0; i < regNumbers.Count; i++)
                    {
                        List<PutmeSlip> results =
                            applicantResults.Where(r => r.JambNumber == regNumbers[i]).ToList();
                        string firstOLevelType = results[0].OLevelExamType;
                        string firstOLevelYear = results[0].OLevelExamYear;
                        int firstOlevelSitting = results[0].OLevelSitting;
                        string OLevelResults = "";
                        int checkSittingChange = 0;
                        string oLevelType = "";
                        string oLevelYear = "";
                        string sitting = "";
                        string FirstSitting = "";
                        string SecondSitting = "";
                        for (int j = 0; j < results.Count; j++)
                        {
                            if (results[j].OLevelSitting != firstOlevelSitting)
                            {
                                checkSittingChange += 1;
                                results[j].Sitting = "Two Sittings";
                                sitting = results[j].Sitting;
                                results[j].OLevelExamType = firstOLevelType + " | " + results[j].OLevelExamType;
                                oLevelType = results[j].OLevelExamType;
                                results[j].OLevelExamYear = firstOLevelYear + " | " + results[j].OLevelExamYear;
                                oLevelYear = results[j].OLevelExamYear;
                            }
                            else
                            {
                                if (oLevelType != "" && oLevelYear != "" && sitting != "")
                                {
                                    results[j].Sitting = sitting;
                                    results[j].OLevelExamYear = oLevelYear;
                                    results[j].OLevelExamType = oLevelType;
                                }
                                else
                                {
                                    results[j].Sitting = "One Sitting";
                                }
                            }

                        }

                        var firstSittingResult = results.Where(r => r.OLevelSitting == 1).ToList();
                        var secondSittingResult = results.Where(r => r.OLevelSitting == 2).ToList();
                        if (firstSittingResult.Count > 0)
                        {
                            //FirstSitting = firstSittingResult[0].OLevelExamType;
                            for (var m = 0; m < firstSittingResult.Count; m++)
                            {
                                FirstSitting += firstSittingResult[m].OLevelSubject + " : " + firstSittingResult[m].OLevelGrade + ",";
                            }
                        }

                        if (secondSittingResult.Count > 0)
                        {
                            SecondSitting = "==== 2ND RESULT ====> ";
                            for (var k = 0; k < secondSittingResult.Count; k++)
                            {
                                SecondSitting += secondSittingResult[k].OLevelSubject + " : " + secondSittingResult[k].OLevelGrade + ",";
                            }
                        }


                        results.LastOrDefault().OLevelResult = FirstSitting + SecondSitting;
                    }

                    masterList = applicantResults.GroupBy(a => a.JambNumber).Select(a => a.Last()).ToList();

                    return masterList.OrderBy(m => m.JambNumber).ToList();
                }
                else
                {
                    applicantResults =
            (from sr in
                repository.GetBy<VW_PUTME_RESULT_O_LEVEL_SUPPLEMENTARY>(
                    a => a.Department_Name == department.Name && a.Application_Form_Setting_Id == FormSetting && a.Institution_Choice_Id == institutionChoice.Id)
             select new PutmeSlip
             {
                 ApplicationNumber = sr.Application_Form_Number,
                 JambNumber = sr.Applicant_Jamb_Registration_Number,
                 Fullname = sr.Last_Name + " " + sr.First_Name + " " + sr.Other_Name,
                 JambScore = sr.Applicant_Jamb_Score,
                 ExamScore = sr.RAW_SCORE,
                 AverageScore = ((sr.RAW_SCORE + sr.Applicant_Jamb_Score) / 2),
                 passportUrl = sr.Image_File_Url,
                 Department = sr.Department_Name,
                 ExamNumber = sr.Application_Exam_Number,
                 PersonId = sr.Person_Id,
                 Sex = sr.Sex_Name,
                 State = sr.State_Name,
                 LocalGoverment = sr.Local_Government_Name,
                 OLevelExamType = sr.O_Level_Type_Name,
                 OLevelExamYear = sr.Exam_Year.ToString(),
                 OLevelSitting = sr.O_Level_Exam_Sitting_Id,
                 OLevelSubject = sr.O_Level_Subject_Name,
                 OLevelGrade = sr.O_Level_Grade_Name,
                 SchoolChoice = institutionChoice.Name

             }).ToList();
                    var masterList = new List<PutmeSlip>();
                    List<string> regNumbers = applicantResults.Select(r => r.JambNumber).Distinct().ToList();
                    for (int i = 0; i < regNumbers.Count; i++)
                    {
                        List<PutmeSlip> results =
                            applicantResults.Where(r => r.JambNumber == regNumbers[i]).ToList();
                        string firstOLevelType = results[0].OLevelExamType;
                        string firstOLevelYear = results[0].OLevelExamYear;
                        int firstOlevelSitting = results[0].OLevelSitting;
                        string OLevelResults = "";
                        int checkSittingChange = 0;
                        string oLevelType = "";
                        string oLevelYear = "";
                        string sitting = "";
                        string FirstSitting = "";
                        string SecondSitting = "";
                        for (int j = 0; j < results.Count; j++)
                        {
                            if (results[j].OLevelSitting != firstOlevelSitting)
                            {
                                checkSittingChange += 1;
                                results[j].Sitting = "Two Sittings";
                                sitting = results[j].Sitting;
                                results[j].OLevelExamType = firstOLevelType + " | " + results[j].OLevelExamType;
                                oLevelType = results[j].OLevelExamType;
                                results[j].OLevelExamYear = firstOLevelYear + " | " + results[j].OLevelExamYear;
                                oLevelYear = results[j].OLevelExamYear;
                            }
                            else
                            {
                                if (oLevelType != "" && oLevelYear != "" && sitting != "")
                                {
                                    results[j].Sitting = sitting;
                                    results[j].OLevelExamYear = oLevelYear;
                                    results[j].OLevelExamType = oLevelType;
                                }
                                else
                                {
                                    results[j].Sitting = "One Sitting";
                                }
                            }

                        }

                        var firstSittingResult = results.Where(r => r.OLevelSitting == 1).ToList();
                        var secondSittingResult = results.Where(r => r.OLevelSitting == 2).ToList();
                        if (firstSittingResult.Count > 0)
                        {
                            //FirstSitting = firstSittingResult[0].OLevelExamType;
                            for (var m = 0; m < firstSittingResult.Count; m++)
                            {
                                FirstSitting += firstSittingResult[m].OLevelSubject + " : " + firstSittingResult[m].OLevelGrade + ",";
                            }
                        }

                        if (secondSittingResult.Count > 0)
                        {
                            SecondSitting = "==== 2ND RESULT ====> ";
                            for (var k = 0; k < secondSittingResult.Count; k++)
                            {
                                SecondSitting += secondSittingResult[k].OLevelSubject + " : " + secondSittingResult[k].OLevelGrade + ",";
                            }
                        }


                        results.LastOrDefault().OLevelResult = FirstSitting + SecondSitting;
                    }

                    masterList = applicantResults.GroupBy(a => a.JambNumber).Select(a => a.Last()).ToList();

                    return masterList.OrderBy(m => m.JambNumber).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


       public List<PutmeSlip> GetPUTMEReportUnMatched(Department Department)
       {
           try
           {
               var Result = (from a in
                                 repository.GetBy<VW_PUTME_UNMATCHED>(
                                     a => a.COURSE == Department.Name
                                         )
                             select new PutmeSlip
                             {
                                 JambNumber = a.REGNO,
                                 Fullname = a.FULLNAME ,
                                 JambScore = a.JAMB_SCORE,
                                 ExamScore = a.RAW_SCORE,
                                 AverageScore = ((a.RAW_SCORE + a.JAMB_SCORE) / 2),
                                 Department = a.COURSE,
                                 ExamNumber = a.EXAMNO,
                             }).ToList();

               return Result;

           }
           catch (Exception ex)
           {

               throw ex;
           }
       }


        public bool Modify(PutmeResult result)
        {
            try
            {
                Expression<Func<PUTME_RESULT, bool>> selector = s => s.ID == result.Id;
                PUTME_RESULT entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }
                entity.REGNO = result.RegNo;
                entity.EXAMNO = result.ExamNo;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool Modify(PutmeResult result, PutmeResultAudit resultAudit)
        {
            try
            {
                Expression<Func<PUTME_RESULT, bool>> selector = p => p.ID == result.Id;
                PUTME_RESULT resultEntity = GetEntityBy(selector);

                bool audited = CreateAudit(result, resultAudit, resultEntity);
                if (audited)
                {
                    if (resultEntity == null || resultEntity.ID <= 0)
                    {
                        throw new Exception(NoItemFound);
                    }

                    resultEntity.REGNO = result.RegNo;
                    resultEntity.EXAMNO = result.ExamNo;
                    resultEntity.FULLNAME = result.FullName;
                    resultEntity.TOTAL = result.Total;

                    int modifiedRecordCount = Save();
                    if (modifiedRecordCount <= 0)
                    {
                        return false;
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool CreateAudit(PutmeResult result, PutmeResultAudit audit, PUTME_RESULT resultEntity)
        {
            try
            {
                if (result.Id == resultEntity.ID)
                {
                    audit.Result_Id = result.Id;
                    audit.New_RegNo = result.ExamNo;
                    PutmeResult oldResult = translator.Translate(resultEntity);
                    audit.Old_RegNo = oldResult.ExamNo;

                    var resultAuditLogic = new PostUtmeResultAuditLogic();
                    PutmeResultAudit personAudit = resultAuditLogic.Create(audit);
                    if (personAudit == null || personAudit.Id <= 0)
                    {
                        return false;
                    }

                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}