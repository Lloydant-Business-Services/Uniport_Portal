using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.PGApplicant.ViewModels
{
    public class PGRegistrationLogonViewModel
    {
        public PGRegistrationLogonViewModel()
        {
            SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
            FeeTypeSelectListItem = Utility.PopulateFeeTypeActiveSelectListItem();
            FeeType = new FeeType();

        }

        public string ConfirmationOrderNumber { get; set; }
        public Session Session { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<SelectListItem> FeeTypeSelectListItem { get; set; }
        public FeeType FeeType { get; set; }
    }
}