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
    
    public partial class STUDENT_ASSIGNED_MATRIC_NUMBER
    {
        public long Person_Id { get; set; }
        public long Student_Number { get; set; }
        public string Student_Matric_Number { get; set; }
        public int Programme_Id { get; set; }
        public int Session_Id { get; set; }
    
        public virtual PROGRAMME PROGRAMME { get; set; }
        public virtual SESSION SESSION { get; set; }
        public virtual PERSON PERSON { get; set; }
    }
}
