using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class BloodGroupLogic : BusinessBaseLogic<BloodGroup, BLOOD_GROUP>
    {
        public BloodGroupLogic()
        {
            translator = new BloodGroupTranslator();
        }
    }
}