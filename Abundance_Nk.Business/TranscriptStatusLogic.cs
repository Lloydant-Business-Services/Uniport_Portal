using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class TranscriptStatusLogic : BusinessBaseLogic<TranscriptStatus, TRANSCRIPT_STATUS>
    {
        public TranscriptStatusLogic()
        {
            translator = new TranscriptStatusTranslator();
        }
    }
}