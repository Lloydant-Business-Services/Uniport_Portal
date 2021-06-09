using System.Net;

namespace Abundance_Nk.Model.Model
{
    public class RemitaRequestState
    {
        public Remita Remita { get; set; }
        public HttpWebRequest Request { get; set; }
        public Payment Payment { get; set; }
    }
}