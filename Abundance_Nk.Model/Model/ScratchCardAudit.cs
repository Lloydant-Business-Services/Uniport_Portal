using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ScratchCardAudit
    {
        public long Id { get; set; }

        public string Pin { get; set; }
        public string SerialNumber { get; set; }

        public string Operation { get; set; }
        public string Action { get; set; }
        public string Client { get; set; }
        public DateTime Time { get; set; }

        public Person NewPerson { get; set; }
        public Person OldPerson { get; set; }
        public ScratchCard ScratchCard { get; set; }
        public User User { get; set; }
    }
}
