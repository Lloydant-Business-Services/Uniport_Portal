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
    
    public partial class APPLICANT_O_LEVEL_RESULT_DETAIL
    {
        public long Applicant_O_Level_Result_Detail_Id { get; set; }
        public long Applicant_O_Level_Result_Id { get; set; }
        public int O_Level_Subject_Id { get; set; }
        public int O_Level_Grade_Id { get; set; }
    
        public virtual APPLICANT_O_LEVEL_RESULT APPLICANT_O_LEVEL_RESULT { get; set; }
        public virtual O_LEVEL_GRADE O_LEVEL_GRADE { get; set; }
        public virtual O_LEVEL_SUBJECT O_LEVEL_SUBJECT { get; set; }
    }
}
