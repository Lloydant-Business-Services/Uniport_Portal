using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class StaffDataViewModel
    {
        public StaffDataViewModel()
        {
            SexSelectList = Utility.PopulateSexSelectListItem();
            StateSelectList = Utility.PopulateStateSelectListItem();
            LgaSelectList = Utility.PopulateLocalGovernmentSelectListItem();
            ReligionSelectList = Utility.PopulateReligionSelectListItem();
            MaritalStatusSelectList = Utility.PopulateMaritalStatusSelectListItem();
            StaffTypeSelectList = Utility.PopulateStaffTypeSelectListItem();
            GenotypeSelectList = Utility.PopulateGenotypeSelectListItem();
            BloodGroupSelectList = Utility.PopulateBloodGroupSelectListItem();
            DepartmentSelectList = Utility.PopulateAllDepartmentSelectListItem();
            DesignationSelectList = Utility.PopulateDesignationSelectListItem();
            GradeLevelSelectList = Utility.PopulateGradeLevelSelectListItem();
            EducationalQualificationSelectList = Utility.PopulateEducationalQualificationSelectListItem();
             UnitSelectList = Utility.PopulateUnitSelectListItem();
            StaffResultGradeSelectList = Utility.PopulateStaffResultGradeSelectListItem();
        }
        public Staff Staff { get; set; }

        public List<EmployeeDetail> StaffList { get; set; }
        public HttpPostedFileBase MyFile { get; set; }
        public EmployeeDetail EmployeeDetail { get; set; }
        public StaffQualification StaffQualification { get; set; }
        public Person Person { get; set; }
        public StaffDepartment StaffDepartment { get; set; }
        public List<SelectListItem> SexSelectList { get; set; }
        public List<SelectListItem> StateSelectList { get; set; }
        public List<SelectListItem> LgaSelectList { get; set; }
        public List<SelectListItem> ReligionSelectList { get; set; }
        public List<SelectListItem> MaritalStatusSelectList { get; set; }
        public List<SelectListItem> StaffTypeSelectList { get; set; }
        public List<SelectListItem> GenotypeSelectList { get; set; }
        public List<SelectListItem> BloodGroupSelectList { get; set; }
        public List<SelectListItem> DepartmentSelectList { get; set; }
        public List<SelectListItem> DesignationSelectList { get; set; }
        public List<SelectListItem> GradeLevelSelectList { get; set; }
        public List<SelectListItem> UnitSelectList { get; set; }
        public List<SelectListItem> EducationalQualificationSelectList { get; set; }
        public List<SelectListItem> StaffResultGradeSelectList { get; set; }
    }
    public class PersonModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        public string SexId { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string DOBString { get; set; }
        public string ContactAddress { get; set; }
        public string StateId { get; set; }
        public string LGAId { get; set; }
        public string ReligionId { get; set; }
        public string MaritalStatusId { get; set; }
        public string Id { get; set; }
        public string StaffTypeId { get; set; }
        public string GenotypeId { get; set; }
        public string BloodGroupId { get; set; }
        public string IsManagement { get; set; }
        public string ProfileDescription { get; set; }

        public string DepartmentId { get; set; }
        public string DateOfPresentAppointment { get; set; }
        public string StaffNumber { get; set; }
        public string DesignationId { get; set; }
        public string GradeLevelId { get; set; }
        public string UnitId { get; set; }
        public string DateOfPreviousAppointment { get; set; }
        public string DateOfRetirement { get; set; }

        public string InstitutionAttended { get; set; }
        public string EducationalQualificationId { get; set; }
        public string StaffResultGradeId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string CertificateNumber { get; set; }
        public string SignatureFileUrl { get; set; }
    }
}