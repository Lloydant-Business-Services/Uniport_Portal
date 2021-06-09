using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Applicant.ViewModels
{
    public class PostJAMBFormPaymentViewModel
    {
        public PostJAMBFormPaymentViewModel()
        {
            Programme = new Programme();
            AppliedCourse = new AppliedCourse();
            AppliedCourse.Programme = new Programme();
            AppliedCourse.Department = new Department();
            SupplementaryCourse = new SupplementaryCourse();
            ApplicantReferee = new List<ApplicantReferee>();
            remitaPayment = new RemitaPayment();

            Person = new Person();
            Person.State = new State();
            StateSelectList = Utility.PopulateStateSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateProgrammeSelectListItem();
            SessionLogic sessionLogic = new SessionLogic();
            var ApplicationActiveSession=sessionLogic.GetModelBy(f => f.Active_Application);
            ApplicationProgrammeSettingSelectListItems = Utility.PopulateApplicationFormSetting(ApplicationActiveSession);
            //ApplicationProgrammeSettingSelectListItems = Utility.PopulateApplicationFormSetting(new Session {Id = 19});
        }

        public List<ApplicantReferee> ApplicantReferee { get; set; }
        public SupplementaryCourse SupplementaryCourse { get; set; }
        public AppliedCourse AppliedCourse { get; set; }
        public BankItRequest BankItRequest { get; set; }
        public ApplicantJambDetail ApplicantJambDetail { get; set; }
        public Department Department { get; set; }
        public RemitaPayment remitaPayment { get; set; }
        public Paystack Paystack { get; set; }

        //[Display(Name = "JAMB Reg. No")]
        //[RegularExpression("^(7)([0-9]{7}[A-Z]{2})$", ErrorMessage = "JAMB Registration No is not valid")]
        public string JambRegistrationNumber { get; set; }

        public Programme Programme { get; set; }
        public List<SelectListItem> StateSelectList { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> DepartmentOptionSelectListItem { get; set; }
        public List<SelectListItem> ApplicationProgrammeSettingSelectListItems { get; set; }
        public FeeType FeeType { get; set; }
        public Person Person { get; set; }
        public decimal Amount { get; set; }
        public Session CurrentSession { get; set; }
        public PaymentType PaymentType { get; set; }
        public Payment Payment { get; set; }
        public ApplicationFormSetting ApplicationFormSetting { get; set; }
        public ApplicationProgrammeFee ApplicationProgrammeFee { get; set; }
        public PaymentEtranzactType PaymentEtranzactType { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public Model.Model.Student Student { get; set; }
        public int HasJambPassport { get; set; }
        public long StudentId { get; set; }
        public Remita Remita { get; internal set; }
        public string Hash { get; internal set; }

        public void Initialise()
        {
            try
            {
                if (Programme.Id == 1 || Programme.Id==3 || Programme.Id==4 || Programme.Id==15 || Programme.Id == 7)
                {
                    CurrentSession = GetCurrentSession();
                }
                else
                {
                    CurrentSession = new Session { Id = 1 };
                }

                if (CurrentSession != null && Programme.Id > 0)
                {
                    FeeType = GetFeeTypeBy(CurrentSession, Programme);
                    if (ApplicationFormSetting == null || ApplicationFormSetting.Id <= 0)
                    {
                        ApplicationFormSetting = GetApplicationFormSettingBy(CurrentSession, Payment);
                    }
                    else
                    {
                        ApplicationFormSetting = GetApplicationFormSettingById(ApplicationFormSetting.Id);
                    }

                    PaymentEtranzactType = GetPaymentTypeBy(FeeType);
                    ApplicationProgrammeFee = GetApplicationProgrammeFeeBy(Programme, FeeType, CurrentSession);
                    if (ApplicationFormSetting != null)
                    {
                        PaymentType = ApplicationFormSetting.PaymentType;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ApplicationProgrammeFee GetApplicationProgrammeFeeBy(Programme Programme, FeeType FeeType,
            Session CurrentSession)
        {
            try
            {
                var applicationProgrammeFeeLogic = new ApplicationProgrammeFeeLogic();
                ApplicationProgrammeFee applicationProgrammeFee =
                    applicationProgrammeFeeLogic.GetModelBy(
                        p =>
                            p.Fee_Type_Id == FeeType.Id && p.Programme_Id == Programme.Id &&
                            p.Session_Id == CurrentSession.Id);
                return applicationProgrammeFee;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Session GetCurrentSession()
         {
            try
            {
                var sessionLogic = new SessionLogic();
                //Session session = sessionLogic.GetModelBy(a => a.Session_Id == 19);
                Session session = sessionLogic.GetModelBy(a => a.Active_Application);
                return session;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FeeType GetFeeTypeBy(Session session, Programme programme)
        {
            try
            {
                var programmeFeeLogic = new ApplicationProgrammeFeeLogic();
                List<ApplicationProgrammeFee> applicationProgrammeFess = programmeFeeLogic.GetListBy(programme, session);
                foreach (ApplicationProgrammeFee item in applicationProgrammeFess)
                {
                    return item.FeeType;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PaymentEtranzactType GetPaymentTypeBy(FeeType feeType)
        {
            var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
            PaymentEtranzactType = paymentEtranzactTypeLogic.GetBy(feeType);

            if (PaymentEtranzactType != null)
            {
                return PaymentEtranzactType;
            }

            return null;
        }

        public ApplicationFormSetting GetApplicationFormSettingBy(Session session, Payment payment)
        {
            try
            {
                var applicationFormSettingLogic = new ApplicationFormSettingLogic();
                var formSettingPayment = new Payment();
                if (payment != null)
                {
                    formSettingPayment.FeeType = payment.FeeType;
                }
                else
                {
                    formSettingPayment.FeeType = new FeeType {Id = 1};
                }

                return applicationFormSettingLogic.GetBy(session, formSettingPayment);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ApplicationFormSetting GetApplicationFormSettingById(int Id)
        {
            try
            {
                var applicationFormSettingLogic = new ApplicationFormSettingLogic();
                return applicationFormSettingLogic.GetModelBy(a => a.Application_Form_Setting_Id == Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}