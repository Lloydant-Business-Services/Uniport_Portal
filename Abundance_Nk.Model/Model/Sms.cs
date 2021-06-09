using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Abundance_Nk.Model.Model
{
    public class Sms
    {
        private string password = "f7dbe54b";
        private string username = "0cd0f2f5";

        public Sms()
        {
            Sender = "Sender";
        }

        public string Sender { get; set; }

        public NexmoResponse SendSMS(string to, string text)
        {
            var wc = new WebClient {BaseAddress = "http://rest.nexmo.com/sms/json"};
            wc.QueryString.Add("api_key", HttpUtility.UrlEncode(username));
            wc.QueryString.Add("api_secret", HttpUtility.UrlEncode(password));
            wc.QueryString.Add("from", HttpUtility.UrlEncode(Sender));
            wc.QueryString.Add("to", HttpUtility.UrlEncode(to));
            wc.QueryString.Add("text", HttpUtility.UrlEncode(text));
            return ParseSmsResponseJson(wc.DownloadString(""));
        }

        private NexmoResponse ParseSmsResponseJson(string json)
        {
            json = json.Replace("-", ""); // hyphens are not allowed in in .NET var names
            return new JavaScriptSerializer().Deserialize<NexmoResponse>(json);
        }
    }

    public class NexmoResponse
    {
        public string Messagecount { get; set; }
        public List<NexmoMessageStatus> Messages { get; set; }
    }

    public class NexmoMessageStatus
    {
        public string Network;
        public string clientRef;
        public string MessageId { get; set; }
        public string To { get; set; }
        public string Status { get; set; }
        public string ErrorText { get; set; }
        public string RemainingBalance { get; set; }
        public string MessagePrice { get; set; }
    }
}