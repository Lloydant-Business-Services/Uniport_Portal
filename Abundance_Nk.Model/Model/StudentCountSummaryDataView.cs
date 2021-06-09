using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentCountSummaryDataView
    {
        public string DepartmentName { get; set; }
        public int NewStudentCount { get; set; }
        public string SessionName { get; set; }
        public int ReturningStudentCount { get; set; }
        public int DepartmentId { get; set; }
        public int ProgrammeId { get; set; }
        public string ProgrammeName { get; set; }
    }
}
