using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class EmployeeDetail : Staff
    {
        public int? YearOfEmployment { get; set; }
        public string StaffNumber { get; set; }
        public Designation Designation { get; set; }
        public GradeLevel GradeLevel { get; set; }
        public Department Department { get; set; }
        public Unit Unit { get; set; }
        public int? UnitId { get; set; }
        public DateTime? DateOfPreviousApointment { get; set; }
        public DateTime? DateOfPresentAppointment { get; set; }
        public DateTime? DateOfRetirement { get; set; }
        public string EmploymentLocation { get; set; }
    }
}
