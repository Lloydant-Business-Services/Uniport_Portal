using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace Abundance_Nk.Web
{
    public partial class PaymentReceipt :Page
    {
        public string message;
        public string order_Id;
        public string rrr;
        public string statuscode;
        public string url;

        protected void Page_Load(object sender,EventArgs e)
        {
            if(Request.Form["orderID"] != null)
            {
                order_Id = Request.Form["orderID"];
            }
            else if(Request.Form["orderID"].ToString() != null)
            {
                order_Id = Request.QueryString["orderID"].ToString();
            }

            var settings = new RemitaSettings();
            var settingsLogic = new RemitaSettingsLogic();
            settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);
            var remitaResponse = new RemitaResponse();
            var remitaPayment = new RemitaPayment();
            var remitaPaymentLogic = new RemitaPaymentLogic();
            remitaPayment = remitaPaymentLogic.GetModelBy(m => m.OrderId == order_Id);
            string remitaVerifyUrl = ConfigurationManager.AppSettings["RemitaVerifyUrl"];
            var remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
            remitaResponse = remitaProcessor.TransactionStatus(remitaVerifyUrl,remitaPayment);
            if(remitaResponse != null && remitaResponse.Status != null)
            {
                message = remitaResponse.Message;
                rrr = remitaResponse.RRR;
                statuscode = remitaResponse.Status;
                remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                remitaPayment.BankCode = remitaResponse.bank;
                remitaPayment.CustomerName = remitaResponse.RemitaDetails.payerName;
                remitaPayment.TransactionAmount = remitaResponse.amount;
                remitaPaymentLogic.Modify(remitaPayment);
            }
            //return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(remitaResponse);
        }

        private string SHA512(string hash_string)
        {
            var sha512 = new SHA512Managed();
            Byte[] EncryptedSHA512 = sha512.ComputeHash(Encoding.UTF8.GetBytes(hash_string));
            sha512.Clear();
            string hashed = BitConverter.ToString(EncryptedSHA512).Replace("-","").ToLower();
            return hashed;
        }
    }
}