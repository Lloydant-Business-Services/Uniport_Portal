using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ProcessedSupplementaryReport
    {
        public string JambRegNumber { get; set; }

        public short Applicant_Jamb_Score { get; set; }

        public string Name { get; set; }

        public double? RAW_SCORE { get; set; }
        public string DEPARTMENT { get; set; }

        public string Session_Name { get; set; }

        public string Institution_Choice_Name { get; set; }

        public string State_Name { get; set; }

        public string Local_Government_Name { get; set; }

        public string Sex_Name { get; set; }

        public decimal? COMPUTED_JAMB_SCORE { get; set; }

        public double? COMPUTED_PUTME_SCORE { get; set; }

        public double? SUM { get; set; }
        public string Qualification1 { get; set; }
        public string Qualification2 { get; set; }
    }
}
