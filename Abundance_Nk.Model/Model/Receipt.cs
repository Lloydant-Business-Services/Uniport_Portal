using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ZXing;
using ZXing.Common;

namespace Abundance_Nk.Model.Model
{
    public class Receipt
    {
        [Display(Name = "Serial Number")]
        public string Number { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Confirmation Order Number")]
        public string ConfirmationOrderNumber { get; set; }

        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Amount In Words")]
        public string AmountInWords { get; set; }

        [Display(Name = "Purpose")]
        public string Purpose { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        public string ApplicationFormNumber { get; set; }

        [Display(Name = "Mode of Payment")]
        public string PaymentMode { get; set; }
        
        [Display(Name = "Receipt Number")]
        public string ReceiptNumber { get; set; }
        public string MatricNumber { get; set; }
        public string QRVerification { get; set; }
        public string Session { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string Programme { get; set; }
        public string Faculty { get; set; }

        public string PaymentId { get; set; }
        public bool IsInterswitchPayment { get; set; }
        public bool IsMonnify { get; set; }
        public string TicketId { get; set; }
        public string Status { get; set; }
        public string RefereeNo { get; set; }
        
    }
    public class ELearningEmail
    {
        public string Name { get; set; }
        public string message { get; set; }
        public string header { get; set; }
        public string footer { get; set; }
    }

}