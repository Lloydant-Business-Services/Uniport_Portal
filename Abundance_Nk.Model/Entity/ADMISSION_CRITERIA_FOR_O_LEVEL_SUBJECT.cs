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
    
    public partial class ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT()
        {
            this.ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE = new HashSet<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE>();
        }
    
        public int Admission_Criteria_For_O_Level_Subject_Id { get; set; }
        public int Admission_Criteria_Id { get; set; }
        public int O_Level_Subject_Id { get; set; }
        public int Minimum_O_Level_Grade_Id { get; set; }
        public bool Is_Compulsory { get; set; }
    
        public virtual ADMISSION_CRITERIA ADMISSION_CRITERIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE> ADMISSION_CRITERIA_FOR_O_LEVEL_SUBJECT_ALTERNATIVE { get; set; }
        public virtual O_LEVEL_GRADE O_LEVEL_GRADE { get; set; }
        public virtual O_LEVEL_SUBJECT O_LEVEL_SUBJECT { get; set; }
    }
}
