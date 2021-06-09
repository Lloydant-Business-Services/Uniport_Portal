using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class PaymentMonnifyLogic : BusinessBaseLogic<PaymentMonnify, PAYMENT_MONNIFY>
    {
        private RestClient client;
        protected RestRequest request;
        public string RestUrl = "https://sandbox.monnify.com/api/v1/";
        static string ApiEndPoint = "/invoice/create";
        private string Authorization = "";
        private string Secret = "";
        private string ContractCode = "";

        public PaymentMonnifyLogic(string restUrl,string username, string password,string code)
        {
            translator = new PaymentMonnifyTranslator();
            RestUrl = restUrl;
            Secret = password;
            ContractCode = code;
            client = new RestClient(RestUrl);
            Authorization = Base64Encode(username + ":" + password);
        }

        public PaymentMonnify GetBy(string invoiceNumber)
        {
            return GetModelBy(a => a.PAYMENT.Invoice_Number == invoiceNumber);
        }

        public PAYMENT_MONNIFY GetEntityModelBy(string invoiceNumber)
        {
            return GetEntityBy(a => a.PAYMENT.Invoice_Number == invoiceNumber);
        }

        public PaymentMonnify GenerateInvoice(Payment payment,string Amount, DateTime InvoiceExpiryDate,List<MonnifySplit> monnifySplits = null)
        {
            try
            {
                ValidateInputs(payment,Amount,InvoiceExpiryDate,monnifySplits);

                JsonResultData MonnifyInvoice = null;
                var existingData = GetBy(payment.InvoiceNumber);
                if (existingData == null)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ApiEndPoint = "/invoice/create";
                    request = new RestRequest(ApiEndPoint, Method.POST);
                    request.AddHeader("accept", "application/json");
                    request.AddHeader("Authorization", $"Basic {Authorization}");
                    var postBody = FormatData(payment, InvoiceExpiryDate.ToString("yyyy-MM-dd HH:mm:ss"), Amount, monnifySplits);
                    request.RequestFormat = DataFormat.Json;
                    request.AddBody(postBody);
                    var result = client.Execute(request);
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MonnifyInvoice = JsonConvert.DeserializeObject<JsonResultData>(result.Content);
                        MonnifyInvoice.responseBody.Payment = payment;
                        Create(MonnifyInvoice.responseBody);
                    }
                    if (MonnifyInvoice != null)
                    {
                        return MonnifyInvoice.responseBody;
                    }
                    return null;
                    
                }
                else
                {
                    return existingData;
                }
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public PaymentMonnify GetInvoiceStatus(string invoiceNumber)
        {
            try
            {
                JsonResultData MonnifyInvoice = null;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ApiEndPoint = "/merchant/transactions/query?paymentReference="+invoiceNumber;
                request = new RestRequest(ApiEndPoint, Method.GET);
                request.AddHeader("accept", "application/json");
                request.AddHeader("Authorization", $"Basic {Authorization}");
                var result = client.Execute(request);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MonnifyInvoice = JsonConvert.DeserializeObject<JsonResultData>(result.Content);
                    var invoicePayment = GetEntityModelBy(invoiceNumber);
                    if (invoicePayment != null)
                    {
                        var toHash = Secret+"|"+invoiceNumber+"|"+ MonnifyInvoice.responseBody.Amount +"|" + MonnifyInvoice.responseBody.CompletedOn + "|"+ MonnifyInvoice.responseBody.TransactionReference;
                        //string hash = SHA512(toHash).ToString();

                        //if (hash == MonnifyInvoice.responseBody.TransactionHash)
                        //{
                            if (MonnifyInvoice.responseBody.Fee > 0 && MonnifyInvoice.responseBody.AmountPaid > 0)
                            {
                                invoicePayment.Fee = MonnifyInvoice.responseBody.Fee;
                                invoicePayment.Transaction_Reference = MonnifyInvoice.responseBody.TransactionReference;
                                invoicePayment.Amount = MonnifyInvoice.responseBody.AmountPaid;

                                if (MonnifyInvoice.responseBody.AmountPaid == MonnifyInvoice.responseBody.PayableAmount)
                                {
                                    invoicePayment.Invoice_Status = true;
                                    invoicePayment.Completed = true;
                                    invoicePayment.Completed_Date = MonnifyInvoice.responseBody.CompletedOn;
                                }

                                    var model = translator.TranslateToModel(invoicePayment);
                                    if(model != null)
                                    {
                                        UpdateScratchCardPin(model.Payment);
                                    }
                                    

                                Save();
                            }
                            
                            
                           ;
                        //}
                        
                    }
                    else
                    {
                        Create(MonnifyInvoice.responseBody);
                    }

                }

                if (MonnifyInvoice != null)
                {
                    MonnifyInvoice.responseBody = GetBy(invoiceNumber);
                    return MonnifyInvoice.responseBody;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static Payment UpdateScratchCardPin(Payment payment)
        {
            //Check if payment is scratch card
            PaymentLogic paymentLogic = new PaymentLogic();
            payment = paymentLogic.GetBy(payment.InvoiceNumber);
            if (payment.Id > 0 && payment.FeeType.Id == 7)
            {
                ScratchCard card = new ScratchCard();
                ScratchCardLogic cardLogic = new ScratchCardLogic();
                var cardExists = cardLogic.GetBy(payment.InvoiceNumber);
                if (cardExists == null)
                {
                    card.Batch = new ScratchCardBatch() { Id = 1 };
                    card.FirstUsedDate = DateTime.Now;
                    card.person = payment.Person;
                    card.Pin = payment.InvoiceNumber;
                    card.SerialNumber = payment.SerialNumber.ToString();
                    card.UsageCount = 0;
                    cardLogic.Create(card);
                }


            }
            else if (payment.Id > 0 && payment.FeeType.Id == 8)
            {
                ScratchCard card = new ScratchCard();
                ScratchCardLogic cardLogic = new ScratchCardLogic();
                var cardExists = cardLogic.GetBy(payment.InvoiceNumber);
                if (cardExists == null)
                {
                    card.Batch = new ScratchCardBatch() { Id = 2 };
                    card.FirstUsedDate = DateTime.Now;
                    card.person = payment.Person;
                    card.Pin = payment.InvoiceNumber;
                    card.SerialNumber = payment.SerialNumber.ToString();
                    card.UsageCount = 0;
                    cardLogic.Create(card);
                }

            }
            return payment;
        }


        private MonnifyJsonData FormatData(Payment payment, string InvoiceExpiryDate, string Amount, List<MonnifySplit> monnifySplits)
        {
            if (payment != null)
            {
                MonnifyJsonData data = new MonnifyJsonData
                {
                    amount = Amount,
                    invoiceReference = payment.InvoiceNumber,
                    description = payment.FeeType.Name,
                    currencyCode = "NGN",
                    contractCode = ContractCode,
                    customerEmail = payment.Person.Email ?? payment.Person.LastName + "." + payment.Person.FirstName + "@abiastateuniveristy.edu.ng",
                    customerName = payment.Person.LastName + " " + payment.Person.FirstName + " " + payment.Person.OtherName,
                    expiryDate = InvoiceExpiryDate
                };
                if (monnifySplits != null && monnifySplits.Count > 0)
                {
                    data.incomeSplitConfig = monnifySplits;
                }
                return data;
            }
            return null;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        public void ValidateInputs(Payment payment, string Amount, DateTime InvoiceExpiryDate, List<MonnifySplit> monnifySplits = null)
        {
            if (payment == null)
            {
                throw new NullReferenceException("Payment object is Null");
            }

            if (String.IsNullOrEmpty(payment.InvoiceNumber))
            {
                throw new NullReferenceException("Invoice Number is Null");
            }

            if (payment.FeeType == null)
            {
                throw new NullReferenceException("Fee Type is Null");
            }

            if (String.IsNullOrEmpty(payment.FeeType.Name))
            {
                throw new NullReferenceException("Fee Type Name is Null");
            }

            if (payment.Person == null)
            {
                throw new NullReferenceException("Person is Null");
            }

            if (String.IsNullOrEmpty(payment.Person.LastName) || String.IsNullOrEmpty(payment.Person.FirstName))
            {
                throw new NullReferenceException("Person Name is Null");
            }
        }

    }
}
