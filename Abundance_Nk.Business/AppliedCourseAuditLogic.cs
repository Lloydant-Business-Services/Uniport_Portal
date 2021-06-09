using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class AppliedCourseAuditLogic : BusinessBaseLogic<AppliedCourseAudit, APPLICANT_APPLIED_COURSE_AUDIT>
    {
        public AppliedCourseAuditLogic()
        {
            translator = new AppliedCourseAuditTranslator();
        }
    }
}