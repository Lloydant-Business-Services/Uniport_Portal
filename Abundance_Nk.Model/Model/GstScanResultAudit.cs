using System;

namespace Abundance_Nk.Model.Model
{
    public class GstScanResultAudit
    {
        public long Id { get; set; }
        public GstScan GstScan{ get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string ExamNo { get; set; }
        public string FullName { get; set; }
        public string  DepartmentName { get; set; }
        public decimal RawScore { get; set; }
        public decimal CA { get; set; }
        public decimal Total { get; set; }
        public string SemesterName { get; set; }
        public int SemesterId { get; set; }
        public int SessionId { get; set; }
        public string Operation { get; set; }
        public string Action { get; set; }
        public string Client { get; set; }
        
        public DateTime DateUploaded { get; set; }

        public virtual User User { get; set; }
    }
}
