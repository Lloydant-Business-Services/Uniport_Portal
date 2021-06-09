using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StudentResultTypeLogic : BusinessBaseLogic<StudentResultType, STUDENT_RESULT_TYPE>
    {
        public StudentResultTypeLogic()
        {
            translator = new StudentResultTypeTranslator();
        }

        public StudentResultType GetBy(int id)
        {
            try
            {
                Expression<Func<STUDENT_RESULT_TYPE, bool>> selector = s => s.Student_Result_Type_Id == id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}