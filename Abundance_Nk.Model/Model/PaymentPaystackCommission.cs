using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentPaystackCommission
    {
        public int Id { get; set; }
        public FeeType FeeType { get; set; }
        //public Fee Fee { get; set; }
        public Session Session { get; set; }
        public bool Activated { get; set; }
        public decimal Amount { get; set; }
        public Programme Programme { get; set; }
        public decimal? AddOnFee { get; set; }
        public User User { get; set; }
    
    }
}
