using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ApplicantStatusLogic : BusinessBaseLogic<ApplicantStatus, APPLICANT_STATUS>
    {
        public ApplicantStatusLogic()
        {
            translator = new ApplicantStatusTranslator();
        }
    }
}