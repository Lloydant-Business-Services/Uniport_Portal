using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class SexLogic : BusinessBaseLogic<Sex, SEX>
    {
        public SexLogic()
        {
            base.translator = new SexTranslator();
        }
    }
}