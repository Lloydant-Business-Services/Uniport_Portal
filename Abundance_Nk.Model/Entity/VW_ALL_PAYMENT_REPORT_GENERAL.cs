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
    
    public partial class VW_ALL_PAYMENT_REPORT_GENERAL
    {
        public string Student_Name { get; set; }
        public string Department { get; set; }
        public string Payment_Type { get; set; }
        public string Programme { get; set; }
        public string Level { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime Date_Paid { get; set; }
        public int Department_Id { get; set; }
        public int Session_Id { get; set; }
        public int Programme_Id { get; set; }
        public int Level_Id { get; set; }
        public int Fee_Type_Id { get; set; }
        public string RRR { get; set; }
        public long Payment_Id { get; set; }
    }
}
