using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class InstitutionChoiceLogic : BusinessBaseLogic<InstitutionChoice, INSTITUTION_CHOICE>
    {
        public InstitutionChoiceLogic()
        {
            translator = new InstitutionChoiceTranslator();
        }
    }
}