using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class OLevelTypeLogic : BusinessBaseLogic<OLevelType, O_LEVEL_TYPE>
    {
        public OLevelTypeLogic()
        {
            translator = new OLevelTypeTranslator();
        }

        public bool Modify(OLevelType oLevelType)
        {
            try
            {
                Expression<Func<O_LEVEL_TYPE, bool>> selector = o => o.O_Level_Type_Id == oLevelType.Id;
                O_LEVEL_TYPE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.O_Level_Type_Name = oLevelType.Name;
                entity.O_Level_Type_Description = oLevelType.Description;

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