using Abundance_Nk.Model.Model;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace Abundance_Nk.Business
{
    public class RaveBranchPaymentLogic
    {

        private RestClient client;
        protected RestRequest request;
        public static string RaveEbillsUrl = "http://rave-api-v2.herokuapp.com/flwv3-pug/getpaidx/api/ebills/";
        public static string RaveURL = "https://ravesandboxapi.flutterwave.com/flwv3-pug/getpaidx/api/v2";

        public RaveBranchPaymentLogic()
        {
            client = new RestClient(RaveURL);
        }

        public RaveResponse CreateOrder(RavePayment ravePayment)
        {
            client = new RestClient(RaveEbillsUrl);
            RaveResponse raveResponse = null;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string ApiEndPoint = "/generateorder/";
            request = new RestRequest(ApiEndPoint, Method.POST);
            request.AddHeader("accept", "application/json");
            request.AddParameter("SECKEY", ravePayment.Security_key);
            request.AddParameter("accountnumber", ravePayment.Accountnumber);
            request.AddParameter("narration", ravePayment.Narration);
            request.AddParameter("numberofunits", ravePayment.Numberofunits);
            request.AddParameter("currency", ravePayment.Currency);
            request.AddParameter("amount", ravePayment.Amount);
            request.AddParameter("phonenumber", ravePayment.Phonenumber);
            request.AddParameter("email", ravePayment.Email);
            request.AddParameter("txRef", ravePayment.TxRef);
            request.AddParameter("country", ravePayment.Country);
            request.AddParameter("IP", ravePayment.Ip);
            request.AddParameter("subaccounts", ravePayment.Subaccounts);
            var result = client.Execute(request);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                raveResponse = JsonConvert.DeserializeObject<RaveResponse>(result.Content);
            }
            return raveResponse;
        }

       
        public RaveResponse CreateCardPayment(RavePayment ravePayment)
        {
            RaveResponse raveResponse = null;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string ApiEndPoint = "/hosted/pay/";
            request = new RestRequest(ApiEndPoint, Method.POST);
            request.AddParameter("txref", ravePayment.TxRef);
            request.AddParameter("customer_email", ravePayment.Email);
            request.AddParameter("amount", ravePayment.Amount);
            request.AddParameter("currency", ravePayment.Currency);
            request.AddParameter("country", ravePayment.Country);
            request.AddParameter("redirect_url","https://portal.abiastateuniversity.edu.ng");
            request.AddParameter("PBFPubKey", ravePayment.Public_key);
            var result = client.Execute(request);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                raveResponse = JsonConvert.DeserializeObject<RaveResponse>(result.Content);
            }
            return raveResponse;
        }

        public RaveResponse VerifyTransaction(RavePayment ravePayment)
        {
            RaveResponse raveResponse = null;
           
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string ApiEndPoint = "/verify/";
            request = new RestRequest(ApiEndPoint, Method.POST);
            request.AddParameter("txref", ravePayment.TxRef);
            request.AddParameter("SECKEY",ravePayment.Security_key);
            var result = client.Execute(request);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                raveResponse = JsonConvert.DeserializeObject<RaveResponse>(result.Content);
            }
            return raveResponse;
        }

        public RaveResponse ListTransactions(RaveTransactionQuery raveTransactionQuery)
        {
            RaveResponse raveResponse = null;
            client = new RestClient("https://ravesandboxapi.flutterwave.com/v2/gpx/transactions/query");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string ApiEndPoint = "/raveTransactionQuery/";
            request = new RestRequest(ApiEndPoint, Method.POST);
            request.AddParameter("seckey", raveTransactionQuery.Security_key);
            request.AddParameter("from", raveTransactionQuery.from);
            request.AddParameter("to", raveTransactionQuery.to);
            request.AddParameter("page", raveTransactionQuery.page);
            request.AddParameter("customer_email", raveTransactionQuery.customer_email);
            request.AddParameter("status", raveTransactionQuery.status);
            request.AddParameter("customer_fullname", raveTransactionQuery.customer_fullname);
            request.AddParameter("transaction_reference", raveTransactionQuery.transaction_reference);
            request.AddParameter("currency", raveTransactionQuery.currency);
            var result = client.Execute(request);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                raveResponse = JsonConvert.DeserializeObject<RaveResponse>(result.Content);
            }
            return raveResponse;
        }

    }
}
