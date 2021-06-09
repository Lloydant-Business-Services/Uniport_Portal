using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class BankItRequest
    {
        public string TerminalId { get; set; }
        public string TransactionId { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string ResponseUrl { get; set; }
        public string NotificationUrl { get; set; }
        public string Checksum { get; set; }
        public string LogoUrl { get; set; }
        public string SecretKey { get; set; }
        public string Department { get; set; }
        public string Programme { get; set; }
        public string FeeType { get; set; }
        public string Session { get; set; }
        public string MatricNumber { get; set; }
        public string FullName { get; set; }
    }
}
