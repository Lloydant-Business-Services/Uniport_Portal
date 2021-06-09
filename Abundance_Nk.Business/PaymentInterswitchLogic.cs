using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class PaymentInterswitchLogic:BusinessBaseLogic<PaymentInterswitch,PAYMENT_INTERSWITCH>
    {
        private readonly StudentLogic _studentLogic;
        private readonly PaymentLogic paymentLogic;
        private string MacKey;
        private int ProductId;
        private int PaymentItemId;
        private string responseApi;
        private string returnUrl;
        public PaymentInterswitchLogic()
        {
            MacKey = ConfigurationManager.AppSettings["InterswitchMacKey"];
            ProductId = Convert.ToInt32(ConfigurationManager.AppSettings["InterswitchProductId"]);
            PaymentItemId = Convert.ToInt32(ConfigurationManager.AppSettings["InterswitchPaymentItemId"]);
            responseApi = ConfigurationManager.AppSettings["InterswitchResponseApi"];
            returnUrl = ConfigurationManager.AppSettings["InterswitchRetrunUrl"];

            //MacKey = "CEF793CBBE838AA0CBB29B74D571113B4EA6586D3BA77E7CFA0B95E278364EFC4526ED7BD255A366CDDE11F1F607F0F844B09D93B16F7CFE87563B2272007AB3";
            //ProductId = 6207;
            //PaymentItemId = 101;
            //responseApi = "https://webpay.interswitchng.com/paydirect/api/v1/gettransaction.json ";
            //returnUrl = "http://portal.abiastateuniversity.edu.ng/Common/Interswitch/Listen?"; 
            translator = new PaymentInterswitchTranslator();
        }

        public PaymentInterswitch GetBy(long Id)
        {
            PaymentInterswitch request = null;
            try
            {
                request = GetModelsBy(a => a.Payment_Id == Id).LastOrDefault();
            }
            catch (Exception)
            {
                
                throw;
            }
            return request;
        }
        public PaymentInterswitch GetBy(string TransactionReference)
        {
            PaymentInterswitch request = null;
            try
            {
                request = GetModelBy(a => a.MerchantReference == TransactionReference);
            }
            catch (Exception)
            {
                
                throw;
            }
            return request;
        }
        public PaymentInterswitch GetBy(Payment payment,TranscriptRequest transcriptRequest)
        {
            PaymentInterswitch request = new PaymentInterswitch();
            InterswitchSplitDetailLogic interswitchSplitDetailsLogic = new InterswitchSplitDetailLogic();
            
            try
            {
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == transcriptRequest.student.Id).LastOrDefault();
                List<InterswitchSplitDetails> interswitchSplitDetails = interswitchSplitDetailsLogic.GetModelsBy(s => s.FeeType_Id == (int)FeeTypes.Transcript && s.Activated);
                if (interswitchSplitDetails.Count > 0)
                {
                    string splitAccounts = null;
                    decimal lloydantAmount = 0;
                    decimal schoolAmount = 0;

                    if (transcriptRequest != null && transcriptRequest.DeliveryServiceZone != null && transcriptRequest.DeliveryServiceZone.Id > 0)
                    {
                        if (transcriptRequest.DeliveryServiceZone.SchoolAmount != null) schoolAmount = (decimal)transcriptRequest.DeliveryServiceZone.SchoolAmount * 100;
                        if (transcriptRequest.DeliveryServiceZone.LLoydantAmount != null) lloydantAmount = (decimal)transcriptRequest.DeliveryServiceZone.LLoydantAmount * 100;

                        var Sn = 1;
                        splitAccounts += "<item_detail item_id=\"" + Sn + "\" item_name=\"" + transcriptRequest.DeliveryServiceZone.DeliveryService.Name + " \" item_amt=" + "\"" + transcriptRequest.DeliveryServiceZone.DeliveryServiceAmount + "" + "\"  bank_id=\"" + transcriptRequest.DeliveryServiceZone.DeliveryService.BankCode + "\" acct_num=\"" + transcriptRequest.DeliveryServiceZone.DeliveryService.DeliveryServiceAccount + "\"/>";
                       
                        for (int i = 0; i < interswitchSplitDetails.Count; i++)
                        {
                            Sn += 1;
                            if (interswitchSplitDetails[i].BeneficiaryName.Contains("LLOYDANT BUSINESS"))
                            {
                                splitAccounts += "<item_detail item_id=\"" + Sn + "\" item_name=\"" + interswitchSplitDetails[i].BeneficiaryName + " \" item_amt=" + "\"" + lloydantAmount + "" + "\"  bank_id=\"" + interswitchSplitDetails[i].BankCode + "\" acct_num=\"" + interswitchSplitDetails[i].BeneficiaryAccount + "\"/>";
                            }
                            else if (interswitchSplitDetails[i].BeneficiaryName.Contains("ABIA STATE UNIVERSITY"))
                            {
                                splitAccounts += "<item_detail item_id=\"" + Sn + "\" item_name=\"" + interswitchSplitDetails[i].BeneficiaryName + " \" item_amt=" + "\"" + schoolAmount + "" + "\"  bank_id=\"" + interswitchSplitDetails[i].BankCode + "\" acct_num=\"" + interswitchSplitDetails[i].BeneficiaryAccount + "\"/>";
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < interswitchSplitDetails.Count; i++)
                        {
                            var Sn = i + 1;
                            if (interswitchSplitDetails[i].BeneficiaryAmount > 0)
                            {
                                var splitAmountInKobo = interswitchSplitDetails[i].BeneficiaryAmount * 100;
                                splitAccounts += "<item_detail item_id=\"" + Sn + "\" item_name=\"" + interswitchSplitDetails[i].BeneficiaryName + " \" item_amt=" + "\"" + splitAmountInKobo + "" + "\"  bank_id=\"" + interswitchSplitDetails[i].BankCode + "\" acct_num=\"" + interswitchSplitDetails[i].BeneficiaryAccount + "\"/>";

                            }
                        }
                    }
                   
                    string toHash = null;
                    var amount = payment.FeeDetails.Sum(f => f.Fee.Amount) + 150;

                    string uniqueNumber = DateTime.UtcNow.Ticks.ToString();
                    var invoiceNumber = payment.InvoiceNumber + "_" + uniqueNumber.Substring(uniqueNumber.Length - 5);
                    request.Amount = (int)amount * 100;
                    request.MacKey = MacKey;
                    request.MacKeyNosplit = "CEF793CBBE838AA0CBB29B74D571113B4EA6586D3BA77E7CFA0B95E278364EFC4526ED7BD255A366CDDE11F1F607F0F844B09D93B16F7CFE87563B2272007AB3";
                    request.ProductId = ProductId;
                    request.ResponseApi = responseApi;
                    request.PaymentItemId = PaymentItemId;
                    request.ReturnUrl = returnUrl;
                    request.PaymentReference = invoiceNumber;
                    toHash = request.PaymentReference + request.ProductId + request.PaymentItemId + request.Amount + request.ReturnUrl + MacKey;
                    request.PaymentHash = GetHash(toHash);
                    request.PaymentItemName = payment.FeeType.Name;

                    if (studentLevel != null )
                    {
                        request.XmlDataForSplit =
                       "<payment_item_detail>" +
                       "<item_details detail_ref=" + "\"" + invoiceNumber + "\" college=\" " + studentLevel.Department.Faculty.Name + "\" faculty=\" " + studentLevel.Department.Faculty.Name + " \" department=\"" + studentLevel.Department.Name + "\" Programme=\"" + studentLevel.Programme.Name + "\">" +
                         splitAccounts +
                       "</item_details>" +
                       "</payment_item_detail>";
                    }
                    else
                    {
                        request.XmlDataForSplit =
                       "<payment_item_detail>" +
                       "<item_details detail_ref=" + "\"" + invoiceNumber + "\" college=\" ABSU \" department=\" Default \" faculty=\" Default \">" +
                         splitAccounts +
                       "</item_details>" +
                       "</payment_item_detail>";
                    }
                   
                }
            
            }
            catch (Exception ex) 
            {
                
                throw;
            }
            return request;
        }

        public bool Modify(PaymentInterswitch modifyPaymentInterswitch)
        {
            try
            {
                Expression<Func<PAYMENT_INTERSWITCH, bool>> selector = f => f.Payment_Interswitch_Id == modifyPaymentInterswitch.Id;
                PAYMENT_INTERSWITCH entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Amount = modifyPaymentInterswitch.Amount;
                entity.Card_Number = modifyPaymentInterswitch.CardNumber;
                entity.LeadBankCbnCode = modifyPaymentInterswitch.LeadBankCbnCode;
                entity.LeadBankName = modifyPaymentInterswitch.LeadBankName;
                entity.MerchantReference = modifyPaymentInterswitch.MerchantReference;
                entity.PaymentReference = modifyPaymentInterswitch.PaymentReference;
                entity.Payment_Id = modifyPaymentInterswitch.Payment.Id;
                entity.ResponseCode = modifyPaymentInterswitch.ResponseCode;
                entity.ResponseDescription = modifyPaymentInterswitch.ResponseDescription;
                entity.RetrievalReferenceNumber = modifyPaymentInterswitch.RetrievalReferenceNumber;
                if (modifyPaymentInterswitch.SplitAccounts != null && modifyPaymentInterswitch.SplitAccounts.Count() > 0)
                {
                    entity.SplitAccounts = modifyPaymentInterswitch.SplitAccounts[0];
                }
                entity.TransactionDate = modifyPaymentInterswitch.TransactionDate;
               
                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                   return false;
                }

                return true;
            }
            catch (Exception)
            {
                    
                throw;
            }
        }
        public PaymentInterswitch TransactionStatus(string baseAddress, Payment payment, int amount,string tnxRef)
        {
            var interswitchResponse = new PaymentInterswitch();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                string toHash = ProductId + tnxRef + MacKey;
                string hash = GetHash(toHash);
                WebClient webClient = new WebClient();
                webClient.QueryString.Add("productid", ProductId.ToString());
                webClient.QueryString.Add("transactionreference", tnxRef);
                webClient.QueryString.Add("amount", amount.ToString());
                webClient.Headers["hash"] = hash;
                webClient.Headers["accept"] = "application/json";
                webClient.Headers["content-type"] = "application/json";
                string jsondata = webClient.DownloadString(baseAddress);
                interswitchResponse = new JavaScriptSerializer().Deserialize<PaymentInterswitch>(jsondata);
                if (!string.IsNullOrEmpty(interswitchResponse.ResponseCode))
                {
                    if( interswitchResponse.Amount > 0)
                    {
                        interswitchResponse.Amount -= 15000;
                    }
                   
                    PaymentInterswitchLogic paymentInterswitchLogic = new PaymentInterswitchLogic();
                    var transactionLog = paymentInterswitchLogic.GetBy(tnxRef);
                    interswitchResponse.Payment = payment;
                    interswitchResponse.MerchantReference = tnxRef;

                    if (transactionLog == null)
                    {
                        if (interswitchResponse.TransactionDate.Year == 1)
                        {
                            interswitchResponse.TransactionDate = DateTime.Now;
                        }
                       
                        interswitchResponse = paymentInterswitchLogic.Create(interswitchResponse);
                        interswitchResponse.Payment = payment;
                    }
                    else
                    {
                        if (interswitchResponse.TransactionDate.Year == 1)
                        {
                            interswitchResponse.TransactionDate = DateTime.Now;
                        }
                        interswitchResponse.Id = transactionLog.Id;
                        paymentInterswitchLogic.Modify(interswitchResponse);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return interswitchResponse;
        }

        public PaymentInterswitch TransactionStatus(string tnxRef)
        {
            try
            {
                string invoiceNumber = null;
                PaymentLogic paymentLogic = new PaymentLogic();
                if (tnxRef.Contains("_"))
                {
                    invoiceNumber = tnxRef.Split('_').FirstOrDefault();
                }
                var payment = paymentLogic.GetBy(invoiceNumber);
                if (payment != null && payment.Id > 0 )
                {
                    StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                    StudentPayment studentPayment = studentPaymentLogic.GetBy(payment);
                   if (studentPayment != null)
                   {
                       var amountWithCommission = studentPayment.Amount + 150;
                       var amount = amountWithCommission * 100;
                       return TransactionStatus(responseApi, payment, Convert.ToInt32(amount), tnxRef);
                   }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return null;
        }
        private string GetHash(string toHash)
        {
            try
            {

                System.Security.Cryptography.SHA512Managed sha512 = new System.Security.Cryptography.SHA512Managed();
                Byte[] EncryptedSHA512 = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(toHash));
                sha512.Clear();
                string hashed = BitConverter.ToString(EncryptedSHA512).Replace("-", "").ToLower();
                return hashed;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// This Method gets Successful Transaction for the first condition,Pending Transaction for the second and failed Transaction for the else
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="sortOption"></param>
        /// <returns>List of interswitch payments </returns>
        public List<PaymentEtranzactView> GetPaymentBy(string dateFrom, string dateTo, string sortOption)
        {
            try
            {
                DateTime processedDateFrom = new DateTime();
                DateTime processedDateTo = new DateTime();
                List<PaymentEtranzactView> payments = new List<PaymentEtranzactView>();
                processedDateFrom = ConvertToDate(dateFrom);
                processedDateTo = ConvertToDate(dateTo);
                TimeSpan ts = new TimeSpan(00, 00, 0);
                processedDateFrom = processedDateFrom.Date + ts;
                ts = new TimeSpan(23, 59, 0);
                processedDateTo = processedDateTo.Date + ts;

                if (sortOption.Contains("00"))
                {
                    payments = (from sr in repository.GetBy<VW_PAYMENT_INTERSWITCH>(p => (p.TransactionDate >= processedDateFrom && p.TransactionDate <= processedDateTo) && p.ResponseCode.Equals("00"))
                                select new PaymentEtranzactView
                                {
                                    InvoiceNumber = sr.Invoice_Number,
                                    PaymentModeId = sr.Payment_Mode_Id,
                                    FeeTypeId = sr.Fee_Type_Id,
                                    SessionId = sr.Session_Id,
                                    PersonId = sr.Person_Id,
                                    FeeTypeName = sr.Fee_Type_Name,
                                    TransactionAmount = sr.Amount,
                                    FullName = sr.Expr1,
                                    SessionName = sr.Session_Name,
                                    MatricNumber = sr.Matric_NO,
                                    LevelName = sr.Level_Name,
                                    DepartmentName = sr.Department_Name,
                                    FacultyName = sr.Faculty_Name,
                                    ProgrammeName = sr.Programme_Name,
                                    ConfirmationNo = sr.PaymentReference,
                                    TransactionDate = sr.TransactionDate,
                                    ResponseDecription = sr.ResponseDescription,
                                    MearchantReference = sr.MerchantReference,
                                    ResponseCode = sr.ResponseCode,
                                    Issuccessful = true,
                                }).ToList();
                }
                else if (sortOption.Contains("Z0"))
                {
                    payments =
                        (from sr in
                             repository.GetBy<VW_PAYMENT_INTERSWITCH>(
                                 p =>
                                     (p.TransactionDate >= processedDateFrom && p.TransactionDate <= processedDateTo) &&
                                     p.ResponseCode.Contains("Z0"))
                         select new PaymentEtranzactView
                         {
                             InvoiceNumber = sr.Invoice_Number,
                             PaymentModeId = sr.Payment_Mode_Id,
                             FeeTypeId = sr.Fee_Type_Id,
                             SessionId = sr.Session_Id,
                             PersonId = sr.Person_Id,
                             FeeTypeName = sr.Fee_Type_Name,
                             TransactionAmount = sr.Amount,
                             FullName = sr.Expr1,
                             SessionName = sr.Session_Name,
                             MatricNumber = sr.Matric_NO,
                             LevelName = sr.Level_Name,
                             DepartmentName = sr.Department_Name,
                             FacultyName = sr.Faculty_Name,
                             ProgrammeName = sr.Programme_Name,
                             ConfirmationNo = sr.PaymentReference,
                             TransactionDate = sr.TransactionDate,
                             MearchantReference = sr.MerchantReference,
                             ResponseDecription = sr.ResponseDescription,
                             ResponseCode = sr.ResponseCode,
                             Issuccessful = false
                         }).ToList();
                }
                else
                {
                    payments =
                        (from sr in
                             repository.GetBy<VW_PAYMENT_INTERSWITCH>(
                                 p =>
                                     (p.TransactionDate >= processedDateFrom && p.TransactionDate <= processedDateTo) &&
                                     !p.ResponseCode.Contains("Z0") && !p.ResponseCode.Contains("00"))
                         select new PaymentEtranzactView
                         {
                             InvoiceNumber = sr.Invoice_Number,
                             PaymentModeId = sr.Payment_Mode_Id,
                             FeeTypeId = sr.Fee_Type_Id,
                             SessionId = sr.Session_Id,
                             PersonId = sr.Person_Id,
                             FeeTypeName = sr.Fee_Type_Name,
                             TransactionAmount = sr.Amount,
                             FullName = sr.Expr1,
                             SessionName = sr.Session_Name,
                             MatricNumber = sr.Matric_NO,
                             LevelName = sr.Level_Name,
                             DepartmentName = sr.Department_Name,
                             FacultyName = sr.Faculty_Name,
                             ProgrammeName = sr.Programme_Name,
                             ConfirmationNo = sr.PaymentReference,
                             TransactionDate = sr.TransactionDate,
                             MearchantReference = sr.MerchantReference,
                             ResponseDecription = sr.ResponseDescription,
                             ResponseCode = sr.ResponseCode,
                             Issuccessful = false
                         }).ToList();
                }


                return payments;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        private DateTime ConvertToDate(string date)
        {
            DateTime newDate = new DateTime();
            try
            {
                //newDate = DateTime.Parse(date);
                string[] dateSplit = date.Split('-');
                newDate = new DateTime(Convert.ToInt32(dateSplit[0]), Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[2]));
            }
            catch (Exception)
            {
                throw;
            }

            return newDate;
        }

    }
}
