using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentClearance
    {
        public string SessionName { get; set; }
        public string Feetype { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
