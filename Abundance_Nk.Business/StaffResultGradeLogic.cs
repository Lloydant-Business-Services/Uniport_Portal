
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Translator;
using System.Linq.Expressions;

namespace Abundance_Nk.Business
{
    public class StaffResultGradeLogic : BusinessBaseLogic<StaffResultGrade, STAFF_RESULT_GRADE>
    {
        public StaffResultGradeLogic()
        {
            translator = new StaffResultGradeTranslator();
        }

        public bool Modify(StaffResultGrade resultGrade)
        {
            try
            {
                Expression<Func<STAFF_RESULT_GRADE, bool>> selector = r => r.Staff_Result_Grade_Id == resultGrade.Id;
                STAFF_RESULT_GRADE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Staff_Result_Grade_Id = resultGrade.Id;

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
