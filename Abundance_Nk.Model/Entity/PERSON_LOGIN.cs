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
    
    public partial class PERSON_LOGIN
    {
        public long Person_Id { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public int Security_Question_Id { get; set; }
        public string Security_Answer { get; set; }
        public bool Is_Activated { get; set; }
        public bool Is_First_Logon { get; set; }
    
        public virtual SECURITY_QUESTION SECURITY_QUESTION { get; set; }
        public virtual PERSON PERSON { get; set; }
    }
}
