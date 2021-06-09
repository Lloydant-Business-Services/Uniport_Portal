using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AdmittedStudentBreakdownView
    {
        public long AdmissionListId { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int ProgrammeId { get; set; }
        public string ProgrammeName { get; set; }
        public int SessionId { get; set; }
        public string SessionName { get; set; }
    }
}
