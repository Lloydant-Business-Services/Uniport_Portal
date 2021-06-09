using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class StudentAssignedMatricNumber
    {
        public Person Person { get; set; }
        public long StudentNumber { get; set; }
        public string StudentMatricNumber { get; set; }
        public Programme Programme { get; set; }
        public Session Session { get; set; }
    }
}
