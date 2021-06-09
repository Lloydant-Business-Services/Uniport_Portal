using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Translator
{
    public class StaffResultGradeTranslator : TranslatorBase<StaffResultGrade, STAFF_RESULT_GRADE>
    {
        public override StaffResultGrade TranslateToModel(STAFF_RESULT_GRADE entity)
        {
            try
            {
                StaffResultGrade resultGrade = null;
                if (entity != null)
                {
                    resultGrade = new StaffResultGrade();
                    resultGrade.Id = entity.Staff_Result_Grade_Id;
                    
                }

                return resultGrade;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STAFF_RESULT_GRADE TranslateToEntity(StaffResultGrade resultGrade)
        {
            try
            {
                STAFF_RESULT_GRADE entity = null;
                if (resultGrade != null)
                {
                    entity = new STAFF_RESULT_GRADE();
                    entity.Staff_Result_Grade_Id = resultGrade.Id;
                    entity.Grade = resultGrade.Grade;
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
