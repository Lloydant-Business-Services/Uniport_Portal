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
    
    public partial class COURSE_EVALUATION_QUESTION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COURSE_EVALUATION_QUESTION()
        {
            this.COURSE_EVALUATION_ANSWER = new HashSet<COURSE_EVALUATION_ANSWER>();
        }
    
        public int Id { get; set; }
        public int Section { get; set; }
        public string Question { get; set; }
        public int Score { get; set; }
        public bool Activated { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COURSE_EVALUATION_ANSWER> COURSE_EVALUATION_ANSWER { get; set; }
    }
}
