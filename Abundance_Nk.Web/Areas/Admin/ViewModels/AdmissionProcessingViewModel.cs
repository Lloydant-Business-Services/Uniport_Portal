using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class AdmissionProcessingViewModel
    {
        private readonly ApplicantLogic applicantLogic;
        private readonly ApplicationFormLogic applicationFormLogic;

        public AdmissionProcessingViewModel()
        {
            Session = new Session();
            applicantLogic = new ApplicantLogic();
            applicationFormLogic = new ApplicationFormLogic();
            ApplicationForms = new List<ApplicationForm>();

            SessionSelectList = Utility.PopulateSessionSelectListItem();
        }

        public Session Session { get; set; }
        public List<Model.Model.Applicant> Applicants { set; get; }
        public List<ApplicationForm> ApplicationForms { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public List<ApplicationSummaryReport> ApplicationSummaryReport { get; set; }
        public Programme Programme { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime DateFrom { get; set; }

        public List<ApplicationForm> GetApplicationsBy(bool rejected,Session session)
        {
            try
            {
                ApplicationForms =
                    applicationFormLogic.GetModelsBy(
                        af => af.Rejected == rejected && af.APPLICATION_FORM_SETTING.Session_Id == session.Id);
                return ApplicationForms;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void GetApplicantByStatus(ApplicantStatus.Status status)
        {
            try
            {
                Applicants = applicantLogic.GetModelsBy(a => a.Applicant_Status_Id == (int)status);
            }
            catch(Exception)
            {
                throw;
            }
        }
        
    }

    public class ApplicantsPerSession
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string PhoneNo { get; set; }
        public string ImageUrl { get; set; }
        public string ApplicationFormNo { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string DepartmentOption { get; set; }
        public string FullName { get; set; }
        public DateTime ApplicationSubmittedOn { get; set; }
    }
}