using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class TranscriptProcessorViewModel
    {
        public TranscriptProcessorViewModel()
        {
            transcriptSelectList = Utility.PopulateTranscriptStatusSelectListItem();
            transcriptClearanceSelectList = Utility.PopulateTranscriptClearanceStatusSelectListItem();
            transcriptRequest = new TranscriptRequest();
            StateSelectList = Utility.PopulateStateSelectListItem();
            CountrySelectList = Utility.PopulateCountrySelectListItem();
            GeoZoneSelectListItems = Utility.PopulateGeoZoneSelectListItem();
            DeliverySerivceSelectListItems = Utility.PopulateDeliveryServiceSelectListItem();
            FeeTypeSelectListItems = Utility.PopulateFeeTypeSelectListItem();
            StatusSelecList = Utility.PopulateDispatchStatusSelectListItem();
            DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem();
            TranscriptStatusSelectItem = Utility.PopulateTranscriptStatusSelectListItem();
        }

        public List<SelectListItem> transcriptSelectList { get; set; }
        public List<SelectListItem> transcriptClearanceSelectList { get; set; }
        public List<TranscriptRequest> transcriptRequests { get; set; }
        public TranscriptRequest transcriptRequest { get; set; }
        public TranscriptStatus transcriptStatus { get; set; }
        public TranscriptClearanceStatus transcriptClearanceStatus { get; set; }
        public Person Person { get; set; }
        public Model.Model.Student Student { get; set; }
        public List<SelectListItem> StateSelectList { get; set; }
        public List<SelectListItem> CountrySelectList { get; set; }
        public String RequestDateString { get; set; }
        public List<DeliveryServiceZone> DeliveryServiceZones { get; set; }
        public List<DeliveryService> DeliveryServices { get; set; }
        public DeliveryService DeliveryService { get; set; }
        public List<SelectListItem> DeliverySerivceSelectListItems { get; set; }
        public List<SelectListItem> GeoZoneSelectListItems { get; set; }
        public List<SelectListItem> FeeTypeSelectListItems { get; set; }
        public DeliveryServiceZone DeliveryServiceZone { get; set; }
        public GeoZone GeoZone { get; set; }
        public List<GeoZone> GeoZones { get; set; }
        public Fee Fee { get; set; }
        public List<StateGeoZone> StateGeoZones { get; set; }
        public StateGeoZone StateGeoZone { get; set; }
        public List<SelectListItem> StatusSelecList { get; set; }

        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> TranscriptStatusSelectItem { get; set; }
        
    }

}