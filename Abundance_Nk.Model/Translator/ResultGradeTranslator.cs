using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class ResultGradeTranslator : TranslatorBase<ResultGrade, RESULT_GRADE>
    {
        public override ResultGrade TranslateToModel(RESULT_GRADE entity)
        {
            try
            {
                ResultGrade resultGrade = null;
                if (entity != null)
                {
                    resultGrade = new ResultGrade();
                    resultGrade.Id = entity.Result_Grade_Id;
                    resultGrade.CGPAFrom = entity.CGPA_From;
                    resultGrade.CGPATo = entity.CGPA_To;
                    resultGrade.LevelOfPassCode = entity.Level_of_Pass_Code;
                    resultGrade.LevelOfPass = entity.Level_of_Pass;
                }

                return resultGrade;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override RESULT_GRADE TranslateToEntity(ResultGrade resultGrade)
        {
            try
            {
                RESULT_GRADE entity = null;
                if (resultGrade != null)
                {
                    entity = new RESULT_GRADE();
                    entity.Result_Grade_Id = resultGrade.Id;
                    entity.CGPA_From = resultGrade.CGPAFrom;
                    entity.CGPA_To = resultGrade.CGPATo;
                    entity.Level_of_Pass_Code = resultGrade.LevelOfPassCode;
                    entity.Level_of_Pass = resultGrade.LevelOfPass;
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}