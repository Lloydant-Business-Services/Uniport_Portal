using Abundance_Nk.Model.Model;
using System.Collections.Generic;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class VerificationViewModel
    {
        public PaymentEtranzact PaymentEtranzact { get; set; }
        public PaymentVerification PaymentVerification { get; set; }
        public List<PaymentVerification> PaymentVerifications { get; set; }
        public Receipt receipt { get; set; }
    }
}