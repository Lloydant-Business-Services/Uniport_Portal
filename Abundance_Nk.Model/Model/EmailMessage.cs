using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class EmailMessage
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
    }

    public class PasswordEmailReset
    {
        public string Name { get; set; }
        public string message { get; set; }
        public string header { get; set; }
        public string footer { get; set; }
        public string confirmationLink { get; set; }
    }
}
