using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicantReferee
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Rank { get; set; }
        public string Department { get; set; }
        public string Institution { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public ApplicationForm ApplicationForm { get; set; }

    }
}
