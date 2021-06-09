using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class TranscriptClearanceStatus
    {
        public int TranscriptClearanceStatusId { get; set; }

        [Display(Name = "Clearance Status")]
        public string TranscriptClearanceStatusName { get; set; }

        public string TranscriptClearanceStatusDescription { get; set; }
    }
}