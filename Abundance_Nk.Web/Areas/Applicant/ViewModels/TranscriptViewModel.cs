using System;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Abundance_Nk.Business;
using RestSharp;
using RestSharp.Authenticators;

namespace Abundance_Nk.Web.Areas.Applicant.ViewModels
{
    public class TranscriptViewModel
    {
        
        public TranscriptViewModel()
        {
            StateSelectList = Utility.PopulateStateSelectListItem();
            CountrySelectList = Utility.PopulateCountrySelectListItem();
            FeesTypeSelectList = Utility.PopulateVerificationFeeTypeSelectListItem();
            DepartmentSelectList = Utility.PopulateDepartmentSelectListItem();
            ProgrammeSelectList = Utility.PopulateAllProgrammeSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            transcriptRequest = new TranscriptRequest();
            StudentVerification = new StudentVerification();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            DeliveryServiceSelectList = Utility.PopulateDeliveryServiceSelectListItem();
            SemesterSelectList = Utility.PopulateSemesterSelectListItem();
        }

        public List<SelectListItem> LevelSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectList { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }
        public List<SelectListItem> FeesTypeSelectList { get; set; }
        public List<SelectListItem> StateSelectList { get; set; }
        public List<SelectListItem> CountrySelectList { get; set; }
        public TranscriptRequest transcriptRequest { get; set; }
        public List<TranscriptRequest> TranscriptRequests { get; set; }
        public TranscriptStatus transcriptStatus { get; set; }
        public TranscriptClearanceStatus transcriptClearanceStatus { get; set; }
        public StudentVerification StudentVerification { get; set; }
        public PaymentEtranzact PaymentEtranzact { get; set; }
        public bool Paymentstatus { get; set; }
        public RemitaPayment RemitaPayment { get; set; }
        public RemitaPayementProcessor RemitaPayementProcessor { get; set; }
        public string RemitaBaseUrl { get; set; }
        public string Hash { get; set; }

        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public Level Level { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public Session Session { get; set; }
        public bool Activated { get; set; }
        public DeliveryService DeliveryService { get; set; }
        public DeliveryServiceZone DeliveryServiceZone { get; set; }
        public List<SelectListItem> DeliveryServiceSelectList { get; set; }
        public InterswitchResponse InterswitchResponse { get; set; }
        public Semester Semester { get; set; }
        public List<SelectListItem> SemesterSelectList { get; set; }
        public List<Model.Model.Student> Students { get; set; }
        public List<StudentLevel> StudentLevels { get; set; }
        public List<CourseRegistration> CourseRegistrations { get; set; }


    }
}