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
    
    public partial class REPORT_CARD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public REPORT_CARD()
        {
            this.REPORT_CARD_COMMENT = new HashSet<REPORT_CARD_COMMENT>();
        }
    
        public long Report_Card_Id { get; set; }
        public int Session_Term_Id { get; set; }
        public long Student_Id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REPORT_CARD_COMMENT> REPORT_CARD_COMMENT { get; set; }
        public virtual SESSION_SEMESTER SESSION_SEMESTER { get; set; }
        public virtual STUDENT STUDENT { get; set; }
    }
}
