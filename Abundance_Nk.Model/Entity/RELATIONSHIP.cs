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
    
    public partial class RELATIONSHIP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RELATIONSHIP()
        {
            this.APPLICANT_SPONSOR = new HashSet<APPLICANT_SPONSOR>();
            this.NEXT_OF_KIN = new HashSet<NEXT_OF_KIN>();
            this.STUDENT_SPONSOR = new HashSet<STUDENT_SPONSOR>();
        }
    
        public int Relationship_Id { get; set; }
        public string Relationship_Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APPLICANT_SPONSOR> APPLICANT_SPONSOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NEXT_OF_KIN> NEXT_OF_KIN { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_SPONSOR> STUDENT_SPONSOR { get; set; }
    }
}
