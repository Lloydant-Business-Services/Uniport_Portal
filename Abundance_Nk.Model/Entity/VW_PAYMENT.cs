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
    
    public partial class VW_PAYMENT
    {
        public long Payment_Id { get; set; }
        public long Person_Id { get; set; }
        public int Payment_Mode_Id { get; set; }
        public int Payment_Type_Id { get; set; }
        public int Person_Type_Id { get; set; }
        public int Fee_Type_Id { get; set; }
        public Nullable<int> Session_Id { get; set; }
        public Nullable<long> Payment_Serial_Number { get; set; }
        public string Invoice_Number { get; set; }
        public System.DateTime Date_Paid { get; set; }
        public Nullable<int> Payment_Channnel_Id { get; set; }
        public Nullable<System.DateTime> Transaction_Date { get; set; }
        public Nullable<int> Payment_Etranzact_Type_Id { get; set; }
        public Nullable<long> Payment_Terminal_Id { get; set; }
        public string Receipt_No { get; set; }
        public string Confirmation_No { get; set; }
        public string Bank_Code { get; set; }
        public string Bank_Name { get; set; }
        public string Branch_Code { get; set; }
        public string Branch_Name { get; set; }
        public Nullable<decimal> Transaction_Amount { get; set; }
        public string Fee_Type_Name { get; set; }
        public string Payment_Mode_Name { get; set; }
        public string Payment_Type_Name { get; set; }
        public string Session_Name { get; set; }
    }
}
