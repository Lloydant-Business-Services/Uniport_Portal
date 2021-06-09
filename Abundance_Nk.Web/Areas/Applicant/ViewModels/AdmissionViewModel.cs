using System.Collections.Generic;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Transactions;
using Abundance_Nk.Web.Models;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using System.Configuration;

namespace Abundance_Nk.Web.Areas.Applicant.ViewModels
{
    public class AdmissionViewModel :OLevelResultViewModel
    {
        private readonly AdmissionListLogic admissionListLogic;
        private readonly ApplicantLogic applicantLogic;
        private readonly ApplicationFormLogic applicationFormLogic;
        private readonly AppliedCourseLogic appliedCourseLogic;
        private readonly OnlinePaymentLogic onlinePaymentLogic;
        private readonly PaymentEtranzactLogic paymentEtranzactLogic;
        private readonly RemitaPaymentLogic remitaPaymentLogic;
        private readonly PaymentLogic paymentLogic;

        public AdmissionViewModel()
        {
            ApplicationForm = new ApplicationForm();
            ApplicationForm.Person = new Person();

            Applicant = new Model.Model.Applicant();
            Applicant.Status = new ApplicantStatus();

            AppliedCourse = new AppliedCourse();
            AppliedCourse.Programme = new Programme();
            AppliedCourse.Department = new Department();
            admissionList = new AdmissionList();
            admissionList.Deprtment = new Department();
            Payment = new Payment();
            paymentEtranzactLogic = new PaymentEtranzactLogic();

            Invoice = new Invoice();
            Invoice.Payment = new Payment();
            Invoice.Payment.FeeType = new FeeType();
            //Invoice.Payment.Fee.Type = new FeeType();
            Invoice.Person = new Person();

            paymentLogic = new PaymentLogic();
            appliedCourseLogic = new AppliedCourseLogic();
            applicationFormLogic = new ApplicationFormLogic();
            applicantLogic = new ApplicantLogic();
            onlinePaymentLogic = new OnlinePaymentLogic();
            admissionListLogic = new AdmissionListLogic();
            remitaPaymentLogic = new RemitaPaymentLogic();

            ScratchCard = new ScratchCard();
            ChangeOfCourseApplies = false;
            PaymentModeSelectListItem = Utility.PopulatePaymentModeFirstYearSelectListItem();
        }
        public List<SelectListItem> PaymentModeSelectListItem { get; set; }
        public bool Loaded { get; set; }
        public ScratchCard ScratchCard { get; set; }
        public Remita remita { get; set; }
        public RemitaPayment remitaPayment { get; set; }
        public RemitaResponse remitaResponse { get; set; }
        public Receipt Receipt { get; set; }
        public Invoice Invoice { get; set; }
        public Model.Model.Applicant Applicant { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public AppliedCourse AppliedCourse { get; set; }
        public AdmissionList admissionList { get; set; }

        public Payment Payment { get; set; }
        public bool IsAdmitted { get; set; }
        public int ApplicantStatusId { get; set; }

        [Display(Name = "Acceptance Confirmation Number")]
        public string AcceptanceConfirmationOrderNumber { get; set; }

        [Display(Name = "School Fees Confirmation Number")]
        public string SchoolFeesConfirmationOrderNumber { get; set; }

        [Display(Name = "Acceptance Invoice Number")]
        public string AcceptanceInvoiceNumber { get; set; }

        public bool ChangeOfCourseApplies { get; set; }

        [Display(Name = "Acceptance Receipt Number")]
        public string AcceptanceReceiptNumber { get; set; }

        [Display(Name = "School Fees Invoice Number")]
        public string SchoolFeesInvoiceNumber { get; set; }

        [Display(Name = "School Fees Receipt Number")]
        public string SchoolFeesReceiptNumber { get; set; }

        public void GetApplicationBy(long formId)
        {
            try
            {
                ApplicationForm = applicationFormLogic.GetBy(formId);
                if(ApplicationForm != null && ApplicationForm.Id > 0)
                {
                    AppliedCourse = appliedCourseLogic.GetBy(ApplicationForm);
                    admissionList = admissionListLogic.GetBy(ApplicationForm.Id);
                    Payment = paymentLogic.GetBy(ApplicationForm.Payment.Id);
                    Applicant = applicantLogic.GetByFormId(ApplicationForm.Id);
                    IsAdmitted = admissionListLogic.IsAdmitted(ApplicationForm);

                    if(Applicant != null && Applicant.Status != null)
                    {
                        ApplicantStatusId = Applicant.Status.Id;
                    }

                    //get acceptance payment
                    var acceptanceFee = new FeeType { Id = (int)FeeTypes.AcceptanceFee };
                    Payment acceptancePayment = paymentLogic.GetBy(ApplicationForm.Person,acceptanceFee);
                    if(acceptancePayment != null)
                    {
                        AcceptanceInvoiceNumber = acceptancePayment.InvoiceNumber;
                        //PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetBy(acceptancePayment);
                        RemitaPayment remitaPayment= remitaPaymentLogic.GetBy(acceptancePayment.Id);
                        if(remitaPayment != null)
                        {
                            AcceptanceConfirmationOrderNumber = remitaPayment.RRR;
                            AcceptanceReceiptNumber = remitaPayment.Receipt_No;
                        }
                    }

                    //get school fees payment
                    var schoolFees = new FeeType { Id = (int)FeeTypes.SchoolFees };
                    Payment schoolFeesPayment = paymentLogic.GetBy(ApplicationForm.Person,schoolFees);
                    if(schoolFeesPayment != null)
                    {
                        SchoolFeesInvoiceNumber = schoolFeesPayment.InvoiceNumber;
                        RemitaPayment remitaPayment = remitaPaymentLogic.GetBy(schoolFeesPayment.Id);
                        //PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetBy(schoolFeesPayment);
                        if(remitaPayment != null)
                        {
                            SchoolFeesConfirmationOrderNumber = remitaPayment.RRR;
                            SchoolFeesReceiptNumber = remitaPayment.Receipt_No;
                        }
                    }

                    Loaded = true;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }
        public PaymentMode PaymentMode { get; set; }
        public ApplicationForm GetApplicationFormBy(string formNumber)
        {
            try
            {
                return applicationFormLogic.GetModelBy(f => f.Application_Form_Number == formNumber);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ApplicationForm GetApplicationFormByJambNumber(string formNumber)
        {
            try
            {
                List<ApplicantJambDetail> applicantJambDetails = new  List<ApplicantJambDetail>();
                var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                applicantJambDetails = applicantJambDetailLogic.GetModelsBy(a => a.Applicant_Jamb_Registration_Number == formNumber && a.APPLICATION_FORM != null);
                foreach (ApplicantJambDetail applicantJambDetail in applicantJambDetails)
                {
                    if(applicantJambDetail != null && applicantJambDetail.ApplicationForm != null)
                    {
                        AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                        if (admissionListLogic.IsAdmitted(applicantJambDetail.ApplicationForm))
                        {
                             return applicationFormLogic.GetModelBy(f => f.Application_Form_Number == applicantJambDetail.ApplicationForm.Number);
                        }
                   
                    }
                }
               

                return new ApplicationForm();
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void GetInvoiceBy(string invoiceNumber)
        {
            try
            {
                Invoice = new Invoice();
                var paymentEtranzactType = new PaymentEtranzactType();
                var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                Payment payment = paymentLogic.GetBy(invoiceNumber);

                var studentLevel = new StudentLevel();
                var levelLogic = new StudentLevelLogic();
                studentLevel = levelLogic.GetBy(payment.Person.Id);
                Invoice.remitaPayment= remitaPaymentLogic.GetBy(payment.Id);
                if(studentLevel != null)
                {
                    payment.FeeDetails = paymentLogic.SetFeeDetails(payment,studentLevel.Programme.Id,
                        studentLevel.Level.Id,payment.PaymentMode.Id,studentLevel.Department.Id,payment.Session.Id);
                    paymentEtranzactType =
                        paymentEtranzactTypeLogic.GetModelsBy(
                            m =>
                                m.Level_Id == studentLevel.Level.Id && m.Programme_Id == studentLevel.Programme.Id &&
                                m.Payment_Mode_Id == payment.PaymentMode.Id && m.Session_Id == payment.Session.Id &&
                                m.Fee_Type_Id == payment.FeeType.Id && m.Session_Id == payment.Session.Id)
                            .FirstOrDefault();
                    Invoice.paymentEtranzactType = paymentEtranzactType;
                    Invoice.Department = studentLevel.Department.Name;
                    Invoice.Level = studentLevel.Level.Name;
                    Invoice.ProgrammeName = studentLevel.Programme != null ? studentLevel.Programme.Name : "";
                    Invoice.Session = payment.Session != null ? payment.Session.Name : "";
                }
                else
                {
                    var list = new AdmissionList();
                    var listLogic = new AdmissionListLogic();
                    list = listLogic.GetBy(payment.Person);
                    if(list != null)
                    {
                        var level = new Level();
                        level = SetLevel(list.Form.ProgrammeFee.Programme);
                        payment.FeeDetails = paymentLogic.SetFeeDetails(payment,list.Programme.Id,
                            level.Id,payment.PaymentMode.Id,list.Deprtment.Id,payment.Session.Id);
                        paymentEtranzactType =
                            paymentEtranzactTypeLogic.GetModelsBy(
                                m =>
                                    m.Level_Id == level.Id && m.Programme_Id == list.Programme.Id &&
                                    m.Payment_Mode_Id == payment.PaymentMode.Id && m.Session_Id == payment.Session.Id &&
                                    m.Fee_Type_Id == payment.FeeType.Id && m.Session_Id == payment.Session.Id)
                                .FirstOrDefault();
                        Invoice.paymentEtranzactType = paymentEtranzactType;
                        Invoice.Department = list.Deprtment.Name;
                        Invoice.Level = "100 Level";
                        Invoice.ProgrammeName = list.Programme != null ? list.Programme.Name : "";
                        Invoice.Session = payment.Session != null ? payment.Session.Name : "";
                    }
                }

                Invoice.Payment = payment;
                Invoice.Session = payment.Session.Name;
                Invoice.Person = payment.Person;
                Invoice.JambRegistrationNumber = "";
                AcceptanceInvoiceNumber = payment.InvoiceNumber;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private Level SetLevel(Programme programme)
        {
            try
            {
                Level level;
                switch(programme.Id)
                {
                    case 1:
                    {
                        return level = new Level { Id = 1 };
                    }
                    case 2:
                    {
                        return level = new Level { Id = 1 };
                    }
                    case 3:
                    {
                        return level = new Level { Id = 1 };
                    }
                    case 4:
                    {
                        return level = new Level { Id = 1 };
                    }
                    default:
                    return level = new Level { Id = 1 };
                }
                return level = new Level();
            }
            catch(Exception)
            {
                throw;
            }
        }

        public Receipt GetReceiptBy(string invoiceNumber)
        {
            try
            {
                Payment payment = paymentLogic.GetBy(invoiceNumber);
                if(payment == null || payment.Id <= 0)
                {
                    return null;
                }

                RemitaPayment remitaPayment = remitaPaymentLogic.GetModelBy(o => o.Payment_Id == payment.Id);
                if(remitaPayment != null && remitaPayment.Status!=null && remitaPayment.Description != null && (remitaPayment.Status.Contains("01") || remitaPayment.Description.Contains("manual")) )
                {
                    //if(payment.FeeDetails == null || payment.FeeDetails.Count <= 0)
                    //{
                    //    throw new Exception("Fee Details for " + payment.FeeType.Name +
                    //                        " not set! please contact your system administrator.");
                    //}

                    StudentLogic studentLogic = new StudentLogic();
                    Model.Model.Student student = studentLogic.GetBy(remitaPayment.payment.Person.Id);
                    var MatricNumber = "";
                    var ApplicationFormNumber = "";
                    var department = "";
                    var programme = "";
                    var faculty = "";
                    if(student != null)
                    {
                        MatricNumber = student.MatricNumber;
                        ApplicationFormNumber = student.ApplicationForm.Number;
                        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                        StudentLevel studentLevel = studentLevelLogic.GetBy(student.Id);
                        if (studentLevel != null)
                        {
                            department = studentLevel.Department.Name;
                            faculty = studentLevel.Department.Faculty.Name;
                            programme = studentLevel.Programme.Name;
                        }
                        else
                        {
                            AdmissionListLogic admissionLogic = new AdmissionListLogic();
                            AdmissionList admissionList = admissionLogic.GetBy(remitaPayment.payment.Person);
                            if (admissionList != null)
                            {
                                department = admissionList.Deprtment.Name;
                                programme = admissionList.Programme.Name;
                                faculty = admissionList.Deprtment.Faculty.Name;
                            }
                        }
                    }
                    else
                    {
                        AdmissionListLogic appliedCourseLogic = new AdmissionListLogic();
                        AdmissionList admissionList = appliedCourseLogic.GetBy(remitaPayment.payment.Person);
                        if (admissionList != null)
                        {
                            department = admissionList.Deprtment.Name;
                            programme = admissionList.Programme.Name;
                            faculty = admissionList.Deprtment.Faculty.Name;
                        }
                    }
                    var amount = (decimal)remitaPayment.TransactionAmount;
                    Receipt = BuildReceipt(payment.Person.FullName, payment.InvoiceNumber, remitaPayment, amount, payment.FeeType.Name, MatricNumber, ApplicationFormNumber, payment.Session.Name, "100 Level", department,programme,faculty);
                }

                return Receipt;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<Receipt> GetReceiptByAsync(string invoiceNumber)
        {
            try
            {
                Payment payment = paymentLogic.GetBy(invoiceNumber);
                if (payment == null || payment.Id <= 0)
                {
                    return null;
                }

                PaymentEtranzact paymentEtranzact = await paymentEtranzactLogic.GetModelByAsync(o => o.Payment_Id == payment.Id);
                if (paymentEtranzact != null)
                {
                    //if (payment.FeeDetails == null || payment.FeeDetails.Count <= 0)
                    //{
                    //    throw new Exception("Fee Details for " + payment.FeeType.Name +
                    //                        " not set! please contact your system administrator.");
                    //}

                    StudentLogic studentLogic = new StudentLogic();
                    Model.Model.Student student = await studentLogic.GetByAsync(paymentEtranzact.Payment.Payment.Person.Id);
                    var MatricNumber = "";
                    var ApplicationFormNumber = "";
                    var department = "";
                    var programme = "";
                    var faculty = "";
                    if (student != null)
                    {
                        MatricNumber = student.MatricNumber;
                        ApplicationFormNumber = student.ApplicationForm.Number;
                        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                        StudentLevel studentLevel = studentLevelLogic.GetBy(student.Id);
                        if (studentLevel != null)
                        {
                            department = studentLevel.Department.Name;
                            faculty = studentLevel.Department.Faculty.Name;
                            programme = studentLevel.Programme.Name;
                        }
                        else
                        {
                            AdmissionListLogic admissionLogic = new AdmissionListLogic();
                            AdmissionList admissionList = admissionLogic.GetBy(paymentEtranzact.Payment.Payment.Person);
                            if (admissionList != null)
                            {
                                department = admissionList.Deprtment.Name;
                                programme = admissionList.Programme.Name;
                                faculty = admissionList.Deprtment.Faculty.Name;
                            }
                        }
                    }
                    else
                    {
                        AdmissionListLogic appliedCourseLogic = new AdmissionListLogic();
                        AdmissionList admissionList = appliedCourseLogic.GetBy(paymentEtranzact.Payment.Payment.Person);
                        if (admissionList != null)
                        {
                            department = admissionList.Deprtment.Name;
                            programme = admissionList.Programme.Name;
                            faculty = admissionList.Deprtment.Faculty.Name;
                        }
                    }
                    var amount = (decimal)paymentEtranzact.TransactionAmount;
                    Receipt = BuildReceipt(payment.Person.FullName, payment.InvoiceNumber, paymentEtranzact, amount, payment.FeeType.Name, MatricNumber, ApplicationFormNumber, payment.Session.Name, "100 Level",department,programme,faculty);
                }

                return Receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public Payment GenerateInvoice(ApplicationForm applicationForm,ApplicantStatus.Status status,FeeType feeType,PaymentMode paymentMode)
        {
            try
            {
                var payment = new Payment();
                payment = paymentLogic.GetBy(applicationForm.Person, feeType);
                if (payment != null)
                {
                    return payment;
                }

                payment = new Payment();
                payment.PaymentMode = paymentMode;
                payment.PaymentType = new PaymentType { Id = applicationForm.Setting.PaymentType.Id };
                payment.PersonType = new PersonType { Id = applicationForm.Setting.PersonType.Id };
                payment.Person = applicationForm.Person;
                payment.DatePaid = DateTime.Now;
                payment.FeeType = feeType;
                payment.Session = applicationForm.Setting.Session;
                
                //Check if student is in admission list 
                var list = new AdmissionList();
                var listLogic = new AdmissionListLogic();
                list = listLogic.GetBy(applicationForm.Id);
                if (list != null && list.Id > 0)
                {
                    if (list.Programme.Id == (int)Programmes.Undergraduate && (list.Deprtment.Id == (int)Departments.Medicine || list.Deprtment.Id == (int)Departments.Nursing))
                    {
                        payment.PaymentMode = new PaymentMode() { Id = (int)PaymentModes.FullInstallment };
                       // return null;
                    }
                }
                            
                Payment newPayment = null;

               

                OnlinePayment newOnlinePayment = null;
                using(var transaction = new TransactionScope())
                {
                    newPayment = paymentLogic.Create(payment);
                    if(newPayment != null)
                    {
                        if (feeType.Id == (int)FeeTypes.SchoolFees)
                        {
                           
                            int LevelId = GetLevel(applicationForm.ProgrammeFee.Programme.Id);
                            newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment,applicationForm.ProgrammeFee.Programme.Id,LevelId,payment.PaymentMode.Id,list.Deprtment.Id,applicationForm.Setting.Session.Id);
                        }
                        else if (feeType.Id == (int)FeeTypes.AcceptanceFee && applicationForm.ProgrammeFee.Programme.Id > 1)
                        {
                            feeType.Id = (int)FeeTypes.AcceptanceFee;
                            newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, applicationForm.ProgrammeFee.Programme.Id, 1, payment.PaymentMode.Id, list.Deprtment.Id, applicationForm.Setting.Session.Id);


                            //string MonnifySUBAccount = ConfigurationManager.AppSettings["MonnifySUBAccount"].ToString();
                            //string MonnifyURL = ConfigurationManager.AppSettings["MonnifyUrl"].ToString();
                            //string MonnifyUser = ConfigurationManager.AppSettings["MonnifyApiKey"].ToString();
                            //string MonnifySecrect = ConfigurationManager.AppSettings["MonnifyContractCode"].ToString();
                            //string MonnifyCode = ConfigurationManager.AppSettings["MonnifyCode"].ToString();

                            ////Split used here is school account. So substract commision from total fees thats what is split amount
                            //decimal Total = newPayment.FeeDetails.Sum(x => x.Fee.Amount) + 150;
                            //decimal SchoolCut = Total - 1350;

                            //MonnifySplit split = new MonnifySplit { splitAmount = SchoolCut.ToString(), subAccountCode = MonnifySUBAccount };
                            //payment.InvoiceNumber = newPayment.InvoiceNumber;
                            //payment.FeeType.Name = "Acceptance Fee";
                            //PaymentMonnifyLogic paymentMonnifyLogic = new PaymentMonnifyLogic(MonnifyURL, MonnifyUser, MonnifySecrect, MonnifyCode);
                            //paymentMonnifyLogic.GenerateInvoice(payment, Total.ToString(), DateTime.Now.AddDays(7), new List<MonnifySplit> { split });

                        }

                        var channel = new PaymentChannel { Id = (int)PaymentChannel.Channels.Etranzact };
                        var onlinePaymentLogic = new OnlinePaymentLogic();
                        var onlinePayment = new OnlinePayment();
                        onlinePayment.Channel = channel;
                        onlinePayment.Payment = newPayment;
                        newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                    }

                    applicantLogic.UpdateStatus(applicationForm,status);
                    newPayment.Session = payment.Session;
                    newPayment.Person = payment.Person;
                    transaction.Complete();
                }

                return newPayment;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Payment> GenerateInvoiceAsync(ApplicationForm applicationForm, ApplicantStatus.Status status, FeeType feeType, PaymentMode paymentMode)
        {
            try
            {
                var payment = new Payment();
                payment.PaymentMode = paymentMode;
                payment.PaymentType = new PaymentType { Id = applicationForm.Setting.PaymentType.Id };
                payment.PersonType = new PersonType { Id = applicationForm.Setting.PersonType.Id };
                payment.Person = applicationForm.Person;
                payment.DatePaid = DateTime.Now;
                payment.FeeType = feeType;
                payment.Session = applicationForm.Setting.Session;

                Payment existingPayment = await paymentLogic.GetByAsync(payment.FeeType, payment.Person, payment.PaymentMode, payment.Session);

                if (await paymentLogic.PaymentAlreadyMadeAsync(payment))
                {
                    return paymentLogic.GetBy(applicationForm.Person, feeType);
                }
                //if (existingPayment != null && existingPayment.Id > 0)
                //{
                //    return paymentLogic.GetBy(applicationForm.Person, feeType);
                //}

                //Check if student is in admission list 
                var list = new AdmissionList();
                var listLogic = new AdmissionListLogic();
                list = listLogic.GetBy(applicationForm.Id);
                if (list != null && list.Id > 0)
                {
                    if (list.Programme.Id == (int)Programmes.Undergraduate && (list.Deprtment.Id == (int)Departments.Medicine || list.Deprtment.Id == (int)Departments.Nursing))
                    {
                        payment.PaymentMode = new PaymentMode() { Id = (int)PaymentModes.FullInstallment };
                        // return null;
                    }
                }

                Payment newPayment = null;
                OnlinePayment newOnlinePayment = null;
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                  Payment[]  paymentTask = await Task.WhenAll(paymentLogic.CreateAsync(payment));
                  newPayment = paymentTask.FirstOrDefault();
                  // newPayment = paymentTask.Result;

                    if (newPayment != null)
                    {
                        if (feeType.Id == 3)
                        {
                            int LevelId = GetLevel(applicationForm.ProgrammeFee.Programme.Id);
                            newPayment.FeeDetails = await paymentLogic.SetFeeDetailsListAsync(newPayment, applicationForm.ProgrammeFee.Programme.Id, LevelId, payment.PaymentMode.Id, list.Deprtment.Id, applicationForm.Setting.Session.Id);
                        
                        }
                        else if (feeType.Id == (int)FeeTypes.AcceptanceFee && applicationForm.ProgrammeFee.Programme.Id > 1)
                        {
                            feeType.Id = (int)FeeTypes.AcceptanceFee;
                            //newPayment.FeeDetails = await paymentLogic.SetFeeDetailsList(feeType);
                        }

                        var channel = new PaymentChannel { Id = (int)PaymentChannel.Channels.Etranzact };
                        var onlinePaymentLogic = new OnlinePaymentLogic();
                        var onlinePayment = new OnlinePayment();
                        onlinePayment.Channel = channel;
                        onlinePayment.Payment = newPayment;

                        

                        onlinePaymentLogic.CreateAsync(onlinePayment);
                    }

                    applicantLogic.UpdateStatus(applicationForm, status);
                    newPayment.Session = payment.Session;
                    newPayment.Person = payment.Person;
                    transaction.Complete();
                }

                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Receipt GenerateReceipt(string invoiceNumber,long formId,ApplicantStatus.Status status)
        {
            try
            {
                Payment payment = paymentLogic.GetBy(invoiceNumber);
                if(payment == null || payment.Id <= 0)
                {
                    return null;
                }

                Receipt receipt = null;
                ApplicationForm applicationForm = applicationFormLogic.GetBy(formId);
                if(applicationForm != null && applicationForm.Id > 0)
                {
                    RemitaPayment remitaPayment = remitaPaymentLogic.GetBy(payment.Id);
                    if(remitaPayment != null && (remitaPayment.Status.Contains("01") || remitaPayment.Description.Contains("manual")))
                    {
                        using(var transaction = new TransactionScope())
                        {
                            bool updated = onlinePaymentLogic.UpdateTransactionNumber(payment, remitaPayment.ConfirmationNo);
                            applicantLogic.UpdateStatus(applicationForm,status);
                            transaction.Complete();
                        }
                        StudentLogic studentLogic = new StudentLogic();
                        Model.Model.Student student = studentLogic.GetBy(remitaPayment.payment.Person.Id);
                        var MatricNumber = "";
                        var ApplicationFormNumber = "";
                        var department = "";
                        var programme = "";
                        var faculty = "";

                        if(student != null)
                        {
                            MatricNumber = student.MatricNumber;
                            ApplicationFormNumber = student.ApplicationForm.Number;
                            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                            StudentLevel studentLevel = studentLevelLogic.GetBy(student.Id);
                            if (studentLevel != null)
                            {
                                department = studentLevel.Department.Name;
                                faculty = studentLevel.Department.Faculty.Name;
                                programme = studentLevel.Programme.Name;
                            }
                            else
                            {
                                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                                AdmissionList admissionList = admissionListLogic.GetBy(remitaPayment.payment.Person);
                                if (admissionList != null)
                                {
                                    department = admissionList.Deprtment.Name;
                                    programme = admissionList.Programme.Name;
                                    faculty = admissionList.Deprtment.Faculty.Name;
                                }
                            }
                        }
                        else
                        {
                            AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                            AdmissionList admissionList = admissionListLogic.GetBy(remitaPayment.payment.Person);
                            if (admissionList != null)
                            {
                                department = admissionList.Deprtment.Name;
                                programme = admissionList.Programme.Name;
                                faculty = admissionList.Deprtment.Faculty.Name;
                            }
                        }
                        decimal amount = (decimal)remitaPayment.TransactionAmount;
                        receipt = BuildReceipt(applicationForm.Person.FullName, invoiceNumber, remitaPayment, amount, payment.FeeType.Name, MatricNumber, applicationForm.Number, payment.Session.Name, "100 Level", department,programme,faculty);
                    }
                }

                return receipt;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Receipt> GenerateReceiptAsync(string invoiceNumber, long formId, ApplicantStatus.Status status)
        {
            try
            {
                Payment payment = await  paymentLogic.GetByAsync(invoiceNumber);
                if (payment == null || payment.Id <= 0)
                {
                    return null;
                }
                string department = "";
                string faculty = "";
                string programme = "";

                Receipt receipt = null;
                ApplicationForm applicationForm = applicationFormLogic.GetBy(formId);
                if (applicationForm != null && applicationForm.Id > 0)
                {
                    PaymentEtranzact paymentEtranzact = await paymentEtranzactLogic.GetByAsync(payment);
                    if (paymentEtranzact != null)
                    {
                        using (var transaction = new TransactionScope())
                        {
                            bool updated = onlinePaymentLogic.UpdateTransactionNumber(payment, paymentEtranzact.ConfirmationNo);
                            applicantLogic.UpdateStatus(applicationForm, status);
                            transaction.Complete();
                        }
                        StudentLogic studentLogic = new StudentLogic();
                        Model.Model.Student student = await studentLogic.GetByAsync(paymentEtranzact.Payment.Payment.Person.Id);
                        var MatricNumber = "";
                        var ApplicationFormNumber = "";
                        if (student != null)
                        {
                            MatricNumber = student.MatricNumber;
                            ApplicationFormNumber = student.ApplicationForm.Number;
                            StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                            StudentLevel studentLevel = studentLevelLogic.GetBy(student.Id);
                            if (studentLevel != null)
                            {
                                department = studentLevel.Department.Name;
                                faculty = studentLevel.Department.Faculty.Name;
                                programme = studentLevel.Programme.Name;
                            }
                            else
                            {
                                AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                                AdmissionList admissionList = admissionListLogic.GetBy(paymentEtranzact.Payment.Payment.Person);
                                if (admissionList != null)
                                {
                                    department = admissionList.Deprtment.Name;
                                    programme = admissionList.Programme.Name;
                                    faculty = admissionList.Deprtment.Faculty.Name;
                                }
                            }
                        }
                        else
                        {
                            AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                            AdmissionList admissionList = admissionListLogic.GetBy(paymentEtranzact.Payment.Payment.Person);
                            if (admissionList != null)
                            {
                                department = admissionList.Deprtment.Name;
                                programme = admissionList.Programme.Name;
                                faculty = admissionList.Deprtment.Faculty.Name;
                            }
                        }
                        decimal amount = (decimal)paymentEtranzact.TransactionAmount;
                        receipt = BuildReceipt(applicationForm.Person.FullName, invoiceNumber, paymentEtranzact, amount, payment.FeeType.Name, MatricNumber, applicationForm.Number, payment.Session.Name, "100 Level", department,programme,faculty);
                    }
                }

                return receipt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Receipt BuildReceipt(string name,string invoiceNumber,PaymentEtranzact paymentEtranzact,decimal amount,string purpose,string MatricNumber,string ApplicationFormNumber,string session, string level,string department,string programme,string faculty)
        {
            try
            {
                var receipt = new Receipt();
                receipt.Number = paymentEtranzact.ReceiptNo;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = paymentEtranzact.ConfirmationNo;
                receipt.Amount = amount;
                receipt.AmountInWords = NumberToWords((int) amount);
                receipt.Purpose = purpose;
                receipt.Date = paymentEtranzact.TransactionDate.Value;
                receipt.ApplicationFormNumber = ApplicationFormNumber;
                receipt.MatricNumber = MatricNumber;
                receipt.QRVerification = "http://portal.abiastateuniversity.edu.ng//Common/Credential/Receipt?pmid=" + paymentEtranzact.Payment.Payment.Id;
                receipt.Session = session;
                receipt.Level = level;
                receipt.ReceiptNumber = paymentEtranzact.Payment.Payment.Id.ToString();
                receipt.Department = department;
                receipt.Programme = programme;
                receipt.Faculty = faculty;
                receipt.PaymentMode = paymentEtranzact.Payment.Payment.PaymentMode.Name;
                receipt.PaymentId = Utility.Encrypt(paymentEtranzact.Payment.Payment.Id.ToString()); 
                return receipt;
            }
            catch(Exception)
            {
                throw;
            }
        }
        public Receipt BuildReceipt(string name, string invoiceNumber, RemitaPayment remitaPayment, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber, string session, string level, string department, string programme, string faculty)
        {
            try
            {
                var receipt = new Receipt();
                receipt.Number = remitaPayment.Receipt_No;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = remitaPayment.RRR;
                receipt.Amount = amount;
                receipt.AmountInWords = NumberToWords((int)amount);
                receipt.Purpose = purpose;
                receipt.Date = remitaPayment.TransactionDate;
                receipt.ApplicationFormNumber = ApplicationFormNumber;
                receipt.MatricNumber = MatricNumber;
                receipt.QRVerification = "http://192.169.152.37/Common/Credential/Receipt?pmid=" + remitaPayment.payment.Id;
                receipt.Session = session;
                receipt.Level = level;
                receipt.ReceiptNumber = remitaPayment.payment.Id.ToString();
                receipt.Department = department;
                receipt.Programme = programme;
                receipt.Faculty = faculty;
                receipt.PaymentMode = remitaPayment.payment.PaymentMode.Name;
                receipt.PaymentId = Utility.Encrypt(remitaPayment.payment.Id.ToString());
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Int32 GetLevel(int ProgrammeId)
        {
            try
            {
                //set mode of study
                switch(ProgrammeId)
                {
                    case 1:
                    {
                        return 1;
                    }
                    case 2:
                    {
                        return 1;
                    }
                    case 3:
                    {
                        return 1;
                    }
                    case 4:
                    {
                        return 1;
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
            return 1;
        }
        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
    }
}