using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Abundance_Nk.Business;

namespace Abundance_Nk.Model.Model
{
    public class RemitaPayementProcessor
    {
        private readonly string apiKey;
        private RemitaResponse remitaResponse;

        public RemitaPayementProcessor(string _apiKey)
        {
            apiKey = _apiKey;
        }

        public string HashPaymentDetailToSHA512(string hash_string)
        {
            var sha512 = new SHA512Managed();
            Byte[] EncryptedSHA512 = sha512.ComputeHash(Encoding.UTF8.GetBytes(hash_string));
            sha512.Clear();
            string hashed = BitConverter.ToString(EncryptedSHA512).Replace("-", "").ToLower();
            return hashed;
        }

        public RemitaResponse PostJsonDataToUrl(string baseAddress, Remita remitaObj, Payment payment)
        {
            remitaResponse = new RemitaResponse();
            try
            {
                string toHash = remitaObj.merchantId + remitaObj.serviceTypeId + remitaObj.orderId +
                                remitaObj.totalAmount + remitaObj.responseurl + apiKey;

                string json = "";
                string jsondata = "";
                if (remitaObj != null)
                {
                    remitaObj.hash = HashPaymentDetailToSHA512(toHash);
                    json = new JavaScriptSerializer().Serialize(remitaObj);
                    using (var request = new WebClient())
                    {
                        request.Headers[HttpRequestHeader.Accept] = "application/json";
                        request.Headers[HttpRequestHeader.ContentType] = "application/json";
                        jsondata = request.UploadString(baseAddress, "POST", json);
                    }
                    jsondata = jsondata.Replace("jsonp(", "");
                    jsondata = jsondata.Replace(")", "");

                    remitaResponse = new JavaScriptSerializer().Deserialize<RemitaResponse>(jsondata);
                }
            }
            catch (Exception ex)
            {
                remitaResponse.Message = ex.Message;
                throw ex;
            }
            return remitaResponse;
        }

        public RemitaResponse PostHtmlDataToUrlold(string baseAddress, Remita remitaObj, Payment payment)
        {
            remitaResponse = new RemitaResponse();
            try
            {
                string toHash = remitaObj.merchantId + remitaObj.serviceTypeId + remitaObj.orderId +
                                remitaObj.totalAmount + remitaObj.responseurl + apiKey;

                string param = "";
                string postdata = "";
                if (remitaObj != null)
                {
                    remitaObj.hash = HashPaymentDetailToSHA512(toHash);
                    param = "payerName=" + remitaObj.payerName + "&merchantId=" + remitaObj.merchantId +
                            "&serviceTypeId=" + remitaObj.serviceTypeId + "&orderId=" + remitaObj.orderId + "&hash=" +
                            remitaObj.hash + "&payerEmail=" + remitaObj.payerEmail + "&payerPhone=" +
                            remitaObj.payerPhone + "&amt=" + remitaObj.totalAmount + "&responseurl=" +
                            remitaObj.responseurl;

                    var request = (HttpWebRequest) WebRequest.Create("https://login.remita.net/remita/ecomm/init.reg");

                    string postData = "payerName=" + remitaObj.payerName;
                    postData += "&merchantId=" + remitaObj.merchantId;
                    postData += "&serviceTypeId=" + remitaObj.serviceTypeId;
                    postData += "&orderId=" + remitaObj.orderId;
                    postData += "&hash=" + remitaObj.hash;
                    postData += "&payerEmail=" + remitaObj.payerEmail;
                    postData += "&payerPhone=" + remitaObj.payerPhone;
                    postData += "&amt=" + remitaObj.totalAmount;
                    postData += "&responseurl=" + remitaObj.responseurl;

                    byte[] data = Encoding.ASCII.GetBytes(postData);

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;

                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse) request.GetResponse();

                    string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    //remitaResponse = new JavaScriptSerializer().Deserialize<RemitaResponse>(responseString);

                    //string URL = "https://login.remita.net/remita/ecomm/init.reg";

                    //using(WebClient wc = new WebClient())
                    //{
                    //    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    //    string HtmlResult = wc.UploadString(URL,param);
                    //}
                }
            }
            catch (Exception ex)
            {
                remitaResponse.Message = ex.Message;
                throw ex;
            }
            return remitaResponse;
        }
        public RemitaResponse PostHtmlDataToUrl(string baseAddress, Remita remitaObj, Payment payment)
        {
            remitaResponse = new RemitaResponse();
            try
            {
                string toHash = remitaObj.merchantId + remitaObj.serviceTypeId + remitaObj.orderId + remitaObj.totalAmount + remitaObj.responseurl + apiKey;

                string param = "";
                string postdata = "";
                if (remitaObj != null)
                {
                    remitaObj.payerEmail = string.IsNullOrEmpty(remitaObj.payerEmail) ? "support@lloydant.com" : remitaObj.payerEmail;

                    remitaObj.hash = HashPaymentDetailToSHA512(toHash);
                    param = "payerName=" + remitaObj.payerName + "&merchantId=" + remitaObj.merchantId + "&serviceTypeId=" + remitaObj.serviceTypeId + "&orderId=" + remitaObj.orderId + "&hash=" + remitaObj.hash + "&payerEmail=" + remitaObj.payerEmail + "&payerPhone=" + remitaObj.payerPhone + "&amt=" + remitaObj.totalAmount + "&responseurl=" + remitaObj.responseurl;

                    var request = (HttpWebRequest)WebRequest.Create("https://login.remita.net/remita/ecomm/init.reg");

                    var postData = "payerName=" + remitaObj.payerName;
                    postData += "&merchantId=" + remitaObj.merchantId;
                    postData += "&serviceTypeId=" + remitaObj.serviceTypeId;
                    postData += "&orderId=" + remitaObj.orderId;
                    postData += "&hash=" + remitaObj.hash;
                    postData += "&payerEmail=" + remitaObj.payerEmail;
                    postData += "&payerPhone=" + remitaObj.payerPhone;
                    postData += "&amt=" + remitaObj.totalAmount;
                    postData += "&responseurl=" + remitaObj.responseurl;
                    postData += "&paymenttype=RRRGEN";

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
                    remitaResponse.RRR = HttpUtility.ParseQueryString(queryString).Get("RRR").Contains(',') ? HttpUtility.ParseQueryString(queryString).Get("RRR").Split(',')[0] : HttpUtility.ParseQueryString(queryString).Get("RRR");
                    remitaResponse.orderId = HttpUtility.ParseQueryString(queryString).Get("orderID").Contains(',') ? HttpUtility.ParseQueryString(queryString).Get("orderID").Split(',')[0] : HttpUtility.ParseQueryString(queryString).Get("orderID");
                    remitaResponse.StatusCode = HttpUtility.ParseQueryString(queryString).Get("statuscode").Contains(',') ? HttpUtility.ParseQueryString(queryString).Get("statuscode").Split(',')[0] : HttpUtility.ParseQueryString(queryString).Get("statuscode");
                    remitaResponse.Status = HttpUtility.ParseQueryString(queryString).Get("status").Contains(',') ? HttpUtility.ParseQueryString(queryString).Get("status").Split(',')[0] : HttpUtility.ParseQueryString(queryString).Get("status");
                }
            }
            catch (Exception ex)
            {
                remitaResponse.Message = ex.Message;
                throw ex;
            }
            return remitaResponse;
        }

