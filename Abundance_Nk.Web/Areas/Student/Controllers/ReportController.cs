using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    public class ReportController :BaseController
    {
        // GET: Student/Report
        public ActionResult CertificateOfEligibilityReport(long studentId)
        {
            try
            {
                if(studentId == null || studentId <= 0)
                {
                    return null;
                }

                int reportType = 1;
                ViewBag.StudentId = studentId.ToString();
                ViewBag.ReportType = reportType.ToString();
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View();
        }

        public ActionResult CheckingCredential(long studentId)
        {
            try
            {
                if(studentId == null || studentId <= 0)
                {
                    return null;
                }

                int reportType = 2;
                ViewBag.StudentId = studentId.ToString();
                ViewBag.ReportType = reportType.ToString();
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View();
        }

        public ActionResult PersonalInformationReport(long studentId)
        {
            try
            {
                if(studentId == null || studentId <= 0)
                {
                    return null;
                }

                int reportType = 3;
                ViewBag.StudentId = studentId.ToString();
                ViewBag.ReportType = reportType.ToString();
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View();
        }

        public ActionResult UnderTakingReport(long studentId)
        {
            try
            {
                if(studentId == null || studentId <= 0)
                {
                    return null;
                }

                int reportType = 4;
                ViewBag.StudentId = studentId.ToString();
                ViewBag.ReportType = reportType.ToString();
            }
            catch(Exception ex)
            {
                SetMessage("Error! " + ex.Message,Message.Category.Error);
            }

            return View();
        }
        public ActionResult PrintReceipt(long paymentId)
        {
            try
            {
                if (paymentId == null || paymentId <= 0)
                {
                    return null;
                }

                ViewBag.PaymentId = paymentId.ToString();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View();
        }
        public ActionResult DownloadReport(string path)
        {
            try
            {
                return File(Server.MapPath(path),"application/pdf","StudentInformation.pdf");
            }
            catch(Exception)
            {
                throw;
            }
        }
        public ActionResult DownloadReceipt(string path)
        {
            try
            {
                return File(Server.MapPath(path), "application/pdf", "StudentReceipt.pdf");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}