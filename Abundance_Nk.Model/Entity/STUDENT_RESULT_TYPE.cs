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
    
    public partial class STUDENT_RESULT_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public STUDENT_RESULT_TYPE()
        {
            this.STUDENT_RESULT = new HashSet<STUDENT_RESULT>();
        }
    
        public int Student_Result_Type_Id { get; set; }
        public string Student_Result_Type_Name { get; set; }
        public string Student_Result_Type_Description { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_RESULT> STUDENT_RESULT { get; set; }
    }
}
