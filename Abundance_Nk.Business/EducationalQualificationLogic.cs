using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class EducationalQualificationLogic : BusinessBaseLogic<EducationalQualification, EDUCATIONAL_QUALIFICATION>
    {
        public EducationalQualificationLogic()
        {
            translator = new EducationalQualificationTranslator();
        }
    }
}