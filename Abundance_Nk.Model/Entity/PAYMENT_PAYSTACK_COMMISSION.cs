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
    
    public partial class PAYMENT_PAYSTACK_COMMISSION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PAYMENT_PAYSTACK_COMMISSION()
        {
            this.PAYMENT_PAYSTACK_COMMISSION_AUDIT = new HashSet<PAYMENT_PAYSTACK_COMMISSION_AUDIT>();
        }
    
        public int Payment_PayStack_Commission_Id { get; set; }
        public int FeeType_Id { get; set; }
        public decimal Amount { get; set; }
        public int Session_Id { get; set; }
        public Nullable<bool> Activated { get; set; }
        public Nullable<int> Programme_Id { get; set; }
        public Nullable<decimal> AddedFee { get; set; }
        public Nullable<long> SetUpBy { get; set; }
    
        public virtual FEE_TYPE FEE_TYPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAYMENT_PAYSTACK_COMMISSION_AUDIT> PAYMENT_PAYSTACK_COMMISSION_AUDIT { get; set; }
        public virtual PROGRAMME PROGRAMME { get; set; }
        public virtual SESSION SESSION { get; set; }
        public virtual USER USER { get; set; }
    }
}
