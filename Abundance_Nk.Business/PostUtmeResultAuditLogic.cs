using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class PostUtmeResultAuditLogic : BusinessBaseLogic<PutmeResultAudit, PUTME_RESULT_AUDIT>
    {
        public PostUtmeResultAuditLogic()
        {
            translator = new PutmeResultAuditTranslator();
        }
    }
}