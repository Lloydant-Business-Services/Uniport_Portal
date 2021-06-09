using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StaffDepartment
    {
        public int Id { get; set; }
        public long StaffUserID { get; set; }
        public Department Department { get; set; }
        public System.DateTime DateEntered { get; set; }
        public bool IsHead { get; set; }
        public DepartmentOption DepartmentOption { get; set; }

    }
}
