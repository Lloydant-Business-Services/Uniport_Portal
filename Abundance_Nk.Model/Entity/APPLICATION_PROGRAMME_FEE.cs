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
    
    public partial class APPLICATION_PROGRAMME_FEE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public APPLICATION_PROGRAMME_FEE()
        {
            this.APPLICATION_FORM = new HashSet<APPLICATION_FORM>();
        }
    
        public int Application_Programme_Fee_Id { get; set; }
        public int Programme_Id { get; set; }
        public int Fee_Type_Id { get; set; }
        public int Session_Id { get; set; }
        public System.DateTime Date_Entered { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APPLICATION_FORM> APPLICATION_FORM { get; set; }
        public virtual FEE_TYPE FEE_TYPE { get; set; }
        public virtual PROGRAMME PROGRAMME { get; set; }
        public virtual SESSION SESSION { get; set; }
    }
}
