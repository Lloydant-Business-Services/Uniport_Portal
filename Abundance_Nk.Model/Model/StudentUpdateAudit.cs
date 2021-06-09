using System;

namespace Abundance_Nk.Model.Model
{
    public class StudentUpdateAudit
    {
        public long Id { get; set; }
        public Student Student { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
    }
}