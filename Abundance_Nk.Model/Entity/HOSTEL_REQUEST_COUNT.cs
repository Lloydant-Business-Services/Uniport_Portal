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
    
    public partial class HOSTEL_REQUEST_COUNT
    {
        public int Hostel_Request_Count_Id { get; set; }
        public int Level_Id { get; set; }
        public byte Sex_Id { get; set; }
        public long Total_Count { get; set; }
        public System.DateTime Last_Modified { get; set; }
        public bool Approved { get; set; }
        public System.DateTime Date_Set { get; set; }
    
        public virtual LEVEL LEVEL { get; set; }
        public virtual SEX SEX { get; set; }
    }
}
