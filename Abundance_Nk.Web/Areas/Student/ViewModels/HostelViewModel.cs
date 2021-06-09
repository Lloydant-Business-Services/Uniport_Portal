using System.Collections.Generic;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Student.ViewModels
{
    public class HostelViewModel
    {
        public HostelViewModel()
        {
            FeeTypeSelectListItem = Utility.PopulateFeeTypeSelectListItem();
        }

        public string ConfirmationOrder { get; set; }
        public Model.Model.Student Student { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public Person Person { get; set; }
        public FeeType FeeType { get; set; }
        public Payment Payment { get; set; }
        public Session Session { get; set; }
        public HostelAllocation HostelAllocation { get; set; }
        public List<SelectListItem> FeeTypeSelectListItem { get; set; }
        public HostelFee HostelFee { get; set; }

        public string QRVerification { get; set; }
    }
}