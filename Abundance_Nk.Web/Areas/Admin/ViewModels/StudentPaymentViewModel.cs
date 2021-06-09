using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class StudentPaymentViewModel
    {

        public StudentPaymentViewModel()
        {
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
            SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
        }
        [Required]
        public string InvoiceNumber { get; set; }
        public long Payment_Id { get; set; }

        public long Person_Id { get; set; }
        public Session Session { get; set; }
        public int Session_Id { get; set; }
        public Level Level { get; set; }
        public int Level_Id { get; set; }

        public List<SelectListItem> LevelSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public Person person { get; set; }
        public decimal Amount { get;set;}
        public bool Status { get; set; }
        public string FullName { get; set; }
     
    }
}