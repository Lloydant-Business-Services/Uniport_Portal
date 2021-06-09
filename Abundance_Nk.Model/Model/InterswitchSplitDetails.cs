using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class InterswitchSplitDetails
    {
        public int Id { get; set; }
        public string BankCode { get; set; }
        public string BeneficiaryAccount { get; set; }
        public string BeneficiaryName { get; set; }
        public decimal BeneficiaryAmount { get; set; }
        public bool Activated { get; set; }
        public virtual FeeType FeeType { get; set; }
    }
}
