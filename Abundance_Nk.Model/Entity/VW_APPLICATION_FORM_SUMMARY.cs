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
    
    public partial class VW_APPLICATION_FORM_SUMMARY
    {
        public int Programme_Id { get; set; }
        public int Department_Id { get; set; }
        public string Programme_Name { get; set; }
        public string Department_Name { get; set; }
        public int Session_Id { get; set; }
        public string Session_Name { get; set; }
        public int Application_Form_Setting_Id { get; set; }
        public Nullable<int> Application_Form_Count { get; set; }
    }
}
