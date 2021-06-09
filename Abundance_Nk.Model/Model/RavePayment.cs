using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class RavePayment
    {
        public string Public_key { get; set; }
        public string Security_key { get; set; }
        public string Accountnumber { get; set; }
        public string Narration { get; set; }
        public int Numberofunits { get; set; } = 1;
        public string Currency { get; set; } = "NGN";
        public string Amount { get; set; }
        public string Phonenumber { get; set; }
        public string Email { get; set; }
        public string TxRef { get; set; }
        public string Country { get; set; } = "NG";
        public string Ip { get; set; } = "127.0.0.7";
        public List<RaveSubAccount> Subaccounts { get; set; }
    }

    public class RaveSubAccount
    {
        public string Id { get; set; }
        public string Transaction_Charge { get; set; }
        public string Transaction_Split_Ratio { get; set; }
        public string Transaction_Charge_Type { get; set; }
    }

    public class RaveResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public RaveResponseData Data { get; set; }
    }

    public class RaveResponseData
    {
        public string order { get; set; }
        public string link { get; set; }
        public string flwRef { get; set; }
        public string txRef { get; set; }
        public string updated { get; set; }
        public string txid { get; set; }
        public string amount { get; set; }
        public string chargedamount { get; set; }
        public string chargecode { get; set; }
        public string chargemessage { get; set; }
        public string authmodel { get; set; }
        public string status { get; set; }
        public string narration { get; set; }
        public string paymenttype { get; set; }
        public string appfee { get; set; }
        public string createdAt { get; set; }
        public string currency { get; set; }
        public string event_type { get; set; }
        public string charged_amount { get; set; }
    }

    public class RaveTransactionQuery
    {
        public string Security_key { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string page { get; set; }
        public string customer_email { get; set; }
        public string status { get; set; }
        public string transaction_reference { get; set; }
        public string customer_fullname { get; set; }
        public string currency { get; set; }

    }
}
