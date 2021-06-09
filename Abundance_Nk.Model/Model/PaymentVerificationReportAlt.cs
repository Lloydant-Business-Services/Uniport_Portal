﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Model.Model
{
    public class PaymentVerificationReportAlt
    {

        public string ReceiptNumber { get; set; }
        public string Serial { get; set; }
        public string Department { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentType { get; set; }
        public string Level { get; set; }
        public string Programme { get; set; }
        public string FeeType { get; set; }
        public string StudentName { get; set; }
        public string PaymentReference { get; set; }
        public string Session { get; set; }
        public string PaymentAmount { get; set; }
        public string VerificationOfficer { get; set; }
        public DateTime DateVerified { get; set; }
        public DateTime? DatePaid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long UserId { get; set; }
        public string MatricNumber { get; set; }


    }
}
