using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class InstitutionTypeLogic : BusinessBaseLogic<InstitutionType, INSTITUTION_TYPE>
    {
        public InstitutionTypeLogic()
        {
            translator = new InstitutionTypeTranslator();
        }

        public bool Modify(InstitutionType institutionType)
        {
            try
            {
                Expression<Func<INSTITUTION_TYPE, bool>> selector = s => s.Institution_Type_Id == institutionType.Id;
                INSTITUTION_TYPE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Institution_Type_Name = institutionType.Name;
                entity.Institution_Type_Description = institutionType.Description;

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