using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClosedXML.Excel;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class BursaryPaymentViewModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool PayStackSortOption { get; set; }
        public bool ETrazactSortOption { get; set; }
        public bool CompletedTransactions { get; set; }
        public bool PendingTransactions { get; set; }
        public bool FailedTransctions { get; set; }
    }
}