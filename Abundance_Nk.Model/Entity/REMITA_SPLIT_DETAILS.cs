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
    
    public partial class REMITA_SPLIT_DETAILS
    {
        public int Id { get; set; }
        public string Bank_Code { get; set; }
        public string Beneficiary_Account { get; set; }
        public string Beneficiary_Name { get; set; }
        public decimal Beneficiary_Amount { get; set; }
        public bool Activated { get; set; }
    }
}
