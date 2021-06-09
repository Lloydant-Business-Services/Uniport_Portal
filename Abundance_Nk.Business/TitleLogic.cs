using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class TitleLogic : BusinessBaseLogic<Title, TITLE>
    {
        public TitleLogic()
        {
            translator = new TitleTranslator();
        }
    }
}