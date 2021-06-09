using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ClassAdmitCard
    {
        public string Fullname { get; set; }
        public string passport_url { get; set; }
        public string Matric_Number { get; set; }
        public string Department { get; set; }
        public string Level { get; set; }
        public string Programme { get; set; }
        public string Session { get; set; }
        public string ConfirmationNumber { get; set; }
        public string RegisteredCourses { get; set; }
        public long RegisteredCourseId { get; set; }
    }
}
