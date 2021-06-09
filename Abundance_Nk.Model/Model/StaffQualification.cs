using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StaffQualification
    {
        public int Id { get; set; }
        public string InstitutionAttended { get; set; }
        public EducationalQualification EducationalQualification { get; set; }
        public StaffResultGrade StaffResultGrade { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public string CertificateNumber { get; set; }
        public Staff Staff { get; set; }
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set; }
    }
}
