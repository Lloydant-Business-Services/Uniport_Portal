using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Transactions;
using Abundance_Nk.Business.eTranzactWebService;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class PaymentEtranzactLogic : BusinessBaseLogic<PaymentEtranzact, PAYMENT_ETRANZACT>
    {
        private string baseUrl = "https://www.etranzact.net/WebConnectPlus/query.jsp";

        public PaymentEtranzactLogic()
        {
            translator = new PaymentEtranzactTranslator();
        }

        public PaymentEtranzact Create(PaymentEtranzact paymentEtranzact, PaymentEtranzactAudit audit)
        {
            try
            {
                var newPaymentEtranzact =  base.Create(paymentEtranzact);
                if (paymentEtranzact != null)
                {
                    CreateAudit(newPaymentEtranzact,audit);
                }
                return newPaymentEtranzact;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool CreateAudit(PaymentEtranzact paymentEtranzact,PaymentEtranzactAudit audit)
        {
            try
            {
                audit.EtranzactType = paymentEtranzact.EtranzactType;
                audit.BankCode = paymentEtranzact.BankCode;
                audit.BranchCode = paymentEtranzact.BranchCode;
                audit.ConfirmationNo = paymentEtranzact.PaymentCode;
                audit.CustomerAddress = paymentEtranzact.CustomerAddress;
                audit.CustomerID = paymentEtranzact.CustomerID;
                audit.CustomerName = paymentEtranzact.CustomerName;
                audit.MerchantCode = paymentEtranzact.MerchantCode;
                audit.PaymentCode = paymentEtranzact.PaymentCode;
                audit.ReceiptNo = paymentEtranzact.ReceiptNo;
                audit.TransactionAmount = paymentEtranzact.TransactionAmount;
                audit.TransactionDate = DateTime.Now;
                audit.TransactionDescription = paymentEtranzact.TransactionDescription;
                audit.Used = false;
                audit.Terminal = paymentEtranzact.Terminal;
                audit.UsedBy = paymentEtranzact.UsedBy;
                audit.Payment = paymentEtranzact.Payment;
                PaymentEtranzactAuditLogic etranzactAuditLogic = new PaymentEtranzactAuditLogic();
                etranzactAuditLogic.Create(audit);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PaymentEtranzact GetBy(Payment payment)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p => p.Payment_Id == payment.Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaymentEtranzact> GetByAsync(Payment payment)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p => p.Payment_Id == payment.Id;
                return await GetModelByAsync(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Function to validate payment details from eTranzact table
        public bool ValidatePin(PaymentEtranzact etranzactPayment, Payment payment, decimal Amount)
        {
            try
            {
                decimal accepatanceDecrease = Amount + 10000M;
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector =
                    p =>
                        p.Confirmation_No == etranzactPayment.ConfirmationNo && p.Customer_Id == payment.InvoiceNumber &&
                        (p.Transaction_Amount == Amount || p.Transaction_Amount == 10000 || p.Transaction_Amount == 2150
                         || (p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == 2 && p.Transaction_Amount == accepatanceDecrease));
                List<PaymentEtranzact> etranzactPayments = GetModelsBy(selector);
                if (etranzactPayments != null && etranzactPayments.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool IsPinOnTable(string confirmationOrderNumber)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p =>p.Confirmation_No == confirmationOrderNumber;
                List<PaymentEtranzact> etranzactPayments = GetModelsBy(selector);
                if (etranzactPayments != null && etranzactPayments.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool IsInvoiceOnTable(string invoiceNumber)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p => p.Customer_Id == invoiceNumber;
                List<PaymentEtranzact> etranzactPayments = GetModelsBy(selector);
                if (etranzactPayments != null && etranzactPayments.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsPinUsed(string confirmationOrderNumber, int personId)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector =
                    p =>
                        p.Confirmation_No == confirmationOrderNumber && p.Used == true &&
                        (p.Used_By_Person_Id != personId ||p.Used_By_Person_Id == null );
                List<PaymentEtranzact> etranzactPayments = GetModelsBy(selector);
                if (etranzactPayments != null && etranzactPayments.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PaymentEtranzact RetrievePin(string confirmationNo, PaymentTerminal TerminalID)
        {
            try
            {
                string ReceiptNo = "";
                string PaymentCode = "";
                string MerchantCode = "";
                string TransactionAmount = "";
                string TransactionDescription = "";
                string BankCode = "";
                string BankBranchCode = "";
                string CustomerName = "";
                string CustomerAddress = "";
                string CustomerId = "";
                //string Session = "";

                var hsParams = new Hashtable();
                hsParams.Clear();
                var payoutletTransaction = new queryPayoutletTransaction();
                var gateWayResponse = new queryPayoutletTransactionResponse();

                payoutletTransaction.confirmationNo = confirmationNo.Trim();

                payoutletTransaction.terminalId = TerminalID.TerminalId;
                var ws = new QueryPayoutletTransactionClient();

                gateWayResponse = ws.queryPayoutletTransaction(payoutletTransaction);
                string Result = gateWayResponse.@return;

                var paymentEtz = new PaymentEtranzact();
                if (Result != "-1")
                {
                    String[] RSplit = Result.Replace("%20&", "%20and").Replace("%20", " ").Split('&');
                    String[] Rsplitx;
                    foreach (string s in RSplit)
                    {
                        Rsplitx = s.Split('=');
                        hsParams.Add(Rsplitx[0], Rsplitx[1]);
                    }

                    ReceiptNo = hsParams["RECEIPT_NO"].ToString().Trim();
                    PaymentCode = hsParams["PAYMENT_CODE"].ToString().Trim();
                    MerchantCode = hsParams["MERCHANT_CODE"].ToString().Trim();
                    TransactionAmount = hsParams["TRANS_AMOUNT"].ToString().Trim();
                    TransactionDescription = hsParams["TRANS_DESCR"].ToString().Trim();
                    BankCode = hsParams["BANK_CODE"].ToString().Trim();
                    BankBranchCode = hsParams["BRANCH_CODE"].ToString().Trim();
                    CustomerName = hsParams["CUSTOMER_NAME"].ToString().Trim();
                    CustomerAddress = hsParams["CUSTOMER_ADDRESS"].ToString().Trim();
                    CustomerId = hsParams["CUSTOMER_ID"].ToString().Trim();
                    //Session = "1";
                    hsParams.Clear();

                    paymentEtz.BankCode = BankCode;
                    paymentEtz.BranchCode = BankBranchCode;
                    paymentEtz.ConfirmationNo = PaymentCode;
                    paymentEtz.CustomerAddress = CustomerAddress;
                    paymentEtz.CustomerID = CustomerId;
                    paymentEtz.CustomerName = CustomerName;
                    paymentEtz.MerchantCode = MerchantCode;
                    paymentEtz.PaymentCode = PaymentCode;
                    paymentEtz.ReceiptNo = ReceiptNo;
                    paymentEtz.TransactionAmount = Convert.ToDecimal(TransactionAmount);
                    paymentEtz.TransactionDate = DateTime.Now;
                    paymentEtz.TransactionDescription = TransactionDescription;
                    paymentEtz.Used = false;
                    paymentEtz.Terminal = TerminalID;
                    paymentEtz.UsedBy = 0;

                    var paymentEtranzactType = new PaymentEtranzactType();
                    var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                    paymentEtranzactType =
                        paymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == TerminalID.FeeType.Id)
                            .LastOrDefault();
                    paymentEtz.EtranzactType = paymentEtranzactType;

                    var payment = new Payment();
                    var paymentLogic = new PaymentLogic();
                    payment = paymentLogic.GetModelBy(m => m.Invoice_Number == CustomerId);

                    if (payment != null)
                    {
                        var onlinePayment = new OnlinePayment();
                        var onlinePaymentLogic = new OnlinePaymentLogic();
                        onlinePayment =
                            onlinePaymentLogic.GetModelBy(
                                c =>
                                    c.PAYMENT_CHANNEL.Payment_Channnel_Id == (int) PaymentChannel.Channels.Etranzact &&
                                    c.Payment_Id == payment.Id);

                        paymentEtz.Payment = onlinePayment;
                    }

                    base.Create(paymentEtz);
                    return paymentEtz;
                }


                return paymentEtz;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while verifying pin!");
            }
        }

        public bool UpdatePin(Payment payment, Person person)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p => p.Payment_Id == payment.Id;
                PAYMENT_ETRANZACT paymentEtranzactEntity = GetEntityBy(selector);

                if (paymentEtranzactEntity == null || paymentEtranzactEntity.Payment_Id <= 0)
                {
                    return false;
                }

                paymentEtranzactEntity.Used = true;
                paymentEtranzactEntity.Used_By_Person_Id = person.Id;

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

        public PaymentEtranzact RetrievePinsWithoutInvoice(string confirmationNo, string ivn, FeeType FeeType,  PaymentTerminal TerminalID)
        {
            try
            {
                var paymentEtz = new PaymentEtranzact();
                using (
                    var transaction = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions {IsolationLevel = IsolationLevel.Snapshot}))
                {
                    string ReceiptNo = "";
                    string PaymentCode = "";
                    string MerchantCode = "";
                    string TransactionAmount = "";
                    string TransactionDescription = "";
                    string BankCode = "";
                    string BankBranchCode = "";
                    string CustomerName = "";
                    string CustomerAddress = "";
                    string CustomerId = "";
                    //string Session = "";

                    
                    var hsParams = new Hashtable();
                    hsParams.Clear();
                    string Result = "";

                    var request = (HttpWebRequest)HttpWebRequest.Create(baseUrl);

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    string postData = "TERMINAL_ID=" + TerminalID.TerminalId + "&CONFIRMATION_NO=" + confirmationNo;
                    byte[] bytes = Encoding.UTF8.GetBytes(postData);
                    request.ContentLength = bytes.Length;

                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);

                    WebResponse response = request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    var reader = new StreamReader(stream);

                    Result = reader.ReadToEnd();
                    stream.Dispose();
                    reader.Dispose();

                    if (Result != "-1")
                    {
                        String[] RSplit = Result.Replace("\r\n", "").Replace("</html>", "").Replace("%20&", "%20and").Replace("%20", " ").Split('&');
                        String[] Rsplitx;
                        foreach (string s in RSplit)
                        {
                            Rsplitx = s.Split('=');
                            hsParams.Add(Rsplitx[0], Rsplitx[1]);
                        }

                        ReceiptNo = hsParams["RECEIPT_NO"].ToString().Trim();
                        PaymentCode = hsParams["PAYMENT_CODE"].ToString().Trim();
                        MerchantCode = hsParams["MERCHANT_CODE"].ToString().Trim();
                        TransactionAmount = hsParams["TRANS_AMOUNT"].ToString().Trim();
                        TransactionDescription = hsParams["TRANS_DESCR"].ToString().Trim();
                        BankCode = hsParams["BANK_CODE"].ToString().Trim();
                        BankBranchCode = hsParams["BRANCH_CODE"].ToString().Trim();
                        CustomerName = hsParams["CUSTOMER_NAME"].ToString().Trim();
                        CustomerAddress = hsParams["CUSTOMER_ADDRESS"].ToString().Trim();
                        CustomerId = hsParams["CUSTOMER_ID"].ToString().Trim();
                        hsParams.Clear();

                        paymentEtz.BankCode = BankCode;
                        paymentEtz.BranchCode = BankBranchCode;
                        paymentEtz.ConfirmationNo = PaymentCode;
                        paymentEtz.CustomerAddress = CustomerAddress;
                        paymentEtz.CustomerName = CustomerName;
                        paymentEtz.MerchantCode = MerchantCode;
                        paymentEtz.PaymentCode = PaymentCode;
                        paymentEtz.ReceiptNo = ReceiptNo;
                        paymentEtz.TransactionAmount = Convert.ToDecimal(TransactionAmount)-350;
                        paymentEtz.TransactionDate = DateTime.Now;
                        paymentEtz.TransactionDescription = TransactionDescription;
                        paymentEtz.Used = false;
                        paymentEtz.Terminal = TerminalID;
                        paymentEtz.UsedBy = 0;


                        var feeType = new FeeType();
                        feeType =  FeeType;
                        var pet = new PaymentEtranzactType();
                        pet = GetPaymentTypeBy(feeType);

                        var paymentLogic = new PaymentLogic();
                        Payment payment = paymentLogic.GetBy(ivn);


                        var paymentEtranzactType = new PaymentEtranzactType();
                        var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                        paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == feeType.Id).FirstOrDefault();
                        paymentEtz.EtranzactType = paymentEtranzactType;
                        if (payment != null)
                        {
                            paymentEtz.Payment = new OnlinePayment(){Payment = payment };
                            paymentEtz.CustomerID = payment.InvoiceNumber;
                        }
                        

                        base.Create(paymentEtz);
                        transaction.Complete();
                        return paymentEtz;
                    }
                }


                return paymentEtz;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public FeeType GetFeeTypeBy(Session session, Programme programme)
        //{
        //    try
        //    {
        //        ApplicationProgrammeFeeLogic programmeFeeLogic = new ApplicationProgrammeFeeLogic();
        //        ApplicationProgrammeFee A = new ApplicationProgrammeFee();
        //        A = programmeFeeLogic.GetBy(programme, session);

        //        if (A != null)
        //        {
        //            return A.FeeType;
        //        }

        //        return null;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public PaymentEtranzactType GetPaymentTypeBy(FeeType feeType)
        {
            var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
            var p = new PaymentEtranzactType();
            p = paymentEtranzactTypeLogic.GetBy(feeType);

            if (p != null)
            {
                return p;
            }

            return null;
        }

        public ApplicationFormSetting GetApplicationFormSettingBy(Session session)
        {
            try
            {
                var applicationFormSettingLogic = new ApplicationFormSettingLogic();
                return applicationFormSettingLogic.GetBy(session);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Session GetCurrentSession()
        {
            try
            {
                var currentSessionLogic = new CurrentSessionSemesterLogic();
                CurrentSessionSemester currentSessionSemester = currentSessionLogic.GetCurrentSessionTerm();

                if (currentSessionSemester != null && currentSessionSemester.SessionSemester != null)
                {
                    return currentSessionSemester.SessionSemester.Session;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FeeType GetFeeTypeBy(Session session, Programme programme)
        {
            try
            {
                var programmeFeeLogic = new ApplicationProgrammeFeeLogic();
                List<ApplicationProgrammeFee> applicationProgrammeFess = programmeFeeLogic.GetListBy(programme, session);
                foreach (ApplicationProgrammeFee item in applicationProgrammeFess)
                {
                    if (item.FeeType.Id <= 6)
                    {
                        return item.FeeType;
                    }
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PaymentEtranzact RetrieveEtranzactWebServicePinDetails(string confirmationNo, PaymentTerminal TerminalID)
        {
            try
            {
                string ReceiptNo = "";
                string PaymentCode = "";
                string MerchantCode = "";
                string TransactionAmount = "";
                string TransactionDescription = "";
                string BankCode = "";
                string BankBranchCode = "";
                string CustomerName = "";
                string CustomerAddress = "";
                string CustomerId = "";
                //string Session = "";

                var hsParams = new Hashtable();
                hsParams.Clear();
                string Result = "";

                var request = (HttpWebRequest) WebRequest.Create(baseUrl);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string postData = "TERMINAL_ID=" + TerminalID.TerminalId + "&CONFIRMATION_NO=" + confirmationNo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                var reader = new StreamReader(stream);

                Result = reader.ReadToEnd();
                stream.Dispose();
                reader.Dispose();


                var paymentEtz = new PaymentEtranzact();
                if (Result != "-1" && Result != "\r\n\r\nSUCCESS=-1\r\n\r\n\r\n</html>" && Result.Length > 10)
                {
                    String[] RSplit =
                        Result.Replace("\r\n", "")
                            .Replace("</html>", "")
                            .Replace("=%20&", "=FPIDEFAULT&")
                            .Replace("%20&", "%20and")
                            .Replace("%20", " ")
                            .Split('&');
                    String[] Rsplitx;
                    foreach (string s in RSplit)
                    {
                        Rsplitx = s.Split('=');
                        hsParams.Add(Rsplitx[0], Rsplitx[1]);
                    }

                    if (hsParams.Count < 5)
                    {
                        throw new Exception("Pin might not be valid, please cross check and try again");
                    }
                    ReceiptNo = hsParams["RECEIPT_NO"].ToString().Trim();
                    PaymentCode = hsParams["PAYMENT_CODE"].ToString().Trim();
                    MerchantCode = hsParams["MERCHANT_CODE"].ToString().Trim();
                    TransactionAmount = hsParams["TRANS_AMOUNT"].ToString().Trim();
                    TransactionDescription = hsParams["TRANS_DESCR"].ToString().Trim();
                    BankCode = hsParams["BANK_CODE"].ToString().Trim();
                    CustomerName = hsParams["CUSTOMER_NAME"].ToString().Trim();
                    BankBranchCode = hsParams["BRANCH_CODE"].ToString().Trim();
                    CustomerId = hsParams["CUSTOMER_ID"].ToString().Trim();
                    CustomerAddress = hsParams["CUSTOMER_ADDRESS"].ToString().Trim();

                    hsParams.Clear();

                    paymentEtz.BankCode = BankCode;
                    paymentEtz.BranchCode = BankBranchCode;
                    paymentEtz.ConfirmationNo = PaymentCode;
                    paymentEtz.CustomerAddress = CustomerAddress;
                    paymentEtz.CustomerID = CustomerId;
                    paymentEtz.CustomerName = CustomerName;
                    paymentEtz.MerchantCode = MerchantCode;
                    paymentEtz.PaymentCode = PaymentCode;
                    paymentEtz.ReceiptNo = ReceiptNo;
                    paymentEtz.TransactionAmount = Convert.ToDecimal(TransactionAmount);
                    paymentEtz.TransactionDate = DateTime.Now;
                    paymentEtz.TransactionDescription = TransactionDescription;
                    paymentEtz.Used = false;
                    paymentEtz.Terminal = TerminalID;
                    paymentEtz.UsedBy = 0;
                     paymentEtz.Payment = new OnlinePayment();
                    PaymentLogic paymentLogic = new PaymentLogic();
                    paymentEtz.Payment.Payment = paymentLogic.GetBy(CustomerId);

                    return paymentEtz;
                }


                return paymentEtz;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PaymentEtranzact RetrievePinAlternative(string confirmationNo, PaymentTerminal TerminalID)
        {
            try
            {
                string ReceiptNo = "";
                string PaymentCode = "";
                string MerchantCode = "";
                string TransactionAmount = "";
                string TransactionDescription = "";
                string BankCode = "";
                string BankBranchCode = "";
                string CustomerName = "";
                string CustomerAddress = "";
                string CustomerId = "";
                //string Session = "";


                var hsParams = new Hashtable();
                hsParams.Clear();
                string Result = "";
                // using System.Net;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(baseUrl);
                //var request = (HttpWebRequest) HttpWebRequest.Create(baseUrl);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string postData = "TERMINAL_ID=" + TerminalID.TerminalId + "&CONFIRMATION_NO=" + confirmationNo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                WebResponse response = request.GetResponse();
                //WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                var reader = new StreamReader(stream);

                Result = reader.ReadToEnd();
                stream.Dispose();
                reader.Dispose();


                var paymentEtz = new PaymentEtranzact();
                if (Result != "-1" && Result.Length > 10)
                {
                    String[] RSplit =
                        Result.Replace("\r\n", "")
                            .Replace("</html>", "")
                            .Replace("%20&", "%20and")
                            .Replace("%20", " ")
                            .Split('&');
                    String[] Rsplitx;
                    foreach (string s in RSplit)
                    {
                        Rsplitx = s.Split('=');
                        hsParams.Add(Rsplitx[0], Rsplitx[1]);
                    }
                    if (hsParams.Count < 5)
                    {
                        throw new Exception("Pin might not be valid, please cross check and try again");
                    }
                    ReceiptNo = hsParams["RECEIPT_NO"].ToString().Trim();
                    PaymentCode = hsParams["PAYMENT_CODE"].ToString().Trim();
                    MerchantCode = hsParams["MERCHANT_CODE"].ToString().Trim();
                    TransactionAmount = hsParams["TRANS_AMOUNT"].ToString().Trim();
                    TransactionDescription = hsParams["TRANS_DESCR"].ToString().Trim();
                    BankCode = hsParams["BANK_CODE"].ToString().Trim();
                    BankBranchCode = hsParams["BRANCH_CODE"].ToString().Trim();
                    CustomerName = hsParams["CUSTOMER_NAME"].ToString().Trim();
                    CustomerAddress = hsParams["CUSTOMER_ADDRESS"].ToString().Trim();
                    CustomerId = hsParams["CUSTOMER_ID"].ToString().Trim();
                    //Session = "1";
                    hsParams.Clear();

                    paymentEtz.BankCode = BankCode;
                    paymentEtz.BranchCode = BankBranchCode;
                    paymentEtz.ConfirmationNo = PaymentCode;
                    paymentEtz.CustomerAddress = CustomerAddress;
                    paymentEtz.CustomerID = CustomerId;
                    paymentEtz.CustomerName = CustomerName;
                    paymentEtz.MerchantCode = MerchantCode;
                    paymentEtz.PaymentCode = PaymentCode;
                    paymentEtz.ReceiptNo = ReceiptNo;
                    paymentEtz.TransactionAmount = Convert.ToDecimal(TransactionAmount);
                    paymentEtz.TransactionDate = DateTime.Now;
                    paymentEtz.TransactionDescription = TransactionDescription;
                    paymentEtz.Used = false;
                    paymentEtz.Terminal = TerminalID;
                    paymentEtz.UsedBy = 0;

                    var paymentEtranzactType = new PaymentEtranzactType();
                    var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                    paymentEtranzactType =
                        paymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == TerminalID.FeeType.Id)
                            .FirstOrDefault();

                    paymentEtz.EtranzactType = paymentEtranzactType;

                    var payment = new Payment();
                    var paymentLogic = new PaymentLogic();
                    //CustomerId
                    payment = paymentLogic.GetModelBy(m => m.Invoice_Number == CustomerId);

                    if (payment != null)
                    {
                        var onlinePayment = new OnlinePayment();
                        var onlinePaymentLogic = new OnlinePaymentLogic();
                        onlinePayment =
                            onlinePaymentLogic.GetModelBy(
                                c =>
                                    c.PAYMENT_CHANNEL.Payment_Channnel_Id == (int) PaymentChannel.Channels.Etranzact &&
                                    c.Payment_Id == payment.Id);

                        paymentEtz.Payment = onlinePayment;
                    }

                    var DoesTransactionExist = GetModelsBy(a => a.Confirmation_No == paymentEtz.ConfirmationNo).FirstOrDefault();
                    if (DoesTransactionExist != null)
                    {
                        return DoesTransactionExist;
                    }
                    else
                    {
                        base.Create(paymentEtz);
                        return paymentEtz;
                    }
                    
                }


                return paymentEtz;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public bool HasStudentPaidFirstInstallmentOrCompletedFeesForSession(long StudentId, Session session, FeeType feeType)
        {
            try
            {
              
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p =>(p.ONLINE_PAYMENT.PAYMENT.Person_Id == StudentId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == feeType.Id && p.ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id == 1) ||(p.ONLINE_PAYMENT.PAYMENT.Person_Id == StudentId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == feeType.Id && p.ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id == 2);
                PaymentEtranzact payment = GetModelBy(selector);
                if (payment != null && payment.ConfirmationNo != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }
        public bool HasStudentPaidSugTishipFeeForSession(long StudentId, Session session, FeeType feeType)
        {
            try
            {

                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p =>p.ONLINE_PAYMENT.PAYMENT.Person_Id == StudentId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == feeType.Id && p.ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id == 1;
                PaymentEtranzact payment = GetModelBy(selector);
                if (payment != null && payment.ConfirmationNo != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

        public bool HasStudentCompletedFeesForSession(long StudentId, Session session, FeeType feeType)
        {
            try
            {
                //Expression<Func<PAYMENT_ETRANZACT, bool>> previousselector =p =>(p.ONLINE_PAYMENT.PAYMENT.Person_Id == StudentId && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == feeType.Id && p.ONLINE_PAYMENT.PAYMENT.Session_Id == 1);
                //PaymentEtranzact previousPaymentEtranzactpayment = GetModelBy(previousselector);
                //if (previousPaymentEtranzactpayment != null)
                //{
                //    return true;
                //}
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector =
                    p =>
                        (p.ONLINE_PAYMENT.PAYMENT.Person_Id == StudentId &&
                         p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id &&
                         p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == feeType.Id &&
                         p.ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id == 1) ||
                        (p.ONLINE_PAYMENT.PAYMENT.Person_Id == StudentId &&
                         p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id &&
                         p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == feeType.Id &&
                         p.ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id == 3);
                PaymentEtranzact payment = GetModelBy(selector);
                if (payment != null && payment.ConfirmationNo != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

        public bool Modify(PaymentEtranzact paymetEtranzact, PaymentEtranzactAudit audit)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector =
                    p => p.Confirmation_No == paymetEtranzact.ConfirmationNo;
                PAYMENT_ETRANZACT paymentEtranzactEntity = GetEntityBy(selector);

                if (paymentEtranzactEntity != null)
                {
                    PaymentVerificationLogic paymentVerificationLogic = new PaymentVerificationLogic();
                    PaymentVerification paymentVerification = paymentVerificationLogic.GetBy(paymetEtranzact.Payment.Payment.Id);
                    if (paymentVerification == null)
                    {
                        paymentEtranzactEntity.Payment_Id = paymetEtranzact.Payment.Payment.Id;
                        paymentEtranzactEntity.Customer_Id = paymetEtranzact.Payment.Payment.InvoiceNumber;
                        paymentEtranzactEntity.Confirmation_No = paymetEtranzact.ConfirmationNo;
                        paymentEtranzactEntity.Transaction_Amount = paymetEtranzact.TransactionAmount;

                        int modified = Save();
                        CreateAudit(paymetEtranzact, audit);
                        if (modified > 0)
                        {
                            return true;
                        }
                    }
                   
                }

                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool Delete(PaymentEtranzact paymetEtranzact)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector =p => p.Payment_Id == paymetEtranzact.Payment.Payment.Id;
                return Delete(selector);
            }
            catch (Exception)
            {
                
                throw;
            }
            return false;
        }

        public List<PaymentEtranzactView> GetPaymentBy(string dateFrom, string dateTo)
        {
            try
            {
                DateTime processedDateFrom = new DateTime();
                DateTime processedDateTo = new DateTime();

                processedDateFrom = ConvertToDate(dateFrom);
                processedDateTo = ConvertToDate(dateTo);
                TimeSpan ts = new TimeSpan(00, 00, 0);
                processedDateFrom = processedDateFrom.Date + ts;
                ts = new TimeSpan(23, 59, 0);
                processedDateTo = processedDateTo.Date + ts;

                List<PaymentEtranzactView> payments = (from sr in repository.GetBy<VW_PAYMENT_ETRANZACT>(p => (p.Transaction_Date >= processedDateFrom && p.Transaction_Date <= processedDateTo))
                                                       select new PaymentEtranzactView
                                                       {
                                                           InvoiceNumber = sr.Invoice_Number,
                                                           PaymentModeId = sr.Payment_Mode_Id,
                                                           FeeTypeId = sr.Fee_Type_Id,
                                                           SessionId = sr.Session_Id,
                                                           PersonId = sr.Person_Id,
                                                           FeeTypeName = sr.Fee_Type_Name,
                                                           TransactionAmount = sr.Transaction_Amount,
                                                           FullName = sr.Expr1,
                                                           SessionName = sr.Session_Name,
                                                           MatricNumber = sr.Matric_Number,
                                                           LevelName = sr.Level_Name,
                                                           DepartmentName = sr.Department_Name,
                                                           FacultyName = sr.Faculty_Name,
                                                           ProgrammeName = sr.Programme_Name,
                                                           ConfirmationNo = sr.Confirmation_No,
                                                           TransactionDate = sr.Transaction_Date,
                                                           PaymentMode =  sr.Payment_Mode_Name
                                                       }).ToList();
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
        public async Task<bool> PaymentAlreadyMadeAsync(Payment payment)
        {
            try
            {
                Expression<Func<PAYMENT_ETRANZACT, bool>> selector =
                    p =>
                        p.Payment_Id== payment.Id;
                List<PaymentEtranzact> payments = await GetModelsByAsync(selector);
                if (payments != null && payments.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool HasStudentPaidSugFeeForSession(long StudentId, Session session, FeeType feeType)
        {
            try
            {

                Expression<Func<PAYMENT_ETRANZACT, bool>> selector = p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == StudentId && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == feeType.Id && p.ONLINE_PAYMENT.PAYMENT.Payment_Mode_Id == 1;
                PaymentEtranzact payment = GetModelBy(selector);
                if (payment != null && payment.ConfirmationNo != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

    }
}