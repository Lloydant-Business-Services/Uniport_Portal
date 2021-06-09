using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicantRefereeGradingResponse
    {
        public long Id { get; set; }
        public RefereeGradingCategory RefereeGradingCategory { get; set; }
        public RefereeGradingSystem RefereeGradingSystem { get; set; }
        public ApplicantRefereeResponse ApplicantRefereeResponse { get; set; }
    }
}
