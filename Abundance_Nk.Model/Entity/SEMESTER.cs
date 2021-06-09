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
    
    public partial class SEMESTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SEMESTER()
        {
            this.COURSE = new HashSet<COURSE>();
            this.COURSE_EVALUATION_ANSWER = new HashSet<COURSE_EVALUATION_ANSWER>();
            this.COURSE_UNIT = new HashSet<COURSE_UNIT>();
            this.GST_SCAN_RESULT = new HashSet<GST_SCAN_RESULT>();
            this.SESSION_SEMESTER = new HashSet<SESSION_SEMESTER>();
            this.STUDENT_COURSE_REGISTRATION_DETAIL_AUDIT = new HashSet<STUDENT_COURSE_REGISTRATION_DETAIL_AUDIT>();
            this.STUDENT_COURSE_REGISTRATION_DETAIL = new HashSet<STUDENT_COURSE_REGISTRATION_DETAIL>();
            this.STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED = new HashSet<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED>();
            this.STUDENT_EXAM_RAW_SCORE_SHEET_RESULT = new HashSet<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT>();
            this.COURSE_ALLOCATION = new HashSet<COURSE_ALLOCATION>();
        }
    
        public int Semester_Id { get; set; }
        public string Semester_Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COURSE> COURSE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COURSE_EVALUATION_ANSWER> COURSE_EVALUATION_ANSWER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COURSE_UNIT> COURSE_UNIT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GST_SCAN_RESULT> GST_SCAN_RESULT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SESSION_SEMESTER> SESSION_SEMESTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_COURSE_REGISTRATION_DETAIL_AUDIT> STUDENT_COURSE_REGISTRATION_DETAIL_AUDIT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_COURSE_REGISTRATION_DETAIL> STUDENT_COURSE_REGISTRATION_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED> STUDENT_EXAM_RAW_SCORE_SHEET_RESULT_NOT_REGISTERED { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT> STUDENT_EXAM_RAW_SCORE_SHEET_RESULT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COURSE_ALLOCATION> COURSE_ALLOCATION { get; set; }
    }
}
