using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AdmissionListModel
    {
        public long PersonId { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Programme { get; set; }
        public int ProgrammeId { get; set; }
        public int DepartmentId { get; set; }
        public string ApplicationNumber { get; set; }
        public string ExamNumber { get; set; }
        public string JambNumber { get; set; }
        public string AdmissionListType { get; set; }
        public Nullable<System.DateTime> DateUploaded { get; set; }
        public string SessionName { get; set; }
        public int SessionId { get; set; }
        public string MatricNumber { get; set; }
        public string BloodGroup { get; set; }
        public int YearOfAdmission { get; set; }
        public int YearOfGraduation { get; set; }
        public string ImageUrl { get; set; }
    }
}
