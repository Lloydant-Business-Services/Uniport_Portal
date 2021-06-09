using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Areas.Common.Controllers;
using Abundance_Nk.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class VerficationController :BaseController
    {
        public VerificationViewModel ViewModel;

        // GET: Admin/Verfication
        [Authorize(Roles = "Admin,Verification,Bursar")]
        public ActionResult Index()
        {
            ViewModel = new VerificationViewModel();
            return View(ViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Verification,Bursar")]
        public async Task<ActionResult> Index(VerificationViewModel viewModel)
        {
            try
            {
                if(viewModel.PaymentEtranzact.ConfirmationNo != null)
                {
                    int startIndex = viewModel.PaymentEtranzact.ConfirmationNo.IndexOf("pmid=");
                    int pmid = Convert.ToInt32(viewModel.PaymentEtranzact.ConfirmationNo.Substring(startIndex).Split('=')[1]);
                    if(pmid > 0)
                    {
                        Payment payment = new Payment() { Id = pmid };
                        var loggeduser = new UserLogic();
                        var paymentEtranzactLogic = new PaymentEtranzactLogic();
                        var paymentVerificationLogic = new PaymentVerificationLogic();
                        viewModel.PaymentEtranzact = await paymentEtranzactLogic.GetByAsync(payment);
                        if(viewModel.PaymentEtranzact != null)
                        {
                            viewModel.PaymentVerification = await paymentVerificationLogic.GetByAsync(pmid);
                            if(viewModel.PaymentVerification == null)
                            {
                                string client = Request.LogonUserIdentity.Name + " ( " + HttpContext.Request.UserHostAddress + ")";
                                viewModel.PaymentVerification = new PaymentVerification();
                                viewModel.PaymentVerification.Payment = viewModel.PaymentEtranzact.Payment.Payment;
                                viewModel.PaymentVerification.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                                viewModel.PaymentVerification.DateVerified = DateTime.Now;
                                viewModel.PaymentVerification.Comment = client;
                                viewModel.PaymentVerification = paymentVerificationLogic.Create(viewModel.PaymentVerification);
                                viewModel.PaymentVerification.Payment = viewModel.PaymentEtranzact.Payment.Payment;
                                viewModel.PaymentVerification.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                                viewModel.PaymentVerification.DateVerified = DateTime.Now;
                                viewModel.PaymentVerification.Comment = client;
                            }
                        }
                        else
                        {
                            PaystackLogic paystackLogic = new PaystackLogic();
                            PaymentLogic paymentLogic = new PaymentLogic();
                            payment = paymentLogic.GetBy(pmid);
                            if (payment != null && payment.InvoiceNumber != null)
                            {
                                var paystack = paystackLogic.ValidatePayment(payment.InvoiceNumber);
                                if (paystack != null && paystack.Id > 0)
                                {
                                    viewModel.PaymentVerification = await paymentVerificationLogic.GetByAsync(pmid);
                                    if (viewModel.PaymentVerification == null)
                                    {
                                        string client = Request.LogonUserIdentity.Name + " ( " + HttpContext.Request.UserHostAddress + ")";
                                        viewModel.PaymentVerification = new PaymentVerification();
                                        viewModel.PaymentVerification.Payment = payment;
                                        viewModel.PaymentVerification.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                                        viewModel.PaymentVerification.DateVerified = DateTime.Now;
                                        viewModel.PaymentVerification.Comment = client;
                                        viewModel.PaymentVerification = paymentVerificationLogic.Create(viewModel.PaymentVerification);
                                        viewModel.PaymentVerification.Payment = payment;
                                        viewModel.PaymentVerification.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                                        viewModel.PaymentVerification.DateVerified = DateTime.Now;
                                        viewModel.PaymentVerification.Comment = client;

                                        viewModel.PaymentEtranzact = new PaymentEtranzact();
                                        viewModel.PaymentEtranzact.TransactionAmount = Convert.ToDecimal(paystack.Amount)/100;
                                    }
                                    else
                                    {
                                        viewModel.PaymentEtranzact = new PaymentEtranzact();
                                        viewModel.PaymentEtranzact.TransactionAmount = Convert.ToDecimal(paystack.Amount)/100;
                                    }
                                }
                                else if (paystack == null)
                                {
                                    PaymentInterswitchLogic paymentInterswitchLogic = new PaymentInterswitchLogic();
                                    PaymentInterswitch paymentInterswitch = paymentInterswitchLogic.GetBy(pmid);
                                    if (paymentInterswitch?.Id > 0)
                                    {
                                        var paymentValidatedInterswitch = paymentInterswitchLogic.TransactionStatus(paymentInterswitch.MerchantReference);
                                        if (paymentValidatedInterswitch != null && paymentValidatedInterswitch.Id > 0 && paymentValidatedInterswitch.ResponseCode == "00")
                                        {
                                            viewModel.PaymentVerification = await paymentVerificationLogic.GetByAsync(paymentValidatedInterswitch.Payment.Id);
                                            if (viewModel.PaymentVerification == null)
                                            {
                                                string client = Request.LogonUserIdentity.Name + " ( " + HttpContext.Request.UserHostAddress + ")";
                                                viewModel.PaymentVerification = new PaymentVerification();
                                                viewModel.PaymentVerification.Payment = payment;
                                                viewModel.PaymentVerification.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                                                viewModel.PaymentVerification.DateVerified = DateTime.Now;
                                                viewModel.PaymentVerification.Comment = client;
                                                viewModel.PaymentVerification = paymentVerificationLogic.Create(viewModel.PaymentVerification);
                                                viewModel.PaymentVerification.Payment = payment;
                                                viewModel.PaymentVerification.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                                                viewModel.PaymentVerification.DateVerified = DateTime.Now;
                                                viewModel.PaymentVerification.Comment = client;

                                                viewModel.PaymentEtranzact = new PaymentEtranzact();
                                                viewModel.PaymentEtranzact.TransactionAmount = Convert.ToDecimal(paymentValidatedInterswitch.Amount) / 100;
                                            }
                                            else
                                            {
                                                viewModel.PaymentEtranzact = new PaymentEtranzact();
                                                viewModel.PaymentEtranzact.TransactionAmount = Convert.ToDecimal(paymentValidatedInterswitch.Amount) / 100;
                                            }
                                        }
                                    
                                    }
                                    else if (paymentInterswitch == null)
                                    {
                                        string MonnifyURL = ConfigurationManager.AppSettings["MonnifyUrl"].ToString();
                                        string MonnifyUser = ConfigurationManager.AppSettings["MonnifyApiKey"].ToString();
                                        string MonnifySecrect = ConfigurationManager.AppSettings["MonnifyContractCode"].ToString();
                                        string MonnifyCode = ConfigurationManager.AppSettings["MonnifyCode"].ToString();
                                        PaymentMonnifyLogic paymentMonnifyLogic = new PaymentMonnifyLogic(MonnifyURL, MonnifyUser, MonnifySecrect, MonnifyCode);
                                        var PaymentMonnify = paymentMonnifyLogic.GetBy(payment.InvoiceNumber);

                                        if (PaymentMonnify != null && PaymentMonnify.Completed && PaymentMonnify.Payment != null)
                                        {
                                            StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                                            var studentPayment = studentPaymentLogic.GetModelsBy(f => f.Payment_Id == payment.Id).FirstOrDefault();
                                            viewModel.PaymentVerification = await paymentVerificationLogic.GetByAsync(PaymentMonnify.Payment.Id);
                                            if (viewModel.PaymentVerification == null)
                                            {
                                                string client = Request.LogonUserIdentity.Name + " ( " + HttpContext.Request.UserHostAddress + ")";
                                                viewModel.PaymentVerification = new PaymentVerification();
                                                viewModel.PaymentVerification.Payment = payment;
                                                viewModel.PaymentVerification.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                                                viewModel.PaymentVerification.DateVerified = DateTime.Now;
                                                viewModel.PaymentVerification.Comment = client;
                                                viewModel.PaymentVerification = paymentVerificationLogic.Create(viewModel.PaymentVerification);
                                                viewModel.PaymentVerification.Payment = payment;
                                                viewModel.PaymentVerification.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                                                viewModel.PaymentVerification.DateVerified = DateTime.Now;
                                                viewModel.PaymentVerification.Comment = client;

                                                viewModel.PaymentEtranzact = new PaymentEtranzact();
                                                
                                                viewModel.PaymentEtranzact.TransactionAmount = studentPayment?.Id>0? Convert.ToDecimal(studentPayment.Amount): Convert.ToDecimal(PaymentMonnify.Amount);
                                            }
                                            else
                                            {
                                                viewModel.PaymentEtranzact = new PaymentEtranzact();
                                                viewModel.PaymentEtranzact.TransactionAmount = studentPayment?.Id > 0 ? Convert.ToDecimal(studentPayment.Amount) : Convert.ToDecimal(PaymentMonnify.Amount);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                     SetMessage("Payment Could not be verified! Please ensure that student has made payment",Message.Category.Warning);
                                }
                            }
                          
                           
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred" + ex.Message,Message.Category.Error);
            }
            return View(viewModel);
        }

        public ActionResult Report()
        {
            return View();
        }

        public ActionResult Receipt()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Verification,Bursar,Student Account Officer")]
        public async Task<ActionResult> Receipt(VerificationViewModel viewModel)
        {
            long serial = viewModel.PaymentEtranzact.Payment.Payment.SerialNumber ?? 0;
            CredentialController credential = new CredentialController();
            PaymentVerificationLogic paymentVerificationLogic = new PaymentVerificationLogic();
            viewModel.receipt = credential.GetReceiptBy(serial);
            if (viewModel.receipt == null)
            {
                SetMessage("Receipt Number entered couldn't be verified! Please cross check and re-enter",Message.Category.Error);
               
            }
            else
            {
                viewModel.PaymentVerification = await paymentVerificationLogic.GetByAsync(serial);
                return View("PrintOut", viewModel);
            }
            return View();
        }

        public ActionResult StudentPaymentReport()
        {
            return View();
        }

        public JsonResult GetStudentPaymentDetails(string matricNumber)
        {
            var paymentHistory = new PaymentHistory();
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();

                if (!string.IsNullOrEmpty(matricNumber))
                {
                    Model.Model.Student student = studentLogic.GetBy(matricNumber);
                    var paymentLogic = new PaymentLogic();
                    paymentHistory.Payments = paymentLogic.GetBy(student);
                    List<PaymentView> paystackPayments = paymentLogic.GetPaystackPaymentBy(student);
                    paymentHistory.Payments = paymentHistory.Payments.Where(p => p.ConfirmationOrderNumber != null).ToList();
                    paymentHistory.Payments.AddRange(paystackPayments);
                    paymentHistory.Student = student;
                    paymentHistory.StudentLevel = studentLevelLogic.GetBy(student.Id);
                    paymentHistory.IsError = false;
                }
                else
                {
                    paymentHistory.IsError = true;
                    paymentHistory.ErrorMessage = "Kindly Enter a Matric_Number to Continue";
                }
            }
            catch (Exception ex)
            {
                paymentHistory.IsError = true;
                paymentHistory.ErrorMessage = "Error Occured" + ex;
              
            }
            return Json(paymentHistory,JsonRequestBehavior.AllowGet);
        }

        public ActionResult PaymentVerificationReport()
        {
            return View();
        }
        public ContentResult GetVerificationPaymentReport(string dateFrom, string dateTo)
        {
            List<PaymentVerificationReportData> paymentReportArrayList = new List<PaymentVerificationReportData>();
            PaymentVerificationReportData paymentReportArray = new PaymentVerificationReportData();
            var result = new ContentResult();
            try
            {

                if (string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo) )
                {
                    return null;
                }

                    JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                    PaymentVerificationLogic paymentVerificationLogic = new PaymentVerificationLogic();
                    List<PaymentVerificationReportAlt> paymentVerificaionReport = paymentVerificationLogic.GetVerificationReport(dateFrom, dateTo);
                    if (paymentVerificaionReport != null && paymentVerificaionReport.Count > 0)
                    {
                        List<long> userIdList = paymentVerificaionReport.Select(s => s.UserId).Distinct().ToList();
                        for (int i = 0; i < userIdList.Count; i++)
                        {
                            long currentUserId = userIdList[i];
                            paymentReportArray = new PaymentVerificationReportData();
                            var paymentReportData = paymentVerificaionReport.Where(s => s.UserId == currentUserId);
                            paymentReportArray.Count = paymentReportData.Count().ToString();
                            paymentReportArray.VerificationOfficer = paymentReportData.FirstOrDefault().VerificationOfficer;
                            paymentReportArray.IsError = false;
                            paymentReportArray.UserId = currentUserId;
                            paymentReportArrayList.Add(paymentReportArray);
                        }
                        paymentReportArray.PaymentVerificationView = serializer.Serialize(paymentVerificaionReport);

                    }
                    else
                    {
                        paymentReportArray.IsError = true;
                        paymentReportArray.ErrorMessage = "Error Occured Invalid parameter";
                        paymentReportArrayList.Add(paymentReportArray);
                    }
                var serializedList = serializer.Serialize(paymentReportArrayList);
                result = new ContentResult
                {
                    Content = serializedList,
                    ContentType = "application/json"
                };
                return result;
            }
            catch (Exception ex)
            {

                paymentReportArray.IsError = true;
                paymentReportArray.ErrorMessage = "Error Occured" + ex;
                paymentReportArrayList.Add(paymentReportArray);
            }
            return result;
        }
        public ContentResult GetPaymentDetailsByVerificationOfficer(string userId, string dateFrom, string dateTo, string paymentReportData)
        {
            List<PaymentVerificationReportData> paymentReportArrayList = new List<PaymentVerificationReportData>();
            PaymentVerificationReportData paymentReportArray = new PaymentVerificationReportData();
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                if (string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo) && string.IsNullOrEmpty(userId))
                {
                    return null;
                }

                long id = Convert.ToInt32(userId);
                
                    List<PaymentVerificationReportAlt> paymentVerificationReportView = serializer.Deserialize<List<PaymentVerificationReportAlt>>(paymentReportData);
                    var verificationOffcierReceipts = paymentVerificationReportView.Where(s => s.UserId == id).ToList();

                    for (int i = 0; i < verificationOffcierReceipts.Count; i++)
                    {
                        paymentReportArray = new PaymentVerificationReportData();
                        paymentReportArray.FullName = verificationOffcierReceipts[i].StudentName;
                        paymentReportArray.MatricNumber = verificationOffcierReceipts[i].MatricNumber ?? "-";
                        paymentReportArray.FeeTypeName = verificationOffcierReceipts[i].FeeType;
                        paymentReportArray.Amount = String.Format("{0:N}", verificationOffcierReceipts[i].PaymentAmount);
                        paymentReportArray.LevelName = verificationOffcierReceipts[i].Level ?? "-";
                        paymentReportArray.ConfirmationNo = verificationOffcierReceipts[i].PaymentReference ?? "-";
                        paymentReportArray.SessionName = verificationOffcierReceipts[i].Session;
                        paymentReportArray.Receipt = verificationOffcierReceipts[i].ReceiptNumber;
                        paymentReportArrayList.Add(paymentReportArray);

                    }
                
              
                var serializedList = serializer.Serialize(paymentReportArrayList);
                var result = new ContentResult
                {
                    Content = serializedList,
                    ContentType = "application/json"
                };
                return result;

            }
            catch (Exception ex)
            {

                paymentReportArray.IsError = true;
                paymentReportArray.ErrorMessage = "Error Occured" + ex;
                paymentReportArrayList.Add(paymentReportArray);
            }
            return null;
        }
    }
}