using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class AdmissionListAuditLogic : BusinessBaseLogic<AdmissionListAudit, ADMISSION_LIST_AUDIT>
    {
        public AdmissionListAuditLogic()
        {
            translator = new AdmissionListAuditTranslator();
        }
    }
}