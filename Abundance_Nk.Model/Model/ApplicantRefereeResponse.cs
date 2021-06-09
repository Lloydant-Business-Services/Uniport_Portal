using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicantRefereeResponse
    {
        public long RefereeResponseId { get; set; }
        public decimal DurationKnownApplicant { get; set; }
        public string Remark { get; set; }
        public bool CanAcceptApplicant { get; set; }
        public string RelevantInformation { get; set; }
        public bool DiscloseResponse { get; set; }
        public string FullName { get; set; }
        public string SignatureUrl { get; set; }
        public DateTime DateOfResponse { get; set; }
        public ApplicantReferee ApplicantReferee { get; set; }
        public RefereeGradingSystem RefereeGradingSystem { get; set; }
    }
}
