using System;

namespace Abundance_Nk.Model.Model
{
    public class ApplicantClearance
    {
        public ApplicationForm ApplicationForm { get; set; }
        public bool Cleared { get; set; }
        public DateTime DateCleared { get; set; }
    }
}