using System.Linq;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using Abundance_Nk.Web.Controllers;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace Abundance_Nk.Web.Areas.Common.Controllers
{
    [AllowAnonymous]
    public class EtranzactController :BaseController
    {
        //
        // GET: /Common/Etranzact/
        public ActionResult Index(string payeeid,string payment_type)
        {
            
            return View();
        }

        public ActionResult RetrievePin()
        {
            if (Request.QueryString["RECEIPT_NO"] != null)
            {
                var d = System.Web.HttpContext.Current.Request.UrlReferrer;
                String clientIP = (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null) ? System.Web.HttpContext.Current.Request.UserHostAddress : System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                //if (clientIP != "197.255.244.10" && clientIP != "197.255.244.5")
                //{
                //    return Json("Transaction Status = false -1", "text/html", JsonRequestBehavior.AllowGet);
                //}

                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                PaymentEtranzact paymentEtranzact = new PaymentEtranzact();
                paymentEtranzact.ReceiptNo = Request.QueryString["PAYMENT_CODE"];
                paymentEtranzact.PaymentCode = Request.QueryString["RECEIPT_NO"];
                paymentEtranzact.ConfirmationNo = Request.QueryString["RECEIPT_NO"];
                paymentEtranzact.MerchantCode = Request.QueryString["MERCHANT_CODE"];
                paymentEtranzact.TransactionAmount = Convert.ToDecimal(Request.QueryString["TRANS_AMOUNT"]);
                paymentEtranzact.TransactionDescription = Request.QueryString["TRANS_DESCR"];
                paymentEtranzact.BankCode = Request.QueryString["BANK_CODE"];
                paymentEtranzact.BranchCode = Request.QueryString["BRANCH_CODE"];
                paymentEtranzact.CustomerName = clientIP; // Request.QueryString["CUSTOMER_NAME"];
                paymentEtranzact.CustomerAddress = "FETC" + Request.QueryString["CUSTOMER_ADDRESS"];
                paymentEtranzact.CustomerID = Request.QueryString["CUSTOMER_ID"];
                paymentEtranzact.TransactionDate = Convert.ToDateTime(Request.QueryString["TRANS_DATE"]);
                paymentEtranzact.Used = false;
                paymentEtranzact.UsedBy = 0;

                if (paymentEtranzactLogic.IsPinOnTable(paymentEtranzact.ConfirmationNo))
                {
                    return Json("Transaction Status = false 1", "text/html", JsonRequestBehavior.AllowGet);
                    //return Json("Transaction Status = true", JsonRequestBehavior.AllowGet);
                }
                if (paymentEtranzactLogic.IsInvoiceOnTable(paymentEtranzact.CustomerID))
                {
                    return Json("Transaction Status = false 1", "text/html", JsonRequestBehavior.AllowGet);
                }

                if (String.IsNullOrEmpty(Request.QueryString["RECEIPT_NO"]) && String.IsNullOrEmpty(Request.QueryString["PAYMENT_CODE"]) && String.IsNullOrEmpty(Request.QueryString["CUSTOMER_ID"]) && String.IsNullOrEmpty(Request.QueryString["TRANS_AMOUNT"]) && String.IsNullOrEmpty(Request.QueryString["TRANS_DATE"]))
                {
                    return Json("Transaction Status = false 3", "text/html", JsonRequestBehavior.AllowGet);
                    //return Json("Transaction Status = true", JsonRequestBehavior.AllowGet);
                }


                Payment payment = new Payment();
                PaymentLogic paymentLogic = new PaymentLogic();
                payment = paymentLogic.GetBy(paymentEtranzact.CustomerID);
                if (payment != null && payment.Id > 0)
                {
                    OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                    OnlinePayment onlinePayment = onlinePaymentLogic.GetBy(payment.Id);

                    paymentEtranzact.UsedBy = payment.Person.Id;
                    paymentEtranzact.Payment = onlinePayment;
                    PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();
                    PaymentTerminal paymentTerminal = paymentTerminalLogic.GetBy(payment);
                    if (paymentTerminal != null)
                    {
                        paymentEtranzact.Terminal = paymentTerminal;
                    }
                    else
                    {
                        paymentEtranzact.Terminal = new PaymentTerminal() { Id = 1, FeeType = new FeeType() { Id = 1 } };
                    }
                    var paymentEtranzactType = new PaymentEtranzactType();
                    var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                    paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == paymentEtranzact.Terminal.FeeType.Id).LastOrDefault();
                    paymentEtranzact.EtranzactType = paymentEtranzactType;
                    paymentEtranzactLogic.Create(paymentEtranzact);
                    string json = "";
                    //return Json(new { paymentEtranzact }, "text/html", JsonRequestBehavior.AllowGet);
                    return Json("Transaction Status = true", "text/html", JsonRequestBehavior.AllowGet);

                }

            }

            return View();

        }

        [AllowAnonymous]
        public ActionResult BankItNotifications(FormCollection formData)
        {
            string statusDescription = "";
            try
            {
                string successStatus = formData["SUCCESS"];
                string etranzactTransactionId = formData["TRANSACTION_REF"];
                string transactionId = formData["TRANSACTION_ID"];
                string terminalId = formData["TERMINAL_ID"];
                string responseUrl = formData["RESPONSE_URL"];
                string finalCheckSum = formData["FINAL_CHECKSUM"];
                string newCheckSum = formData["CHECKSUM"];
                string transactionNumber = formData["TRANS_NUM"];
                string description = formData["DESCRIPTION"];
                string secretKey = formData["SECRET_KEY"];
                string amount = formData["AMOUNT"];

                string logoUrl = formData["LOGO_URL"];
                string phoneNumber = formData["phonenumber"];
                string method = formData["METHOD"];
                string bank = formData["BANK"];
                string account = formData["ACCOUNT"];

                string msg = formData["msg"];

                if (string.IsNullOrEmpty(transactionId))
                {
                    statusDescription = successStatus;
                }
                else
                {
                    System.Security.Cryptography.SHA256Managed sha256 = new System.Security.Cryptography.SHA256Managed();
                    Byte[] EncryptedSHA512 = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(successStatus + amount + terminalId + transactionId + ConfigurationManager.AppSettings["EtranzactResponseUrl"].ToString() + "DEMO_KEY"));
                    sha256.Clear();
                    string finalCheck = BitConverter.ToString(EncryptedSHA512).Replace("-", "").ToLower();

                    if (finalCheckSum == finalCheck)
                    {
                        switch (successStatus)
                        {
                            case "0":
                                statusDescription = "Transaction successful. Payment accepted";
                                break;
                            case "-1":
                                statusDescription = "Transaction timeout or invalid parameters or unsuccessful transaction in the case of Query History";
                                break;
                            case "1":
                                statusDescription = "Destination Card Not Found";
                                break;
                            case "2":
                                statusDescription = "Card Number Not Found";
                                break;
                            case "3":
                                statusDescription = "Invalid Card PIN";
                                break;
                            case "4":
                                statusDescription = "Card Expiration Incorrect";
                                break;
                            case "5":
                                statusDescription = "Insufficient balance";
                                break;
                            case "6":
                                statusDescription = "Spending Limit Exceeded";
                                break;
                            case "7":
                                statusDescription = "Internal System Error Occurred, please contact the service provider";
                                break;
                            case "8":
                                statusDescription = "Financial Institution cannot authorize transaction, Please try later";
                                break;
                            case "9":
                                statusDescription = "PIN tries Exceeded";
                                break;
                            case "10":
                                statusDescription = "Card has been locked";
                                break;
                            case "11":
                                statusDescription = "Invalid Terminal Id";
                                break;
                            case "12":
                                statusDescription = "Payment Timeout";
                                break;
                            case "13":
                                statusDescription = "Destination card has been locked";
                                break;
                            case "14":
                                statusDescription = "Card has expired";
                                break;
                            case "15":
                                statusDescription = "PIN change required";
                                break;
                            case "16":
                                statusDescription = "Invalid Amount";
                                break;
                            case "17":
                                statusDescription = "Card has been disabled";
                                break;
                            case "18":
                                statusDescription = "Unable to credit this account immediately, credit will be done later";
                                break;
                            case "19":
                                statusDescription = "Transaction not permitted on terminal";
                                break;
                            case "20":
                                statusDescription = "Exceeds withdrawal frequency";
                                break;
                            case "21":
                                statusDescription = "Destination Card has expired";
                                break;
                            case "22":
                                statusDescription = "Destination Card Disabled";
                                break;
                            case "23":
                                statusDescription = "Source Card Disabled";
                                break;
                            case "24":
                                statusDescription = "Invalid Bank Account";
                                break;
                            case "25":
                                statusDescription = "Insufficient Balance";
                                break;
                            case "26":
                                statusDescription = "CHECKSUM/FINAL_CHECKSUM error";
                                break;
                            default:
                                statusDescription = "Your Transaction was not Successful. No amount was debited from your account.";
                                break;
                        }
                    }
                    else
                    {
                        statusDescription += "Wrong FinalCheckSum, " + finalCheck + " : " + finalCheckSum;
                    }

                    if (successStatus == "0" && finalCheckSum == finalCheck)
                    {
                        PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                        PaymentTerminalLogic paymentTerminalLogic = new PaymentTerminalLogic();

                        PaymentEtranzact paymentEtz = new PaymentEtranzact();

                        paymentEtz.BankCode = bank;
                        paymentEtz.BranchCode = bank;
                        paymentEtz.ConfirmationNo = etranzactTransactionId;
                        paymentEtz.CustomerID = transactionId;
                        paymentEtz.MerchantCode = null;
                        paymentEtz.PaymentCode = transactionNumber;
                        paymentEtz.ReceiptNo = transactionNumber;
                        paymentEtz.TransactionAmount = Convert.ToDecimal(amount);
                        paymentEtz.TransactionDate = DateTime.Now;
                        paymentEtz.TransactionDescription = description + " via Bank Payment";
                        paymentEtz.Used = false;
                        paymentEtz.UsedBy = 0;

                        PaymentTerminal terminal = paymentTerminalLogic.GetModelsBy(p => p.Terminal_Id == terminalId).LastOrDefault();
                        paymentEtz.Terminal = terminal;

                        Payment payment = new Payment();
                        PaymentLogic paymentLogic = new PaymentLogic();

                        payment = paymentLogic.GetModelBy(m => m.Invoice_Number == transactionId);

                        if (payment != null)
                        {
                            AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                            AppliedCourse appliedCourse = appliedCourseLogic.GetModelsBy(a => a.Person_Id == payment.Person.Id).LastOrDefault();
                            if (appliedCourse != null)
                            {
                                paymentEtz.CustomerAddress = appliedCourse.Programme.Name + " " + appliedCourse.Department.Name;
                            }
                            else
                            {
                                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                                StudentLevel studetLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id).LastOrDefault();
                                if (studetLevel != null)
                                {
                                    paymentEtz.CustomerAddress = studetLevel.Programme.Name + " " + studetLevel.Department.Name;
                                }
                            }

                            paymentEtz.CustomerName = payment.Person.FullName;

                            OnlinePayment onlinePayment = new OnlinePayment();
                            OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
                            onlinePayment = onlinePaymentLogic.GetModelBy(c => c.PAYMENT_CHANNEL.Payment_Channnel_Id == (int)PaymentChannel.Channels.Etranzact && c.Payment_Id == payment.Id);

                            paymentEtz.Payment = onlinePayment;

                            PaymentEtranzactType paymentEtranzactType = new PaymentEtranzactType();
                            PaymentEtranzactTypeLogic paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                            paymentEtranzactType = paymentEtranzactTypeLogic.GetModelBy(p => p.Fee_Type_Id == payment.FeeType.Id);

                            paymentEtz.EtranzactType = paymentEtranzactType;
                        }

                        if (terminal != null && payment != null)
                        {
                            paymentEtranzactLogic.Create(paymentEtz);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            SetMessage(statusDescription, Message.Category.Information);

            return View();
        }
    
    }
}