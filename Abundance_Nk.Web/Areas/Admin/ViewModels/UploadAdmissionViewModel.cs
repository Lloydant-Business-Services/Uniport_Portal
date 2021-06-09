using System;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class UploadAdmissionViewModel
    {
        public AppliedCourseLogic appliedCourseLogic;
        private DepartmentLogic departmentLogic;
        public PersonLogic personLogic;

        public UploadAdmissionViewModel()
        {
            appliedCourseLogic = new AppliedCourseLogic();
            personLogic = new PersonLogic();
            departmentLogic = new DepartmentLogic();
            SessionSelectListItem = Utility.PopulateSessionSelectListItem();
            AllSessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            AdmissionListTypeSelectListItem = Utility.PopulateAdmissionListTypeSelectListItem();
            if(Programme != null && Programme.Id > 0)
            {
                DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(Programme);
            }
        }

        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public Session CurrentSession { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        [Display(Name = "Admission List Type")]
        public AdmissionListType AdmissionListType { get; set; }
        public AdmissionListBatch AdmissionListBatch { get; set; }
        public List<SelectListItem> AdmissionListTypeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> DepartmentOpionSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<AdmissionList> ExistingAdmissions { get; set; }
        public AppliedCourse AppliedCourse { get; set; }
        public Person Person { get; set; }
        public HttpPostedFileBase File { get; set; }
        public List<AdmissionList> AdmissionList { get; set; }
        public AdmissionList AdmissionListDetail { get; set; }
        public List<AppliedCourse> AppliedCourseList { get; set; }

        [Display(Name = "Exam No / Application No")]
        public string SearchString { get; set; }
        public List<UnregisteredStudent> UnregisteredAdmissionList { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<PhotoCard> Applicants { get; set; }
        public List<AdmissionListModel> AdmissionModelList { get; set; }
        public List<SelectListItem> AllSessionSelectListItem { get; set; }
    }
}