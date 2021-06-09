using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class AbilityLogic : BusinessBaseLogic<Ability, ABILITY>
    {
        public AbilityLogic()
        {
            translator = new AbilityTranslator();
        }

    }
}