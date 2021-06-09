using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class SemesterLogic : BusinessBaseLogic<Semester, SEMESTER>
    {
        public SemesterLogic()
        {
            translator = new SemesterTranslator();
        }

        public bool Modify(Semester semester)
        {
            try
            {
                Expression<Func<SEMESTER, bool>> selector = s => s.Semester_Id == semester.Id;
                SEMESTER entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Semester_Name = semester.Name;

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