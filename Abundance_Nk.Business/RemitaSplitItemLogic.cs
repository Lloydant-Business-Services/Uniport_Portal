using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class RemitaSplitItemLogic : BusinessBaseLogic<RemitaSplitItems, REMITA_SPLIT_DETAILS>
    {
        public RemitaSplitItemLogic()
        {
            translator = new RemitaSplitItemsTranslator();
        }


        public RemitaSplitItems GetBy(long Id)
        {
            try
            {
                Expression<Func<REMITA_SPLIT_DETAILS, bool>> selector = a => a.Id == Id;
                RemitaSplitItems remitaSettings = GetModelBy(selector);
                return remitaSettings;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}