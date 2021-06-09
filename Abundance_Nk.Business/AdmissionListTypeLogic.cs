using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class AdmissionListTypeLogic : BusinessBaseLogic<AdmissionListType, ADMISSION_LIST_TYPE>
    {
        public AdmissionListTypeLogic()
        {
            translator = new AdmissionListTypeTranslator();
        }
    }
}