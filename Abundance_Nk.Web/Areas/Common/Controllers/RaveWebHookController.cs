using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Common.Controllers
{
    [AllowAnonymous]
    public class RaveWebHookController : Controller
    {
        // GET: Common/RaveWebHook
        public ActionResult Index(RaveResponseData raveResponseData)
        {
            string[] keys = Request.Form.AllKeys;
            string transaction =  Request["txRef"];
            //var Ravejson = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            //return Ravejson;
            return View();
        }
    }
}