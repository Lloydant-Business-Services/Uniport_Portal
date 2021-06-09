using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class GstViewModel
    {
        public GstViewModel()
        {
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            DepartmentSelectListItem = Utility.PopulateAllDepartmentSelectListItem();
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
            SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
            SemesterSelectListItem = Utility.PopulateSemesterSelectListItem();
           // AnswerSelectListItem = Utility.PopulateAllGstAnswerSelectListItem();
            if(Programme != null && Programme.Id > 0)
            {
                DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(Programme);
            }

        }
        public string ScannedFile { get; set; }
        public string ScannedFilePath { get; set; }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public Session CurrentSession { get; set; }
        public GstScanAnswer GstScanAnswer { get; set; }
        public GstScan GstScan { get; set; }
        public List<GstScan> GstScanList { get; set; } 
        public Level level { get; set; }
        public List<SelectListItem> AnswerSelectListItem { get; set; }
        public List<SelectListItem> FeeTypeSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> LevelSelectListItem { get; set; }
        public List<SelectListItem> PaymentModeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<SelectListItem> SemesterSelectListItem { get; set; }
        public Semester Semester { get; set; }
        public Course Course { get; set; }
       
      


    }
}