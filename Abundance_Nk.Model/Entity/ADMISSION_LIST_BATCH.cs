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
    
    public partial class ADMISSION_LIST_BATCH
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ADMISSION_LIST_BATCH()
        {
            this.ADMISSION_LIST = new HashSet<ADMISSION_LIST>();
        }
    
        public int Admission_List_Batch_Id { get; set; }
        public int Admission_List_Type_Id { get; set; }
        public System.DateTime Date_Uploaded { get; set; }
        public string Uploaded_File_Path { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ADMISSION_LIST> ADMISSION_LIST { get; set; }
        public virtual ADMISSION_LIST_TYPE ADMISSION_LIST_TYPE { get; set; }
    }
}
