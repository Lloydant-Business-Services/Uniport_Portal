using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class GenotypeLogic : BusinessBaseLogic<Genotype, GENOTYPE>
    {
        public GenotypeLogic()
        {
            translator = new GenotypeTranslator();
        }
    }
}