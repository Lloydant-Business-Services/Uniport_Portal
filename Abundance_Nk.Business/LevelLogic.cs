using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class LevelLogic : BusinessBaseLogic<Level, LEVEL>
    {
        public LevelLogic()
        {
            translator = new LevelTranslator();
        }

        public List<Level> GetONDs()
        {
            try
            {
                Expression<Func<LEVEL, bool>> selector = l => l.Level_Id <= 2;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Level> GetHNDs()
        {
            try
            {
                Expression<Func<LEVEL, bool>> selector = l => l.Level_Id > 2;
                return base.GetModelsBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Modify(Level level)
        {
            try
            {
                Expression<Func<LEVEL, bool>> selector = f => f.Level_Id == level.Id;
                LEVEL entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Level_Name = level.Name;
                entity.Level_Description = level.Description;

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