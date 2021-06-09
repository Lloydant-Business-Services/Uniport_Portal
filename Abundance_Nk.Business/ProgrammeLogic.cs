using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ProgrammeLogic : BusinessBaseLogic<Programme, PROGRAMME>
    {
        public ProgrammeLogic()
        {
            translator = new ProgrammeTranslator();
        }

        public bool Modify(Programme programme)
        {
            try
            {
                Expression<Func<PROGRAMME, bool>> selector = p => p.Programme_Id == programme.Id;
                PROGRAMME entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Programme_Name = programme.Name;
                entity.Programme_Description = programme.Description;

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