using System;
using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class SessionSemester
    {
        [Display(Name = "Session/Semester")]
        public int Id { get; set; }

        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Name { get; set; }
    }
}