        public RemitaResponse TransactionStatus(string baseAddress, RemitaPayment remitaPayment)
        {
            remitaResponse = new RemitaResponse();
            try
            {
                string json = "";
                string jsondata = "";
                if (remitaPayment != null)
                {
                    string toHash = remitaPayment.RRR.Trim() + apiKey.Trim() + remitaPayment.MerchantCode.Trim();
                    string hash = HashPaymentDetailToSHA512(toHash);
                    string URI = baseAddress;
                    string myParameters = "/" + remitaPayment.MerchantCode + "/" + remitaPayment.RRR + "/" + hash +
                                          "/json/status.reg";
                    json = URI + myParameters;
                    jsondata = new WebClient().DownloadString(json);
                    remitaResponse = new JavaScriptSerializer().Deserialize<RemitaResponse>(jsondata);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return remitaResponse;
        }

        public RemitaPayment GenerateRRR(string ivn, string remitaBaseUrl, string description,
            List<RemitaSplitItems> splitItems, RemitaSettings settings, decimal? Amount)
        {
            try
            {
                var remitaPyament = new RemitaPayment();
                var remitaLogic = new RemitaPaymentLogic();
                var payment = new Payment();
                var pL = new PaymentLogic();
                payment = pL.GetModelBy(p => p.Invoice_Number == ivn);

                if (payment.Person.Email == null)
                {
                    payment.Person.Email = "test@lloydant.com";
                }

                if (Amount == 0)
                {
                    Amount = payment.FeeDetails.Sum(p => p.Fee.Amount);
                }
                long milliseconds = DateTime.Now.Ticks;
                string testid = milliseconds.ToString();
                var remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
                var remita = new Remita
                {
                    merchantId = settings.MarchantId,
                    serviceTypeId = settings.serviceTypeId,
                    orderId = testid,
                    totalAmount = (decimal) Amount,
                    payerName = payment.Person.FullName,
                    payerEmail = payment.Person.Email,
                    payerPhone = payment.Person.MobilePhone,
                    responseurl = settings.Response_Url,
                    lineItems = splitItems,
                };

                RemitaResponse remitaResponse = remitaPayementProcessor.PostJsonDataToUrl(remitaBaseUrl, remita, payment);
                if (remitaResponse.Status != null && remitaResponse.StatusCode.Equals("025"))
                {
                    remitaPyament = new RemitaPayment();
                    remitaPyament.payment = payment;
                    remitaPyament.RRR = remitaResponse.RRR;
                    remitaPyament.OrderId = remitaResponse.orderId;
                    remitaPyament.Status = remitaResponse.StatusCode + ":" + remitaResponse.Status;
                    remitaPyament.TransactionAmount = remita.totalAmount;
                    remitaPyament.TransactionDate = DateTime.Now;
                    remitaPyament.MerchantCode = remita.merchantId;
                    remitaPyament.Description = description+" "+"manual";
                    if (remitaLogic.GetBy(payment.Id) == null)
                    {
                        remitaLogic.Create(remitaPyament);
                    }

                    return remitaPyament;
                }
                if (remitaResponse.StatusCode.Trim().Equals("028"))
                {
                    remitaPyament = new RemitaPayment();
                    remitaPyament = remitaLogic.GetModelBy(r => r.OrderId == payment.InvoiceNumber);
                    if (remitaPyament != null)
                    {
                        return remitaPyament;
                    }
                }
                remitaPyament = null;
                return remitaPyament;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Use only for non split transactions
        public RemitaPayment GenerateRRRCard(string ivn, string remitaBaseUrl, string description,
            RemitaSettings settings, decimal? Amount)
        {
            try
            {
                var remitaPyament = new RemitaPayment();
                var remitaLogic = new RemitaPaymentLogic();
                var payment = new Payment();
                var pL = new PaymentLogic();
                payment = pL.GetModelBy(p => p.Invoice_Number == ivn);

                if (payment.Person.Email == null)
                {
                    payment.Person.Email = "test@lloydant.com";
                }

                if (Amount == 0)
                {
                    Amount = payment.FeeDetails.Sum(p => p.Fee.Amount);
                }
                long milliseconds = DateTime.Now.Ticks;
                string testid = milliseconds.ToString();
                var remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
                var remita = new Remita
                {
                    merchantId = settings.MarchantId,
                    serviceTypeId = settings.serviceTypeId,
                    orderId = testid,
                    totalAmount = (decimal) Amount,
                    payerName = payment.Person.FullName,
                    payerEmail = payment.Person.Email,
                    payerPhone = payment.Person.MobilePhone,
                    responseurl = settings.Response_Url,
                    paymenttype = "MasterCard",
                };

                RemitaResponse remitaResponse = remitaPayementProcessor.PostHtmlDataToUrl(remitaBaseUrl, remita, payment);
                if (remitaResponse.Status != null && remitaResponse.StatusCode.Equals("025"))
                {
                    remitaPyament = new RemitaPayment();
                    remitaPyament.payment = payment;
                    remitaPyament.RRR = remitaResponse.RRR;
                    remitaPyament.OrderId = remitaResponse.orderId;
                    remitaPyament.Status = remitaResponse.StatusCode + ":" + remitaResponse.Status;
                    remitaPyament.TransactionAmount = remita.totalAmount;
                    remitaPyament.TransactionDate = DateTime.Now;
                    remitaPyament.MerchantCode = remita.merchantId;
                    remitaPyament.Description = description+" "+"manual";
                    if (remitaLogic.GetBy(payment.Id) == null)
                    {
                        remitaLogic.Create(remitaPyament);
                    }

                    return remitaPyament;
                }
                if (remitaResponse.StatusCode.Trim().Equals("028"))
                {
                    remitaPyament = new RemitaPayment();
                    remitaPyament = remitaLogic.GetModelBy(r => r.OrderId == payment.InvoiceNumber);
                    if (remitaPyament != null)
                    {
                        return remitaPyament;
                    }
                }
                remitaPyament = null;
                return remitaPyament;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void GetTransactionStatus(string rrr, string remitaVerifyUrl, int RemitaSettingId)
        {
            var settings = new RemitaSettings();
            var settingsLogic = new RemitaSettingsLogic();
            settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == RemitaSettingId);
            var remitaResponse = new RemitaResponse();
            var remitaPayment = new RemitaPayment();
            var remitaPaymentLogic = new RemitaPaymentLogic();
            remitaPayment = remitaPaymentLogic.GetModelsBy(m => m.RRR == rrr).FirstOrDefault();
            remitaResponse = TransactionStatus(remitaVerifyUrl, remitaPayment);
            if (remitaResponse != null && remitaResponse.Status != null)
            {
                remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                remitaPaymentLogic.Modify(remitaPayment);
            }
        }

        public RemitaPayment GetStatus(string order_Id)
        {
            var settings = new RemitaSettings();
            var settingsLogic = new RemitaSettingsLogic();
            settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);
            var remitaResponse = new RemitaResponse();
            var remitaPayment = new RemitaPayment();
            var remitaPaymentLogic = new RemitaPaymentLogic();
            remitaPayment = remitaPaymentLogic.GetModelBy(m => m.OrderId == order_Id);
            string remitaVerifyUrl = "https://login.remita.net/remita/ecomm";
            var remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
            remitaResponse = remitaProcessor.TransactionStatus(remitaVerifyUrl, remitaPayment);
            if (remitaResponse != null && remitaResponse.Status != null)
            {
                remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                remitaPaymentLogic.Modify(remitaPayment);
                return remitaPayment;
            }
            return remitaPayment;
        }
    }
}