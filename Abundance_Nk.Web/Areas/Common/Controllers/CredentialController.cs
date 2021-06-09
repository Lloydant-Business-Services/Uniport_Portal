using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Common.Controllers
{
    [AllowAnonymous]
    public class CredentialController : BaseController
    {
        string QRVerificationUrl = ConfigurationManager.AppSettings["QRVerificationUrl"].ToString();
        public ActionResult ApplicationForm(long fid)
        {
            try
            {
                var applicationFormViewModel = new ApplicationFormViewModel();
                var form = new ApplicationForm { Id = fid };

                applicationFormViewModel.GetApplicationFormBy(form);
                if (applicationFormViewModel.Person != null && applicationFormViewModel.Person.Id > 0)
                {
                    applicationFormViewModel.SetApplicantAppliedCourse(applicationFormViewModel.Person);
                }

                return View(applicationFormViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult StudentForm(string fid)
        {
            try
            {
                Int64 formId = Convert.ToInt64(Utility.Decrypt(fid));

                var form = new ApplicationForm { Id = formId };
                var studentFormViewModel = new StudentFormViewModel();

                studentFormViewModel.LoadApplicantionFormBy(formId);
                if (studentFormViewModel.ApplicationForm.Person != null &&
                    studentFormViewModel.ApplicationForm.Person.Id > 0)
                {
                    studentFormViewModel.LoadStudentInformationFormBy(studentFormViewModel.ApplicationForm.Person.Id);
                }

                return View(studentFormViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult PostUtmeResult(string jn)
        {
            PostUtmeResult result = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(jn))
                {
                    var postUtmeResultLogic = new PostUtmeResultLogic();
                    result = postUtmeResultLogic.GetModelBy(m => m.REGNO == jn);
                    if (result == null || result.Id <= 0)
                    {
                        //SetMessage("Registration Number / Jamb No was not found! Please check that you have typed in the correct detail", Message.Category.Error);
                        return View(result);
                    }
                    result.Fullname = result.Fullname;
                    result.Regno = result.Regno;
                    result.Eng = result.Eng;
                    result.Sub2 = result.Sub2;
                    result.Sub3 = result.Sub3;
                    result.Sub4 = result.Sub4;
                    result.Scr2 = result.Scr2;
                    result.Scr3 = result.Scr3;
                    result.Scr4 = result.Scr4;
                    result.Total = result.Total;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Operation failed! " + ex.Message, Message.Category.Error);
            }

            return View(result);
        }

        public ActionResult Receipt(long pmid)
        {
            Receipt receipt = null;

            try
            {
                receipt = GetReceiptBy(pmid);
                if (receipt == null)
                {
                    SetMessage("No receipt found!", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(receipt);
        }

        public Receipt GetReceiptBy(long pmid)
        {
            Receipt receipt = null;
            var paymentLogic = new PaymentLogic();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            StudentLevel studentLevel = new StudentLevel();
            try
            {
                Payment payment = paymentLogic.GetBy(pmid);
                if (payment == null || payment.Id <= 0)
                {
                    return null;
                }
                RemitaPayment remitaPayment = remitaPaymentLogic.GetModelBy(o => o.Payment_Id == payment.Id);


                if (remitaPayment?.payment?.Id>0 && (remitaPayment.Status.Contains("01") || remitaPayment.Description.Contains("manual")))
                {
                    var matricNumber = "";
                    var level = "";
                    var department = "";
                    var programme = "";
                    var faculty = "";
                    StudentLogic studentLogic = new StudentLogic();

                    Model.Model.Student student = studentLogic.GetBy(payment.Person.Id);

                    if (student != null)
                    {
                        matricNumber = student.MatricNumber;
                        StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                        StudentPayment studentPayment = studentPaymentLogic.GetModelBy(a => a.Payment_Id == pmid);
                        if (studentPayment != null)
                        {
                            level = studentPayment.Level.Name;
                        }
                        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                        studentLevel = studentLevelLogic.GetBy(student.Id);
                        if (studentLevel != null)
                        {
                            department = studentLevel.Department.Name;
                            programme = studentLevel.Programme.Name;
                            faculty = studentLevel.Department.Faculty.Name;
                            level = level == "" ? studentLevel.Level.Name : level;
                        }
                    }

                    var amount = (decimal)remitaPayment.TransactionAmount;
                    receipt = BuildReceipt(payment.Person.FullName, payment.InvoiceNumber, remitaPayment, amount, payment.FeeType.Name, matricNumber, "", payment.Session.Name, level, department, programme, faculty);
                }
                
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Receipt BuildReceipt(string name, string invoiceNumber, RemitaPayment remitaPayment, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber, string session, string level, string department, string programme, string faculty)
        {
            try
            {
                var receipt = new Receipt();

                ShortFallLogic shortFallLogic = new ShortFallLogic();
                ShortFall shortFall = shortFallLogic.GetModelsBy(s => s.PAYMENT.Invoice_Number == invoiceNumber).LastOrDefault();
                if (shortFall != null && !string.IsNullOrEmpty(shortFall.Description))
                {
                    receipt.Description = shortFall.Description;
                }

                receipt.Number = remitaPayment.Receipt_No;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = remitaPayment.RRR;
                receipt.Amount = amount;
                receipt.AmountInWords = NumberToWords((int)amount);
                receipt.Purpose = purpose;
                receipt.PaymentMode = remitaPayment.payment.PaymentMode.Name;
                receipt.Date = (DateTime)remitaPayment.TransactionDate;
                receipt.QRVerification = QRVerificationUrl + remitaPayment.payment.Id;
                receipt.MatricNumber = MatricNumber;
                receipt.Session = session;
                receipt.Level = level;
                receipt.ReceiptNumber = remitaPayment.payment.SerialNumber.ToString();
                receipt.Department = department;
                receipt.Programme = programme;
                receipt.Faculty = faculty;
                receipt.PaymentId = Utility.Encrypt(remitaPayment.payment.Id.ToString());
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Receipt BuildReceipt(string name, string invoiceNumber, Paystack paystack, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber, string session, string level, string department, string programme, string faculty)
        {
            try
            {
                var receipt = new Receipt();
                receipt.Number = paystack.reference;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = paystack.reference;
                receipt.Amount = amount;
                receipt.AmountInWords = NumberToWords((int)amount);
                receipt.Purpose = purpose;
                receipt.PaymentMode = paystack.Payment.PaymentMode.Name;
                receipt.Date = (DateTime)paystack.transaction_date;
                receipt.QRVerification = QRVerificationUrl + paystack.Payment.Id;
                receipt.MatricNumber = MatricNumber;
                receipt.Session = session;
                receipt.Level = level;
                receipt.ReceiptNumber = paystack.Payment.SerialNumber.ToString();
                receipt.Department = department;
                receipt.Programme = programme;
                receipt.Faculty = faculty;
                receipt.PaymentId = paystack.Payment.Id.ToString();
                receipt.PaymentId = Utility.Encrypt(paystack.Payment.Id.ToString());
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Receipt BuildReceipt(string name, string invoiceNumber, PaymentMonnify paymentMonnify, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber, string session, string level, string department, string programme, string faculty)
        {
            try
            {
                var receipt = new Receipt();
                receipt.Number = paymentMonnify.TransactionReference;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = paymentMonnify.AccountName;
                receipt.Amount = amount;
                receipt.AmountInWords = NumberToWords((int)amount);
                receipt.Purpose = purpose;
                receipt.PaymentMode = paymentMonnify.Payment.PaymentMode.Name;
                receipt.Date = (DateTime)paymentMonnify.CompletedOn;
                receipt.QRVerification = QRVerificationUrl + paymentMonnify.Payment.Id;
                receipt.MatricNumber = MatricNumber;
                receipt.Session = session;
                receipt.Level = level;
                receipt.ReceiptNumber = paymentMonnify.Payment.SerialNumber.ToString();
                receipt.Department = department;
                receipt.Programme = programme;
                receipt.Faculty = faculty;
                receipt.PaymentId = paymentMonnify.Payment.Id.ToString();
                receipt.PaymentId = Utility.Encrypt(paymentMonnify.Payment.Id.ToString());
                receipt.IsMonnify = true;
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Receipt BuildReceipt(string name, string invoiceNumber, PaymentInterswitch paymentInterswitch, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber, string session, string level, string department, string programme, string faculty)
        {
            try
            {
                var receipt = new Receipt();
                receipt.Number = paymentInterswitch.RetrievalReferenceNumber;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = paymentInterswitch.MerchantReference;
                receipt.Amount = amount;
                receipt.AmountInWords = NumberToWords((int)amount);
                receipt.Purpose = purpose;
                receipt.PaymentMode = paymentInterswitch.Payment.PaymentMode.Name;
                receipt.Date = (DateTime)paymentInterswitch.TransactionDate;
                receipt.QRVerification = QRVerificationUrl + paymentInterswitch.Payment.Id;
                receipt.MatricNumber = MatricNumber;
                receipt.Session = session;
                receipt.Level = level;
                receipt.ReceiptNumber = paymentInterswitch.Payment.SerialNumber.ToString();
                receipt.Department = department;
                receipt.Programme = programme;
                receipt.Faculty = faculty;
                receipt.PaymentId = paymentInterswitch.Payment.Id.ToString();
                receipt.PaymentId = Utility.Encrypt(paymentInterswitch.Payment.Id.ToString());
                receipt.IsInterswitchPayment = true;
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult AdmissionLetter(string fid)
        {
            AdmissionLetter admissionLetter = null;
            Int64 formId = Convert.ToInt64(Utility.Decrypt(fid));
            try
            {
                admissionLetter = GetAdmissionLetterBy(formId);
                if (admissionLetter.Programme.Id == (int)Programmes.FullTimeMasters || admissionLetter.Programme.Id == (int)Programmes.FullTimePGD || admissionLetter.Programme.Id == (int)Programmes.FullTimePHD || admissionLetter.Programme.Id == (int)Programmes.PartMasters || admissionLetter.Programme.Id == (int)Programmes.PartTimePGD || admissionLetter.Programme.Id == (int)Programmes.PartTimePHD)
                {
                    return View("AdmissionLetterPG", admissionLetter);
                }
                if (admissionLetter.Programme.Id == (int)Programmes.JUPEB)
                {
                    return View("AdmissionLetterJUPEB", admissionLetter);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(admissionLetter);
        }

        public ActionResult PrintAcceptanceLetter(string fid, string pmid)
        {
            AdmissionLetter admissionLetter = null;
            Int64 formId = Convert.ToInt64(Utility.Decrypt(fid));
            string paymentId = Utility.Decrypt(pmid);
            var applicationFormLogic = new ApplicationFormLogic();
            ApplicationForm applicationForm = applicationFormLogic.GetBy(formId);

            try
            {
                if (!ValidatePayment(paymentId, applicationForm.Person.Id.ToString()))
                {
                    admissionLetter = GetAcceptanceLetterBy(formId);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(admissionLetter);
        }

        public ActionResult AcceptanceLetter(string fid, string pmid)
        {
            AdmissionLetter admissionLetter = null;
            Int64 formId = Convert.ToInt64(Utility.Decrypt(fid));
            string paymentId = Utility.Decrypt(pmid);
            var applicationFormLogic = new ApplicationFormLogic();
            ApplicationForm applicationForm = applicationFormLogic.GetBy(formId);

            try
            {
                if (ValidatePayment(paymentId, applicationForm.Person.Id.ToString()))
                {
                    admissionLetter = GetAcceptanceLetterBy(formId);
                    if (admissionLetter.Programme.Id == (int)Programmes.FullTimeMasters || admissionLetter.Programme.Id == (int)Programmes.FullTimePGD || admissionLetter.Programme.Id == (int)Programmes.FullTimePHD || admissionLetter.Programme.Id == (int)Programmes.PartMasters || admissionLetter.Programme.Id == (int)Programmes.PartTimePGD || admissionLetter.Programme.Id == (int)Programmes.PartTimePHD)
                    {
                        return View("AcceptanceLetterPG", admissionLetter);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View("AcceptanceLetter", admissionLetter);
        }

        public AdmissionLetter GetAcceptanceLetterBy(long formId)
        {
            try
            {
                AdmissionLetter admissionLetter = null;
                var applicationFormLogic = new ApplicationFormLogic();
                var admissionListLogic = new AdmissionListLogic();
                ApplicationForm applicationForm = applicationFormLogic.GetBy(formId);
                var admissionList = new AdmissionList();
                admissionList = admissionListLogic.GetBy(formId);
                if (applicationForm != null && applicationForm.Id > 0)
                {
                    var remitaPaymentLogic = new RemitaPaymentLogic();
                    var remitaPayment = new RemitaPayment();
                    remitaPayment =
                        remitaPaymentLogic.GetModelBy(
                            p =>
                                p.PAYMENT.Person_Id == applicationForm.Person.Id &&
                                p.PAYMENT.FEE_TYPE.Fee_Type_Id == (int)FeeTypes.AcceptanceFee);


                    var degreeAwardedLogic = new DegreeAwardedLogic();
                    DegreeAwarded degreeAwarded = degreeAwardedLogic.GetBy(admissionList.Deprtment, admissionList.Programme);
                    if (degreeAwarded == null)
                    {
                        degreeAwarded = new DegreeAwarded();
                        degreeAwarded.Degree = "Degree";
                    }

                    admissionLetter = new AdmissionLetter();
                    admissionLetter.applicationform = applicationForm;
                    admissionLetter.Person = applicationForm.Person;
                    admissionLetter.Session = applicationForm.Setting.Session;
                    admissionLetter.Department = admissionList.Deprtment;
                    admissionLetter.Programme = admissionList.Programme;
                    admissionLetter.RegistrationEndDate = applicationForm.Setting.RegistrationEndDate;
                    admissionLetter.RegistrationEndTime = applicationForm.Setting.RegistrationEndTime;
                    admissionLetter.RegistrationEndTimeString = applicationForm.Setting.RegistrationEndTimeString;
                    admissionLetter.DegreeAwarded = degreeAwarded;

                    if (remitaPayment != null)
                    {
                        admissionLetter.AcceptancePin = remitaPayment.RRR;
                        admissionLetter.AcceptanceAmount = remitaPayment.TransactionAmount.ToString();
                    }

                    if (admissionLetter.Session == null || admissionLetter.Session.Id <= 0)
                    {
                        throw new Exception(
                            "Session not set for this admission period! Please contact your system administrator.");
                    }
                    if (!admissionLetter.RegistrationEndDate.HasValue)
                    {
                        throw new Exception(
                            "Registration End Date not set for this admission period! Please contact your system administrator.");
                    }
                    if (!admissionLetter.RegistrationEndTime.HasValue)
                    {
                        throw new Exception(
                            "Registration End Time not set for this admission period! Please contact your system administrator.");
                    }
                }

                return admissionLetter;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ValidatePayment(string pmid, string pid)
        {
            var paymentLogic = new PaymentLogic();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();

            try
            {
                Payment payment = paymentLogic.GetBy(pmid);
                if (payment == null || payment.Id <= 0)
                {
                    return false;
                }

                RemitaPayment remitaPayment = remitaPaymentLogic.GetModelBy(o => o.Payment_Id == payment.Id);
                if (remitaPayment != null && (remitaPayment.Status.Contains("01") || remitaPayment.Description.Contains("manual")))
                {


                    if (payment.FeeType.Id != (int)FeeTypes.AcceptanceFee)
                    {
                        return false;
                    }
                    if (payment.Person.Id.ToString() != pid)
                    {
                        return false;
                    }
                    return true;
                }


                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
            return false;
        }

        public AdmissionLetter GetAdmissionLetterBy(long formId)
        {
            try
            {
                AdmissionLetter admissionLetter = null;
                var applicationFormLogic = new ApplicationFormLogic();

                ApplicationForm applicationForm = applicationFormLogic.GetBy(formId);


                if (applicationForm != null && applicationForm.Id > 0)
                {
                    var list = new AdmissionList();
                    var listLogic = new AdmissionListLogic();
                    list = listLogic.GetBy(applicationForm.Id);


                    var degreeAwardedLogic = new DegreeAwardedLogic();
                    DegreeAwarded degreeAwarded = degreeAwardedLogic.GetBy(list.Deprtment, list.Programme);
                    if (degreeAwarded == null)
                    {
                        degreeAwarded = new DegreeAwarded();
                        degreeAwarded.Degree = "Degree";
                    }

                    //var feeDetailLogic = new FeeDetailLogic();
                    //List<FeeDetail> feeDetails =
                    //    feeDetailLogic.GetModelsBy(f => f.Fee_Type_Id == (int) FeeTypes.SchoolFees);

                    var appliedCourseLogic = new AppliedCourseLogic();
                    AppliedCourse appliedCourse =
                        appliedCourseLogic.GetModelBy(a => a.Person_Id == applicationForm.Person.Id);
                    if (appliedCourse == null)
                    {
                        throw new Exception(
                            "Applicant Applied Course cannot be found! Please contact your system administrator.");
                    }


                    admissionLetter = new AdmissionLetter();
                    admissionLetter.Person = applicationForm.Person;
                    admissionLetter.Session = applicationForm.Setting.Session;
                    //admissionLetter.FeeDetails = feeDetails;
                    admissionLetter.Programme = list.Programme;
                    admissionLetter.Department = list.Deprtment;
                    admissionLetter.RegistrationEndDate = applicationForm.Setting.RegistrationEndDate;
                    admissionLetter.RegistrationEndTime = applicationForm.Setting.RegistrationEndTime;
                    admissionLetter.RegistrationEndTimeString = applicationForm.Setting.RegistrationEndTimeString;
                    admissionLetter.ProgrammeType = list.Programme.Description;
                    admissionLetter.DegreeAwarded = degreeAwarded;
                    admissionLetter.ApplicationNumber = applicationForm.Number;


                    if (admissionLetter.Session == null || admissionLetter.Session.Id <= 0)
                    {
                        throw new Exception(
                            "Session not set for this admission period! Please contact your system administrator.");
                    }
                    if (!admissionLetter.RegistrationEndDate.HasValue)
                    {
                        throw new Exception(
                            "Registration End Date not set for this admission period! Please contact your system administrator.");
                    }
                    if (!admissionLetter.RegistrationEndTime.HasValue)
                    {
                        throw new Exception(
                            "Registration End Time not set for this admission period! Please contact your system administrator.");
                    }

                    var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                    ApplicantJambDetail applicantJambDetail =
                        applicantJambDetailLogic.GetModelBy(a => a.Application_Form_Id == applicationForm.Id);
                    if (applicantJambDetail != null)
                    {
                        admissionLetter.JambNumber = applicantJambDetail.JambRegistrationNumber;
                    }

                    admissionLetter.QRVerification = "http://portal.abiastateuniversity.edu.ng/Common/Credential/AdmissionLetter?fid=" + Utility.Encrypt(formId.ToString());
                }

                return admissionLetter;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult AdmissionSlip(string fid)
        {
            AdmissionLetter admissionLetter = null;
            Int64 formId = Convert.ToInt64(Utility.Decrypt(fid));
            try
            {
                admissionLetter = GetAdmissionLetterBy(formId);
            }
            catch (Exception)
            {
                throw;
            }

            return View(admissionLetter);
        }

        public ActionResult FinancialClearanceSlip(string pid)
        {
            try
            {
                Int64 paymentid = Convert.ToInt64(Utility.Decrypt(pid));
                var studentLogic = new StudentLogic();
                Model.Model.Student student = studentLogic.GetBy(paymentid);


                var paymentLogic = new PaymentLogic();
                var paymentHistory = new PaymentHistory();
                paymentHistory.Payments = paymentLogic.GetBy(student);
                paymentHistory.Student = student;

                return View(paymentHistory);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Invoice(string pmid)
        {
            try
            {
                Int64 paymentid = Convert.ToInt64(Utility.Decrypt(pmid));
                var paymentLogic = new PaymentLogic();
                PaystackLogic paystackLogic = new PaystackLogic();
                Payment payment = paymentLogic.GetBy(paymentid);


                var invoice = new Invoice();
                invoice.Person = payment.Person;
                invoice.Payment = payment;
                invoice.Session = payment.Session.Name;

                invoice.Paystack = paystackLogic.GetBy(payment);

                var student = new Model.Model.Student();
                var studentLogic = new StudentLogic();
                student = studentLogic.GetBy(payment.Person.Id);

                var paymentEtranzactLogic = new PaymentEtranzactLogic();
                var paymentEtranzactType = new PaymentEtranzactType();
                var PaymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();


                var studentLevel = new StudentLevel();
                var levelLogic = new StudentLevelLogic();

                var studentPayment = new StudentPayment();
                var studentPaymentLogic = new StudentPaymentLogic();
                studentPayment = studentPaymentLogic.GetModelBy(a => a.Payment_Id == payment.Id);

                studentLevel = levelLogic.GetBy(student.MatricNumber);
                if (studentLevel != null && studentPayment != null)
                {
                    invoice.Level = studentLevel.Level.Name;
                    invoice.Department = studentLevel.Department.Name;
                    invoice.MatricNumber = student.MatricNumber;
                    payment.FeeDetails = paymentLogic.SetFeeDetails(payment, studentLevel.Programme.Id,
                        studentPayment.Level.Id, payment.PaymentMode.Id, studentLevel.Department.Id, payment.Session.Id);
                    paymentEtranzactType =
                        PaymentEtranzactTypeLogic.GetModelsBy(
                            p =>
                                p.Level_Id == studentPayment.Level.Id && p.Payment_Mode_Id == payment.PaymentMode.Id &&
                                p.Fee_Type_Id == payment.FeeType.Id && p.Programme_Id == studentLevel.Programme.Id &&
                                p.Session_Id == payment.Session.Id).FirstOrDefault();
                    invoice.paymentEtranzactType = paymentEtranzactType;
                }
                else
                {
                    var list = new AdmissionList();
                    var listLogic = new AdmissionListLogic();
                    list = listLogic.GetBy(payment.Person);
                    if (list != null)
                    {
                        var level = new Level();
                        level = SetLevel(list.Form.ProgrammeFee.Programme);
                        payment.FeeDetails = paymentLogic.SetFeeDetails(payment, list.Programme.Id, level.Id, payment.PaymentMode.Id, list.Deprtment.Id, list.Form.Setting.Session.Id);
                        invoice.Level = studentLevel.Level.Name;
                        invoice.Department = studentLevel.Department.Name;
                    }
                }


                string MonnifyURL = ConfigurationManager.AppSettings["MonnifyUrl"].ToString();
                string MonnifyUser = ConfigurationManager.AppSettings["MonnifyApiKey"].ToString();
                string MonnifySecrect = ConfigurationManager.AppSettings["MonnifyContractCode"].ToString();
                string MonnifyCode = "";
                //if (payment.FeeType.Id == 81)
                //{
                //     MonnifyCode = ConfigurationManager.AppSettings["MonnifyMatricGownCode"].ToString();
                //}
                //else
                //{
                    MonnifyCode = ConfigurationManager.AppSettings["MonnifyCode"].ToString();
                //}
                
                PaymentMonnifyLogic paymentMonnifyLogic = new PaymentMonnifyLogic(MonnifyURL,MonnifyUser, MonnifySecrect,MonnifyCode);
                invoice.PaymentMonnify = paymentMonnifyLogic.GetBy(payment.InvoiceNumber);

                int[] couriereFeeTypes = {(int)FeeTypes.IntlTranscriptRequestZone1, (int)FeeTypes.IntlTranscriptRequestZone2, (int)FeeTypes.IntlTranscriptRequestZone3,
                                               (int)FeeTypes.IntlTranscriptRequestZone4, (int)FeeTypes.IntlTranscriptRequestZone5,(int)FeeTypes.IntlTranscriptRequestZone6,
                                               (int)FeeTypes.IntlTranscriptRequestZone7, (int)FeeTypes.IntlTranscriptRequestZone8,(int)FeeTypes.CourierServiceDomesticZone1,
                                               (int)FeeTypes.CourierServiceDomesticZone2,(int)FeeTypes.CourierServiceDomesticZone3,(int)FeeTypes.CourierServiceDomesticZone4,
                                               (int)FeeTypes.CourierServiceDomesticZone5,(int)FeeTypes.CourierServiceDomesticZone6,(int)FeeTypes.CourierServiceDomesticZone7,
                                               (int)FeeTypes.CourierServiceDomesticZone8 };


                // Interswitch Payment for Transcript
                if (payment.FeeType.Id.Equals((int)FeeTypes.Transcript) || payment.FeeType.Id.Equals((int)FeeTypes.TranscriptCertificateVerification)
                    || payment.FeeType.Id.Equals((int)FeeTypes.CertificateVerification) || couriereFeeTypes.Contains(payment.FeeType.Id))
                {
                    PaymentInterswitchLogic paymentInterswitchLogic = new PaymentInterswitchLogic();
                    TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                    TranscriptRequest transcriptRequest = transcriptRequestLogic.GetBy(invoice.Payment);
                    if (transcriptRequest != null)
                    {
                        var failedTransction = paymentInterswitchLogic.GetBy(payment.Id);

                        if (failedTransction != null && failedTransction.ResponseCode != "00" && failedTransction.TransactionDate.Date == DateTime.Now.Date)
                        {
                            SetMessage("Error Occured! Previous Payment Could not be completed. REASON:" + " " + failedTransction.ResponseDescription + " " + "TRANSACTION REFERENCE: " + failedTransction.MerchantReference, Message.Category.Error);
                        }
                        invoice.PaymentInterswitch = paymentInterswitchLogic.GetBy(payment, transcriptRequest);
                    }
                }
                PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetBy(payment);
                if (paymentEtranzact != null)
                {
                    invoice.Paid = true;
                }
                return View(invoice);
            }
            catch (Exception ex)
            {
                throw;
            }
            return View();
        }

        public ActionResult ShortFallInvoice(string pmid, string amount)
        {
            try
            {
                int paymentid = Convert.ToInt32(pmid);
                var paymentLogic = new PaymentLogic();
                ShortFallLogic shortFallLogic = new ShortFallLogic();

                Payment payment = paymentLogic.GetBy(paymentid);
                ShortFall shortFall = shortFallLogic.GetModelBy(s => s.Payment_Id == paymentid);


                if ((payment.FeeType.Id == (int)FeeTypes.ShortFall))
                {
                    if (TempData["FeeDetail"] != null)
                    {
                        payment.FeeDetails = (List<FeeDetail>)TempData["FeeDetail"];
                    }
                    var invoice = new Invoice();
                    invoice.Person = payment.Person;
                    invoice.Payment = payment;

                    if (shortFall != null && !string.IsNullOrEmpty(shortFall.Description))
                    {
                        invoice.Description = shortFall.Description;
                    }

                    var paymentMode = new PaymentMode { Id = 1 };
                    var paymentEtranzact = new PaymentEtranzact();
                    var paymentEtranzactLogic = new PaymentEtranzactLogic();
                    paymentEtranzact = paymentEtranzactLogic.GetModelBy(p => p.Payment_Id == payment.Id);
                    var paymentEtranzactType = new PaymentEtranzactType();
                    var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();


                    paymentEtranzactType = paymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == payment.FeeType.Id).FirstOrDefault();
                    invoice.paymentEtranzactType = paymentEtranzactType;


                    invoice.Amount = Convert.ToDecimal(amount);

                    return View(invoice);
                }
                var oldPerson = new Person();
                oldPerson = (Person)TempData["OldPerson"];
                var viewModel = new PostJambViewModel();
                var appliedCourse = new AppliedCourse();
                var applicationForm = new ApplicationForm();
                var applicationFormLogic = new ApplicationFormLogic();
                var appliedCourseLogic = new AppliedCourseLogic();
                var session = new Session();
                var sessionLogic = new SessionLogic();
                var applicationFormSetting = new ApplicationFormSetting();
                var applicantJambDetail = new ApplicantJambDetail();
                var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                applicantJambDetail = applicantJambDetailLogic.GetModelBy(p => p.Person_Id == oldPerson.Id);
                var applicationFormSettingLogic = new ApplicationFormSettingLogic();
                applicationFormSetting = applicationFormSettingLogic.GetModelBy(p => p.Application_Form_Setting_Id == 1);
                session = sessionLogic.GetModelBy(p => p.Activated == true);
                appliedCourse = appliedCourseLogic.GetModelBy(p => p.Person_Id == payment.Person.Id);
                applicationForm = applicationFormLogic.GetModelBy(p => p.Person_Id == payment.Person.Id);
                viewModel.AppliedCourse = appliedCourse;
                viewModel.Person = payment.Person;
                viewModel.Session = session;
                viewModel.Programme = appliedCourse.Programme;
                viewModel.ApplicationFormNumber = applicationForm.Number;
                viewModel.ApplicationFormSetting = applicationFormSetting;
                viewModel.ApplicantJambDetail = applicantJambDetail;
                viewModel.ApplicationForm = applicationForm;
                TempData["viewModel"] = viewModel;
                return RedirectToAction("PostJAMBSlip", new { controller = "Form", area = "Applicant" });
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Level SetLevel(Programme programme)
        {
            try
            {
                Level level;
                switch (programme.Id)
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
                            return level = new Level { Id = 3 };
                        }
                    case 4:
                        {
                            return level = new Level { Id = 3 };
                        }
                }
                return level = new Level();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult TranscriptInvoice(string pmid)
        {
            try
            {
                Int64 paymentid = Convert.ToInt64(Utility.Decrypt(pmid));
                var paymentLogic = new PaymentLogic();
                Payment payment = paymentLogic.GetBy(paymentid);

                var invoice = new Invoice();
                invoice.Person = payment.Person;
                invoice.Payment = payment;

                var remitaPayment = new RemitaPayment();
                var remitaPaymentLogic = new RemitaPaymentLogic();
                remitaPayment = remitaPaymentLogic.GetBy(payment.Id);
                if (remitaPayment != null)
                {
                    invoice.remitaPayment = remitaPayment;
                    invoice.Amount = remitaPayment.TransactionAmount;
                }

                return View(invoice);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult CardPayment()
        {
            var viewModel = (TranscriptViewModel)TempData["TranscriptViewModel"];
            TempData.Keep("TranscriptViewModel");

            return View(viewModel);
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
        public int GetStudentPreviousLevelBy(int currentsessionId, int previoussessionId, int currentLeveId)
        {
            int i = 0;
            try
            {
                SessionLogic sessionLogic = new SessionLogic();
                var currentSession = sessionLogic.GetModelBy(c => c.Session_Id == currentsessionId);
                var previousSession = sessionLogic.GetModelBy(c => c.Session_Id == previoussessionId);
                if (currentSession != null && previousSession != null)
                {
                    var currentSessionFirstSplit = Convert.ToInt32((currentSession.Name.Split('/'))[0]);
                    var previousSessionFirstSplit = Convert.ToInt32((previousSession.Name.Split('/'))[0]);
                    var differenceInSession = currentSessionFirstSplit - previousSessionFirstSplit;
                    i = currentLeveId - differenceInSession;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return i;
        }

    }
}