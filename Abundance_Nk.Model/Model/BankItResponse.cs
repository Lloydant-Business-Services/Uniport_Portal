using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class BankItResponse
    {
        public string TransactionId { get; set; }
        public string TransactionRef { get; set; }
        public decimal Amount { get; set; }
        public string FinalChecksum { get; set; }
        public string Description { get; set; }
        public string TerminalId { get; set; }
        public string Success { get; set; }
        public string SecretKey { get; set; }
        public string ResponseUrl { get; set; }
    }
}
