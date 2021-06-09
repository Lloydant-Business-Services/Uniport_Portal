using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentInterswitch
    {
      public Payment Payment { get; set; }
      public int  Amount { get; set; }
      public string  CardNumber { get; set; }
      public string  MerchantReference { get; set; }
      public string  PaymentReference { get; set; }
      public string  RetrievalReferenceNumber { get; set; }
      public string  LeadBankCbnCode { get; set; }
      public string  LeadBankName { get; set; }
      public string[]  SplitAccounts { get; set; }
      public DateTime  TransactionDate { get; set; }
      [Display(Name = "Response Code")]
      public string  ResponseCode { get; set; }
      [Display(Name = "Response Code Description")]
      public string  ResponseDescription { get; set; }
      public string MacKey { get; set; }
      public int ProductId { get; set; }
      public  int PaymentItemId { get; set; }
      public string ResponseApi { get; set; }
      public string ReturnUrl { get; set; }
      public string PaymentHash { get; set; }
      public string PaymentItemName { get; set; }
      public string XmlDataForSplit { get; set; }
      public string PaymentItemIdNoSplit { get; set; }
      public string MacKeyNosplit { get; set; }
       public string PayementHashNoSplit { get; set; }
       public long Id { get; set; }
    }
   
}
