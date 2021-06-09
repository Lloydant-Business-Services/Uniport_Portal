using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [System.Web.Mvc.AllowAnonymous]
    public class PaymentReportApiController : Controller
    {
       
        //[HttpGet]
        public JsonResult DailyPayment()
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                DateTime start = DateTime.Today.Date;
                DateTime end = start.AddDays(1).AddSeconds(-1);
                var count = paymentLogic.GetPaymentCount(start,end);
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public JsonResult WeeklyPayment()
        {
            try
            {
                DayOfWeek day = DateTime.Today.DayOfWeek;
                DateTime getDate = DateTime.Today.Date;
                DateTime end = getDate.AddDays(1).AddSeconds(-1);
                DateTime start = DateTime.Today;
                switch (day)
                {
                    case DayOfWeek.Monday:
                        start = getDate.AddDays(0);
                        break;
                    case DayOfWeek.Tuesday:
                        start = getDate.AddDays(-1);
                        break;
                    case DayOfWeek.Wednesday:
                        start = getDate.AddDays(-2);
                        break;
                    case DayOfWeek.Thursday:
                        start = getDate.AddDays(-3);
                        break;
                    case DayOfWeek.Friday:
                        start = getDate.AddDays(-4);
                        break;
                    case DayOfWeek.Saturday:
                        start = getDate.AddDays(-5);
                        break;
                    case DayOfWeek.Sunday:
                        start = getDate.AddDays(-6);
                        break;
                }

                PaymentLogic paymentLogic = new PaymentLogic();
                var count = paymentLogic.GetPaymentCount(start, end);
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public JsonResult MonthlyPayment()
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                DateTime getDate = DateTime.Today.Date;
                DateTime start = new DateTime(getDate.Year, getDate.Month, 1);
                DateTime end = getDate.AddDays(1).AddSeconds(-1);
                var count = paymentLogic.GetPaymentCount(start, end);
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public JsonResult DailyPaymentByBank()
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                DateTime start = DateTime.Today.Date;
                DateTime end = start.AddDays(1).AddSeconds(-1);
                var bankCount = paymentLogic.GetPaymentCountByBank(start,end);
                return Json(bankCount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public JsonResult WeeklyPaymentByBank()
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                DayOfWeek day = DateTime.Today.DayOfWeek;
                DateTime getDate = DateTime.Today.Date;
                DateTime end = getDate.AddDays(1).AddSeconds(-1);
                DateTime start = DateTime.Today;
                switch (day)
                {
                    case DayOfWeek.Monday:
                        start = getDate.AddDays(0);
                        break;
                    case DayOfWeek.Tuesday:
                        start = getDate.AddDays(-1);
                        break;
                    case DayOfWeek.Wednesday:
                        start = getDate.AddDays(-2);
                        break;
                    case DayOfWeek.Thursday:
                        start = getDate.AddDays(-3);
                        break;
                    case DayOfWeek.Friday:
                        start = getDate.AddDays(-4);
                        break;
                    case DayOfWeek.Saturday:
                        start = getDate.AddDays(-5);
                        break;
                    case DayOfWeek.Sunday:
                        start = getDate.AddDays(-6);
                        break;
                }
                var bankCount = paymentLogic.GetPaymentCountByBank(start, end);
                return Json(bankCount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public JsonResult MonthlyPaymentByBank()
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                DateTime getDate = DateTime.Today.Date;
                DateTime start = new DateTime(getDate.Year, getDate.Month, 1);
                DateTime end = getDate.AddDays(1).AddSeconds(-1);
                var bankCount = paymentLogic.GetPaymentCountByBank(start, end);
                return Json(bankCount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public  JsonResult DailyPaymentByChannels()
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                DateTime start = DateTime.Today.Date;
                DateTime end = start.AddDays(1).AddSeconds(-1);
                var channelCount = paymentLogic.GetPaymentCountByChannel(start,end);
                return Json(channelCount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public JsonResult WeeklyPaymentByChannels()
        {
            try
            {
                DayOfWeek day = DateTime.Today.DayOfWeek;
                DateTime getDate = DateTime.Today.Date;
                DateTime end = getDate.AddDays(1).AddSeconds(-1);
                DateTime start = DateTime.Today;
                switch (day)
                {
                    case DayOfWeek.Monday:
                        start = getDate.AddDays(0);
                        break;
                    case DayOfWeek.Tuesday:
                        start = getDate.AddDays(-1);
                        break;
                    case DayOfWeek.Wednesday:
                        start = getDate.AddDays(-2);
                        break;
                    case DayOfWeek.Thursday:
                        start = getDate.AddDays(-3);
                        break;
                    case DayOfWeek.Friday:
                        start = getDate.AddDays(-4);
                        break;
                    case DayOfWeek.Saturday:
                        start = getDate.AddDays(-5);
                        break;
                    case DayOfWeek.Sunday:
                        start = getDate.AddDays(-6);
                        break;
                }
                PaymentLogic paymentLogic = new PaymentLogic();
                var channelCount = paymentLogic.GetPaymentCountByChannel(start, end);
                return Json(channelCount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public JsonResult MonthlyPaymentByChannels()
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                DateTime getDate = DateTime.Today.Date;
                DateTime start = new DateTime(getDate.Year, getDate.Month, 1);
                DateTime end = getDate.AddDays(1).AddSeconds(-1);
                var channelCount = paymentLogic.GetPaymentCountByChannel(start, end);
                return Json(channelCount, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public JsonResult DailyPaymentByFeeType()
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                DateTime start = DateTime.Today.Date;
                DateTime end = start.AddDays(1).AddSeconds(-1);
                var countByFeetype = paymentLogic.GetPaymentCountByFeeType(start, end);
                return Json(countByFeetype, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public JsonResult WeeklyPaymentByFeeType()
        {
            try
            {
                DayOfWeek day = DateTime.Today.DayOfWeek;
                DateTime getDate = DateTime.Today.Date;
                DateTime end = getDate.AddDays(1).AddSeconds(-1);
                DateTime start = DateTime.Today;
                switch (day)
                {
                    case DayOfWeek.Monday:
                        start = getDate.AddDays(0);
                        break;
                    case DayOfWeek.Tuesday:
                        start = getDate.AddDays(-1);
                        break;
                    case DayOfWeek.Wednesday:
                        start = getDate.AddDays(-2);
                        break;
                    case DayOfWeek.Thursday:
                        start = getDate.AddDays(-3);
                        break;
                    case DayOfWeek.Friday:
                        start = getDate.AddDays(-4);
                        break;
                    case DayOfWeek.Saturday:
                        start = getDate.AddDays(-5);
                        break;
                    case DayOfWeek.Sunday:
                        start = getDate.AddDays(-6);
                        break;
                }
                PaymentLogic paymentLogic = new PaymentLogic();
                var countByFeetype = paymentLogic.GetPaymentCountByFeeType(start, end);
                return Json(countByFeetype, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public JsonResult MonthlyPaymentByFeeType()
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                DateTime getDate = DateTime.Today.Date;
                DateTime start = new DateTime(getDate.Year, getDate.Month, 1);
                DateTime end = getDate.AddDays(1).AddSeconds(-1);
                var countByFeetype = paymentLogic.GetPaymentCountByFeeType(start, end);
                return Json(countByFeetype, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


    }
}
