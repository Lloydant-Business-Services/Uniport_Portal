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
    
    public partial class MENU_IN_ROLE
    {
        public int Menu_In_Role_Id { get; set; }
        public long Menu_Id { get; set; }
        public bool Activated { get; set; }
        public int Role_Id { get; set; }
    
        public virtual MENU MENU { get; set; }
        public virtual ROLE ROLE { get; set; }
    }
}
