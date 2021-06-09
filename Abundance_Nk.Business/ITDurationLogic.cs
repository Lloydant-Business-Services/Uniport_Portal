using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ITDurationLogic : BusinessBaseLogic<ITDuration, IT_DURATION>
    {
        public ITDurationLogic()
        {
            translator = new ITDurationTranslator();
        }
    }
}