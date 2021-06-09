using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class TranscriptRequestModel
    {
        public long TranscriptRequestId { get; set; }
        public long? PaymentId { get; set; }
        public long? Studentid { get; set; }
        public string Name { get; set; }
        public string RegNo { get; set; }
        public string Email { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public int ProgrammeId { get; set; }
        public int LevelId { get; set; }
        public int FacultyId { get; set; }
        public System.DateTime DateRequested { get; set; }
        public string DestinationAddress { get; set; }
        public string DestinationStateId { get; set; }
        public string DestinationCountryId { get; set; }
        public int?TranscriptClearanceStatusId { get; set; }
        public int TranscriptStatusId { get; set; }
        public string TranscriptStatusName { get; set; }
        public string DeliveryZone { get; set; }
        public string DeliveryService { get; set; }
    }
}
