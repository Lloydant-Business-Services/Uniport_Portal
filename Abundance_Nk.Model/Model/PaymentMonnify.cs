using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    [DataContract()]
    public class PaymentMonnify
    {
        public Payment Payment { get; set; }
        [DataMember(Name = "invoiceReference")]
        public string InvoiceReference { get; set; }
        [DataMember(Name = "paymentMethod")]
        public string PaymentMethod { get; set; }
        [DataMember(Name = "createdOn")]
        public DateTime DateCreated { get; set; }
        [DataMember(Name = "amount")]
        public Decimal Amount { get; set; }
        [DataMember(Name = "fee")]
        public Decimal? Fee { get; set; }
        [DataMember(Name = "invoiceStatus")]
        public string InvoiceStatusText { get; set; }

        public bool InvoiceStatus { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "expiryDate")]
        public DateTime? ExpiryDate { get; set; }
        [DataMember(Name = "checkoutUrl")]
        public string CheckoutUrl { get; set; }
        [DataMember(Name = "accountNumber")]
        public string AccountNumber { get; set; }
        [DataMember(Name = "accountName")]
        public string AccountName { get; set; }
        [DataMember(Name = "bankName")]
        public string BankName { get; set; }
        [DataMember(Name = "bankCode")]
        public string BankCode { get; set; }
        [DataMember(Name = "payableAmount")]
        public Decimal PayableAmount { get; set; }
        [DataMember(Name = "amountPaid")]
        public Decimal AmountPaid { get; set; }
        [DataMember(Name = "completed")]
        public bool Completed { get; set; }
        [DataMember(Name = "completedOn")]
        public DateTime? CompletedOn { get; set; }
        [DataMember(Name = "transactionReference")]
        public string TransactionReference { get; set; }
        [DataMember(Name = "transactionHash")]
        public string TransactionHash { get; set; }
        
    }
    public class MonnifySplit
    {
        public string subAccountCode { get; set; }
        public string splitAmount { get; set; }
    }

    public class MonnifyResponse
    {
        public bool status { get; set; }
        public string description { get; set; }
        public PaymentMonnify PaymentMonnify { get; set; }
    }
    public class MonnifyJsonData
    {
        public string amount { get; set; }
        public string invoiceReference { get; set; }
        public string description { get; set; }
        public string currencyCode { get; set; }
        public string contractCode { get; set; }
        public string customerEmail { get; set; }
        public string customerName { get; set; }
        public string expiryDate { get; set; }
        public List<MonnifySplit> incomeSplitConfig { get; set; } = new List<MonnifySplit>();

    }
    public class JsonResultData
    {
        public bool requestSuccessful { get; set; }
        public string responseMessage { get; set; }
        public string responseCode { get; set; }
        public PaymentMonnify responseBody { get; set; }
    }
    public class MonnifyReturnResponse
    {
        public string transactionReference { get; set; }
        public string paymentReference { get; set; }
        public string amountPaid { get; set; }
        public string totalPayable { get; set; }
        public DateTime paidOn { get; set; }
        public bool paymentStatus { get; set; }
        public string accountReference { get; set; }
        public string paymentDescription { get; set; }
        public string transactionHash { get; set; }
    }
}
