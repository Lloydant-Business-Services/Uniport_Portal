using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class DegreeAwarded
    {
        public long Id { get; set; }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public string Degree { get; set; }
        public int? Duration { get; set; }
    }
}
