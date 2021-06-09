using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Abundance_Nk.Web.Areas.Applicant.ViewModels
{
    public class InterswitchResponse
    {
        [Display(Name = "Transaction Reference(Invoice Number)")]
        public string txnRef { get; set; }

        [Display(Name = "Payment Reference")]
        public string payRef { get; set; }

        [Display(Name = "Bank Reference")]
        public string retRef { get; set; }

        [Display(Name = "Card Number")]
        public string cardNum { get; set; }

        [Display(Name = "Amount")]
        public string apprAmt { get; set; }

        [Display(Name = "Amount Paid")]
        public int? amt { get; set; }

        [Display(Name = "Response Code")]
        public string rspcode { get; set; }

        [Display(Name = "Response")]
        public string resp { get; set; }

        [Display(Name = "Response Description")]
        public string desc { get; set; }

        [Display(Name = "Date Time")]
        public string dtime { get; set; }

    }

    public class CustomerInformationRequest
    {
        public string ServiceUrl { get; set; }
        public string ServiceUsername { get; set; }
        public string ServicePassword { get; set; }
        public string MerchantReference { get; set; }
        public string CustReference { get; set; }
        public string PaymentItemCategoryCode { get; set; }
        public string PaymentItemCode { get; set; }
        public string TerminalId { get; set; }
        public string Amount { get; set; }
        public string FTPUrl { get; set; }
        public string FTPUsername { get; set; }
        public string FTPPassword { get; set; }

    }
}
