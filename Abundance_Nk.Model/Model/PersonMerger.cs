using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PersonMerger
    {
        public long PersonMergerId { get; set; }

        public  Person OldPerson { get; set; }
        public Person NewPerson { get; set; }
    }
}
