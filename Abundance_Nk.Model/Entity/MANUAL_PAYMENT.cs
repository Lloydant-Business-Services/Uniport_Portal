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
    
    public partial class MANUAL_PAYMENT
    {
        public long Manual_Payment_Id { get; set; }
        public long Person_Id { get; set; }
        public int FeeType_Id { get; set; }
        public int Session_Id { get; set; }
        public decimal Amount { get; set; }
        public string Invoice_Number { get; set; }
        public System.DateTime Date_Approved { get; set; }
        public long Approver_Officer_Id { get; set; }
    
        public virtual FEE_TYPE FEE_TYPE { get; set; }
        public virtual SESSION SESSION { get; set; }
        public virtual USER USER { get; set; }
        public virtual PERSON PERSON { get; set; }
    }
}
