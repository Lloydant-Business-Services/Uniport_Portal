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
    
    public partial class VW_CORRECT_DE_APPLICANTS
    {
        public long Application_Form_Id { get; set; }
        public Nullable<long> Serial_Number { get; set; }
        public string Application_Form_Number { get; set; }
        public Nullable<int> Application_Exam_Serial_Number { get; set; }
        public string Application_Exam_Number { get; set; }
        public int Application_Form_Setting_Id { get; set; }
        public int Application_Programme_Fee_Id { get; set; }
        public long Payment_Id { get; set; }
        public long Person_Id { get; set; }
        public System.DateTime Date_Submitted { get; set; }
        public bool Release { get; set; }
        public bool Rejected { get; set; }
        public string Reject_Reason { get; set; }
        public string Remarks { get; set; }
        public long Expr1 { get; set; }
        public string Applicant_Jamb_Registration_Number { get; set; }
        public Nullable<short> Applicant_Jamb_Score { get; set; }
        public Nullable<int> Institution_Choice_Id { get; set; }
        public Nullable<long> Expr2 { get; set; }
        public Nullable<int> Subject1 { get; set; }
        public Nullable<int> Subject2 { get; set; }
        public Nullable<int> Subject3 { get; set; }
        public Nullable<int> Subject4 { get; set; }
    }
}
