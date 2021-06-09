using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class GstScan
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string MatricNumber { get; set; }
        public string CourseCode { get; set; }
        public string ProgrammeName { get; set; }
        public string DepartmentName { get; set; }
        public string SemesterName { get; set; }
        public string SessionName { get; set; }
        public decimal Score { get; set; }
        public string Shaded { get; set; }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public Course Course { get; set; }
        public Student Student { get; set; }
        public string CourseTitle { get; set; }
        public decimal? RawScore { get; set; }
        public decimal? Ca { get; set; }
        public decimal? Total { get; set; }
        public int SN { get; set; }
        public int SemesterId { get; set; }
        public int SessionId { get; set; }

    }
}
