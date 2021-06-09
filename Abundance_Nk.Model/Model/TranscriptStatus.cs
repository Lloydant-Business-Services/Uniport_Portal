using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class TranscriptStatus
    {
        public int TranscriptStatusId { get; set; }

        [Display(Name = "Transcript Status")]
        public string TranscriptStatusName { get; set; }

        public string TranscriptStatusDescription { get; set; }
    }
}