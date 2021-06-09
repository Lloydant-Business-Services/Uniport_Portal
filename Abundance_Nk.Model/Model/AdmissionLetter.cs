using System;
using System.Collections.Generic;

namespace Abundance_Nk.Model.Model
{
    public class AdmissionLetter
    {
        public Person Person { get; set; }
        public Session Session { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public Faculty Faculty { get; set; }
        public List<FeeDetail> FeeDetails { get; set; }
        public ApplicationForm applicationform { get; set; }
        public string ProgrammeType { get; set; }
        public DateTime? RegistrationEndDate { get; set; }
        public TimeSpan? RegistrationEndTime { get; set; }
        public string RegistrationEndTimeString { get; set; }
        public string JambNumber { get; set; }
        public string AcceptancePin { get; set; }
        public string AcceptanceAmount { get; set; }
        public string QRVerification { get; set; }
        public DegreeAwarded DegreeAwarded { get; set; }
        public string ApplicationNumber { get; set; }
    }
}