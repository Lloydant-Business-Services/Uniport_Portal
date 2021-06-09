using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class PaymentPaystackViewModel
    {
        public PaymentPaystackViewModel()
        {
            FeeSelectListItem = Utility.PopulateFeeSelectListItem();
            FeeTypeSelectListItem = Utility.PopulateFeeTypeSelectListItem();
            SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
            ProgrammeSelectListItem=Utility.PopulateAllProgrammeSelectListItem();
          
        }

        public PaymentPaystackCommission PaymentPaystackCommission { get; set; }
        public List<SelectListItem> FeeSelectListItem { get; set; }
        public List<SelectListItem> FeeTypeSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }

        public List<PaymentPaystackCommission> PaymentPaystackCommissions { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
    }
}