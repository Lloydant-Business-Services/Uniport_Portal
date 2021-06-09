using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentResultTypeTranslator : TranslatorBase<StudentResultType, STUDENT_RESULT_TYPE>
    {
        public override StudentResultType TranslateToModel(STUDENT_RESULT_TYPE entity)
        {
            try
            {
                StudentResultType model = null;
                if (entity != null)
                {
                    model = new StudentResultType();
                    model.Id = entity.Student_Result_Type_Id;
                    model.Name = entity.Student_Result_Type_Name;
                    model.Description = entity.Student_Result_Type_Description;

                    //model.MaximumScoreObtainable = entity.Maximum_Score_Obtainable;
                    //model.MaximumPercentScoreObtainable = entity.Maximum_Percent_Score_Obtainable;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_RESULT_TYPE TranslateToEntity(StudentResultType model)
        {
            try
            {
                STUDENT_RESULT_TYPE entity = null;
                if (model != null)
                {
                    entity = new STUDENT_RESULT_TYPE();
                    entity.Student_Result_Type_Id = model.Id;
                    entity.Student_Result_Type_Name = model.Name;
                    entity.Student_Result_Type_Description = model.Description;

                    //entity.Maximum_Score_Obtainable = model.MaximumScoreObtainable;
                    //entity.Maximum_Percent_Score_Obtainable = model.MaximumPercentScoreObtainable;
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