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
    
    public partial class REFEREE_GRADING_SYSTEM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public REFEREE_GRADING_SYSTEM()
        {
            this.APPLICANT_REFEREE_GRADING_RESPONSE = new HashSet<APPLICANT_REFEREE_GRADING_RESPONSE>();
            this.APPLICANT_REFEREE_RESPONSE = new HashSet<APPLICANT_REFEREE_RESPONSE>();
        }
    
        public int Id { get; set; }
        public string Score { get; set; }
        public bool Active { get; set; }
        public int Type { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APPLICANT_REFEREE_GRADING_RESPONSE> APPLICANT_REFEREE_GRADING_RESPONSE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APPLICANT_REFEREE_RESPONSE> APPLICANT_REFEREE_RESPONSE { get; set; }
    }
}
