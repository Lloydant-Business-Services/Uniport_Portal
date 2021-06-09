using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentMonnifyCommission
    {
        public int Id { get; set; }
        public FeeType FeeType { get; set; }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public Level Level { get; set; }
        public bool UseLevel { get; set; }
        public decimal Commission { get; set; }
    }
}
