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
    
    public partial class JAMB_RECORD
    {
        public long Jamb_Record_Id { get; set; }
        public string Jamb_Registration_Number { get; set; }
        public string Candidate_Name { get; set; }
        public string First_Choice_Institution { get; set; }
        public string Subject1 { get; set; }
        public string Subject2 { get; set; }
        public string Subject3 { get; set; }
        public string Subject4 { get; set; }
        public Nullable<short> Total_Jamb_Score { get; set; }
        public Nullable<int> Department_Id { get; set; }
        public Nullable<int> Programme_Id { get; set; }
        public Nullable<int> Local_Government_Id { get; set; }
        public Nullable<byte> Sex_Id { get; set; }
        public string Passport_Url { get; set; }
        public Nullable<int> Session_Id { get; set; }
        public string State_Id { get; set; }
    }
}
