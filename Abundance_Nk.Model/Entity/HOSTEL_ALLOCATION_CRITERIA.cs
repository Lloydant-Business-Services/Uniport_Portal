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
    
    public partial class HOSTEL_ALLOCATION_CRITERIA
    {
        public long Id { get; set; }
        public int Hostel_Id { get; set; }
        public int Department_Id { get; set; }
        public int Programme_Id { get; set; }
        public int Level_Id { get; set; }
        public int Series_Id { get; set; }
        public long Room_Id { get; set; }
        public int Corner_Id { get; set; }
    
        public virtual DEPARTMENT DEPARTMENT { get; set; }
        public virtual HOSTEL HOSTEL { get; set; }
        public virtual HOSTEL_ROOM HOSTEL_ROOM { get; set; }
        public virtual HOSTEL_ROOM_CORNER HOSTEL_ROOM_CORNER { get; set; }
        public virtual HOSTEL_SERIES HOSTEL_SERIES { get; set; }
        public virtual LEVEL LEVEL { get; set; }
        public virtual PROGRAMME PROGRAMME { get; set; }
    }
}
