using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ScratchCardAuditLogic : BusinessBaseLogic<ScratchCardAudit, SCRATCH_CARD_AUDIT>
    {
        public ScratchCardAuditLogic()
        {
            translator = new ScratchCardAuditTranslator();
        }
    }
}
