using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PostGraduateReportModel
    {
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        public string Title { get; set; }
        public string ApplicationNumber { get; set; }
        public string Course { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string Programme { get; set; }
        public string RefereeName { get; set; }
        public string RefereeRank { get; set; }
        public string RefereeDepartment { get; set; }
        public string RefereeInstitution { get; set; }
        public string RefereeEmail { get; set; }
        public string RefereePhone { get; set; }
    }
}
