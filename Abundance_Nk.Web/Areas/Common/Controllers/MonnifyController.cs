using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Common.Controllers
{

    [AllowAnonymous]
    public class MonnifyController : Controller
    {
        // GET: Common/Monnify
        public ActionResult Listen(MonnifyReturnResponse paymentMonnify)
        {
            string MonnifyURL = ConfigurationManager.AppSettings["MonnifyUrl"].ToString();
            string MonnifyUser = ConfigurationManager.AppSettings["MonnifyApiKey"].ToString();
            string MonnifySecrect = ConfigurationManager.AppSettings["MonnifyContractCode"].ToString();
            string MonnifyCode = ConfigurationManager.AppSettings["MonnifyCode"].ToString();
            PaymentMonnifyLogic paymentMonnifyLogic = new PaymentMonnifyLogic(MonnifyURL, MonnifyUser, MonnifySecrect, MonnifyCode);

            if (paymentMonnify != null && !string.IsNullOrEmpty(paymentMonnify.paymentReference))
            {
                paymentMonnifyLogic.GetInvoiceStatus(paymentMonnify.paymentReference);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            return View();
        }
    }
}