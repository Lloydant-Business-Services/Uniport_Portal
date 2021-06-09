using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentPaystackCommissionAudit
    {
        public int Id { get; set; }
        public FeeType FeeType { get; set; }
        //public Fee Fee { get; set; }
        public Session Session { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public PaymentPaystackCommission PaymentPaystackCommission { get; set; }
        public string Action { get; set; }
        public string Operation { get; set; }
        public string Client { get; set; }
        public User User { get; set; }
        public Programme Programme { get; set; }

    }
}
