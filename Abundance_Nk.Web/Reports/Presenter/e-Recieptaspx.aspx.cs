using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using Microsoft.Reporting.WebForms;
using ZXing;

namespace Abundance_Nk.Web.Reports.Presenter
{
    public partial class e_Recieptaspx : System.Web.UI.Page
    {
        private int reportType;
        private long paymentId;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                if (!IsPostBack)
                {
                    if (Request.QueryString["paymentId"] != null )
                    {
                        paymentId = Convert.ToInt64(Utility.Decrypt(Request.QueryString["paymentId"]));
                        BuildReport(paymentId);
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }

        private void BuildReport(long paymentId)
        {
            var paymentLogic = new PaymentLogic();
            try
            {
                Payment payment = paymentLogic.GetBy(paymentId);

                List<Receipt> receipts = paymentLogic.GetReceiptsBy(payment.Id);
               
                string bind_dsStudentReport = "dsReceipt";
                string reportPath = "";
                string returnAction = "";
                reportPath = @"Reports\Receipt.rdlc";

                string fileName = "Receipt_" + payment.Person.Id + ".pdf";

                if (Directory.Exists(Server.MapPath("~/Content/studentReceiptReportFolder")))
                {

                    if (File.Exists(Server.MapPath("~/Content/studentReceiptReportFolder" + fileName)))
                    {
                        File.Delete(Server.MapPath("~/Content/studentReceiptReportFolder" + fileName));
                    }
                }
                else
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/studentReceiptReportFolder"));
                }

                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                var rptViewer = new ReportViewer();
                rptViewer.Visible = false;
                rptViewer.Reset();
                rptViewer.LocalReport.DisplayName = "Receipt";
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = reportPath;
                rptViewer.LocalReport.EnableExternalImages = true;

                string imageFilePath = GenerateQrCode(receipts.FirstOrDefault().QRVerification, payment.Id);
                string imagePath = new Uri(Server.MapPath(imageFilePath)).AbsoluteUri;
                ReportParameter QRParameter = new ReportParameter("ImagePath", imagePath);

                //string reportMark = Server.MapPath("/Content/Images/absu.bmp");
                string reportMark = Server.MapPath("/Content/Images/nau.bmp");
                string addressPath = new Uri(reportMark).AbsoluteUri;
                ReportParameter waterMark = new ReportParameter("AddressPath", addressPath);

                ReportParameter[] reportParams = { QRParameter, waterMark };

                rptViewer.LocalReport.SetParameters(reportParams);

                rptViewer.LocalReport.DataSources.Add(new ReportDataSource(bind_dsStudentReport.Trim(), receipts));

                byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                string path = Server.MapPath("~/Content/studentReceiptReportFolder");
                string savelocation = "";

                savelocation = Path.Combine(path, fileName);

                File.WriteAllBytes(savelocation, bytes);

                var urlHelp = new UrlHelper(HttpContext.Current.Request.RequestContext);
                Response.Redirect(
                    urlHelp.Action("DownloadReceipt",
                        new
                        {
                            controller = "Report",
                            area = "Student",
                            path = "~/Content/studentReceiptReportFolder/" + fileName
                        }), false);
                //return File(Server.MapPath(savelocation), "application/zip", reportData.FirstOrDefault().Fullname.Replace(" ", "") + ".zip");
                //Response.Redirect(savelocation, false);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + ex.InnerException.Message;
            }
        }
        public string GenerateQrCode(string url, long paymentId, string alt = "QR code", int height = 100, int width = 100, int margin = 0)
        {
            try
            {
                string folderPath = "~/Content/QRCodes/";
                string imagePath = "~/Content/QRCodes/" + paymentId + ".Jpeg";
                string verifyUrl = url;

                string wildCard = paymentId + "*.*";
                IEnumerable<string> files = Directory.EnumerateFiles(Server.MapPath("~/Content/QRCodes/"), wildCard, SearchOption.TopDirectoryOnly);

                if (files != null && files.Count() > 0)
                {
                    return imagePath;
                }
                // If the directory doesn't exist then create it.
                if (!Directory.Exists(Server.MapPath(folderPath)))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var barcodeWriter = new BarcodeWriter();
                barcodeWriter.Format = BarcodeFormat.QR_CODE;
                var result = barcodeWriter.Write(verifyUrl);

                string barcodePath = Server.MapPath(imagePath);
                var barcodeBitmap = new Bitmap(result);
                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(barcodePath, FileMode.Create))
                    {
                        barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }

                return imagePath;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Receipt BuildReceipt(string name, string invoiceNumber, PaymentEtranzact paymentEtranzact, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber, string session, string level, string department, string programme, string faculty)
        {
            try
            {
                var receipt = new Receipt();

                ShortFallLogic shortFallLogic = new ShortFallLogic();
                ShortFall shortFall = shortFallLogic.GetModelsBy(s => s.PAYMENT.Invoice_Number == invoiceNumber).LastOrDefault();
                if (shortFall != null && !string.IsNullOrEmpty(shortFall.Description))
                {
                    receipt.Description = shortFall.Description;
                }

                receipt.Number = paymentEtranzact.ReceiptNo;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = paymentEtranzact.ConfirmationNo;
                receipt.Amount = amount;
                receipt.AmountInWords = NumberToWords((int)amount);
                receipt.Purpose = purpose;
                receipt.PaymentMode = paymentEtranzact.Payment.Payment.PaymentMode.Name;
                receipt.Date = (DateTime)paymentEtranzact.TransactionDate;
                receipt.QRVerification = "http://portal.abiastateuniversity.edu.ng//Common/Credential/Receipt?pmid=" + paymentEtranzact.Payment.Payment.Id;
                receipt.MatricNumber = MatricNumber;
                receipt.Session = session;
                receipt.Level = level;
                receipt.ReceiptNumber = paymentEtranzact.Payment.Payment.SerialNumber.ToString();
                receipt.Department = department;
                receipt.Programme = programme;
                receipt.Faculty = faculty;
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Receipt BuildReceipt(string name, string invoiceNumber, Paystack paystack, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber, string session, string level, string department, string programme, string faculty)
        {
            try
            {
                var receipt = new Receipt();
                receipt.Number = paystack.reference;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = paystack.reference;
                receipt.Amount = amount;
                receipt.AmountInWords = NumberToWords((int)amount);
                receipt.Purpose = purpose;
                receipt.PaymentMode = paystack.Payment.PaymentMode.Name;
                receipt.Date = (DateTime)paystack.transaction_date;
                receipt.QRVerification = "http://portal.abiastateuniversity.edu.ng//Common/Credential/Receipt?pmid=" + paystack.Payment.Id;
                receipt.MatricNumber = MatricNumber;
                receipt.Session = session;
                receipt.Level = level;
                receipt.ReceiptNumber = paystack.Payment.SerialNumber.ToString();
                receipt.Department = department;
                receipt.Programme = programme;
                receipt.Faculty = faculty;
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
    }
}