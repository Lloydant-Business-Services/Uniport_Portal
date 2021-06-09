using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class TranscriptClearanceStatusLogic :
        BusinessBaseLogic<TranscriptClearanceStatus, TRANSCRIPT_CLEARANCE_STATUS>
    {
        public TranscriptClearanceStatusLogic()
        {
            translator = new TranscriptClearanceStatusTranslator();
        }
    }
}