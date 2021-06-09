//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Abundance_Nk.Model.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class REMITA_PAYMENT
    {
        public long Payment_Id { get; set; }
        public string RRR { get; set; }
        public string OrderId { get; set; }
        public string Status { get; set; }
        public string Receipt_No { get; set; }
        public string Description { get; set; }
        public string Merchant_Code { get; set; }
        public decimal Transaction_Amount { get; set; }
        public System.DateTime Transaction_Date { get; set; }
        public string Confirmation_No { get; set; }
        public string Bank_Code { get; set; }
        public string Branch_Code { get; set; }
        public string Customer_Id { get; set; }
        public string Customer_Name { get; set; }
        public string Customer_Address { get; set; }
        public Nullable<bool> Used { get; set; }
        public Nullable<long> Used_By_Person_Id { get; set; }
    
        public virtual PAYMENT PAYMENT { get; set; }
    }
}
