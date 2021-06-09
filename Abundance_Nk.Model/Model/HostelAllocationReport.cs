using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class HostelAllocationReport
    {
        public string HostelName { get; set; }
        public string SeriesName { get; set; }
        public string RoomNumber { get; set; }
        public bool Reserved { get; set; }
        public string CornerName { get; set; }
        public string FullName { get; set; }
        public string MatricNumber {get; set; }
        public string SessionName { get; set; }

        public int HostelId { get; set; }

        public int SeriesId { get; set; }

        public long RoomId { get; set; }

        public int CornerId { get; set; }
    }
}
