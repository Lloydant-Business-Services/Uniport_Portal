using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class PaymentRegistrationReportController :BaseController
    {
        public ActionResult Payment()
        {
            try
            {
                List<SelectListItem> FeeTypeList = Utility.PopulateFeeTypeSelectListItem();
                ViewBag.FeeType = new SelectList(FeeTypeList,"Id","Name");
            }
            catch(Exception ex)
            {
                SetMessage("Error: " + ex.Message,Message.Category.Error);
            }

            return View();
        }

        public ActionResult PaymentReport(FeeType model)
        {
            ViewBag.FeeType = model.Id;
            return View();
        }
    }
}