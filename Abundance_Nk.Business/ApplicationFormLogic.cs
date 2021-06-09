using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Configuration;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class ApplicationFormLogic : BusinessBaseLogic<ApplicationForm, APPLICATION_FORM>
    {
        private readonly AdmissionListLogic admissionListLogic;
        private readonly PaymentLogic paymentLogic;
        private readonly ScratchCardLogic scratchCardLogic;
        private ApplicantJambDetailLogic applicantJambDetailLogic;
        private CardPaymentLogic cardPaymentLogic;
        private Abundance_NkEntities abundanceNkEntities;
        public ApplicationFormLogic()
        {
            translator = new ApplicationFormTranslator();
            scratchCardLogic = new ScratchCardLogic();
            cardPaymentLogic = new CardPaymentLogic();
            paymentLogic = new PaymentLogic();
            applicantJambDetailLogic = new ApplicantJambDetailLogic();
            admissionListLogic = new AdmissionListLogic();
            abundanceNkEntities = new Abundance_NkEntities();
        }

        public override ApplicationForm Create(ApplicationForm model)
        {
           var result = abundanceNkEntities.STP_INSERT_APPLICATION_FORM(model.SerialNumber, model.Number, model.ExamSerialNumber,model.ExamNumber,model.Setting.Id,model.ProgrammeFee.Id,model.Payment.Id,model.Person.Id,model.DateSubmitted,model.Release,model.Rejected,model.RejectReason,model.Remarks).FirstOrDefault();
           ApplicationForm form = new ApplicationForm();
            if (result != null)
            {
                form.Number = result.Application_Form_Number ?? "";
                form.ExamNumber = result.Application_Exam_Number ?? "";
                form.RejectReason = result.Reject_Reason ?? "";
                form.Id = result.Application_Form_Id ;
                form.SerialNumber = result.Serial_Number ?? 0;
            }
            return form;

        }

        public bool IsValidApplicationNumberAndPin(string applicationNumber, string pin)
        {
            try
            {
                AdmissionList admissionList = admissionListLogic.GetBy(applicationNumber);
                if (admissionList == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
                

                //if ((admissionList.Programme.Id == 1 || admissionList.Programme.Id == 4) &&(admissionList.Form.Setting.Session.Id == 19))
                //{
                //    PaystackLogic paystackLogic = new PaystackLogic();
                //    string PaystackSecrect = ConfigurationManager.AppSettings["PaystackSecrect"].ToString();
                //    paystackLogic.VerifyPayment(new Payment() { InvoiceNumber = pin }, PaystackSecrect);

                //    string MonnifyURL = ConfigurationManager.AppSettings["MonnifyUrl"].ToString();
                //    string MonnifyUser = ConfigurationManager.AppSettings["MonnifyApiKey"].ToString();
                //    string MonnifySecrect = ConfigurationManager.AppSettings["MonnifyContractCode"].ToString();
                //    string MonnifyCode = ConfigurationManager.AppSettings["MonnifyCode"].ToString();
                //    PaymentMonnifyLogic paymentMonnifyLogic = new PaymentMonnifyLogic(MonnifyURL, MonnifyUser, MonnifySecrect, MonnifyCode);
                //    var PaymentMonnify = paymentMonnifyLogic.GetInvoiceStatus(pin);


                //    ScratchCard card = scratchCardLogic.GetBy(pin);
                //    if (card == null)
                //    {
                //        throw new Exception("Invalid Pin!");
                //    }
                //    else
                //    {
                //        //card has been used
                //        if (scratchCardLogic.IsPinUsed(pin, applicationNumber))
                //        {
                //            throw new Exception("Pin entered does not belong to applicant '" + admissionList.Form.Person.FullName + "' !");

                //        }
                //        else
                //        {
                //            if (card.Batch.Price == 2000)
                //            {
                //                scratchCardLogic.UpdatePin(pin, new Person { Id = admissionList.Form.Person.Id });

                //                return true;
                //            }
                //            else
                //            {
                //                throw new Exception("Pin entered is not valid for admission status checking.");

                //            }
                //        }

                //    }

                //}
                //else
                //{
                //    return true;
                //}
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ApplicationForm Create(ApplicationForm form, AppliedCourse appliedCourse)
        {
            try
            {
                ApplicationForm newForm = base.Create(form);
                if (newForm == null || newForm.Id <= 0)
                {
                    throw new Exception("Application Form creation failed!");
                }

                newForm.Setting = form.Setting;
                newForm.ProgrammeFee = form.ProgrammeFee;
                newForm = SetNextApplicationFormNumber(newForm);
                newForm = SetNextExamNumber(newForm, appliedCourse);

                SetApplicationAndExamNumber(newForm);

                return newForm;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SetRejectReason(ApplicationForm form)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = a => a.Application_Form_Id == form.Id;
                APPLICATION_FORM entity = GetEntityBy(selector);

                entity.Reject_Reason = form.RejectReason;
                entity.Rejected = form.Rejected;
                entity.Release = form.Release;

                int modifiedRecordCount = Save();

                return modifiedRecordCount > 0 ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool AcceptOrReject(List<ApplicationForm> applications, bool accept)
        {
            try
            {
                if (applications == null || applications.Count <= 0)
                {
                    throw new Exception("No aplication found to Accept!");
                }

                bool done = false;
                foreach (ApplicationForm application in applications)
                {
                    Expression<Func<APPLICATION_FORM, bool>> selector = a => a.Application_Form_Id == application.Id;
                    APPLICATION_FORM entity = GetEntityBy(selector);

                    if (entity != null)
                    {
                        entity.Rejected = accept;
                    }

                    done = repository.Save() > 0 ? true : false;
                }

                return done;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ApplicationForm SetNextApplicationFormNumber(ApplicationForm form)
        {
            try
            {
                form.SerialNumber = form.Id;
                form.Number = "ABSU/" + form.ProgrammeFee.Programme.ShortName + "/" + DateTime.Now.ToString("yy") + "/" +
                              PaddNumber(form.Id, 10);

                return form;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ApplicationForm SetNextExamNumber(ApplicationForm form, AppliedCourse appliedCourse)
        {
            try
            {
                //Func<VW_APPLICATION_EXAM_NUMBER, int> selector = s => s.Application_Exam_Serial_Number.Value;
                //int rawMaxExamSerialNumber = repository.GetMaxValueBy(selector);

                int newExamSerialNumber = 0;
                List<ApplicationForm> applicationForms =
                    (from a in
                        repository.GetBy<VW_APPLICATION_EXAM_NUMBER>(
                            a =>
                                a.Application_Form_Setting_Id == form.Setting.Id &&
                                a.Programme_Id == appliedCourse.Programme.Id &&
                                a.Department_Id == appliedCourse.Department.Id)
                        select new ApplicationForm
                        {
                            Id = a.Application_Form_Id,
                            SerialNumber = a.Serial_Number,
                            ExamSerialNumber = a.Application_Exam_Serial_Number,
                            ExamNumber = a.Application_Exam_Number,
                        }).ToList();

                if (applicationForms != null && applicationForms.Count > 0)
                {
                    int rawMaxExamSerialNumber = applicationForms.Max(a => a.ExamSerialNumber.Value);
                    newExamSerialNumber = rawMaxExamSerialNumber + 1;
                }
                else
                {
                    newExamSerialNumber = 1;
                }

                form.ExamSerialNumber = newExamSerialNumber;
                form.ExamNumber = appliedCourse.Department.Code + PaddNumber(newExamSerialNumber, 5);

                return form;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ApplicationForm> GetAllHndApplicants()
        {
            try
            {
                List<ApplicationForm> applicationForms =
                    (from a in
                        repository.GetBy<APPLICATION_FORM>(
                            a =>
                                a.PERSON.APPLICANT_APPLIED_COURSE.Programme_Id == 3 ||
                                a.PERSON.APPLICANT_APPLIED_COURSE.Programme_Id == 4)
                        select new ApplicationForm
                        {
                            Id = a.Application_Form_Id,
                            SerialNumber = a.Serial_Number,
                            ExamSerialNumber = a.Application_Exam_Serial_Number,
                            ExamNumber = a.Application_Exam_Number,
                            Number = a.Application_Form_Number,
                        }).ToList();
                return applicationForms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string PaddNumber(long id, int maxCount)
        {
            try
            {
                string idInString = id.ToString();
                string paddNumbers = "";
                if (idInString.Count() < maxCount)
                {
                    int zeroCount = maxCount - id.ToString().Count();
                    var builder = new StringBuilder();
                    for (int counter = 0; counter < zeroCount; counter++)
                    {
                        builder.Append("0");
                    }

                    builder.Append(id);
                    paddNumbers = builder.ToString();
                    return paddNumbers;
                }

                return paddNumbers;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PhotoCard> GetPostJAMBApplications(Session session)
        {
            try
            {
                IEnumerable<PhotoCard> applications =
                    from a in repository.GetBy<VW_POST_JAMP_APPLICATION>(a => a.Session_Id == session.Id)
                    select new PhotoCard
                    {
                        PersonId = a.Person_Id,
                        Name = a.Name,
                        AplicationNumber = a.Application_Form_Id,
                        PaymentNumber = a.Payment_Id,
                        FirstChoiceDepartment = a.First_Choice_Department_Name,
                        MobilePhone = a.Mobile_Phone,
                        AppliedProgrammeName = a.Programme_Name,
                        PassportUrl = a.Image_File_Url,
                        AplicationFormNumber = a.Application_Form_Number,
                    };

                return applications.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PhotoCard> GetPostJAMBApplicationsBy(Session session, Programme programme, SortOption sortOpton)
        {
            try
            {
                IEnumerable<PhotoCard> applications =
                    from a in
                        repository.GetBy<VW_POST_JAMP_APPLICATION>(
                            a => a.Session_Id == session.Id && a.Programme_Id == programme.Id)
                    select new PhotoCard
                    {
                        PersonId = a.Person_Id,
                        Name = a.Name,
                        AplicationNumber = a.Application_Form_Id,
                        PaymentNumber = a.Payment_Id,
                        FirstChoiceDepartment = a.First_Choice_Department_Name,
                        MobilePhone = a.Mobile_Phone,
                        AppliedProgrammeName = a.Programme_Name,
                        PassportUrl = a.Image_File_Url,
                        AplicationFormNumber = a.Application_Form_Number,
                        SessionName = a.Session_Name,
                        AplicationSerialNumber = a.Serial_Number,
                        ExamNumber = a.Application_Exam_Number,
                        ExamSerialNumber = a.Application_Exam_Serial_Number,
                    };

                applications = SortApplicantList(sortOpton, applications);

                return applications.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PhotoCard> GetPostJAMBApplicationsBy(Session session, Programme programme, Department department,
            SortOption sortOpton)
        {
            try
            {
                IEnumerable<PhotoCard> applications =
                    from a in
                        repository.GetBy<VW_POST_JAMP_APPLICATION>(
                            a =>
                                a.Session_Id == session.Id && a.Programme_Id == programme.Id &&
                                a.Department_Id == department.Id)
                    select new PhotoCard
                    {
                        PersonId = a.Person_Id,
                        Name = a.Name,
                        AplicationNumber = a.Application_Form_Id,
                        PaymentNumber = a.Payment_Id,
                        FirstChoiceDepartment = a.First_Choice_Department_Name,
                        MobilePhone = a.Mobile_Phone,
                        AppliedProgrammeName = a.Programme_Name,
                        PassportUrl = a.Image_File_Url,
                        AplicationFormNumber = a.Application_Form_Number,
                        SessionName = a.Session_Name,
                        AplicationSerialNumber = a.Serial_Number,
                        ExamNumber = a.Application_Exam_Number,
                        ExamSerialNumber = a.Application_Exam_Serial_Number,
                    };

                applications = SortApplicantList(sortOpton, applications);

                return applications.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Slug> GetPostJAMBSlugDataBy(Session session, Programme programme, Department department)
        {
            try
            {
                if (programme.Id == 1)
                {
                    IEnumerable<Slug> applications =
                        from a in
                            repository.GetBy<VW_POST_JAMP_APPLICATION>(
                                a =>
                                    a.Session_Id == session.Id && a.Programme_Id == programme.Id &&
                                    a.Department_Id == department.Id)
                        select new Slug
                        {
                            PersonId = a.Person_Id,
                            Name = a.Name,
                            AplicationNumber = a.Application_Form_Id,
                            PaymentNumber = a.Payment_Id,
                            FirstChoiceDepartment = a.First_Choice_Department_Name,
                            MobilePhone = a.Mobile_Phone,
                            AppliedProgrammeName = a.Programme_Name,
                            PassportUrl = a.Image_File_Url,
                            AplicationFormNumber = a.Application_Form_Number,
                            SessionName = a.Session_Name,
                            ExamNumber = a.Application_Exam_Number,
                            JambNumber = a.Applicant_Jamb_Registration_Number,
                            JambScore = a.Applicant_Jamb_Score,
                            Sex = a.Sex_Name,
                            Choice = a.Institution_Choice_Name,
                            Form = a.Application_Form_Setting_Name

                        };
                    if (applications != null && applications.Count() > 0)
                    {
                        applications = applications.OrderBy(a => a.ExamNumber);
                    }
                    return applications.ToList();
                }
                else
                {
                    IEnumerable<Slug> applications =
                        from a in
                            repository.GetBy<VW_POST_JAMP_APPLICATION>(
                                a =>
                                    a.Session_Id == session.Id && a.Programme_Id == programme.Id &&
                                    a.Department_Id == department.Id)
                        select new Slug
                        {
                            PersonId = a.Person_Id,
                            Name = a.Name,
                            AplicationNumber = a.Application_Form_Id,
                            PaymentNumber = a.Payment_Id,
                            FirstChoiceDepartment = a.First_Choice_Department_Name,
                            MobilePhone = a.Mobile_Phone,
                            AppliedProgrammeName = a.Programme_Name,
                            PassportUrl = a.Image_File_Url,
                            AplicationFormNumber = a.Application_Form_Number,
                            SessionName = a.Session_Name,
                            ExamNumber = a.Application_Exam_Number
                        };
                    if (applications != null && applications.Count() > 0)
                    {
                        applications = applications.OrderBy(a => a.ExamNumber);
                    }
                    return applications.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IEnumerable<PhotoCard> SortApplicantList(SortOption sortOpton, IEnumerable<PhotoCard> applications)
        {
            try
            {
                if (applications != null && applications.Count() > 0)
                {
                    switch (sortOpton)
                    {
                        case SortOption.Name:
                        {
                            applications = applications.OrderBy(a => a.Name);
                            //applications = applications.OrderBy(a => a.FirstChoiceDepartment).OrderBy(a => a.Name);
                            break;
                        }
                        case SortOption.ExamNo:
                        {
                            applications = applications.OrderBy(a => a.ExamSerialNumber);
                            //applications = applications.OrderBy(a => a.FirstChoiceDepartment).OrderBy(a => a.ExamSerialNumber);
                            break;
                        }
                        case SortOption.ApplicationNo:
                        {
                            applications = applications.OrderBy(a => a.AplicationSerialNumber);
                            //applications = applications.OrderBy(a => a.FirstChoiceDepartment).OrderBy(a => a.AplicationSerialNumber);
                            break;
                        }
                    }
                }

                return applications;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private APPLICATION_FORM GetEntityBy(Person person)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = s => s.Person_Id == person.Id;
                APPLICATION_FORM entity = GetEntityBy(selector);

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SetApplicationAndExamNumber(ApplicationForm applicationForm)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = af => af.Application_Form_Id == applicationForm.Id;
                APPLICATION_FORM entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Application_Form_Number = applicationForm.Number;
                entity.Serial_Number = applicationForm.SerialNumber;
                entity.Application_Exam_Number = applicationForm.ExamNumber;
                entity.Application_Exam_Serial_Number = applicationForm.ExamSerialNumber;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(ApplicationForm applicationForm)
        {
            try
            {
                APPLICATION_FORM entity = GetEntityBy(applicationForm.Person);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Application_Form_Id = applicationForm.Id;
                entity.Application_Form_Setting_Id = applicationForm.Setting.Id;
                entity.Application_Programme_Fee_Id = applicationForm.ProgrammeFee.Id;
                entity.Payment_Id = applicationForm.Payment.Id;
                entity.Person_Id = applicationForm.Person.Id;
                entity.Date_Submitted = DateTime.Now;
                entity.Release = applicationForm.Release;
                entity.Rejected = applicationForm.Rejected;
                entity.Remarks = applicationForm.Remarks;


                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ApplicationFormSummary> GetSummary(Session session)
        {
            try
            {
                List<ApplicationFormSummary> applicationForms =
                    (from a in repository.GetBy<VW_APPLICATION_FORM_SUMMARY>(a => a.Session_Id == session.Id)
                        select new ApplicationFormSummary
                        {
                            ProgrammeId = a.Programme_Id,
                            ProgrammeName = a.Programme_Name,
                            DepartmentName = a.Department_Name,
                            SessionName = a.Session_Name,
                            FormCount = (int) a.Application_Form_Count,
                        }).ToList();

                return applicationForms;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ApplicationForm GetBy(long applicationFormId)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = a => a.Application_Form_Id == applicationFormId;
                return GetModelBy(selector);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ApplicationForm GetBy(Person person)
        {
            try
            {
                Expression<Func<APPLICATION_FORM, bool>> selector = a => a.Person_Id == person.Id;
                ApplicationForm applicationForm = GetModelBy(selector);
                if (applicationForm != null && applicationForm.Id > 0)
                {
                    applicationForm.Payment = paymentLogic.GetBy(applicationForm.Payment.Id);
                }
                return applicationForm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PhotoCard> GetApplicantList(Session session, DateTime dateFrom, DateTime dateTo)
        {
            List<PhotoCard> applicants = new List<PhotoCard>();
            try
            {
                applicants = (from a in repository.GetBy<VW_POST_JAMP_APPLICATION>(a => a.Session_Id == session.Id && a.Date_Submitted >= dateFrom && a.Date_Submitted <= dateTo)
                              select new PhotoCard
                              {
                                  Name = a.Name,
                                  MobilePhone = a.Mobile_Phone,
                                  AplicationFormNumber = a.Application_Form_Number,
                                  ExamNumber = a.Application_Exam_Number,
                                  AppliedProgrammeName = a.Programme_Name,
                                  FirstChoiceDepartment = a.First_Choice_Department_Name,
                                  SessionName = a.Session_Name,
                                  DateSubmitted = a.Date_Submitted.ToLongDateString(),
                                  JambRegNumber = a.Applicant_Jamb_Registration_Number
                              }).ToList();
            }
            catch (Exception)
            {
                throw;
            }

            return applicants.OrderBy(a => a.AppliedProgrammeName).ThenBy(a => a.FirstChoiceDepartment).ThenBy(a => a.AplicationFormNumber).ToList();
        }
        public List<PutmeResultModel> GetPutmeResults(Session session)
        {
            List<PutmeResultModel> results = new List<PutmeResultModel>();
            try
            {
                if (session != null && session.Id > 0)
                {
                    results = (from a in repository.GetBy<VW_PUTME_RESULT>(a => a.Session_Id == session.Id)
                               select new PutmeResultModel
                               {
                                   Fullname = a.FULLNAME,
                                   PhoneNumber = a.Mobile_Phone,
                                   Sex = a.Sex_Name,
                                   ApplicationNumber = a.Application_Form_Number,
                                   ExamNumber = a.Application_Exam_Number,
                                   JambScore = a.JAMBSCORE != null ? Convert.ToDouble(a.JAMBSCORE) : 0,
                                   ExamScore = a.RAW_SCORE,
                                   Programme = a.PROGRAMME,
                                   Department = a.Department_Name,
                                   JambNumber = a.Applicant_Jamb_Registration_Number
                               }).ToList();
                }
                else
                {
                    results = (from a in repository.GetBy<PUTME_RESULT>()
                               select new PutmeResultModel
                               {
                                   Fullname = a.FULLNAME,
                                   ExamNumber = a.EXAMNO,
                                   JambScore = a.JAMBSCORE != null ? Convert.ToDouble(a.JAMBSCORE) : 0,
                                   ExamScore = a.RAW_SCORE,
                                   Programme = a.PROGRAMME,
                                   Department = a.COURSE,
                                   JambNumber = a.REGNO
                               }).ToList();
                }

            }
            catch (Exception)
            {
                throw;
            }

            return results.OrderBy(a => a.Programme).ThenBy(a => a.Department).ThenBy(a => a.ApplicationNumber).ToList();
        }

        public async Task<List<Slug>> GetPostJambSlugDataBulk(Session session)
        {
            try
            {
                List<Slug> applications = (from a in await repository.GetByAsync<VW_POST_JAMP_APPLICATION>(a => a.Session_Id == session.Id && a.Applicant_Jamb_Registration_Number != null)
                                           select new Slug
                                           {
                                               PersonId = a.Person_Id,
                                               Name = a.Name,
                                               AplicationNumber = a.Application_Form_Id,
                                               PaymentNumber = a.Payment_Id,
                                               FirstChoiceDepartment = a.First_Choice_Department_Name,
                                               MobilePhone = a.Mobile_Phone,
                                               AppliedProgrammeName = a.Programme_Name,
                                               PassportUrl = a.Image_File_Url,
                                               AplicationFormNumber = a.Application_Form_Number,
                                               SessionName = a.Session_Name,
                                               ExamNumber = a.Application_Exam_Number,
                                               JambNumber = a.Applicant_Jamb_Registration_Number,
                                               JambScore = a.Applicant_Jamb_Score,
                                               Sex = a.Sex_Name,
                                               Choice = a.Institution_Choice_Name,
                                               Form = a.Application_Form_Setting_Name


                                           }).ToList();
                if (applications.Count > 0)
                {
                    applications = applications.OrderBy(a => a.AppliedProgrammeName).ThenBy(a => a.FirstChoiceDepartment).ThenBy(a => a.ExamNumber).ToList();
                }
                return applications.ToList();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PhotoCard> GetApplicantListBulk(Session session, DateTime dateFrom, DateTime dateTo)
        {
            List<PhotoCard> applicants = new List<PhotoCard>();
            try
            {
                applicants = (from a in repository.GetBy<VW_POST_JAMP_APPLICATION>(a => a.Session_Id == session.Id && (a.Date_Submitted >= dateFrom && a.Date_Submitted <= dateTo))
                              select new PhotoCard
                              {
                                  Name = a.Name,
                                  MobilePhone = a.Mobile_Phone,
                                  AplicationFormNumber = a.Application_Form_Number,
                                  ExamNumber = a.Application_Exam_Number,
                                  AppliedProgrammeName = a.Programme_Name,
                                  FirstChoiceDepartment = a.First_Choice_Department_Name,
                                  SessionName = a.Session_Name,
                                  DateSubmitted = a.Date_Submitted.ToLongDateString()
                              }).ToList();


            }
            catch (Exception)
            {
                throw;
            }

            return applicants.OrderBy(a => a.AppliedProgrammeName).ThenBy(a => a.FirstChoiceDepartment).ThenBy(a => a.AplicationFormNumber).ToList();
        }
        public List<ApplicationSummaryReport> GetApplicantsPerSession(int sessionId, Programme programme, DateTime dateFrom, DateTime dateTo)
        {
            List<ApplicationSummaryReport> applicationSummaryReport = new List<ApplicationSummaryReport>();
            try
            {
                //List<ApplicationSummaryReport> applicantList = null;
                
                if (programme != null && programme.Id > 0 && sessionId > 0)
                {

                    
                    if (dateFrom != DateTime.MinValue && dateFrom != DateTime.MinValue)
                    {
                        DateTime applicationFrom = Convert.ToDateTime(dateFrom);
                        DateTime applicationTo = Convert.ToDateTime(dateTo);
                         applicationSummaryReport = (from sr in repository.GetBy<VW_APPLICATION_SUMMARY_REPORT>(x => x.Session_Id == sessionId && x.Programme_Id == programme.Id && (x.Date_Submitted >= applicationFrom && x.Date_Submitted <= applicationTo))
                                                select new ApplicationSummaryReport
                                                {
                                                    ApplicationFormNo=sr.Application_Form_Number,
                                                    Department=sr.Department_Name,
                                                    FullName=sr.Full_Name,
                                                    ImageUrl=sr.Image_File_Url,
                                                    ApplicationSubmittedOn=sr.Date_Submitted,
                                                    Programme=sr.Programme_Name,
                                                    Session=sr.Session_Name,
                                                    DepartmentOption=sr.Department_Option_Name
                                                }).OrderBy(x => x.ApplicationSubmittedOn).ToList();
                        //AF = applicationFormLogic.GetModelsBy(af => af.APPLICATION_FORM_SETTING.Session_Id == sessionId && af.APPLICATION_PROGRAMME_FEE.Programme_Id == programme.Id && (af.Date_Submitted >= applicationFrom && af.Date_Submitted <= applicationTo));
                    }
                    else
                    {
                        applicationSummaryReport = (from sr in repository.GetBy<VW_APPLICATION_SUMMARY_REPORT>(x => x.Session_Id == sessionId && x.Programme_Id == programme.Id)
                                                    select new ApplicationSummaryReport
                                                    {
                                                        ApplicationFormNo = sr.Application_Form_Number,
                                                        Department = sr.Department_Name,
                                                        FullName = sr.Full_Name,
                                                        ImageUrl = sr.Image_File_Url,
                                                        ApplicationSubmittedOn = sr.Date_Submitted,
                                                        Programme = sr.Programme_Name,
                                                        Session = sr.Session_Name,
                                                        DepartmentOption = sr.Department_Option_Name
                                                    }).OrderBy(x => x.ApplicationSubmittedOn).ToList();
                        //AF = applicationFormLogic.GetModelsBy(af => af.APPLICATION_FORM_SETTING.Session_Id == sessionId && af.APPLICATION_PROGRAMME_FEE.Programme_Id == programme.Id);
                    }
                }


                return applicationSummaryReport;
            }
            catch (Exception ex) { throw ex; }
        }
        //public List<CbtCandidates>GetApplicationForCBT(int Session)
        //{

        //}
    }
}