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
    
    public partial class EDUCATIONAL_QUALIFICATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EDUCATIONAL_QUALIFICATION()
        {
            this.APPLICANT_PREVIOUS_EDUCATION = new HashSet<APPLICANT_PREVIOUS_EDUCATION>();
            this.STAFF_QUALIFICATION = new HashSet<STAFF_QUALIFICATION>();
        }
    
        public int Educational_Qualification_Id { get; set; }
        public string Educational_Qualification_Abbreviation { get; set; }
        public string Educational_Qualification_Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APPLICANT_PREVIOUS_EDUCATION> APPLICANT_PREVIOUS_EDUCATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STAFF_QUALIFICATION> STAFF_QUALIFICATION { get; set; }
    }
}
