using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class FeeDetailViewModel
    {
        private DepartmentLogic departmentLogic;
        private FeeDetailLogic feeDetailLogic;
        private FeeTypeLogic feeTypeLogic;
        private LevelLogic levelLogic;
        private PaymentModeLogic paymentModeLogic;
        private ProgrammeLogic programmeLogic;
        private SessionLogic sessionLogic;

        public FeeDetailViewModel()
        {
            feeSelectListItem = Utility.PopulateFeeSelectListItem();
            FeeTypeSelectListItem = Utility.PopulateFeeTypeSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            DepartmentSelectListItem = Utility.PopulateAllDepartmentSelectListItem();
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
            PaymentModeSelectListItem = Utility.PopulatePaymentModeSelectListItem();
            SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
            if(Programme != null && Programme.Id > 0)
            {
                DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(Programme);
            }
        }

        public Fee fee { get; set; }
        public FeeDetail feeDetail { get; set; }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public Session CurrentSession { get; set; }
        public Level level { get; set; }
        public PaymentMode paymentMode { get; set; }
        public FeeType feeType { get; set; }
        public List<FeeDetail> feeDetails { get; set; }
        public List<SelectListItem> feeSelectListItem { get; set; }
        public List<SelectListItem> FeeTypeSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> LevelSelectListItem { get; set; }
        public List<SelectListItem> PaymentModeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<DepartmentalSchoolFees> DepartmentalSchoolFeeList { get; set; }

        public void GetFeeDetails()
        {
            try
            {
                feeDetailLogic = new FeeDetailLogic();
                feeDetails = feeDetailLogic.GetAll();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public int SN { get; set; }

        public string Id { get; set; }
        public PaymentTerminal PaymentTerminal { get; set; }
        public PaymentEtranzactType PaymentEtranzactType { get; set; }
        public ApplicationFormSetting ApplicationFormSetting { get; set; }
        
        public string UploadedFilePath { get; set; }
        public FeeSetup FeeSetup { get; set; }
        public List<FeeSetup> FeeSetups { get; set; }
    }
    public partial class ArrayJsonView
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
        public string Programme { get; set; }
        public string Department { get; set; }
        public string Level { get; set; }
        public string FeeType { get; set; }
        public string Fee { get; set; }
        public string Session { get; set; }
        public string PaymentMode { get; set; }
        public string TerminalId { get; set; }
        public string EtranzactTypeName { get; set; }
        public string FeeName { get; set; }
        public string Amount { get; set; }
        public string FeeDescription { get; set; }
        public string FeeTypeName { get; set; }
        public string FeeTypeDescription { get; set; }
        public string EnteredBy { get; set; }
        public string PersonType { get; set; }
        public string PaymentType { get; set; }
        public string Name { get; set; }

        public string FormSettingName { get; set; }
    }
    public partial class FeeSetupJsonView
    {
        public string FeeTypeId { get; set; }
        public string FeeSetupName { get; set; }
        public string Activated { get; set; }
        public string PaymentModeId { get; set; }
        public string SessionId { get; set; }
        public string Amount { get; set; }
        public string FeeSetUpId { get; set; }
    }
}