using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Applicant.Controllers;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Applicant.ViewModels
{
    public class PostUtmeResultViewModel
    {
        public PostUtmeResultViewModel()
        {
            Result = new PutmeResult();
            Programme = new Programme();
            ApplicationFormSetting = new ApplicationFormSetting();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();

            ExamSelectListItem = PopulateScreeningExams();
        }

        public List<AdmissionListDropDownModel> AdmissionListDropDownModels { get; set; }
        public PutmeResult Result { get; set; }
        public PutmeSlip ResultSlip { get; set; }
        public List<PutmeResult> Results { get; set; }
        public ApplicantJambDetail jambDetail { get; set; }
        public ApplicationForm ApplicationDetail { get; set; }
        public Programme Programme { get; set; }
        public ApplicationFormSetting ApplicationFormSetting { get; set; }
        public ApplicantJambDetail ApplicantJambDetail { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> ExamSelectListItem { get; set; }
        public List<SelectListItem> ResultSelectListItem { get; set; }

        [Required(ErrorMessage = "Please Enter your Examination Number")]
        public string JambRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Please Enter your Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter your Etranzact Confirmation Number")]
        public string PinNumber { get; set; }

        public List<SelectListItem> PopulateScreeningExams()
        {
            var selectItemList = new List<SelectListItem>();

            var list = new SelectListItem();
            list.Value = "";
            list.Text = "Select Exam";
            selectItemList.Add(list);

            var list1 = new SelectListItem();
            list1.Value = "19";
            list1.Text = "PUTME Screening";
            selectItemList.Add(list1);


            var list2 = new SelectListItem();
            list2.Value = "2";
            list2.Text = "Supplementary Screening";
            selectItemList.Add(list2);

            return selectItemList;
        }
        public bool ValidatePin()
        {
            return true;
        }
    }
}