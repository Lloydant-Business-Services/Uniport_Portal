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
    
    public partial class STUDENT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public STUDENT()
        {
            this.COURSE_EVALUATION_ANSWER = new HashSet<COURSE_EVALUATION_ANSWER>();
            this.E_ASSIGNMENT_SUBMISSION = new HashSet<E_ASSIGNMENT_SUBMISSION>();
            this.E_CHAT_RESPONPSE = new HashSet<E_CHAT_RESPONPSE>();
            this.HOSTEL_BLACKLIST = new HashSet<HOSTEL_BLACKLIST>();
            this.REPORT_CARD = new HashSet<REPORT_CARD>();
            this.GST_SCAN = new HashSet<GST_SCAN>();
            this.STUDENT_COURSE_REGISTRATION = new HashSet<STUDENT_COURSE_REGISTRATION>();
            this.STUDENT_EMPLOYMENT_INFORMATION = new HashSet<STUDENT_EMPLOYMENT_INFORMATION>();
            this.STUDENT_EXAM_RAW_SCORE_SHEET_RESULT = new HashSet<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT>();
            this.STUDENT_LEVEL = new HashSet<STUDENT_LEVEL>();
            this.STUDENT_RESULT_DETAIL = new HashSet<STUDENT_RESULT_DETAIL>();
            this.STUDENT_SCORE = new HashSet<STUDENT_SCORE>();
            this.STUDENT_SEMESTER = new HashSet<STUDENT_SEMESTER>();
            this.STUDENT_UPDATE_AUDIT = new HashSet<STUDENT_UPDATE_AUDIT>();
            this.TRANSCRIPT_REQUEST = new HashSet<TRANSCRIPT_REQUEST>();
        }
    
        public long Person_Id { get; set; }
        public Nullable<long> Application_Form_Id { get; set; }
        public int Student_Type_Id { get; set; }
        public int Student_Category_Id { get; set; }
        public int Student_Status_Id { get; set; }
        public Nullable<long> Student_Number { get; set; }
        public string Matric_Number { get; set; }
        public Nullable<int> Title_Id { get; set; }
        public Nullable<int> Marital_Status_Id { get; set; }
        public Nullable<int> Blood_Group_Id { get; set; }
        public Nullable<int> Genotype_Id { get; set; }
        public string School_Contact_Address { get; set; }
        public Nullable<bool> Activated { get; set; }
        public string Reason { get; set; }
        public string Reject_Category { get; set; }
        public string Password_hash { get; set; }
    
        public virtual APPLICATION_FORM APPLICATION_FORM { get; set; }
        public virtual BLOOD_GROUP BLOOD_GROUP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COURSE_EVALUATION_ANSWER> COURSE_EVALUATION_ANSWER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<E_ASSIGNMENT_SUBMISSION> E_ASSIGNMENT_SUBMISSION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<E_CHAT_RESPONPSE> E_CHAT_RESPONPSE { get; set; }
        public virtual GENOTYPE GENOTYPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HOSTEL_BLACKLIST> HOSTEL_BLACKLIST { get; set; }
        public virtual MARITAL_STATUS MARITAL_STATUS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REPORT_CARD> REPORT_CARD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GST_SCAN> GST_SCAN { get; set; }
        public virtual STUDENT_ACADEMIC_INFORMATION STUDENT_ACADEMIC_INFORMATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_COURSE_REGISTRATION> STUDENT_COURSE_REGISTRATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_EMPLOYMENT_INFORMATION> STUDENT_EMPLOYMENT_INFORMATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_EXAM_RAW_SCORE_SHEET_RESULT> STUDENT_EXAM_RAW_SCORE_SHEET_RESULT { get; set; }
        public virtual STUDENT_FINANCE_INFORMATION STUDENT_FINANCE_INFORMATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_LEVEL> STUDENT_LEVEL { get; set; }
        public virtual STUDENT_ND_RESULT STUDENT_ND_RESULT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_RESULT_DETAIL> STUDENT_RESULT_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_SCORE> STUDENT_SCORE { get; set; }
        public virtual STUDENT_SPONSOR STUDENT_SPONSOR { get; set; }
        public virtual STUDENT_CATEGORY STUDENT_CATEGORY { get; set; }
        public virtual STUDENT_STATUS STUDENT_STATUS { get; set; }
        public virtual STUDENT_TYPE STUDENT_TYPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_SEMESTER> STUDENT_SEMESTER { get; set; }
        public virtual TITLE TITLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_UPDATE_AUDIT> STUDENT_UPDATE_AUDIT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSCRIPT_REQUEST> TRANSCRIPT_REQUEST { get; set; }
        public virtual PERSON PERSON { get; set; }
    }
}
