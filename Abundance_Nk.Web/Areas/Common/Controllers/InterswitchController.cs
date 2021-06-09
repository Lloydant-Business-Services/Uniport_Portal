using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Common.Controllers
{
    [AllowAnonymous]
    public class InterswitchController : Controller
    {
        // GET: Common/Interswicth
        private int transactionCharge = 150;
        public ActionResult Index(long Id)
        {
            PersonLogic personLogic = new PersonLogic();
            var person = personLogic.GetBy(Id);

            XmlSerializer xsSubmit = new XmlSerializer(typeof(Person));
            var xml = "";

             using(var sww = new StringWriter())
             {
                 using(XmlWriter writer = XmlWriter.Create(sww))
                 {
                     xsSubmit.Serialize(writer, person);
                     xml = sww.ToString(); // Your XML
                 }
             }

            return Content(xml, "text/xml");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CustomerInformationRequest));
            string xml = "";
            CustomerInformationRequest result;
            if (Request.InputStream != null)
            {
                StreamReader stream = new StreamReader(Request.InputStream);
                string data = stream.ReadToEnd();
                if (!string.IsNullOrEmpty(data))
                {
                    XDocument doc = XDocument.Parse(data);
                    XElement quote = doc.Root.Element("CustReference");
                    string invoiceNumber = quote.Value;
                    if (!string.IsNullOrEmpty(invoiceNumber))
                    {
                        xml = CreateXmlDataResponse(invoiceNumber);
                        //using (var sww = new StringWriter())
                        //{
                        //    using (XmlWriter writer = XmlWriter.Create(sww))
                        //    {
                        //        serializer.Serialize(writer, xml);
                        //        xml = sww.ToString(); // Your XML
                        //    }
                        //}
                    }
                }
            }
            return Content(xml, "text/xml");
        }
        
      

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Listen(InterswitchResponse interswitchResponse)
        {
            try
            {
                PaymentInterswitchLogic paymentInterswitchLogic = new PaymentInterswitchLogic();
                var paymentInterSwitch = paymentInterswitchLogic.TransactionStatus(interswitchResponse.txnRef);
                if (paymentInterSwitch != null && paymentInterSwitch.ResponseCode == "00")
                {
                    Receipt receipt = new Receipt();
                    receipt.InvoiceNumber = paymentInterSwitch.Payment.InvoiceNumber;
                    receipt.Amount = paymentInterSwitch.Amount  / 100;
                    receipt.Amount += transactionCharge;
                    receipt.PaymentId = paymentInterSwitch.Payment.Id.ToString();
                    var templatepath = Server.MapPath("/Areas/Common/Views/Credential/Template.cshtml");

                    EmailMessage message = new EmailMessage();
                    message.Name = paymentInterSwitch.Payment.Person.FullName;
                    //message.Email = paymentInterSwitch.Payment.Person.Email ?? "john.bello@interswitchgroup.com";  
                    message.Email = paymentInterSwitch.Payment.Person.Email ?? "support@lloydant.com";
                    message.Subject = "Abia State University";
                    EmailSenderLogic<Receipt> receiptEmailSenderLogic = new EmailSenderLogic<Receipt>(templatepath, receipt);

                    receiptEmailSenderLogic.Send(message);

                    return RedirectToAction("Receipt", "Credential", new { Area = "Common", pmid = paymentInterSwitch.Payment.Id });
                }
                else if (interswitchResponse != null && interswitchResponse.resp != null)
                {
                    return View(interswitchResponse);
                }
                else
                {
                    //return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Utility.Encrypt(paymentInterSwitch.Payment.Id.ToString()) });
                    return View(interswitchResponse);
                }
                
            }
            catch (Exception ex)
            {
                
                throw;
            }
            
        }

        private string CreateXmlDataResponse(string invoiceNumber)
        {
            string xmlData = "";
            int status = 0;
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                StudentPaymentLogic studnetPaymentLogic = new StudentPaymentLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                PaystackLogic paystackLogic = new PaystackLogic();
                if (invoiceNumber != null)
                {
                   Payment payment = paymentLogic.GetBy(invoiceNumber);
                    if (payment != null)
                    {
                        payment.Amount = studnetPaymentLogic.GetBy(payment).Amount.ToString(CultureInfo.InvariantCulture);
                        var isPaidOnEtranzact = paymentEtranzactLogic.GetBy(payment);
                        var isPaidOnPayStack = paystackLogic.ValidatePayment(invoiceNumber);
                        if (isPaidOnEtranzact != null || isPaidOnPayStack != null)
                        {
                            status = 2;
                        }
                    }
                    else
                    {
                        status = 1;
                    }

                    xmlData = BuildXml(payment, status);
                }
                 
            }
            catch (Exception ex)
            {
                
                throw;
            }
            return xmlData;
        }

        private string BuildXml(Payment payment, int status)
        {
            string xmlData;
            try
            {
                xmlData = "<CustomerInformationResponse>" +
                     "<Customers>" +
                         "<Customer>" +
                              "<Status>" + status + "</Status>" +
                              "<CustReference>" + payment.InvoiceNumber + "</CustReference>" +
                              "<FirstName>" + payment.Person.FirstName + "</FirstName>" +
                              "<LastName>" + payment.Person.LastName + "</LastName>" +
                               "<Amount>" + payment.Amount + "</Amount>" +
                               "<PaymentItems>" +
                                     "<Item>" +
                                     "<ProductName>" + payment.FeeType.Name + "</ProductName>" +
                                     "<ProductCode>" + payment.FeeType.Id + "</ProductCode>" +
                                     "<Quantity>1</Quantity>" +
                                     "<Price>" + payment.Amount + "</Price>" +
                                     "<Subtotal>" + payment.Amount + "</Subtotal>" +
                                     "<Tax>0</Tax>" +
                                     "<Total>" + payment.Amount + "</Total>" +
                                     "</Item>" +
                              "</PaymentItems>" +
                              "</Customer>" +
                       "</Customers>" +
                  "</CustomerInformationResponse>";
            }
            catch (Exception ex)
            {
                
                throw;
            }
            return xmlData;
        }

    }
}