using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class TranscriptIncidentLog
    {
        public long Id { get; set; }
        public string TicketId { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public string Email { get; set; }
        public string Phone_No { get; set; }
        public System.DateTime Date_Opened { get; set; }
        public Nullable<System.DateTime> Date_Closed { get; set; }
        public TranscriptRequest TranscriptRequest { get; set; }
        public User LoggedUser { get; set; }
        public User ClosedUser { get; set; }
        public Department Department { get; set; }
    }
}
