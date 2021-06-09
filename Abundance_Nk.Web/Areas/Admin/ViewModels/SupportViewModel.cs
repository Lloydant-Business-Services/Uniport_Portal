using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class SupportViewModel
    {
        private readonly OLevelGradeLogic oLevelGradeLogic;
        private readonly OLevelSubjectLogic oLevelSubjectLogic;
        private readonly OLevelTypeLogic oLevelTypeLogic;
        private readonly ResultGradeLogic resultGradeLogic;
        private DepartmentLogic departmentLogic;
        private PaymentLogic paymentLogic;
        private PersonLogic personLogic;

        public SupportViewModel()
        {
            oLevelTypeLogic = new OLevelTypeLogic();
            oLevelGradeLogic = new OLevelGradeLogic();
            oLevelSubjectLogic = new OLevelSubjectLogic();
            resultGradeLogic = new ResultGradeLogic();
            FirstSittingOLevelResult = new OLevelResult();
            FirstSittingOLevelResult.Type = new OLevelType();

            SecondSittingOLevelResult = new OLevelResult();
            SecondSittingOLevelResult.Type = new OLevelType();
            paymentLogic = new PaymentLogic();
            personLogic = new PersonLogic();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            ExamYearSelectList = Utility.PopulateExamYearSelectListItem(1990);
            OLevelTypeSelectList = Utility.PopulateOLevelTypeSelectListItem();
            OLevelGradeSelectList = Utility.PopulateOLevelGradeSelectListItem();
            OLevelSubjectSelectList = Utility.PopulateOLevelSubjectSelectListItem();
            ResultGradeSelectList = Utility.PopulateResultGradeSelectListItem();
            LevelSelectList = Utility.PopulateLevelSelectListItem();
            StateSelectListItem = Utility.PopulateStateSelectListItem();
            FeeTypeSelectList = Utility.PopulateFeeTypeSelectListItem();
            SessionSelectList = Utility.PopulateAllSessionSelectListItem();
            StatusSelectListItem = Utility.PopulateApplicantStatusSelectListItem();
            PaymentModeSelectListItem = Utility.PopulatePaymentModeSelectListItem();
            FormSettingSelectListItem = Utility.PopulateFormSettingSelectListItem();
            LevelList = Utility.GetAllLevels();
            if(Programme != null && Programme.Id > 0)
            {
                DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(Programme);
            }
            PopulateDropdowns();
            InitialiseOLevelResult();
        }

        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        [RegularExpression("^0[0-9]{10}$",ErrorMessage = "Phone number is not valid")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string Pin { get; set; }

        [Display(Name = "Message Content")]
        public string Body { get; set; }

        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public List<SelectListItem> FormSettingSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> StatusSelectListItem { get; set; }
        public List<SelectListItem> PaymentModeSelectListItem { get; set; }
        public List<SelectListItem> StateSelectListItem { get; set; }
        public CurrentSessionSemester CurrentSessionSemester { get; set; }
        public Model.Model.Applicant Applicant { get; set; }
        public Payment Payment { get; set; }
        public AppliedCourse AppliedCourse { get; set; }
        public Person Person { get; set; }
        public ApplicantJambDetail ApplicantJambDetail { get; set; }
        public List<PersonAudit> PersonAudit { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public PersonAudit PersonAuditDetails { get; set; }
        public AppliedCourseAudit AppliedCourseAuditDetails { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public List<OLevelType> OLevelTypes { get; set; }
        public List<OLevelGrade> OLevelGrades { get; set; }
        public List<OLevelSubject> OLevelSubjects { get; set; }
        public List<ResultGrade> ResultGrades { get; set; }
        public List<Model.Model.Student> StudentList { get; set; }
        public List<Payment> Payments { get; set; }
        public List<SelectListItem> ExamYearSelectList { get; set; }
        public List<SelectListItem> OLevelTypeSelectList { get; set; }
        public List<SelectListItem> OLevelGradeSelectList { get; set; }
        public List<SelectListItem> OLevelSubjectSelectList { get; set; }
        public List<SelectListItem> ResultGradeSelectList { get; set; }
        public List<SelectListItem> LevelSelectList { get; set; }
        public OLevelResult FirstSittingOLevelResult { get; set; }
        public OLevelResult SecondSittingOLevelResult { get; set; }
        public OLevelResultDetail FirstSittingOLevelResultDetail { get; set; }
        public OLevelResultDetail SecondSittingOLevelResultDetail { get; set; }
        public List<OLevelResultDetail> FirstSittingOLevelResultDetails { get; set; }
        public List<OLevelResultDetail> SecondSittingOLevelResultDetails { get; set; }
        public List<ApplicantResult> ApplicantResults { get; set; }
        public Model.Model.Applicant Applicants { get; set; }
        public Model.Model.Student Student { get; set; }
        public Level Level { get; set; }
        public string MatricNumber { get; set; }
        public PaymentEtranzact PaymentEtranzact { get; set; }
        public List<PaymentEtranzact> EtranzactPins { get; set; }
        public ScratchCard ScratchCard { get; set; }
        public List<ScratchCard> ScratchCards { get; set; } 
        public FeeType FeeType { get; set; }
        public List<SelectListItem> FeeTypeSelectList { get; set; }
        public HttpPostedFileBase File { get; set; }
        public List<Programme> ProgrammeList { get; set; }
        public List<Department> DepartmentList { get; set; }
        public List<SelectListItem> SessionSelectList { get; set; }
        public Session Session { get; set; }
        public List<SelectListItem> AllSessionSelectList { get; set; }
        public List<Level> LevelList { get; set; }
        public List<Course> Courses { get; set; }
        public Semester Semester { get; set; }
        public string MatricNumberAlt { get; set; }
        public AdmissionList AdmissionList { get; set; }

        public List<CourseAllocation> CourseAllocationList { get; set; }
        public List<User> Users { get; set; }
        public Role Role { get; set; }
        public StudentPayment StudentPayment { get; set; }
        public List<ManualPayment> ManualPayments { get; set; }
        public List<ProgrammeDepartment> ProgrammeDepartments { get; set; }
        public List<UploadStudentData> UploadStudentDataList { get; set; }
        public bool ShowTable { get; set; }
        public void InitialiseOLevelResult()
        {
            try
            {
                var oLevelResultDetails = new List<OLevelResultDetail>();
                var oLevelResultDetail1 = new OLevelResultDetail();
                var oLevelResultDetail2 = new OLevelResultDetail();
                var oLevelResultDetail3 = new OLevelResultDetail();
                var oLevelResultDetail4 = new OLevelResultDetail();
                var oLevelResultDetail5 = new OLevelResultDetail();
                var oLevelResultDetail6 = new OLevelResultDetail();
                var oLevelResultDetail7 = new OLevelResultDetail();
                var oLevelResultDetail8 = new OLevelResultDetail();
                var oLevelResultDetail9 = new OLevelResultDetail();

                var oLevelResultDetail11 = new OLevelResultDetail();
                var oLevelResultDetail22 = new OLevelResultDetail();
                var oLevelResultDetail33 = new OLevelResultDetail();
                var oLevelResultDetail44 = new OLevelResultDetail();
                var oLevelResultDetail55 = new OLevelResultDetail();
                var oLevelResultDetail66 = new OLevelResultDetail();
                var oLevelResultDetail77 = new OLevelResultDetail();
                var oLevelResultDetail88 = new OLevelResultDetail();
                var oLevelResultDetail99 = new OLevelResultDetail();

                FirstSittingOLevelResultDetails = new List<OLevelResultDetail>();
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail1);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail2);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail3);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail4);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail5);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail6);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail7);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail8);
                FirstSittingOLevelResultDetails.Add(oLevelResultDetail9);

                SecondSittingOLevelResultDetails = new List<OLevelResultDetail>();
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail11);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail22);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail33);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail44);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail55);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail66);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail77);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail88);
                SecondSittingOLevelResultDetails.Add(oLevelResultDetail99);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ApplicationForm GetApplicationFormBy(Person person,Payment payment)
        {
            try
            {
                var applicationFormLogic = new ApplicationFormLogic();
                return applicationFormLogic.GetModelBy(a => a.Person_Id == person.Id && a.Payment_Id == payment.Id);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void PopulateDropdowns()
        {
            try
            {
                OLevelTypes = oLevelTypeLogic.GetAll();
                OLevelGrades = oLevelGradeLogic.GetAll();
                OLevelSubjects = oLevelSubjectLogic.GetAll();
                ResultGrades = resultGradeLogic.GetAll();
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public List<StudentLevel> StudentLevels { get; set; }

        public PaymentHistory PaymentHistory { get; set; }
    }

    public class StaffUser
    {
        public int SN { get; set; }
        public string USER_NAME { get; set; }
        public string PASSWORD { get; set; }
    }
    public class UploadStudentData
    {
        public int SN { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        public string JambNo { get; set; }
        public string EmailAddress { get; set; }
        public string Remarks { get; set; }
    }
}