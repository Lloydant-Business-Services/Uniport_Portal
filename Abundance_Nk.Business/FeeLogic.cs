using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class FeeLogic : BusinessBaseLogic<Fee, FEE>
    {
        public FeeLogic()
        {
            translator = new FeeTranslator();
        }

        public bool Modify(Fee fee)
        {
            try
            {
                Expression<Func<FEE, bool>> selector = f => f.Fee_Id == fee.Id;
                FEE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Fee_Name = fee.Name;
                entity.Fee_Description = fee.Description;
                entity.Amount = fee.Amount;

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