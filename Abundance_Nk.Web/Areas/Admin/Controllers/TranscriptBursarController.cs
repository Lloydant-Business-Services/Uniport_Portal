using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class TranscriptBursarController :BaseController
    {
        private readonly MailMessage mail;
        private TranscriptBursarViewModel viewModel;

        public TranscriptBursarController()
        {
            mail = new MailMessage();
        }

        // GET: Admin/TranscriptBursar
        public ActionResult Index()
        {
            //rewrite this
            Person person;
            viewModel = new TranscriptBursarViewModel();
            var requestLogic = new TranscriptRequestLogic();
            var paymentEtranzactLogic = new PaymentEtranzactLogic();
            var remitaPaymentLogic = new RemitaPaymentLogic();

            var personLogic = new PersonLogic();
            viewModel.transcriptRequests = requestLogic.GetAll();
            try
            {
                var transcriptRequestList = new List<TranscriptRequest>();
                foreach(TranscriptRequest transcriptRequest in viewModel.transcriptRequests)
                {
                    person = personLogic.GetModelBy(p => p.Person_Id == transcriptRequest.student.Id);
                    transcriptRequest.student.FullName = person.FullName;
                    if(transcriptRequest.payment != null)
                    {
                        RemitaPayment remitaPayment =
                            remitaPaymentLogic.GetModelBy(p => p.Payment_Id == transcriptRequest.payment.Id);
                        if(remitaPayment != null)
                        {
                            transcriptRequest.ConfirmationOrderNumber = remitaPayment.RRR;
                            transcriptRequest.remitaPayment = remitaPayment;
                            if(remitaPayment.Status.Contains("01:"))
                            {
                                transcriptRequest.Amount = remitaPayment.TransactionAmount.ToString();
                            }
                        }
                        else
                        {
                            transcriptRequest.Amount = "Payment not yet made";
                        }
                    }
                    transcriptRequestList.Add(transcriptRequest);
                }
                viewModel.transcriptRequests = transcriptRequestList;
                return View(viewModel);
            }
            catch(Exception)
            {
                return View(viewModel);
            }
        }

        [HttpGet]
        public ActionResult UpdateStatus(long tid,string confirmationOrder)
        {
            viewModel = new TranscriptBursarViewModel();
            try
            {
                var paymentEtranzact = new PaymentEtranzact();
                var paymentEtranzactLogic = new PaymentEtranzactLogic();
                paymentEtranzact = paymentEtranzactLogic.GetModelBy(p => p.Confirmation_No == confirmationOrder);
                Person person;
                var personLogic = new PersonLogic();
                var transcriptRequestLogic = new TranscriptRequestLogic();
                TranscriptRequest tRequest = transcriptRequestLogic.GetModelBy(t => t.Transcript_Request_Id == tid);
                if(paymentEtranzact == null)
                {
                    SetMessage("Payment cannot be confirmed at the moment",Message.Category.Error);
                    return RedirectToAction("Index");
                }
                tRequest.transcriptStatus = new TranscriptStatus { TranscriptStatusId = 4 };
                transcriptRequestLogic.Modify(tRequest);
                person = personLogic.GetModelBy(p => p.Person_Id == tRequest.student.Id);
                string studentMail = person.Email;
                //MailSender("ugochukwuaronu@gmail.com", "lawsgacc@gmail.com");
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            SetMessage("Payment confirmed",Message.Category.Information);
            return RedirectToAction("Index");
        }

        public void MailSender(string StudentMail,string BursarMail)
        {
            try
            {
                const string Subject = "Transcript Payment Confirmation";
                const string Body =
                    "This is to inform you that your transcript payment has been confirmed and your request processed";
                if(ModelState.IsValid)
                {
                    mail.To.Add(StudentMail);
                    mail.From = new MailAddress(BursarMail);
                    mail.Subject = Subject;
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    var smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 25;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("lawsgacc@gmail.com","gaccount");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    TempData["msg"] = "Mail Sent";
                }
                else
                {
                    TempData["msg"] = "Mail Not Sent";
                }
            }
            catch(Exception e)
            {
                TempData["msg"] = "Mail Not Sent" + "\n" + e.Message;
            }
        }

        public ActionResult GetStatus(string order_Id)
        {
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
                remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                remitaPaymentLogic.Modify(remitaPayment);
            }
            return RedirectToAction("Index");
        }

        public ActionResult UpdateRRRBulk()
        {
            try
            {
                var m = new BackgroundWorker();
                m.DoWork += m_DoWork;
                Task task1 = Task.Run(() => m.RunWorkerAsync());
                RedirectToAction("Index");
            }
            catch(Exception)
            {
                throw;
            }
            return View();
        }

        private void m_DoWork(object sender,DoWorkEventArgs e)
        {
            var settings = new RemitaSettings();
            var settingsLogic = new RemitaSettingsLogic();
            settings = settingsLogic.GetModelBy(s => s.Payment_SettingId == 1);
            var remitaResponse = new RemitaResponse();
            var remitaPaymentLogic = new RemitaPaymentLogic();
            string remitaVerifyUrl = ConfigurationManager.AppSettings["RemitaVerifyUrl"];
            var remitaProcessor = new RemitaPayementProcessor(settings.Api_key);
            List<RemitaPayment> remitaPayments = remitaPaymentLogic.GetModelsBy(m => m.Status.Contains("025"));
            foreach(RemitaPayment remitaPayment in remitaPayments)
            {
                remitaResponse = remitaProcessor.TransactionStatus(remitaVerifyUrl,remitaPayment);
                if(remitaResponse != null && remitaResponse.Status != null)
                {
                    remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                    remitaPaymentLogic.Modify(remitaPayment);
                }
            }
        }

        public ActionResult TranscriptReport()
        {
            try
            {
                viewModel = new TranscriptBursarViewModel();
                var remitaPyamentLogic = new RemitaPaymentLogic();
                viewModel.remitaPayments =
                    remitaPyamentLogic.GetModelsBy(a => a.Status.Contains("01") && a.Description.Contains("Transcript"));
            }
            catch(Exception)
            {
                throw;
            }
            return View(viewModel);
        }
    }
}