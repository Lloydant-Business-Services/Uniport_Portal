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
    
    public partial class HOSTEL_SERIES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HOSTEL_SERIES()
        {
            this.HOSTEL_ALLOCATION = new HashSet<HOSTEL_ALLOCATION>();
            this.HOSTEL_ALLOCATION_CRITERIA = new HashSet<HOSTEL_ALLOCATION_CRITERIA>();
            this.HOSTEL_CRITERIA = new HashSet<HOSTEL_CRITERIA>();
            this.HOSTEL_ROOM = new HashSet<HOSTEL_ROOM>();
        }
    
        public int Series_Id { get; set; }
        public string Series_Name { get; set; }
        public int Hostel_Id { get; set; }
        public bool Activated { get; set; }
    
        public virtual HOSTEL HOSTEL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOSTEL_ALLOCATION> HOSTEL_ALLOCATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOSTEL_ALLOCATION_CRITERIA> HOSTEL_ALLOCATION_CRITERIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOSTEL_CRITERIA> HOSTEL_CRITERIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOSTEL_ROOM> HOSTEL_ROOM { get; set; }
    }
}
