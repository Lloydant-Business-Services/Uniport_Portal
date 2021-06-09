using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StudentUpdateAuditLogic : BusinessBaseLogic<StudentUpdateAudit, STUDENT_UPDATE_AUDIT>
    {
        public StudentUpdateAuditLogic()
        {
            translator = new StudentUpdateAuditTranslator();
        }
    }
}