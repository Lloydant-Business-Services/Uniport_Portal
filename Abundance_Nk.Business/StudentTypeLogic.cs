using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StudentTypeLogic : BusinessBaseLogic<StudentType, STUDENT_TYPE>
    {
        public StudentTypeLogic()
        {
            translator = new StudentTypeTranslator();
        }

        public bool Modify(StudentType studentType)
        {
            try
            {
                Expression<Func<STUDENT_TYPE, bool>> selector = s => s.Student_Type_Id == studentType.Id;
                STUDENT_TYPE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Student_Type_Name = studentType.Name;
                entity.Student_Type_Description = studentType.Description;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}