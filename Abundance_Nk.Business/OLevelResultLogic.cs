using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class OLevelResultLogic : BusinessBaseLogic<OLevelResult, APPLICANT_O_LEVEL_RESULT>
    {
        public OLevelResultLogic()
        {
            translator = new OLevelResultTranslator();
        }

        public bool Modify(OLevelResult oLevelResult)
        {
            try
            {
                Expression<Func<APPLICANT_O_LEVEL_RESULT, bool>> selector =
                    o => o.Applicant_O_Level_Result_Id == oLevelResult.Id;
                APPLICANT_O_LEVEL_RESULT entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (oLevelResult.Person != null && oLevelResult.Person.Id > 0)
                {
                    entity.Person_Id = oLevelResult.Person.Id;
                }
                if (oLevelResult.ExamNumber != null)
                {
                    entity.Exam_Number = oLevelResult.ExamNumber;
                }
                if (oLevelResult.ExamYear != null)
                {
                    entity.Exam_Year = oLevelResult.ExamYear;
                }
                if (oLevelResult.Sitting != null && oLevelResult.Sitting.Id > 0)
                {
                    entity.O_Level_Exam_Sitting_Id = oLevelResult.Sitting.Id;
                }
                if (oLevelResult.Type != null && oLevelResult.Type.Id > 0)
                {
                    entity.O_Level_Type_Id = oLevelResult.Type.Id;
                }
                if (oLevelResult.ApplicationForm != null && oLevelResult.ApplicationForm.Id > 0)
                {
                    entity.Application_Form_Id = oLevelResult.ApplicationForm.Id;
                }
                entity.Scanned_Copy_Url = oLevelResult.ScannedCopyUrl;

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
        public List<OlevelSitting> GetOlevelFirstSettingsBy(List<AdmittedStudentDataView> admittedStudentResult)
        {
            var olevelResultListList = new List<OlevelSitting>();
            try
            {
                if (admittedStudentResult.Count > 0)
                {
                    var firstSittingList = admittedStudentResult.Where(s => s.OLevelExamSittingId == 1).ToList();
                    for (int i = 0; i < firstSittingList.Count; i++)
                    {
                        OlevelSitting olevelSitting = new OlevelSitting();
                        olevelSitting.ExamNumber = firstSittingList[i].ExamNumber;
                        olevelSitting.ExamYear = firstSittingList[i].ExamYear.ToString();
                        olevelSitting.OLevelExamSittingId = firstSittingList[i].OLevelExamSittingId;
                        olevelSitting.OLevelExamSittingName = firstSittingList[i].OLevelExamSittingName;
                        olevelSitting.OLevelSubjectName = firstSittingList[i].OLevelSubjectName;
                        olevelSitting.OLevelGradeName = firstSittingList[i].OLevelGradeName;
                        olevelSitting.OLevelTypeName = firstSittingList[i].OLevelTypeName;

                        olevelResultListList.Add(olevelSitting);
                    }
                }

            }
            catch (Exception ex)
            {
                
                throw;
            }
            return olevelResultListList;
        }
        public List<OlevelSitting> GetOlevelSecondSettingsBy(List<AdmittedStudentDataView> admittedStudentResult)
        {
            var olevelResultListList = new List<OlevelSitting>();
            try
            {
                if (admittedStudentResult.Count > 0)
                {
                    var secondSittingList = admittedStudentResult.Where(s => s.OLevelExamSittingId == 2).ToList();
                    for (int i = 0; i < secondSittingList.Count; i++)
                    {
                        OlevelSitting olevelSitting = new OlevelSitting();
                        olevelSitting.ExamNumber = secondSittingList[i].ExamNumber;
                        olevelSitting.ExamYear = secondSittingList[i].ExamYear.ToString();
                        olevelSitting.OLevelExamSittingId = secondSittingList[i].OLevelExamSittingId;
                        olevelSitting.OLevelExamSittingName = secondSittingList[i].OLevelExamSittingName;
                        olevelSitting.OLevelSubjectName = secondSittingList[i].OLevelSubjectName;
                        olevelSitting.OLevelGradeName = secondSittingList[i].OLevelGradeName;
                        olevelSitting.OLevelTypeName = secondSittingList[i].OLevelTypeName;

                        olevelResultListList.Add(olevelSitting);
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return olevelResultListList;
        } 

    }
}