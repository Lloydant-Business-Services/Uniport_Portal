using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ShortFallLogic : BusinessBaseLogic<ShortFall, SHORT_FALL>
    {
        public ShortFallLogic()
        {
            translator = new ShortFallTranslator();
        }
    }
}