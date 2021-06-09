using System;

namespace Abundance_Nk.Model.Model
{
    public class OnlinePayment
    {
        public Payment Payment { get; set; }
        public PaymentChannel Channel { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}