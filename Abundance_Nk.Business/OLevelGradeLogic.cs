using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class OLevelGradeLogic : BusinessBaseLogic<OLevelGrade, O_LEVEL_GRADE>
    {
        public OLevelGradeLogic()
        {
            translator = new OLevelGradeTranslator();
        }

        public bool Modify(OLevelGrade oLevelGrade)
        {
            try
            {
                Expression<Func<O_LEVEL_GRADE, bool>> selector = o => o.O_Level_Grade_Id == oLevelGrade.Id;
                O_LEVEL_GRADE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.O_Level_Grade_Name = oLevelGrade.Name;
                entity.O_Level_Grade_Description = oLevelGrade.Description;

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