using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class JambRecordLogic : BusinessBaseLogic<JambRecord, JAMB_RECORD>
    {
        public JambRecordLogic()
        {
            translator = new JambRecordTranslator();
        }
    }
}