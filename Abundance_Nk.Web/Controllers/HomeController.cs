using System;
using System.Collections.Generic;
using System.Configuration;
using Abundance_Nk.Business;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using System.Threading.Tasks;

namespace Abundance_Nk.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController :Controller
    {
        public ActionResult Index()
        {

            //var paymentLogic = new PaymentLogic();
            //var MonnifyLogic = new PaymentMonnifyLogic("MK_TEST_SAF7HR5F3F", "4SY6TNL8CK3VPRSBTHTRG2N8XXEGC6NL");
            //Payment payment = paymentLogic.GetBy("ABSU190000459738");
            //if (payment != null)
            //{
            //    MonnifyLogic.GenerateInvoice(payment, "10000", DateTime.Now.AddDays(1));
            //    MonnifyLogic.GetInvoiceStatus(payment.InvoiceNumber);
            //}
            return View();
        }

        public ActionResult About()
        {

            //Invoice invoice = new Invoice();
            //var paymentEtranzactType = new PaymentEtranzactType();
            //var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
            //var paymentLogic = new PaymentLogic();
            //var PaystackLogic = new PaystackLogic();

            //Payment payment = paymentLogic.GetBy("ABSU160000082647");
            //invoice.Payment = payment;
            //invoice.Paystack = PaystackLogic.GetBy(payment);
            //invoice.paymentEtranzactType = paymentEtranzactType;
            //invoice.Level = "100 Level";
            //invoice.Department = "Accountancy";
            //invoice.Person = payment.Person;
            //var templatepath = Server.MapPath("/Areas/Applicant/Views/Form/Invoice.cshtml");
            //EmailMessage message = new EmailMessage();
            //message.Name = "Lawrence";
            //message.Email = "enemali.william@gmail.com";
            //message.Subject = "Abia State University";
            //EmailSenderLogic<Invoice> invoicEmailSenderLogic = new EmailSenderLogic<Invoice>(templatepath, invoice);
            //invoicEmailSenderLogic.Send(message);

            //RavePayment ravePayment = new RavePayment();
            //ravePayment.Public_key = "FLWPUBK-7f04b2c39abdca3fa8d5d6c2e65ee21a-X";
            //ravePayment.Security_key = "FLWSECK-aecfb1d59a3f4b050c349b1ff4b51202-X";
            //ravePayment.Accountnumber = "0023899004";
            //ravePayment.Narration = payment.FeeType.Name;
            //ravePayment.Amount = "100000";
            //ravePayment.Phonenumber = "07031831781";
            //ravePayment.Email = "enemali.william@gmail.com";
            //ravePayment.TxRef = payment.InvoiceNumber;
            //RaveBranchPaymentLogic raveBranchPaymentLogic = new RaveBranchPaymentLogic();
            //var output = raveBranchPaymentLogic.CreateCardPayment(ravePayment);
            
            //var outputw = raveBranchPaymentLogic.VerifyTransaction(ravePayment);

            return View();

        }

        public async Task<ActionResult> Contact()
        {
            PersonLogic personLogic = new PersonLogic();
            ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
            List<Slug> applicants = await applicationFormLogic.GetPostJambSlugDataBulk(new Model.Model.Session() { Id = 19});
            const string TILDA = "~";
            if (applicants != null)
            {
                foreach(Slug slug in applicants)
                {
                    string jambNumber = slug.JambNumber.ToUpper();
                    string savePath = "/Content/Student/JAMB/" + jambNumber + "_Face.jpg";
                    string jambPath = Server.MapPath(TILDA +savePath);
                    if (System.IO.File.Exists(jambPath))
                    {
                        var person =  personLogic.GetBy(slug.PersonId);
                        if (person != null)
                        {
                            person.ImageFileUrl = savePath;
                            await personLogic.Modify(person, slug.PersonId);
                        }
                    }
                }
            }

            return View();
        }
        public ActionResult Chat()
        {
            return View();
        }
        
    }
}