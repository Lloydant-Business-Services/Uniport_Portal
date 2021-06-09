using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Business
{
    public class BankItLogic
    {
        public void ProcessTransaction(BankItRequest bankItRequest)
        {
            BankItResponse bankItResponse = new BankItResponse();
            try
            {
                string toHash = bankItRequest.Amount + bankItRequest.TerminalId + bankItRequest.TransactionId +
                                bankItRequest.ResponseUrl + bankItRequest.SecretKey;
                bankItRequest.Checksum = HashPaymentDetailToSHA256(toHash);
                var request = (HttpWebRequest)WebRequest.Create("http://demo.etranzact.com/bankIT/");

                var postData = "TERMINAL_ID=" + bankItRequest.TerminalId;
                postData += "&TRANSACTION_ID=" + bankItRequest.TransactionId;
                postData += "&AMOUNT=" + bankItRequest.Amount;
                postData += "&DESCRIPTION=" + bankItRequest.Description;
                postData += "&RESPONSE_URL=" + bankItRequest.ResponseUrl;
                postData += "&NOTIFICATION_URL=" + bankItRequest.NotificationUrl;
                postData += "&CHECKSUM=" + bankItRequest.Checksum;
                postData += "&LOGO_URL=" + bankItRequest.LogoUrl;
                postData += "&COL1=" + bankItRequest.Department;
                postData += "&COL2=" + bankItRequest.Programme;
                postData += "&COL3=" + bankItRequest.FeeType;
                postData += "&COL4=" + bankItRequest.Session;
                postData += "&COL5=" + bankItRequest.MatricNumber;
                postData += "&COL6=" + bankItRequest.FullName;

                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                var urlResponse = response.ResponseUri;
                string queryString = urlResponse.Query;
                bankItResponse.TransactionId = HttpUtility.ParseQueryString(queryString).Get("TRANSACTION_ID");
                bankItResponse.TransactionRef = HttpUtility.ParseQueryString(queryString).Get("TRANSACTION_REF");
                bankItResponse.Amount =  Convert.ToDecimal(HttpUtility.ParseQueryString(queryString).Get("AMOUNT"));
                bankItResponse.FinalChecksum = HttpUtility.ParseQueryString(queryString).Get("FINAL_CHECKSUM");
                bankItResponse.Description = HttpUtility.ParseQueryString(queryString).Get("DESCRIPTION");
                bankItResponse.TerminalId = HttpUtility.ParseQueryString(queryString).Get("TERMINAL_ID");
                bankItResponse.Success = HttpUtility.ParseQueryString(queryString).Get("SUCCESS");
                bankItResponse.SecretKey = HttpUtility.ParseQueryString(queryString).Get("SECRET_KEY");
                bankItResponse.ResponseUrl = HttpUtility.ParseQueryString(queryString).Get("RESPONSE_URL");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ValidateTransaction(BankItRequest bankItRequest)
        {
            BankItResponse bankItResponse = new BankItResponse();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("http://demo.etranzact.com/WebConnectPlus/query.jsp");

                var postData = "TERMINAL_ID=" + bankItRequest.TerminalId;
                postData += "&TRANSACTION_ID=" + bankItRequest.TransactionId;
                postData += "&RESPONSE_URL=" + bankItRequest.ResponseUrl;

                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                var urlResponse = response.ResponseUri;
                string queryString = urlResponse.Query;
                bankItResponse.TransactionId = HttpUtility.ParseQueryString(queryString).Get("TRANSACTION_ID");
                bankItResponse.TransactionRef = HttpUtility.ParseQueryString(queryString).Get("TRANSACTION_REF");
                bankItResponse.Amount = Convert.ToDecimal(HttpUtility.ParseQueryString(queryString).Get("AMOUNT"));
                bankItResponse.FinalChecksum = HttpUtility.ParseQueryString(queryString).Get("FINAL_CHECKSUM");
                bankItResponse.Description = HttpUtility.ParseQueryString(queryString).Get("DESCRIPTION");
                bankItResponse.TerminalId = HttpUtility.ParseQueryString(queryString).Get("TERMINAL_ID");
                bankItResponse.Success = HttpUtility.ParseQueryString(queryString).Get("SUCCESS");
                bankItResponse.SecretKey = HttpUtility.ParseQueryString(queryString).Get("SECRET_KEY");
                bankItResponse.ResponseUrl = HttpUtility.ParseQueryString(queryString).Get("RESPONSE_URL");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string HashPaymentDetailToSHA256(string hash_string)
        {
            System.Security.Cryptography.SHA256Managed sha256 = new System.Security.Cryptography.SHA256Managed();
            Byte[] EncryptedSHA256 = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hash_string));
            sha256.Clear();
            string hashed = BitConverter.ToString(EncryptedSHA256).Replace("-", "").ToLower();
            return hashed;

        }
    }
}
