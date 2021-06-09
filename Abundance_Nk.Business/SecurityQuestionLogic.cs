using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class SecurityQuestionLogic : BusinessBaseLogic<SecurityQuestion, SECURITY_QUESTION>
    {
        public SecurityQuestionLogic()
        {
            translator = new SecurityQuestionTranslator();
        }
    }
}