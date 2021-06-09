using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PutmeResultModel
    {
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Sex { get; set; }
        public string ApplicationNumber { get; set; }
        public string JambNumber { get; set; }
        public string ExamNumber { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public double? JambScore { get; set; }
        public double? ExamScore { get; set; }

    }
}
