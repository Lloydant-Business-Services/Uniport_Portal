using Abundance_Nk.Model.Model;
using System.Collections.Generic;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class TranscriptBursarViewModel
    {
        public TranscriptBursarViewModel()
        {
            transcriptRequest = new TranscriptRequest();
        }

        public List<TranscriptRequest> transcriptRequests { get; set; }
        public TranscriptRequest transcriptRequest { get; set; }
        public TranscriptStatus transcriptStatus { get; set; }
        public TranscriptClearanceStatus transcriptClearanceStatus { get; set; }
        public List<RemitaPayment> remitaPayments { get; set; }
    }
}