using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class MaritalStatusLogic : BusinessBaseLogic<MaritalStatus, MARITAL_STATUS>
    {
        public MaritalStatusLogic()
        {
            translator = new MaritalStatusTranslator();
        }
    }
}