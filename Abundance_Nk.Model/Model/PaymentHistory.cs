using System.Collections.Generic;

namespace Abundance_Nk.Model.Model
{
    public class PaymentHistory
    {
        public Student Student { get; set; }
        public List<PaymentView> Payments { get; set; }
        public StudentLevel StudentLevel { get; set; }

        public bool IsError { get; set; }

        public string ErrorMessage { get; set; }
        public List<PaymentView> PreviousPayments { get; set; }
    }
}