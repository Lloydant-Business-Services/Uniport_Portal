using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Applicant.Controllers
{
    [AllowAnonymous]
    public class FormController : BaseController
    {
        private const string ID = "Id";
        private const string NAME = "Name";
        private const string VALUE = "Value";
        private const string TEXT = "Text";
        private const string DEFAULT_PASSPORT = "/Content/Images/default_avatar.png";
        private const string FIRST_SITTING = "FIRST SITTING";
        private const string SECOND_SITTING = "SECOND SITTING";
        private readonly string appRoot = ConfigurationManager.AppSettings["AppRoot"];

        //string ContractCode = ConfigurationManager.AppSettings["MonnifyContractCode"].ToString();
        //string api = ConfigurationManager.AppSettings["MonnifyApiKey"].ToString();
        private PaymentLogic paymentLogic;
        private PersonLogic personLogic;
        private FeeTypeLogic feeTypeLogic;
        private StudentPaymentLogic studentPaymentLogic;
        private JambRecordLogic JambRecordLogic;
        //private PaymentMonnifyLogic paymentMonnifyLogic;
        public FormController()
        {
            paymentLogic = new PaymentLogic();
            personLogic = new PersonLogic();
            feeTypeLogic = new FeeTypeLogic();
            studentPaymentLogic = new StudentPaymentLogic();
            JambRecordLogic = new JambRecordLogic();
            //paymentMonnifyLogic = new PaymentMonnifyLogic();
        }

        private PostJambViewModel viewModel;
        public ActionResult PostJambProgramme()
        {
            var viewModel = new PostJAMBProgrammeViewModel();

            try
            {
                TempData["viewModel"] = null;
                TempData["imageUrl"] = null;
                TempData["ProgrammeViewModel"] = null;
                TempData["PostJAMBFormPaymentViewModel"] = null;
                TempData.Clear();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult PostJambProgramme(PostJAMBProgrammeViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    //live code -- to uncomment later
                    Payment payment = InvalidConfirmationOrderNumber(viewModel.ConfirmationOrderNumber);
                    
                    var appliedCourseLogic = new AppliedCourseLogic();
                    AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(m => m.Person_Id == payment.Person.Id);

                    var supplementaryCourseLogic = new SupplementaryCourseLogic();
                    SupplementaryCourse supplementaryCourse = supplementaryCourseLogic.GetModelBy(m => m.Person_Id == payment.Person.Id);


                    var studentJambDetailLogic = new ApplicantJambDetailLogic();
                    ApplicantJambDetail studentJambDetail = studentJambDetailLogic.GetModelBy(m => m.Person_Id == payment.Person.Id);

                    var jambRecordLogic = new JambRecordLogic();
                    var EnglishLanguage = new OLevelSubject { Id = 1 };
                    JambRecord jambRecord = null;
                    if (studentJambDetail != null)
                    {
                        studentJambDetail.Subject1 = EnglishLanguage;
                        jambRecord = jambRecordLogic.GetModelBy(jr => jr.Jamb_Registration_Number == studentJambDetail.JambRegistrationNumber);
                    }

                    if (jambRecord != null)
                    {
                        var firstChoiceInstitution = new InstitutionChoice { Id = 1 };

                        studentJambDetail.JambScore = jambRecord.TotalJambScore;
                        if (!String.IsNullOrEmpty(jambRecord.Subject2))
                            studentJambDetail.Subject2 = new OLevelSubject { Id = Convert.ToInt32(jambRecord.Subject2) };
                        if (!String.IsNullOrEmpty(jambRecord.Subject3))
                            studentJambDetail.Subject3 = new OLevelSubject { Id = Convert.ToInt32(jambRecord.Subject3) };
                        if (!String.IsNullOrEmpty(jambRecord.Subject1))
                            studentJambDetail.Subject4 = new OLevelSubject { Id = Convert.ToInt32(jambRecord.Subject1) };
                        if (jambRecord.FirstChoiceInstitution == "NAU")
                        {
                            studentJambDetail.InstitutionChoice = firstChoiceInstitution;
                        }

                        studentJambDetailLogic.Modify(studentJambDetail);
                        //Copy student passport from jambdetail
                        string destinationPath = null;
                        string jambPassportFilePath = null;
                        if (jambRecord.ImageFileUrl != null)
                        {
                            var existingImageUrl = payment.Person.ImageFileUrl;
                            if (!string.IsNullOrEmpty(payment.Person.ImageFileUrl))

                                DeleteFileIfExist(payment.Person.ImageFileUrl);

                            SetJambPersonPassportDestination(payment.Person, jambRecord, out jambPassportFilePath, out destinationPath);

                            SaveJambPersonPassport(jambPassportFilePath, destinationPath, payment.Person);
                            if (!string.IsNullOrEmpty(existingImageUrl))
                            {

                                personLogic.Modify(payment.Person);
                            }

                        }
                    }


                    var postJAMBFormPaymentViewModel = new PostJAMBFormPaymentViewModel();
                    //enable display of submit button in the form
                    if (payment.Person.ImageFileUrl != null)
                        postJAMBFormPaymentViewModel.HasJambPassport = 1;

                    if (studentJambDetail != null)
                    {
                        postJAMBFormPaymentViewModel.JambRegistrationNumber = studentJambDetail.JambRegistrationNumber;
                        postJAMBFormPaymentViewModel.ApplicantJambDetail = studentJambDetail;
                    }

                    if (supplementaryCourse != null)
                    {
                        postJAMBFormPaymentViewModel.SupplementaryCourse = supplementaryCourse;
                    }

                    postJAMBFormPaymentViewModel.Payment = payment;
                    if (appliedCourse != null && (appliedCourse.Programme.Id == 1) )
                    {
                        postJAMBFormPaymentViewModel.Programme = appliedCourse.Programme;
                        postJAMBFormPaymentViewModel.AppliedCourse = appliedCourse;
                        postJAMBFormPaymentViewModel.Person = payment.Person;
                        postJAMBFormPaymentViewModel.Programme = appliedCourse.Programme;
                        postJAMBFormPaymentViewModel.Initialise();
                        if (appliedCourse.Setting != null && appliedCourse.Setting.Id > 0)
                        {
                            postJAMBFormPaymentViewModel.ApplicationFormSetting = appliedCourse.Setting;
                        }
                    }
                    else
                    {
                        SetMessage(
                            "The Confirmation Order Number entered does not have a corresponding applied course! Please contact your system administrator",
                            Message.Category.Error);
                        return View(viewModel);
                    }

                    TempData["PostJAMBFormPaymentViewModel"] = postJAMBFormPaymentViewModel;
                    return RedirectToAction("PostJambForm", "Form");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }

            return View(viewModel);
        }

        private Payment InvalidConfirmationOrderNumber(string confirmationOrderNumber)
        {
            var payment = new Payment();
            var etranzactLogic = new PaymentEtranzactLogic();
            PaystackLogic paystackLogic = new PaystackLogic();
            PaymentEtranzact etranzactDetails = new PaymentEtranzact();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            payment=remitaPaymentLogic.GetModelsBy(f => f.RRR == confirmationOrderNumber.Trim() &&( f.Status.Contains("01") || f.Description.Contains("manual"))).Select(f=>f.payment).FirstOrDefault();
            if (payment?.Id > 0)
                return payment;

            if (confirmationOrderNumber.Contains("ABSU"))
            {
                if (paystackLogic.ValidatePayment(confirmationOrderNumber) != null)
                {
                    return payment;
                }
                if (ReturnMonnfyPayment(confirmationOrderNumber) != null)
                {
                    return payment;
                }


                //return null;
            }
            etranzactDetails = etranzactLogic.GetModelBy(m => m.Confirmation_No == confirmationOrderNumber);
            if (etranzactDetails == null || etranzactDetails.ReceiptNo == null)
            {
                var session = new Session { Id = 20 };
                var paymentTerminal = new PaymentTerminal();
                var paymentTerminalLogic = new PaymentTerminalLogic();
                paymentTerminal = paymentTerminalLogic.GetModelBy(p => p.Fee_Type_Id == 1 && p.Session_Id == session.Id);

                etranzactDetails = etranzactLogic.RetrievePinAlternative(confirmationOrderNumber, paymentTerminal);
                if (etranzactDetails != null && etranzactDetails.ReceiptNo != null)
                {
                    var paymentLogic = new PaymentLogic();
                    payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                    if (payment != null && payment.Id > 0)
                    {

                        AppliedCourse appliedCourse = new AppliedCourse();
                        AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                        appliedCourse = appliedCourseLogic.GetBy(payment.Person);
                        if (appliedCourse != null)
                        {
                            var feeDetailLogic = new FeeDetailLogic();
                            decimal feeDetail = feeDetailLogic.GetFeeByDepartmentLevel(appliedCourse.Department, new Level() { Id = 1 }, appliedCourse.Programme, payment.FeeType, payment.Session, payment.PaymentMode);
                            if (payment.Session.Id == session.Id)
                            {
                                if (!etranzactLogic.ValidatePin(etranzactDetails, payment, feeDetail))
                                {
                                    SetMessage(
                                        "The pin amount tied to the pin is not correct. Please contact support@lloydant.com.",
                                        Message.Category.Error);
                                    payment = null;
                                    return payment;
                                }
                            }
                        }

                        //PaymentLogic.SendDetailsToBIR(payment, etranzactDetails, "FORM");
                    }
                    else
                    {
                        SetMessage(
                            "The invoice number attached to the pin doesn't belong to you! Please cross check and try again.",
                            Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage(
                        "Confirmation Order Number entered seems not to be valid! Please cross check and try again.",
                        Message.Category.Error);
                }
            }
            else
            {
                var session = new Session { Id = 20 };
                var paymentLogic = new PaymentLogic();
                payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                if (payment != null && payment.Id > 0)
                {
                    AppliedCourse appliedCourse = new AppliedCourse();
                    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    appliedCourse = appliedCourseLogic.GetBy(payment.Person);
                    if (appliedCourse != null)
                    {
                        var feeDetailLogic = new FeeDetailLogic();
                        decimal feeDetail = feeDetailLogic.GetFeeByDepartmentLevel(appliedCourse.Department, new Level() { Id = 1 }, appliedCourse.Programme, payment.FeeType, payment.Session, payment.PaymentMode);
                        if (payment.Session.Id == session.Id)
                        {
                            if (!etranzactLogic.ValidatePin(etranzactDetails, payment, feeDetail))
                            {
                                SetMessage(
                                    "The pin amount tied to the pin is not correct. Please contact support@lloydant.com.",
                                    Message.Category.Error);
                                payment = null;
                                return payment;
                            }
                        }
                        //PaymentLogic.SendDetailsToBIR(payment, etranzactDetails, "FORM");
                    }
                }
                else
                {
                    SetMessage(
                        "The invoice number attached to the pin doesn't belong to you! Please cross check and try again.",
                        Message.Category.Error);
                }
            }

            return payment;
        }

        private void CreateStudentJambDetail(string jambRegNo, Person person)
        {
            try
            {
                if (string.IsNullOrEmpty(jambRegNo))
                {
                    return;
                }

                var jambDetail = new ApplicantJambDetail();
                jambDetail.JambRegistrationNumber = jambRegNo;
                jambDetail.Person = person;
                var studentJambDetailLogic = new ApplicantJambDetailLogic();
                studentJambDetailLogic.Create(jambDetail);
            }
            catch (Exception ex)
            {
                Bugsnag.AspNet.Client.Current.Notify(ex);
                throw ex;
            }
        }

        private bool InvalidJambRegistrationNumber(PostJAMBFormPaymentViewModel viewModel)
        {
            const int TEN = 10;
            const string SIX = "2";

            try
            {
                if (string.IsNullOrEmpty(viewModel.JambRegistrationNumber))
                {
                    SetMessage("Please enter your JAMB Registration Number!", Message.Category.Error);
                    return true;
                }
                if (viewModel.JambRegistrationNumber.Length != TEN)
                {
                    SetMessage("JAMB Registration Number must be equal to " + TEN + " digits!", Message.Category.Error);
                    return true;
                }

                int lastAlphabet;
                int secondTolastAlphabet;
                int firstEightNumbers;

                string firstEightCharacters = viewModel.JambRegistrationNumber.Substring(0, 8);
                string lastCharacter = viewModel.JambRegistrationNumber.Substring(9, 1);
                string secondToLastCharacter = viewModel.JambRegistrationNumber.Substring(8, 1);
                string firstCharacter = viewModel.JambRegistrationNumber.Substring(0, 1);

                bool lastCharacterIsAlphabet = int.TryParse(lastCharacter, out lastAlphabet);
                bool secondToLastCharacterIsAlphabet = int.TryParse(secondToLastCharacter, out secondTolastAlphabet);
                bool firstEightCharacterIsNumber = int.TryParse(firstEightCharacters, out firstEightNumbers);

                if (firstCharacter != SIX)
                {
                    SetMessage("The JAMB Registration Number must start with a 6", Message.Category.Error);
                    return true;
                }

                if (firstEightCharacterIsNumber == false)
                {
                    SetMessage(
                        "The first eight characters of JAMB Registration Number must all be numbers! Please modify.",
                        Message.Category.Error);
                    return true;
                }

                if (secondToLastCharacterIsAlphabet && lastCharacterIsAlphabet)
                {
                    SetMessage("The last two characters of JAMB Registration Number must be alphabets! Please modify.",
                        Message.Category.Error);
                    return true;
                }
                if (lastCharacterIsAlphabet)
                {
                    SetMessage("The last character of JAMB Registration Number must be an alphabet! Please modify.",
                        Message.Category.Error);
                    return true;
                }
                if (secondToLastCharacterIsAlphabet)
                {
                    SetMessage(
                        "The second to the last character of JAMB Registration Number must be an alphabet! Please modify.",
                        Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Bugsnag.AspNet.Client.Current.Notify(ex);
                throw ex;
            }
        }

        public ActionResult PostJambFormInvoiceGeneration()
        {
            PostJAMBFormPaymentViewModel postJAMBFormPaymentViewModel = new PostJAMBFormPaymentViewModel();

            try
            {
                ViewBag.StateId = postJAMBFormPaymentViewModel.StateSelectList;
                ViewBag.ProgrammeId = postJAMBFormPaymentViewModel.ProgrammeSelectListItem;
                ViewBag.FormSettings = postJAMBFormPaymentViewModel.ApplicationProgrammeSettingSelectListItems;
                ViewBag.DepartmentId = new SelectList(new List<Department>().OrderBy(D => D.Name), ID, NAME);
                ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }

            TempData["PostJAMBFormPaymentViewModel"] = postJAMBFormPaymentViewModel;
            return View(postJAMBFormPaymentViewModel);
        }

        [HttpPost]
        public ActionResult PostJambFormInvoiceGeneration(PostJAMBFormPaymentViewModel viewModel)
        {
            try
            {
                ModelState.Remove("Person.DateOfBirth");
                ModelState.Remove("ApplicationFormSetting.Id");
                ModelState.Remove("AppliedCourse.Option.Id");

                var errors = from modelstate in ModelState.AsQueryable().Where(f => f.Value.Errors.Count > 0)
                             select new { Title = modelstate.Key };

                if (ModelState.IsValid)
                {
                    viewModel.Initialise();
                    if (viewModel.Programme.Id == 1 || viewModel.Programme.Id == 4)
                    {
                        if (IsJambNumberExist(viewModel.JambRegistrationNumber, viewModel.ApplicationFormSetting))
                        {
                            KeepApplicationFormInvoiceGenerationDropDownState(viewModel);
                            SetMessage("This Jamb Number has already been used", Message.Category.Error);
                            return View(viewModel);
                        }
                        if (InvalidJambRegistrationNumber(viewModel))
                        {
                            KeepApplicationFormInvoiceGenerationDropDownState(viewModel);
                            return View(viewModel);
                        }


                        if (InvalidFormSettingsSelection(viewModel))
                        {
                            KeepApplicationFormInvoiceGenerationDropDownState(viewModel);
                            return View(viewModel);
                        }

                        //SetMessage("Applications are closed!" , Message.Category.Error);
                        //return View(viewModel);
                    }

                    if (InvalidDepartmentSelection(viewModel))
                    {
                        KeepApplicationFormInvoiceGenerationDropDownState(viewModel);
                        return View(viewModel);
                    }

                    //using (TransactionScope transaction = new TransactionScope())
                    using (
                        var transaction = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    {

                        Person person = CreatePerson(viewModel);
                        Payment payment = CreatePayment(viewModel);
                        viewModel.Payment = new Payment
                        {
                            Id = payment.Id,
                            InvoiceNumber = payment.InvoiceNumber,
                            PaymentType = payment.PaymentType,
                            Person = person,
                            FeeDetails = payment.FeeDetails,
                            Session = viewModel.ApplicationFormSetting.Session
                        };
                        payment.Session = viewModel.ApplicationFormSetting.Session;
                        AppliedCourse appliedCourse = CreateAppliedCourse(viewModel);
                        CreateStudentJambDetail(viewModel.JambRegistrationNumber, person);

                        payment.FeeType = viewModel.ApplicationFormSetting.FeeType;
                        PaymentLogic paymentLogic = new PaymentLogic();
                        payment.FeeDetails = paymentLogic.SetFeeDetails(payment, viewModel.Programme.Id,
                        1, viewModel.ApplicationFormSetting.PaymentMode.Id, viewModel.AppliedCourse.Department.Id, viewModel.ApplicationFormSetting.Session.Id);
                        
                        transaction.Complete();
                    }

                    TempData["PostJAMBFormPaymentViewModel"] = viewModel;
                    return RedirectToAction("ProcessRemitaPayment", "Form",
                        new { Area = "Applicant" });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }

            KeepApplicationFormInvoiceGenerationDropDownState(viewModel);
            return View(viewModel);
        }
        public ActionResult ProcessRemitaPayment()
        {
            PostJAMBFormPaymentViewModel viewModel = (PostJAMBFormPaymentViewModel)TempData["PostJAMBFormPaymentViewModel"];
            try
            {
                if (viewModel != null)
                {
                    PaymentLogic paymentLogic = new PaymentLogic();
                    //get feedetail

                    viewModel.Payment = paymentLogic.GetBy(viewModel.Payment.Id);
                    viewModel.Payment.FeeDetails = paymentLogic.SetFeeDetails(viewModel.Payment, viewModel.Programme.Id,
                    1, viewModel.ApplicationFormSetting.PaymentMode.Id, viewModel.AppliedCourse.Department.Id, viewModel.ApplicationFormSetting.Session.Id);
                    Payment payment = viewModel.Payment;
                    if (payment != null)
                    {
                        //Get Payment Specific Setting
                        decimal Amt = 0;
                        Amt = payment.FeeDetails.Sum(p => p.Fee.Amount);
                        RemitaSettings settings = new RemitaSettings();
                        RemitaSettingsLogic settingsLogic = new RemitaSettingsLogic();
                        settings = settingsLogic.GetBy(1);

                        Remita remita = new Remita()
                        {
                            merchantId = settings.MarchantId,
                            serviceTypeId = settings.serviceTypeId,
                            orderId = payment.InvoiceNumber.ToString(),
                            totalAmount = (decimal)Amt,
                            payerName = payment.Person.FullName,
                            payerEmail = !string.IsNullOrEmpty(payment.Person.Email) ? payment.Person.Email : "test@lloydant.com",
                            payerPhone = payment.Person.MobilePhone,
                            //responseurl = "http://192.169.152.37/Applicant/Form/RemitaResponse",
                            responseurl = ConfigurationManager.AppSettings["RemitaResponseUrl"].ToString(),
                            paymenttype = "RRRGEN",
                            amt = Amt.ToString()
                        };

                        remita.amt = remita.amt.Split('.')[0];

                        string hash_string = remita.merchantId + remita.serviceTypeId + remita.orderId + remita.amt + remita.responseurl + settings.Api_key;
                        System.Security.Cryptography.SHA512Managed sha512 = new System.Security.Cryptography.SHA512Managed();
                        Byte[] EncryptedSHA512 = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hash_string));
                        sha512.Clear();
                        string hashed = BitConverter.ToString(EncryptedSHA512).Replace("-", "").ToLower();

                        remita.hash = hashed;

                        viewModel.Remita = remita;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            TempData["PostJAMBFormPaymentViewModel"] = viewModel;

            return View(viewModel);
        }
        public ActionResult RemitaResponse(string orderID)
        {
            RemitaResponse remitaResponse = new RemitaResponse();
            PostJAMBFormPaymentViewModel viewModel = (PostJAMBFormPaymentViewModel)TempData["PostJAMBFormPaymentViewModel"];
            if (viewModel == null)
            {
                viewModel = new PostJAMBFormPaymentViewModel();
            }

            try
            {
                string merchant_id = "538661740";
                string apiKey = "918567";
                string hashed;
                string checkstatusurl = ConfigurationManager.AppSettings["RemitaVerifyUrl"].ToString();
                string url;

                if (Request.QueryString["orderID"] != null)
                {
                    PaymentLogic paymentLogic = new PaymentLogic();
                    RemitaPaymentLogic remitaLogic = new RemitaPaymentLogic();
                    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    ApplicationProgrammeFeeLogic programmeFeeLogic = new ApplicationProgrammeFeeLogic();
                    AppliedCourse appliedCourse = new AppliedCourse();
                    ApplicationProgrammeFee programmeFee = new ApplicationProgrammeFee();
                    RemitaPayment remitaPyament = new RemitaPayment();

                    orderID = Request.QueryString["orderID"].ToString();

                    Payment payment = paymentLogic.GetBy(orderID);
                    //paymentLogic.SetFeeDetails(payment);

                    if (payment != null)
                    {
                        remitaPyament = remitaLogic.GetModelBy(p => p.Payment_Id == payment.Id);
                        appliedCourse = appliedCourseLogic.GetModelsBy(a => a.Person_Id == payment.Person.Id).LastOrDefault();
                        if (appliedCourse != null)
                        {
                            programmeFee = programmeFeeLogic.GetModelBy(p => p.Fee_Type_Id == payment.FeeType.Id && p.Session_Id == payment.Session.Id && p.Programme_Id == appliedCourse.Programme.Id);
                        }
                    }
                    else
                    {
                        remitaPyament = null;
                    }

                    string hash_string = orderID + apiKey + merchant_id;
                    System.Security.Cryptography.SHA512Managed sha512 = new System.Security.Cryptography.SHA512Managed();
                    Byte[] EncryptedSHA512 = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hash_string));
                    sha512.Clear();
                    hashed = BitConverter.ToString(EncryptedSHA512).Replace("-", "").ToLower();
                    url = checkstatusurl + "/" + merchant_id + "/" + orderID + "/" + hashed + "/" + "orderstatus.reg";


                    //viewModel.Hash = hashed;

                    if (remitaPyament == null)
                    {
                        string jsondata = new WebClient().DownloadString(url);
                        remitaResponse = JsonConvert.DeserializeObject<RemitaResponse>(jsondata);
                    }

                    if (remitaResponse != null && remitaResponse.Status != null)
                    {
                        if (payment != null)
                        {
                            if (remitaPyament != null)
                            {
                                remitaPyament.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                                remitaLogic.Modify(remitaPyament);
                            }
                            else
                            {
                                remitaPyament = new RemitaPayment();
                                remitaPyament.payment = payment;
                                remitaPyament.RRR = remitaResponse.RRR;
                                remitaPyament.OrderId = remitaResponse.orderId;
                                remitaPyament.Status = remitaResponse.Status;
                                remitaPyament.TransactionAmount = remitaResponse.amount;
                                remitaPyament.TransactionDate = DateTime.Now;
                                remitaPyament.MerchantCode = merchant_id;
                                remitaPyament.Description = "APPLICATION FORM manual";
                                if (remitaLogic.GetBy(payment.Id) == null)
                                {
                                    remitaLogic.Create(remitaPyament);
                                }
                            }

                            viewModel.remitaPayment = remitaPyament;
                            viewModel.Payment = payment;
                            viewModel.Person = payment.Person;

                        }
                        else
                        {
                            RemitaPayment remitaPayment = remitaLogic.GetBy(orderID);
                            string remitaVerifyUrl = ConfigurationManager.AppSettings["RemitaVerifyUrl"].ToString();
                            RemitaPayementProcessor remitaProcessor = new RemitaPayementProcessor(apiKey);
                            remitaResponse = remitaProcessor.TransactionStatus(remitaVerifyUrl, remitaPayment);
                            if (remitaResponse != null && remitaResponse.Status != null)
                            {
                                remitaPayment.Status = remitaResponse.Status + ":" + remitaResponse.StatusCode;
                                remitaLogic.Modify(remitaPayment);

                                payment = paymentLogic.GetBy(remitaPayment.payment.Id);
                                viewModel.remitaPayment = remitaPayment;
                                if (payment != null)
                                {
                                    viewModel.Payment = payment;
                                    viewModel.Person = payment.Person;
                                }
                            }
                            else
                            {
                                SetMessage("Payment does not exist!", Message.Category.Error);
                            }

                            viewModel.remitaPayment = remitaPayment;
                        }

                        string hash = "538661740" + remitaResponse.RRR + "918567";
                        RemitaPayementProcessor myRemitaProcessor = new RemitaPayementProcessor(hash);
                        viewModel.Hash = myRemitaProcessor.HashPaymentDetailToSHA512(hash);
                    }
                    else
                    {
                        SetMessage("Order ID was not generated from this system", Message.Category.Error);
                    }

                    viewModel.Department = appliedCourse != null ? appliedCourse.Department : new Department();
                    viewModel.ApplicationProgrammeFee = programmeFee != null ? programmeFee : new ApplicationProgrammeFee();
                    viewModel.AppliedCourse = appliedCourse;
                }
                else
                {
                    SetMessage("No data was received!", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            TempData["PostJAMBFormPaymentViewModel"] = viewModel;
            return RedirectToAction("Invoice", "Form",
                        new { Area = "Applicant", ivn = viewModel.Payment.InvoiceNumber });
            //return View(viewModel);
        }
        private bool InvalidDepartmentSelection(PostJAMBFormPaymentViewModel viewModel)
        {
            try
            {
                if (viewModel.AppliedCourse.Department == null || viewModel.AppliedCourse.Department.Id <= 0)
                {
                    SetMessage("Please select Department!", Message.Category.Error);
                    return true;
                }


                return false;
            }
            catch (Exception ex)
            {
                Bugsnag.AspNet.Client.Current.Notify(ex);
                throw ex;
            }
        }

        private bool InvalidFormSettingsSelection(PostJAMBFormPaymentViewModel viewModel)
        {
            try
            {
                if (viewModel.ApplicationFormSetting == null || viewModel.ApplicationFormSetting.Id <= 0)
                {
                    SetMessage("Please select Form Type!", Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Bugsnag.AspNet.Client.Current.Notify(ex);
                throw ex;
            }
        }

        private void KeepApplicationFormInvoiceGenerationDropDownState(PostJAMBFormPaymentViewModel viewModel)
        {
            try
            {
                if (viewModel.Person.State != null && !string.IsNullOrEmpty(viewModel.Person.State.Id))
                {
                    ViewBag.StateId = new SelectList(viewModel.StateSelectList, VALUE, TEXT, viewModel.Person.State.Id);
                }
                else
                {
                    ViewBag.StateId = new SelectList(viewModel.StateSelectList, VALUE, TEXT);
                }

                if (viewModel.Programme != null && viewModel.Programme.Id > 0)
                {
                    viewModel.DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(viewModel.Programme);
                    ViewBag.ProgrammeId = new SelectList(viewModel.ProgrammeSelectListItem, VALUE, TEXT,
                        viewModel.Programme.Id);
                    ViewBag.FormSettings = new SelectList(viewModel.ApplicationProgrammeSettingSelectListItems, VALUE,
                        TEXT);

                    if (viewModel.AppliedCourse.Department != null && viewModel.AppliedCourse.Department.Id > 0)
                    {
                        viewModel.DepartmentOptionSelectListItem =
                            Utility.PopulateDepartmentOptionSelectListItem(viewModel.AppliedCourse.Department,
                                viewModel.Programme);

                        ViewBag.DepartmentId = new SelectList(viewModel.DepartmentSelectListItem, VALUE, TEXT,
                            viewModel.AppliedCourse.Department.Id);
                        if (viewModel.AppliedCourse.Option != null && viewModel.AppliedCourse.Option.Id > 0)
                        {
                            ViewBag.DepartmentOptionId = new SelectList(viewModel.DepartmentOptionSelectListItem, VALUE,
                                TEXT, viewModel.AppliedCourse.Option.Id);
                        }
                        else
                        {
                            if (viewModel.DepartmentOptionSelectListItem != null &&
                                viewModel.DepartmentOptionSelectListItem.Count > 0)
                            {
                                ViewBag.DepartmentOptionId = new SelectList(viewModel.DepartmentOptionSelectListItem,
                                    VALUE, TEXT);
                            }
                            else
                            {
                                ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);
                            }
                        }
                    }
                    else
                    {
                        ViewBag.DepartmentId = new SelectList(viewModel.DepartmentSelectListItem, VALUE, TEXT);
                        ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);
                    }
                }
                else
                {
                    ViewBag.ProgrammeId = new SelectList(viewModel.ProgrammeSelectListItem, VALUE, TEXT);
                    ViewBag.DepartmentId = new SelectList(new List<Department>(), ID, NAME);
                    ViewBag.DepartmentOptionId = new SelectList(new List<DepartmentOption>(), ID, NAME);
                    ViewBag.FormSettings = new SelectList(viewModel.ApplicationProgrammeSettingSelectListItems, VALUE,
                        TEXT);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Invoice(string ivn)
        {
            try
            {
                var invoice = new Invoice();
                var paymentEtranzactType = new PaymentEtranzactType();
                var paymentEtranzactTypeLogic = new PaymentEtranzactTypeLogic();
                var paymentLogic = new PaymentLogic();
                var PaystackLogic = new PaystackLogic();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                Payment payment = paymentLogic.GetBy(ivn);
                invoice.Paystack = PaystackLogic.GetBy(payment);
                invoice.remitaPayment = remitaPaymentLogic.GetBy(payment.Id);
                


                var studentLevel = new StudentLevel();
                var levelLogic = new StudentLevelLogic();
                studentLevel = levelLogic.GetBy(payment.Person.Id);
                if (studentLevel != null)
                {
                    payment.FeeDetails = paymentLogic.SetFeeDetails(payment, studentLevel.Programme.Id,
                        studentLevel.Level.Id, payment.PaymentMode.Id, studentLevel.Department.Id, payment.Session.Id);
                    paymentEtranzactType =
                        paymentEtranzactTypeLogic.GetModelsBy(
                            m =>
                                m.Level_Id == studentLevel.Level.Id && m.Programme_Id == studentLevel.Programme.Id &&
                                m.Payment_Mode_Id == payment.PaymentMode.Id && m.Session_Id == payment.Session.Id &&
                                m.Fee_Type_Id == payment.FeeType.Id && m.Session_Id == payment.Session.Id)
                            .FirstOrDefault();
                    invoice.paymentEtranzactType = paymentEtranzactType;
                    invoice.Level = studentLevel.Level.Name;
                    invoice.Department = studentLevel.Department.Name;
                }
                else
                {
                    var appliedCourse = new AppliedCourse();
                    var appliedCourseLogic = new AppliedCourseLogic();
                    appliedCourse = appliedCourseLogic.GetBy(payment.Person);

                    var level = new Level { Id = 1 };
                    payment.FeeDetails = paymentLogic.SetFeeDetails(payment, appliedCourse.Programme.Id,
                        level.Id, payment.PaymentMode.Id, appliedCourse.Department.Id, payment.Session.Id);
                    paymentEtranzactType =
                        paymentEtranzactTypeLogic.GetModelsBy(
                            m =>
                                m.Level_Id == level.Id && m.Programme_Id == appliedCourse.Programme.Id &&
                                m.Payment_Mode_Id == payment.PaymentMode.Id && m.Session_Id == payment.Session.Id &&
                                m.Fee_Type_Id == payment.FeeType.Id && m.Session_Id == payment.Session.Id)
                            .FirstOrDefault();
                    invoice.paymentEtranzactType = paymentEtranzactType;
                    invoice.Level = "100 Level";
                    invoice.Department = appliedCourse.Department.Name;

                }



                invoice.Session = payment.Session.Name;
                invoice.Payment = payment;
                invoice.Person = payment.Person;
                invoice.JambRegistrationNumber = "";
                return View(invoice);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }
            return View();
        }

        public ActionResult PostJambForm()
        {
            var existingViewModel = (PostJambViewModel)TempData["viewModel"];
            var postJAMBFormPaymentViewModel = (PostJAMBFormPaymentViewModel)TempData["PostJAMBFormPaymentViewModel"];
            try
            {
                PopulateAllDropDowns();

                if (existingViewModel != null)
                {
                    viewModel = existingViewModel;
                    SetStudentUploadedPassport(viewModel);
                }

                if (postJAMBFormPaymentViewModel != null)
                {
                    if (postJAMBFormPaymentViewModel != null && postJAMBFormPaymentViewModel.Programme != null)
                    {
                        viewModel.Session = postJAMBFormPaymentViewModel.CurrentSession;
                        viewModel.Programme = postJAMBFormPaymentViewModel.Programme;

                        if ((viewModel.Programme.Id == 1 && (postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 19
                            || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 7 ||
                            postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 9 ||
                            postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 11
                            || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 13 ||
                            postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 17 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 21 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 1)) || viewModel.Programme.Id == 4)
                        {
                            viewModel.PreviousEducation = null;
                            viewModel.ApplicantJambDetail.JambRegistrationNumber = postJAMBFormPaymentViewModel.JambRegistrationNumber;
                            viewModel.ApplicantJambDetail = postJAMBFormPaymentViewModel.ApplicantJambDetail;
                        }
                        else if (viewModel.Programme.Id == 1 && (postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 20 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 8
                            || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 10 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 12 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 14 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 18 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 22))
                        {
                            viewModel.ApplicantJambDetail.JambRegistrationNumber = postJAMBFormPaymentViewModel.JambRegistrationNumber;
                            viewModel.ApplicantJambDetail = postJAMBFormPaymentViewModel.ApplicantJambDetail;
                        }
                        else if (viewModel.Programme.Id == 2 || viewModel.Programme.Id == 5 || viewModel.Programme.Id == 6)
                        {
                            viewModel.ApplicantJambDetail = null;
                        }

                        viewModel.Payment = postJAMBFormPaymentViewModel.Payment;
                        viewModel.AppliedCourse = postJAMBFormPaymentViewModel.AppliedCourse;
                        viewModel.SupplementaryCourse = postJAMBFormPaymentViewModel.SupplementaryCourse;
                        viewModel.ApplicationFormSetting = postJAMBFormPaymentViewModel.ApplicationFormSetting;
                        viewModel.ApplicationProgrammeFee = postJAMBFormPaymentViewModel.ApplicationProgrammeFee;
                        viewModel.remitaPyament = postJAMBFormPaymentViewModel.remitaPayment;
                        viewModel.Person.Id = postJAMBFormPaymentViewModel.Person.Id;
                        viewModel.Person.FirstName = postJAMBFormPaymentViewModel.Person.FirstName;
                        viewModel.Person.LastName = postJAMBFormPaymentViewModel.Person.LastName;
                        viewModel.Person.OtherName = postJAMBFormPaymentViewModel.Person.OtherName;
                        viewModel.Person.FullName = postJAMBFormPaymentViewModel.Person.FullName;
                        viewModel.Person.Email = postJAMBFormPaymentViewModel.Person.Email;
                        viewModel.Person.MobilePhone = postJAMBFormPaymentViewModel.Person.MobilePhone;
                        viewModel.Person.ContactAddress = postJAMBFormPaymentViewModel.Person.ContactAddress;
                        viewModel.Person.DateEntered = postJAMBFormPaymentViewModel.Person.DateEntered;
                        viewModel.Person.State = postJAMBFormPaymentViewModel.Person.State;
                        viewModel.Person.ImageFileUrl = postJAMBFormPaymentViewModel.Person.ImageFileUrl;
                        viewModel.HasJambPassport = postJAMBFormPaymentViewModel.HasJambPassport;

                        SetLgaIfExist(viewModel);
                    }
                }

                ApplicationForm applicationform = viewModel.GetApplicationFormBy(viewModel.Person, viewModel.Payment);
                if ((applicationform != null && applicationform.Id > 0) && viewModel.ApplicationAlreadyExist == false)
                {
                    viewModel.ApplicationAlreadyExist = true;

                    viewModel.LoadApplicantionFormBy(viewModel.Person, viewModel.Payment);
                    SetSelectedSittingSubjectAndGrade(viewModel);

                    SetLgaIfExist(viewModel);

                    SetDateOfBirth(viewModel);
                    if (viewModel.AppliedCourse.Programme.Id == 2 || viewModel.AppliedCourse.Programme.Id == 5 ||
                        viewModel.AppliedCourse.Programme.Id == 6 || (viewModel.AppliedCourse.Programme.Id == 1 && (viewModel.ApplicationFormSetting.Id == 20 || viewModel.ApplicationFormSetting.Id == 8 || viewModel.ApplicationFormSetting.Id == 10 || viewModel.ApplicationFormSetting.Id == 12 || viewModel.ApplicationFormSetting.Id == 14 || viewModel.ApplicationFormSetting.Id == 18 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 22)))
                    {
                        SetPreviousEducationEndDate();
                        SetPreviousEducationStartDate();
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }

            TempData["viewModel"] = viewModel;
            TempData["imageUrl"] = viewModel.Person.ImageFileUrl;
            TempData["PostJAMBFormPaymentViewModel"] = postJAMBFormPaymentViewModel;
            return View(viewModel);
        }

        private void SetLgaIfExist(PostJambViewModel viewModel)
        {
            try
            {
                if (viewModel.Person.State != null && !string.IsNullOrEmpty(viewModel.Person.State.Id))
                {
                    var LocalGovernmentLogic = new LocalGovernmentLogic();
                    List<LocalGovernment> lgas =
                        LocalGovernmentLogic.GetModelsBy(l => l.State_Id == viewModel.Person.State.Id);
                    if (viewModel.Person.LocalGovernment != null && viewModel.Person.LocalGovernment.Id > 0)
                    {
                        ViewBag.LgaId = new SelectList(lgas, ID, NAME, viewModel.Person.LocalGovernment.Id);
                    }
                    else
                    {
                        ViewBag.LgaId = new SelectList(lgas, ID, NAME);
                    }
                }
                else
                {
                    ViewBag.LgaId = new SelectList(new List<LocalGovernment>(), ID, NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }
        }

        private void SetDateOfBirth(PostJambViewModel viewModel)
        {
            try
            {
                if (viewModel.Person.DateOfBirth.HasValue)
                {
                    int noOfDays = DateTime.DaysInMonth(viewModel.Person.YearOfBirth.Id,
                        viewModel.Person.MonthOfBirth.Id);
                    List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                    if (days != null && days.Count > 0)
                    {
                        days.Insert(0, new Value { Name = "--DD--" });
                    }

                    if (viewModel.Person.DayOfBirth != null && viewModel.Person.DayOfBirth.Id > 0)
                    {
                        ViewBag.DayOfBirthId = new SelectList(days, ID, NAME, viewModel.Person.DayOfBirth.Id);
                    }
                    else
                    {
                        ViewBag.DayOfBirthId = new SelectList(days, ID, NAME);
                    }
                }
                else
                {
                    ViewBag.DayOfBirthId = new SelectList(new List<Value>(), ID, NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }
        }

        private void SetPreviousEducationStartDate()
        {
            try
            {
                if (viewModel.PreviousEducation.StartDate != null)
                {
                    int noOfDays = DateTime.DaysInMonth(viewModel.PreviousEducation.StartYear.Id,
                        viewModel.PreviousEducation.StartMonth.Id);
                    List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                    if (days != null && days.Count > 0)
                    {
                        days.Insert(0, new Value { Name = "--DD--" });
                    }

                    if (viewModel.PreviousEducation.StartDay != null && viewModel.PreviousEducation.StartDay.Id > 0)
                    {
                        ViewBag.PreviousEducationStartDayId = new SelectList(days, ID, NAME,
                            viewModel.PreviousEducation.StartDay.Id);
                    }
                    else
                    {
                        ViewBag.PreviousEducationStartDayId = new SelectList(days, ID, NAME);
                    }
                }
                else
                {
                    ViewBag.PreviousEducationStartDayId = new SelectList(new List<Value>(), ID, NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }
        }

        private void SetPreviousEducationEndDate()
        {
            try
            {
                if (viewModel.PreviousEducation.EndDate != null)
                {
                    int noOfDays = DateTime.DaysInMonth(viewModel.PreviousEducation.EndYear.Id,
                        viewModel.PreviousEducation.EndMonth.Id);
                    List<Value> days = Utility.CreateNumberListFrom(1, noOfDays);
                    if (days != null && days.Count > 0)
                    {
                        days.Insert(0, new Value { Name = "--DD--" });
                    }

                    if (viewModel.PreviousEducation.EndDay != null && viewModel.PreviousEducation.EndDay.Id > 0)
                    {
                        ViewBag.PreviousEducationEndDayId = new SelectList(days, ID, NAME,
                            viewModel.PreviousEducation.EndDay.Id);
                    }
                    else
                    {
                        ViewBag.PreviousEducationEndDayId = new SelectList(days, ID, NAME);
                    }
                }
                else
                {
                    ViewBag.PreviousEducationEndDayId = new SelectList(new List<Value>(), ID, NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }
        }

        [HttpPost]
        public ActionResult PostJambForm(PostJambViewModel viewModel)
        {
            var postJAMBFormPaymentViewModel = (PostJAMBFormPaymentViewModel)TempData["PostJAMBFormPaymentViewModel"];
            TempData.Keep("PostJAMBFormPaymentViewModel");

            try
            {
                SetStudentUploadedPassport(viewModel);

                ModelState["ApplicationProgrammeFee.FeeType.Name"].Errors.Clear();
                ModelState["remitaPyament.payment.Id"].Errors.Clear();


                var errors = from modelstate in ModelState.AsQueryable().Where(f => f.Value.Errors.Count > 0)
                             select new { Title = modelstate.Key };

                if (ModelState.IsValid)
                {
                    if (postJAMBFormPaymentViewModel?.AppliedCourse?.Department?.Faculty?.Id > 0)
                        viewModel.AppliedCourse.Department.Faculty = postJAMBFormPaymentViewModel.AppliedCourse.Department.Faculty;

                    if (InvalidDateOfBirth(viewModel))
                    {
                        SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                        return View(viewModel);
                    }

                    if (InvalidNumberOfOlevelSubject(viewModel.FirstSittingOLevelResultDetails,
                        viewModel.SecondSittingOLevelResultDetails))
                    {
                        SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                        return View(viewModel);
                    }

                    if (InvalidOlevelSubjectOrGrade(viewModel.FirstSittingOLevelResultDetails, viewModel.OLevelSubjects,
                        viewModel.OLevelGrades, FIRST_SITTING))
                    {
                        SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                        return View(viewModel);
                    }

                    if (viewModel.SecondSittingOLevelResult != null)
                    {
                        if (viewModel.SecondSittingOLevelResult.ExamNumber != null &&
                            viewModel.SecondSittingOLevelResult.Type != null &&
                            viewModel.SecondSittingOLevelResult.Type.Id > 0 &&
                            viewModel.SecondSittingOLevelResult.ExamYear > 0)
                        {
                            if (InvalidOlevelSubjectOrGrade(viewModel.SecondSittingOLevelResultDetails,
                                viewModel.OLevelSubjects, viewModel.OLevelGrades, SECOND_SITTING))
                            {
                                SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                                return View(viewModel);
                            }
                        }
                        //else if (viewModel.SecondSittingOLevelResult.ExamNumber != null || viewModel.SecondSittingOLevelResult.Type.Id > 0 || viewModel.SecondSittingOLevelResult.ExamYear > 0)
                        //{
                        //    SetMessage("One or more fields not set in " + SECOND_SITTING + " header! Please modify and try again.", Message.Category.Error);

                        //    SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                        //    return View(viewModel);
                        //}
                    }

                    if (InvalidOlevelResultHeaderInformation(viewModel.FirstSittingOLevelResultDetails,
                        viewModel.FirstSittingOLevelResult, FIRST_SITTING))
                    {
                        SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                        return View(viewModel);
                    }

                    if (InvalidOlevelResultHeaderInformation(viewModel.SecondSittingOLevelResultDetails,
                        viewModel.SecondSittingOLevelResult, SECOND_SITTING))
                    {
                        SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                        return View(viewModel);
                    }

                    if (NoOlevelSubjectSpecified(viewModel.FirstSittingOLevelResultDetails,
                        viewModel.FirstSittingOLevelResult, FIRST_SITTING))
                    {
                        SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                        return View(viewModel);
                    }
                    if (NoOlevelSubjectSpecified(viewModel.SecondSittingOLevelResultDetails,
                        viewModel.SecondSittingOLevelResult, SECOND_SITTING))
                    {
                        SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                        return View(viewModel);
                    }

                    if (InvalidOlevelType(viewModel.FirstSittingOLevelResult.Type,
                        viewModel.SecondSittingOLevelResult.Type))
                    {
                        SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                        return View(viewModel);
                    }


                    //if (InvalidOlevelResultSubject(viewModel.FirstSittingOLevelResultDetails, viewModel.OLevelSubjects, viewModel.OLevelGrades, FIRST_SITTING))
                    //{
                    //    SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                    //    return View(viewModel);
                    //}


                    if (viewModel.Programme.Id == 2 || viewModel.Programme.Id == 5 || viewModel.Programme.Id == 6 || (viewModel.Programme.Id == 1 && (viewModel.ApplicationFormSetting.Id == 8 || viewModel.ApplicationFormSetting.Id == 8 || viewModel.ApplicationFormSetting.Id == 10 || viewModel.ApplicationFormSetting.Id == 20)))
                    {
                        if (InvalidPreviousEducationStartAndEndDate(viewModel.PreviousEducation))
                        {
                            SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                            return View(viewModel);
                        }
                    }

                    bool ARCheck = false;
                    for (int i = 0; i < viewModel.FirstSittingOLevelResultDetails.Count; i++)
                    {
                        if (viewModel.FirstSittingOLevelResultDetails[i].Grade.Id == 10)
                        {
                            ARCheck = true;
                        }
                    }
                    for (int i = 0; i < viewModel.SecondSittingOLevelResultDetails.Count; i++)
                    {
                        if (viewModel.SecondSittingOLevelResultDetails[i].Grade.Id == 10)
                        {
                            ARCheck = true;
                        }
                    }
                    if (ARCheck == false)
                    {
                        //if (string.IsNullOrEmpty(viewModel.FirstSittingOLevelResult.ScannedCopyUrl) || viewModel.FirstSittingOLevelResult.ScannedCopyUrl == DEFAULT_PASSPORT)
                        //{
                        //    SetMessage("No o-level scanned copy! Please upload your o-level scanned copy to continue.", Message.Category.Error);
                        //    SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                        //    return View(viewModel);
                        //}  
                    }
                    if (string.IsNullOrEmpty(viewModel.Person.ImageFileUrl) ||
                        viewModel.Person.ImageFileUrl == DEFAULT_PASSPORT)
                    {
                        if (viewModel.PassportUrl != null && viewModel.Person.ImageFileUrl == null || viewModel.Person.ImageFileUrl == DEFAULT_PASSPORT)
                        {
                            viewModel.Person.ImageFileUrl = viewModel.PassportUrl;
                        }
                        else
                        {
                            SetMessage("No Passport uploaded! Please upload your passport to continue.",
                                Message.Category.Error);
                            SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                            return View(viewModel);
                        }

                    }

                    //viewModel.ApplicantJambDetail = UpdateApplicantJambDetail(postJAMBFormPaymentViewModel);
                    if (postJAMBFormPaymentViewModel.ApplicantJambDetail != null)
                    {
                        if (postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject1 != null)
                        {
                            viewModel.ApplicantJambDetail.Subject1 =
                                GetSubject(postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject1);
                        }
                        else
                        {
                            viewModel.ApplicantJambDetail.Subject1 = GetSubject(viewModel.ApplicantJambDetail.Subject1);
                        }
                        if (postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject2 != null)
                        {
                            viewModel.ApplicantJambDetail.Subject2 =
                                GetSubject(postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject2);
                        }
                        else
                        {
                            viewModel.ApplicantJambDetail.Subject2 = GetSubject(viewModel.ApplicantJambDetail.Subject2);
                        }
                        if (postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject3 != null)
                        {
                            viewModel.ApplicantJambDetail.Subject3 =
                                GetSubject(postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject3);
                        }
                        else
                        {
                            viewModel.ApplicantJambDetail.Subject3 = GetSubject(viewModel.ApplicantJambDetail.Subject3);
                        }
                        if (postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject4 != null)
                        {
                            viewModel.ApplicantJambDetail.Subject4 =
                                GetSubject(postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject4);
                        }
                        else
                        {
                            viewModel.ApplicantJambDetail.Subject4 = GetSubject(viewModel.ApplicantJambDetail.Subject4);
                        }
                    }

                    TempData["viewModel"] = viewModel;
                    TempData.Keep("viewModel");
                    return RedirectToAction("PostJambPreview", "Form");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
                SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
                return View(viewModel);
            }

            SetPostJAMBStateVariables(viewModel, postJAMBFormPaymentViewModel);
            return View(viewModel);
        }

        private OLevelSubject GetSubject(OLevelSubject oLevelSubject)
        {
            try
            {
                var oLevelSubjectLogic = new OLevelSubjectLogic();

                if (oLevelSubject == null)
                {
                    return null;
                }
                OLevelSubject thisOLevelSubject =
                    oLevelSubjectLogic.GetModelBy(ol => ol.O_Level_Subject_Id == oLevelSubject.Id);

                return thisOLevelSubject;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool NoOlevelSubjectSpecified(List<OLevelResultDetail> oLevelResultDetails, OLevelResult oLevelResult,
            string sitting)
        {
            try
            {
                if (!string.IsNullOrEmpty(oLevelResult.ExamNumber) ||
                    (oLevelResult.Type != null && oLevelResult.Type.Id > 0) || (oLevelResult.ExamYear > 0))
                {
                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        List<OLevelResultDetail> oLevelResultDetailsEntered =
                            oLevelResultDetails.Where(r => r.Subject.Id > 0).ToList();
                        if (oLevelResultDetailsEntered == null || oLevelResultDetailsEntered.Count <= 0)
                        {
                            SetMessage(
                                "No O-Level Subject specified for " + sitting +
                                "! At least one subject must be specified when Exam Number, O-Level Type and Year are all specified for the sitting.",
                                Message.Category.Error);
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidOlevelType(OLevelType firstSittingOlevelType, OLevelType secondSittingOlevelType)
        {
            try
            {
                if (firstSittingOlevelType != null && secondSittingOlevelType != null)
                {
                    if ((firstSittingOlevelType.Id != secondSittingOlevelType.Id) && firstSittingOlevelType.Id > 0 &&
                        secondSittingOlevelType.Id > 0)
                    {
                        if (firstSittingOlevelType.Id == (int)OLevelTypes.Nabteb)
                        {
                            SetMessage(
                                "NABTEB O-Level Type in " + FIRST_SITTING +
                                " cannot be combined with any other O-Level Type! Please modify.",
                                Message.Category.Error);
                            return true;
                        }
                        if (secondSittingOlevelType.Id == (int)OLevelTypes.Nabteb)
                        {
                            SetMessage(
                                "NABTEB O-Level Type in " + SECOND_SITTING +
                                " cannot be combined with any other O-Level Type! Please modify.",
                                Message.Category.Error);
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidPreviousEducationStartAndEndDate(PreviousEducation previousEducation)
        {
            const int ONE_YEAR = 365;

            try
            {
                if (InvalidPreviousEducationStartDate(previousEducation))
                {
                    return true;
                }
                if (InvalidPreviousEducationEndDate(previousEducation))
                {
                    return true;
                }

                var previousEducationStartDate = new DateTime(previousEducation.StartYear.Id,
                    previousEducation.StartMonth.Id, previousEducation.StartDay.Id);
                var previousEducationEndDate = new DateTime(previousEducation.EndYear.Id, previousEducation.EndMonth.Id,
                    previousEducation.EndDay.Id);

                bool isStartDateInTheFuture = Utility.IsDateInTheFuture(previousEducationStartDate);
                bool isEndDateInTheFuture = Utility.IsDateInTheFuture(previousEducationEndDate);

                if (isStartDateInTheFuture)
                {
                    SetMessage("Previous Education Start Date cannot be a future date!", Message.Category.Error);
                    return true;
                }
                if (isEndDateInTheFuture)
                {
                    SetMessage("Previous Education End Date cannot be a future date!", Message.Category.Error);
                    return true;
                }
                if (Utility.IsStartDateGreaterThanEndDate(previousEducationStartDate, previousEducationEndDate))
                {
                    SetMessage(
                        "Previous Education Start Date '" + previousEducationStartDate.ToShortDateString() +
                        "' cannot be greater than End Date '" + previousEducationEndDate.ToShortDateString() +
                        "'! Please modify and try again.", Message.Category.Error);
                    return true;
                }
                if (Utility.IsDateOutOfRange(previousEducationStartDate, previousEducationEndDate, ONE_YEAR))
                {
                    SetMessage(
                        "Previous Education duration must not be less than one year, twelve months or 365 days to be qualified!",
                        Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetStudentUploadedPassport(PostJambViewModel viewModel)
        {
            if (viewModel != null && viewModel.Person != null && !string.IsNullOrEmpty((string)TempData["imageUrl"]))
            {
                viewModel.Person.ImageFileUrl = (string)TempData["imageUrl"];
            }
            else
            {
                viewModel.Person.ImageFileUrl = DEFAULT_PASSPORT;
                //viewModel.PassportUrl = appRoot + DEFAULT_PASSPORT;
            }

            if (viewModel != null && viewModel.FirstSittingOLevelResult.ScannedCopyUrl == null &&
                !string.IsNullOrEmpty((string)TempData["CredentialimageUrl"]))
            {
                viewModel.FirstSittingOLevelResult.ScannedCopyUrl = (string)TempData["CredentialimageUrl"];
            }
        }

        private bool InvalidPreviousEducationStartDate(PreviousEducation previousEducation)
        {
            try
            {
                if (previousEducation.StartYear == null || previousEducation.StartYear.Id <= 0)
                {
                    SetMessage("Please select Previous Education Start Year!", Message.Category.Error);
                    return true;
                }
                if (previousEducation.StartMonth == null || previousEducation.StartMonth.Id <= 0)
                {
                    SetMessage("Please select Previous Education Start Month!", Message.Category.Error);
                    return true;
                }
                if (previousEducation.StartDay == null || previousEducation.StartDay.Id <= 0)
                {
                    SetMessage("Please select Previous Education Start Day!", Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidPreviousEducationEndDate(PreviousEducation previousEducation)
        {
            try
            {
                if (previousEducation.EndYear == null || previousEducation.EndYear.Id <= 0)
                {
                    SetMessage("Please select Previous Education End Year!", Message.Category.Error);
                    return true;
                }
                if (previousEducation.EndMonth == null || previousEducation.EndMonth.Id <= 0)
                {
                    SetMessage("Please select Previous Education End Month!", Message.Category.Error);
                    return true;
                }
                if (previousEducation.EndDay == null || previousEducation.EndDay.Id <= 0)
                {
                    SetMessage("Please select Previous Education End Day!", Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidDateOfBirth(PostJambViewModel viewModel)
        {
            try
            {
                if (viewModel.Person.YearOfBirth == null || viewModel.Person.YearOfBirth.Id <= 0)
                {
                    SetMessage("Please select Year of Birth!", Message.Category.Error);
                    return true;
                }
                if (viewModel.Person.MonthOfBirth == null || viewModel.Person.MonthOfBirth.Id <= 0)
                {
                    SetMessage("Please select Month of Birth!", Message.Category.Error);
                    return true;
                }
                if (viewModel.Person.DayOfBirth == null || viewModel.Person.DayOfBirth.Id <= 0)
                {
                    SetMessage("Please select Day of Birth!", Message.Category.Error);
                    return true;
                }

                viewModel.Person.DateOfBirth = new DateTime(viewModel.Person.YearOfBirth.Id,
                    viewModel.Person.MonthOfBirth.Id, viewModel.Person.DayOfBirth.Id);
                if (viewModel.Person.DateOfBirth == null)
                {
                    SetMessage("Please enter Date of Birth!", Message.Category.Error);
                    return true;
                }

                TimeSpan difference = DateTime.Now - (DateTime)viewModel.Person.DateOfBirth;
                if (difference.Days == 0)
                {
                    SetMessage("Date of Birth cannot be todays date!", Message.Category.Error);
                    return true;
                }
                if (difference.Days == -1)
                {
                    SetMessage("Date of Birth cannot be yesterdays date date!", Message.Category.Error);
                    return true;
                }

                if (difference.Days < 4380)
                {
                    SetMessage("Applicant cannot be less than twelve years!", Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetPostJAMBStateVariables(PostJambViewModel viewModel,
            PostJAMBFormPaymentViewModel postJAMBFormPaymentViewModel)
        {
            try
            {
                TempData["viewModel"] = viewModel;
                TempData.Keep("viewModel");
                TempData["PostJAMBFormPaymentViewModel"] = postJAMBFormPaymentViewModel;
                TempData.Keep("PostJAMBFormPaymentViewModel");
                TempData["imageUrl"] = viewModel.Person.ImageFileUrl;
                TempData.Keep("imageUrl");


                //SetDateOfBirth(viewModel);
                PopulateAllDropDowns();
                TempData["PostJAMBFormPaymentViewModel"] = postJAMBFormPaymentViewModel;
                TempData.Keep("PostJAMBFormPaymentViewModel");
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }
        }

        private bool InvalidOlevelResultHeaderInformation(List<OLevelResultDetail> resultDetails,
            OLevelResult oLevelResult, string sitting)
        {
            try
            {
                if (resultDetails != null && resultDetails.Count > 0)
                {
                    List<OLevelResultDetail> subjectList = resultDetails.Where(r => r.Subject.Id > 0).ToList();

                    if (subjectList != null && subjectList.Count > 0)
                    {
                        if (string.IsNullOrEmpty(oLevelResult.ExamNumber))
                        {
                            SetMessage("O-Level Exam Number not set for " + sitting + " ! Please modify",
                                Message.Category.Error);
                            return true;
                        }
                        if (oLevelResult.Type == null || oLevelResult.Type.Id <= 0)
                        {
                            SetMessage("O-Level Exam Type not set for " + sitting + " ! Please modify",
                                Message.Category.Error);
                            return true;
                        }
                        if (oLevelResult.ExamYear <= 0)
                        {
                            SetMessage("O-Level Exam Year not set for " + sitting + " ! Please modify",
                                Message.Category.Error);
                            return true;
                        }

                        //if (string.IsNullOrEmpty(oLevelResult.ExamNumber) || oLevelResult.Type == null || oLevelResult.Type.Id <= 0 || oLevelResult.ExamYear <= 0)
                        //{
                        //    SetMessage("Header Information not set for " + sitting + " O-Level Result Details! ", Message.Category.Error);
                        //    return true;
                        //}
                    }
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidOlevelSubjectOrGrade(List<OLevelResultDetail> oLevelResultDetails,
            List<OLevelSubject> subjects, List<OLevelGrade> grades, string sitting)
        {
            try
            {
                List<OLevelResultDetail> subjectList = null;
                if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                {
                    subjectList = oLevelResultDetails.Where(r => r.Subject.Id > 0 || r.Grade.Id > 0).ToList();
                }

                foreach (OLevelResultDetail oLevelResultDetail in subjectList)
                {
                    OLevelSubject subject = subjects.Where(s => s.Id == oLevelResultDetail.Subject.Id).SingleOrDefault();
                    OLevelGrade grade = grades.Where(g => g.Id == oLevelResultDetail.Grade.Id).SingleOrDefault();

                    List<OLevelResultDetail> results =
                        subjectList.Where(o => o.Subject.Id == oLevelResultDetail.Subject.Id).ToList();
                    if (results != null && results.Count > 1)
                    {
                        SetMessage(
                            "Duplicate " + subject.Name.ToUpper() + " Subject detected in " + sitting +
                            "! Please modify.", Message.Category.Error);
                        return true;
                    }
                    if (oLevelResultDetail.Subject.Id > 0 && oLevelResultDetail.Grade.Id <= 0)
                    {
                        SetMessage(
                            "No Grade specified for Subject " + subject.Name.ToUpper() + " in " + sitting +
                            "! Please modify.", Message.Category.Error);
                        return true;
                    }
                    if (oLevelResultDetail.Subject.Id <= 0 && oLevelResultDetail.Grade.Id > 0)
                    {
                        SetMessage(
                            "No Subject specified for Grade" + grade.Name.ToUpper() + " in " + sitting +
                            "! Please modify.", Message.Category.Error);
                        return true;
                    }

                    //else if (oLevelResultDetail.Grade.Id > 0 && oLevelResultDetail.Subject.Id <= 0)
                    //{
                    //    SetMessage("No Grade specified for " + subject.Name.ToUpper() + " for " + sitting + "! Please modify.", Message.Category.Error);
                    //    return true;
                    //}
                    //else if (oLevelResultDetail.Subject.Id <= 0 && oLevelResultDetail.Grade.Id > 0)
                    //{
                    //    SetMessage("No Subject specified for " + grade.Name.ToUpper() + " for " + sitting + "! Please modify.", Message.Category.Error);
                    //    return true;
                    //}
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidNumberOfOlevelSubject(List<OLevelResultDetail> firstSittingResultDetails,
            List<OLevelResultDetail> secondSittingResultDetails)
        {
            const int FIVE = 5;

            try
            {
                int totalNoOfSubjects = 0;

                List<OLevelResultDetail> firstSittingSubjectList = null;
                List<OLevelResultDetail> secondSittingSubjectList = null;

                if (firstSittingResultDetails != null && firstSittingResultDetails.Count > 0)
                {
                    firstSittingSubjectList = firstSittingResultDetails.Where(r => r.Subject.Id > 0).ToList();
                    if (firstSittingSubjectList != null)
                    {
                        totalNoOfSubjects += firstSittingSubjectList.Count;
                    }
                }

                if (secondSittingResultDetails != null && secondSittingResultDetails.Count > 0)
                {
                    secondSittingSubjectList = secondSittingResultDetails.Where(r => r.Subject.Id > 0).ToList();
                    if (secondSittingSubjectList != null)
                    {
                        totalNoOfSubjects += secondSittingSubjectList.Count;
                    }
                }

                if (totalNoOfSubjects == 0)
                {
                    SetMessage("No O-Level Result Details found for both sittings!", Message.Category.Error);
                    return true;
                }
                if (totalNoOfSubjects < FIVE)
                {
                    SetMessage("O-Level Result cannot be less than " + FIVE + " subjects in both sittings!",
                        Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private bool InvalidOlevelResultSubject(List<OLevelResultDetail> resultDetails, List<OLevelSubject> subjects, List<OLevelGrade> grades, string sitting)
        //{
        //    try
        //    {
        //        List<OLevelResultDetail>  subjectList = resultDetails.Where(r => r.Subject.Id > 0).ToList();

        //        if (subjectList == null || subjectList.Count == 0)
        //        {
        //            SetMessage("No O-Level Result Details found!", Message.Category.Error);
        //            return true;
        //        }
        //        else if (subjectList.Count < 8)
        //        {
        //            SetMessage("O-Level Result cannot be less than less 8 subjects in " + sitting + "!", Message.Category.Error);
        //            return true;
        //        }

        //        foreach (OLevelResultDetail oLevelResultDetail in subjectList)
        //        {
        //            OLevelSubject subject = subjects.Where(s => s.Id == oLevelResultDetail.Subject.Id).SingleOrDefault();
        //            OLevelGrade grade = grades.Where(g => g.Id == oLevelResultDetail.Grade.Id).SingleOrDefault();

        //            List<OLevelResultDetail> results = subjectList.Where(o => o.Subject.Id == oLevelResultDetail.Subject.Id).ToList();
        //            if (results != null && results.Count > 1)
        //            {
        //                SetMessage("Duplicate " + subject.Name.ToUpper() + " Subject detected in " + sitting + "! Please modify.", Message.Category.Error);
        //                return true;
        //            }
        //            else if (oLevelResultDetail.Subject.Id > 0 && oLevelResultDetail.Grade.Id <= 0)
        //            {
        //                SetMessage("No Grade specified for " + subject.Name.ToUpper() + " for " + sitting + "! Please modify.", Message.Category.Error);
        //                return true;
        //            }
        //            else if (oLevelResultDetail.Subject.Id <= 0 && oLevelResultDetail.Grade.Id > 0)
        //            {
        //                SetMessage("No Subject specified for " + grade.Name.ToUpper() + " for " + sitting + "! Please modify.", Message.Category.Error);
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public ActionResult PostJambPreview()
        {
            var viewModel = (PostJambViewModel)TempData["viewModel"];

            try
            {
                if (viewModel != null)
                {
                    viewModel.Person.DateOfBirth = new DateTime(viewModel.Person.YearOfBirth.Id,
                        viewModel.Person.MonthOfBirth.Id, viewModel.Person.DayOfBirth.Id);
                    viewModel.Person.State =
                        viewModel.States.Where(m => m.Id == viewModel.Person.State.Id).SingleOrDefault();
                    viewModel.Person.LocalGovernment =
                        viewModel.Lgas.Where(m => m.Id == viewModel.Person.LocalGovernment.Id).SingleOrDefault();
                    viewModel.Person.Sex =
                        viewModel.Genders.Where(m => m.Id == viewModel.Person.Sex.Id).SingleOrDefault();
                    viewModel.Applicant.Ability =
                        viewModel.Abilities.Where(m => m.Id == viewModel.Applicant.Ability.Id).SingleOrDefault();
                    viewModel.Sponsor.Relationship =
                        viewModel.Relationships.Where(m => m.Id == viewModel.Sponsor.Relationship.Id).SingleOrDefault();
                    viewModel.Person.Religion =
                        viewModel.Religions.Where(m => m.Id == viewModel.Person.Religion.Id).SingleOrDefault();

                    if (viewModel.FirstSittingOLevelResult.Type != null)
                    {
                        viewModel.FirstSittingOLevelResult.Type =
                            viewModel.OLevelTypes.Where(m => m.Id == viewModel.FirstSittingOLevelResult.Type.Id)
                                .SingleOrDefault();
                    }

                    if (viewModel.SecondSittingOLevelResult.Type != null)
                    {
                        viewModel.SecondSittingOLevelResult.Type =
                            viewModel.OLevelTypes.Where(m => m.Id == viewModel.SecondSittingOLevelResult.Type.Id)
                                .SingleOrDefault();
                    }

                    if (viewModel.AppliedCourse.Programme.Id == 2 || viewModel.AppliedCourse.Programme.Id == 5 ||
                        viewModel.AppliedCourse.Programme.Id == 6)
                    {
                        viewModel.PreviousEducation.StartDate = new DateTime(viewModel.PreviousEducation.StartYear.Id,
                            viewModel.PreviousEducation.StartMonth.Id, viewModel.PreviousEducation.StartDay.Id);
                        viewModel.PreviousEducation.EndDate = new DateTime(viewModel.PreviousEducation.EndYear.Id,
                            viewModel.PreviousEducation.EndMonth.Id, viewModel.PreviousEducation.EndDay.Id);
                        viewModel.PreviousEducation.Qualification =
                            viewModel.EducationalQualifications.Where(
                                m => m.Id == viewModel.PreviousEducation.Qualification.Id).SingleOrDefault();
                        viewModel.PreviousEducation.ResultGrade =
                            viewModel.ResultGrades.Where(m => m.Id == viewModel.PreviousEducation.ResultGrade.Id)
                                .SingleOrDefault();
                        viewModel.PreviousEducation.ITDuration =
                            viewModel.ITDurations.Where(m => m.Id == viewModel.PreviousEducation.ITDuration.Id)
                                .SingleOrDefault();
                    }
                    else if (viewModel.AppliedCourse.Programme.Id == 1 && (viewModel.ApplicationFormSetting.Id == 8 || viewModel.ApplicationFormSetting.Id == 8 || viewModel.ApplicationFormSetting.Id == 10 || viewModel.ApplicationFormSetting.Id == 14))
                    {
                        viewModel.PreviousEducation.StartDate = new DateTime(viewModel.PreviousEducation.StartYear.Id,
                           viewModel.PreviousEducation.StartMonth.Id, viewModel.PreviousEducation.StartDay.Id);
                        viewModel.PreviousEducation.EndDate = new DateTime(viewModel.PreviousEducation.EndYear.Id,
                            viewModel.PreviousEducation.EndMonth.Id, viewModel.PreviousEducation.EndDay.Id);
                        viewModel.PreviousEducation.Qualification =
                            viewModel.EducationalQualifications.Where(
                                m => m.Id == viewModel.PreviousEducation.Qualification.Id).SingleOrDefault();
                        viewModel.PreviousEducation.ResultGrade =
                            viewModel.ResultGrades.Where(m => m.Id == viewModel.PreviousEducation.ResultGrade.Id)
                                .SingleOrDefault();
                        viewModel.PreviousEducation.ITDuration =
                            viewModel.ITDurations.Where(m => m.Id == viewModel.PreviousEducation.ITDuration.Id)
                                .SingleOrDefault();

                        if (viewModel.SupplementaryCourse != null && viewModel.SupplementaryCourse.Department != null)
                        {
                            viewModel.SupplementaryCourse.Department =
                                viewModel.Departments.Where(o => o.Id == viewModel.SupplementaryCourse.Department.Id)
                                    .SingleOrDefault();
                        }
                    }
                    else
                    {
                        if (viewModel.AppliedCourse.Programme.Id == 1 && viewModel.ApplicantJambDetail.Subject2 != null)
                        {
                            viewModel.ApplicantJambDetail.InstitutionChoice =
                                viewModel.InstitutionChoices.Where(
                                    m => m.Id == viewModel.ApplicantJambDetail.InstitutionChoice.Id).SingleOrDefault();
                            viewModel.ApplicantJambDetail.Subject1 =
                                viewModel.OLevelSubjects.Where(o => o.Id == viewModel.ApplicantJambDetail.Subject1.Id)
                                    .SingleOrDefault();
                            viewModel.ApplicantJambDetail.Subject2 =
                                viewModel.OLevelSubjects.Where(o => o.Id == viewModel.ApplicantJambDetail.Subject2.Id)
                                    .SingleOrDefault();
                            viewModel.ApplicantJambDetail.Subject3 =
                                viewModel.OLevelSubjects.Where(o => o.Id == viewModel.ApplicantJambDetail.Subject3.Id)
                                    .SingleOrDefault();
                            viewModel.ApplicantJambDetail.Subject4 =
                                viewModel.OLevelSubjects.Where(o => o.Id == viewModel.ApplicantJambDetail.Subject4.Id)
                                    .SingleOrDefault();
                        }
                        if (viewModel.SupplementaryCourse != null && viewModel.SupplementaryCourse.Department != null)
                        {
                            viewModel.SupplementaryCourse.Department =
                                viewModel.Departments.Where(o => o.Id == viewModel.SupplementaryCourse.Department.Id)
                                    .SingleOrDefault();
                        }
                    }

                    UpdateOLevelResultDetail(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }

            TempData["viewModel"] = viewModel;
            return View("FormPreview", viewModel);
        }

        [HttpPost]
        public ActionResult PostJambPreview(PostJambViewModel viewModel)
        {
            ApplicationForm newApplicationForm = null;
            var existingViewModel = (PostJambViewModel)TempData["viewModel"];

            try
            {
                if (viewModel.ApplicationAlreadyExist == false)
                {
                    using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    {
                        ApplicationForm applicationForm = new ApplicationForm();
                        ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
                        applicationForm.ProgrammeFee = new ApplicationProgrammeFee
                        {
                            Id = existingViewModel.ApplicationProgrammeFee.Id
                        };
                        applicationForm.Setting = new ApplicationFormSetting
                        {
                            Id = existingViewModel.ApplicationFormSetting.Id
                        };
                        applicationForm.DateSubmitted = DateTime.Now;
                        applicationForm.Person = viewModel.Person;
                        applicationForm.Payment = viewModel.Payment;
                        applicationForm.ProgrammeFee.Programme = existingViewModel.Programme;

                        applicationForm = applicationFormLogic.Create(applicationForm);
                        existingViewModel.ApplicationFormNumber = applicationForm.Number;
                        applicationForm.Person = existingViewModel.Person;
                        existingViewModel.ApplicationForm = applicationForm;
                        newApplicationForm = applicationForm;
                        existingViewModel.Applicant.Person = viewModel.Person;
                        existingViewModel.Applicant.ApplicationForm = newApplicationForm;
                        existingViewModel.Applicant.Status = new ApplicantStatus
                        {
                            Id = (int)ApplicantStatus.Status.SubmittedApplicationForm
                        };
                        var applicantLogic = new ApplicantLogic();
                        applicantLogic.Create(existingViewModel.Applicant);



                        //update application no in applied course object
                        existingViewModel.AppliedCourse.Person = viewModel.Person;
                        existingViewModel.AppliedCourse.ApplicationForm = newApplicationForm;
                        var appliedCourseLogic = new AppliedCourseLogic();
                        appliedCourseLogic.Modify(existingViewModel.AppliedCourse);

                        var sponsorLogic = new SponsorLogic();
                        existingViewModel.Sponsor.ApplicationForm = newApplicationForm;
                        existingViewModel.Sponsor.Person = viewModel.Person;
                        sponsorLogic.Create(existingViewModel.Sponsor);

                        if (existingViewModel.SupplementaryCourse != null && existingViewModel.SupplementaryCourse.Department != null)
                        {
                            var supplementaryCourseLogic = new SupplementaryCourseLogic();
                            existingViewModel.SupplementaryCourse.ApplicationForm = newApplicationForm;
                            existingViewModel.SupplementaryCourse.Person = viewModel.Person;
                            supplementaryCourseLogic.Create(existingViewModel.SupplementaryCourse);
                        }

                        //string CredentialjunkFilePath;
                        //string CredentialdestinationFilePath;
                        //SetPersonCredentialDestination(existingViewModel,out CredentialjunkFilePath,out CredentialdestinationFilePath);
                        //SavePersonPassport(CredentialjunkFilePath,CredentialdestinationFilePath,existingViewModel.Person);


                        var oLevelResultLogic = new OLevelResultLogic();
                        var oLevelResultDetailLogic = new OLevelResultDetailLogic();
                        if (existingViewModel.FirstSittingOLevelResult != null &&
                            existingViewModel.FirstSittingOLevelResult.ExamNumber != null &&
                            existingViewModel.FirstSittingOLevelResult.Type != null &&
                            existingViewModel.FirstSittingOLevelResult.ExamYear > 0)
                        {
                            existingViewModel.FirstSittingOLevelResult.ApplicationForm = newApplicationForm;
                            existingViewModel.FirstSittingOLevelResult.Person = viewModel.Person;
                            existingViewModel.FirstSittingOLevelResult.Sitting = new OLevelExamSitting { Id = 1 };
                            OLevelResult firstSittingOLevelResult =
                                oLevelResultLogic.Create(existingViewModel.FirstSittingOLevelResult);

                            if (existingViewModel.FirstSittingOLevelResultDetails != null &&
                                existingViewModel.FirstSittingOLevelResultDetails.Count > 0 &&
                                firstSittingOLevelResult != null)
                            {
                                List<OLevelResultDetail> olevelResultDetails =
                                    existingViewModel.FirstSittingOLevelResultDetails.Where(
                                        m => m.Grade != null && m.Subject != null).ToList();
                                foreach (OLevelResultDetail oLevelResultDetail in olevelResultDetails)
                                {
                                    oLevelResultDetail.Header = firstSittingOLevelResult;
                                }

                                oLevelResultDetailLogic.Create(olevelResultDetails);
                            }
                        }

                        if (existingViewModel.SecondSittingOLevelResult != null &&
                            existingViewModel.SecondSittingOLevelResult.ExamNumber != null &&
                            existingViewModel.SecondSittingOLevelResult.Type != null &&
                            existingViewModel.SecondSittingOLevelResult.ExamYear > 0)
                        {
                            List<OLevelResultDetail> olevelResultDetails =
                                existingViewModel.SecondSittingOLevelResultDetails.Where(
                                    m => m.Grade != null && m.Subject != null).ToList();
                            if (olevelResultDetails != null && olevelResultDetails.Count > 0)
                            {
                                existingViewModel.SecondSittingOLevelResult.ApplicationForm = newApplicationForm;
                                existingViewModel.SecondSittingOLevelResult.Person = viewModel.Person;
                                existingViewModel.SecondSittingOLevelResult.Sitting = new OLevelExamSitting { Id = 2 };
                                OLevelResult secondSittingOLevelResult =
                                    oLevelResultLogic.Create(existingViewModel.SecondSittingOLevelResult);

                                if (existingViewModel.SecondSittingOLevelResultDetails != null &&
                                    existingViewModel.SecondSittingOLevelResultDetails.Count > 0 &&
                                    secondSittingOLevelResult != null)
                                {
                                    foreach (OLevelResultDetail oLevelResultDetail in olevelResultDetails)
                                    {
                                        oLevelResultDetail.Header = secondSittingOLevelResult;
                                    }

                                    oLevelResultDetailLogic.Create(olevelResultDetails);
                                }
                            }
                        }


                        if (existingViewModel.Programme.Id == 1 && (existingViewModel.ApplicationFormSetting.Id == 19 || existingViewModel.ApplicationFormSetting.Id == 7
                            || existingViewModel.ApplicationFormSetting.Id == 9 || existingViewModel.ApplicationFormSetting.Id == 11 || existingViewModel.ApplicationFormSetting.Id == 13
                            || existingViewModel.ApplicationFormSetting.Id == 17 || existingViewModel.ApplicationFormSetting.Id == 21 || existingViewModel.ApplicationFormSetting.Id == 1))
                        {
                            if (existingViewModel.ApplicantJambDetail != null &&
                                existingViewModel.ApplicantJambDetail.Subject2 != null)
                            {
                                var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                                existingViewModel.ApplicantJambDetail.ApplicationForm = newApplicationForm;
                                existingViewModel.ApplicantJambDetail.Person = viewModel.Person;
                                existingViewModel.ApplicantJambDetail.Subject1 =
                                    existingViewModel.ApplicantJambDetail.Subject1;
                                existingViewModel.ApplicantJambDetail.Subject2 =
                                    existingViewModel.ApplicantJambDetail.Subject2;
                                existingViewModel.ApplicantJambDetail.Subject3 =
                                    existingViewModel.ApplicantJambDetail.Subject3;
                                existingViewModel.ApplicantJambDetail.Subject4 = existingViewModel.ApplicantJambDetail.Subject4;
                                existingViewModel.ApplicantJambDetail.JambScore = existingViewModel.ApplicantJambDetail.JambScore;
                                applicantJambDetailLogic.Modify(existingViewModel.ApplicantJambDetail);
                            }
                        }
                        else if (existingViewModel.Programme.Id == 1 && (existingViewModel.ApplicationFormSetting.Id == 20 || existingViewModel.ApplicationFormSetting.Id == 8 || existingViewModel.ApplicationFormSetting.Id == 10 || existingViewModel.ApplicationFormSetting.Id == 12
                            || existingViewModel.ApplicationFormSetting.Id == 14 || existingViewModel.ApplicationFormSetting.Id == 18 || existingViewModel.ApplicationFormSetting.Id == 22))
                        {
                            if (existingViewModel.ApplicantJambDetail != null)
                            {
                                var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                                existingViewModel.ApplicantJambDetail.ApplicationForm = newApplicationForm;
                                existingViewModel.ApplicantJambDetail.Person = viewModel.Person;

                                existingViewModel.ApplicantJambDetail.JambScore = existingViewModel.ApplicantJambDetail.JambScore;
                                applicantJambDetailLogic.Modify(existingViewModel.ApplicantJambDetail);
                            }

                            if (existingViewModel.PreviousEducation != null && existingViewModel.PreviousEducation.Qualification.Id > 0)
                            {
                                existingViewModel.PreviousEducation.StartDate = new DateTime(existingViewModel.PreviousEducation.StartYear.Id, existingViewModel.PreviousEducation.StartMonth.Id, existingViewModel.PreviousEducation.StartDay.Id);
                                existingViewModel.PreviousEducation.EndDate = new DateTime(existingViewModel.PreviousEducation.EndYear.Id, existingViewModel.PreviousEducation.EndMonth.Id, existingViewModel.PreviousEducation.EndDay.Id);
                                var previousEducationLogic = new PreviousEducationLogic();
                                existingViewModel.PreviousEducation.ApplicationForm = newApplicationForm;
                                existingViewModel.PreviousEducation.Person = viewModel.Person;
                                previousEducationLogic.Create(existingViewModel.PreviousEducation);
                            }
                        }
                        else if (existingViewModel.Programme.Id == 2 || existingViewModel.Programme.Id == 5 ||
                                 existingViewModel.Programme.Id == 6)
                        {
                            if (existingViewModel.PreviousEducation != null)
                            {
                                var previousEducationLogic = new PreviousEducationLogic();
                                existingViewModel.PreviousEducation.ApplicationForm = newApplicationForm;
                                existingViewModel.PreviousEducation.Person = viewModel.Person;
                                previousEducationLogic.Create(existingViewModel.PreviousEducation);
                            }
                        }


                        var paymentEtranzactLogic = new PaymentEtranzactLogic();
                        paymentEtranzactLogic.UpdatePin(existingViewModel.Payment, existingViewModel.Person);

                        string junkFilePath;
                        string destinationFilePath;
                        SetPersonPassportDestination(existingViewModel, out junkFilePath, out destinationFilePath);

                        var personLogic = new PersonLogic();
                        bool personModified = personLogic.Modify(existingViewModel.Person);
                        if (personModified)
                        {
                            SavePersonPassport(junkFilePath, destinationFilePath, existingViewModel.Person);
                            transaction.Complete();

                            //SendSms(existingViewModel.ApplicationForm, existingViewModel.Programme);
                            if (CheckForInternetConnection())
                            {
                                SendSmartSms(existingViewModel.ApplicationForm, existingViewModel.Programme);
                            }

                            TempData["viewModel"] = existingViewModel;
                            return RedirectToAction("AcknowledgmentSlip", "Form",
                                new
                                {
                                    Area = "Applicant",
                                    FormId = Utility.Encrypt(existingViewModel.ApplicationForm.Id.ToString())
                                });
                        }
                        throw new Exception("Passport save operation failed! Please try again.");
                    }
                }
                if (existingViewModel != null && existingViewModel.Person?.ImageFileUrl != null && existingViewModel.Person.ImageFileUrl.Contains("Junk"))
                {
                    string destinationFilePath;
                    string junkFilePath;
                    SetPersonPassportDestination(existingViewModel, out junkFilePath, out destinationFilePath);
                    bool personModified = personLogic.Modify(existingViewModel.Person);
                    if (personModified)
                    {
                        SavePersonPassport(junkFilePath, destinationFilePath, existingViewModel.Person);
                    }
                }
                ApplicationFormLogic formLogic = new ApplicationFormLogic();
                newApplicationForm = formLogic.GetModelsBy(a => a.Application_Form_Number == viewModel.ApplicationForm.Number).LastOrDefault();
                existingViewModel.ApplicationFormNumber = viewModel.ApplicationForm.Number;
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                {
                    if ((existingViewModel.Programme.Id == 1 && (existingViewModel.ApplicationFormSetting.Id == 5 || existingViewModel.ApplicationFormSetting.Id == 7 || existingViewModel.ApplicationFormSetting.Id == 9 || existingViewModel.ApplicationFormSetting.Id == 13 || existingViewModel.ApplicationFormSetting.Id == 11 || existingViewModel.ApplicationFormSetting.Id == 17)) || existingViewModel.Programme.Id == 4)
                    {
                        if (existingViewModel.ApplicantJambDetail != null)
                        {
                            var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                            existingViewModel.ApplicantJambDetail.ApplicationForm = newApplicationForm;
                            existingViewModel.ApplicantJambDetail.Person = viewModel.Person;
                            existingViewModel.ApplicantJambDetail.Subject1 =
                                existingViewModel.ApplicantJambDetail.Subject1;
                            existingViewModel.ApplicantJambDetail.Subject2 =
                                existingViewModel.ApplicantJambDetail.Subject2;
                            existingViewModel.ApplicantJambDetail.Subject3 =
                                existingViewModel.ApplicantJambDetail.Subject3;
                            existingViewModel.ApplicantJambDetail.Subject4 =
                                existingViewModel.ApplicantJambDetail.Subject4;
                            existingViewModel.ApplicantJambDetail.JambScore = existingViewModel.ApplicantJambDetail.JambScore;
                            applicantJambDetailLogic.Modify(existingViewModel.ApplicantJambDetail);
                        }
                    }
                    else if (existingViewModel.Programme.Id == 1 && (existingViewModel.ApplicationFormSetting.Id == 20 || existingViewModel.ApplicationFormSetting.Id == 8 || existingViewModel.ApplicationFormSetting.Id == 10 || existingViewModel.ApplicationFormSetting.Id == 12 || existingViewModel.ApplicationFormSetting.Id == 14 || existingViewModel.ApplicationFormSetting.Id == 18))
                    {
                        var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                        existingViewModel.ApplicantJambDetail.ApplicationForm = newApplicationForm;
                        existingViewModel.ApplicantJambDetail.Person = viewModel.Person;
                        existingViewModel.ApplicantJambDetail.JambScore = existingViewModel.ApplicantJambDetail.JambScore;
                        applicantJambDetailLogic.Modify(existingViewModel.ApplicantJambDetail);
                    }

                    var sponsorLogic = new SponsorLogic();
                    existingViewModel.Sponsor.Person = viewModel.Person;
                    sponsorLogic.Modify(existingViewModel.Sponsor);

                    var personLogic = new PersonLogic();
                    bool personModified = personLogic.Modify(existingViewModel.Person);

                    if (existingViewModel.SupplementaryCourse != null && existingViewModel.SupplementaryCourse.Department != null)
                    {
                        var supplementaryCourseLogic = new SupplementaryCourseLogic();
                        existingViewModel.SupplementaryCourse.Person = existingViewModel.Person;
                        existingViewModel.SupplementaryCourse.ApplicationForm = existingViewModel.ApplicationForm;
                        var supplementaryDept = supplementaryCourseLogic.GetBy(existingViewModel.Person);
                        if (supplementaryDept != null)
                        {
                            bool supplementaryModified = supplementaryCourseLogic.Modify(existingViewModel.SupplementaryCourse);
                        }
                        else
                        {
                            supplementaryCourseLogic.Create(existingViewModel.SupplementaryCourse);
                        }
                    }

                    //string CredentialjunkFilePath;
                    //string CredentialdestinationFilePath;
                    //SetPersonCredentialDestination(existingViewModel,out CredentialjunkFilePath,out CredentialdestinationFilePath);
                    //SavePersonPassport(CredentialjunkFilePath,CredentialdestinationFilePath,existingViewModel.Person);


                    //MODIFY O-LEVEL
                    existingViewModel.SecondSittingOLevelResult.ApplicationForm = existingViewModel.ApplicationForm;
                    existingViewModel.SecondSittingOLevelResult.Person = existingViewModel.Person;
                    existingViewModel.SecondSittingOLevelResult.PersonType = new PersonType { Id = 4 };
                    existingViewModel.SecondSittingOLevelResult.Sitting = new OLevelExamSitting { Id = 2 };


                    ModifyOlevelResult(existingViewModel.FirstSittingOLevelResult,
                        existingViewModel.FirstSittingOLevelResultDetails);
                    ModifyOlevelResult(existingViewModel.SecondSittingOLevelResult,
                        existingViewModel.SecondSittingOLevelResultDetails);

                    //set resject reason
                    if (existingViewModel.Programme.Id == 2 || existingViewModel.Programme.Id == 5 ||
                        existingViewModel.Programme.Id == 6 || (existingViewModel.Programme.Id == 1 && (existingViewModel.ApplicationFormSetting.Id == 20 || existingViewModel.ApplicationFormSetting.Id == 8 || existingViewModel.ApplicationFormSetting.Id == 10 || existingViewModel.ApplicationFormSetting.Id == 14)))
                    {
                        if (existingViewModel.PreviousEducation != null &&
                            existingViewModel.PreviousEducation.SchoolName != null)
                        {
                            var previousEducationLogic = new PreviousEducationLogic();
                            PreviousEducation previousEducation =
                                previousEducationLogic.GetModelBy(
                                    p => p.Application_Form_Id == existingViewModel.ApplicationForm.Id);
                            if (previousEducation != null)
                            {
                                existingViewModel.PreviousEducation.Id = previousEducation.Id;
                                existingViewModel.PreviousEducation.ApplicationForm = existingViewModel.ApplicationForm;
                                existingViewModel.PreviousEducation.Person = existingViewModel.Person;
                                existingViewModel.PreviousEducation.StartDate = new DateTime(existingViewModel.PreviousEducation.StartYear.Id, existingViewModel.PreviousEducation.StartMonth.Id, existingViewModel.PreviousEducation.StartDay.Id);
                                existingViewModel.PreviousEducation.EndDate = new DateTime(existingViewModel.PreviousEducation.EndYear.Id, existingViewModel.PreviousEducation.EndMonth.Id, existingViewModel.PreviousEducation.EndDay.Id);
                                previousEducationLogic.Modify(existingViewModel.PreviousEducation);
                            }
                        }

                        //newApplicationForm.Release = false;
                        //existingViewModel.AppliedCourse.Person = existingViewModel.Person;

                        //AdmissionCriteriaLogic admissionCriteriaLogic = new AdmissionCriteriaLogic();
                        //string rejectReason = admissionCriteriaLogic.EvaluateApplication(existingViewModel.AppliedCourse,existingViewModel.PreviousEducation);
                        ////string rejectReason = admissionCriteriaLogic.EvaluateApplication(existingViewModel.AppliedCourse);
                        //if(string.IsNullOrEmpty(rejectReason))
                        //{
                        //    newApplicationForm.Rejected = false;
                        //}
                        //else
                        //{
                        //    newApplicationForm.Rejected = true;
                        //    newApplicationForm.RejectReason = rejectReason;

                        //    if(!applicationFormLogic.SetRejectReason(newApplicationForm))
                        //    {
                        //        throw new Exception("Rejected! " + rejectReason);
                        //    }
                        //}

                        //existingViewModel.ApplicationForm = newApplicationForm;
                    }
                    transaction.Complete();
                }

                existingViewModel.ApplicationFormNumber = viewModel.ApplicationForm.Number;
                TempData["viewModel"] = existingViewModel;
                return RedirectToAction("AcknowledgmentSlip", "Form",
                    new { Area = "Applicant", FormId = Utility.Encrypt(existingViewModel.ApplicationForm.Id.ToString()) });

                //get newly created application form
            }
            catch (Exception ex)
            {
                newApplicationForm = null;
                viewModel.ApplicationForm = null;
                existingViewModel.ApplicationForm = null;

                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }

            TempData["viewModel"] = existingViewModel;
            return View(existingViewModel);
        }

        public ActionResult PostJAMBSlip(PostJambViewModel viewModel)
        {
            var existingViewModel = (PostJambViewModel)TempData["viewModel"];

            TempData["viewModel"] = existingViewModel;
            return View(existingViewModel);
        }

        public ActionResult ExamSlip()
        {
            var existingViewModel = (PostJambViewModel)TempData["viewModel"];

            TempData["viewModel"] = existingViewModel;
            return View(existingViewModel);
        }

        public ActionResult AcknowledgmentSlip(string FormId)
        {
            try
            {
                Int64 Fid = Convert.ToInt64(Utility.Decrypt(FormId));
                viewModel = new PostJambViewModel();

                var applicationFormSettingLogic = new ApplicationFormSettingLogic();
                var applicationFormLogic = new ApplicationFormLogic();
                var supplementaryCourseLogic = new SupplementaryCourseLogic();
                var appliedCourseLogic = new AppliedCourseLogic();
                var oLevelResultLogic = new OLevelResultLogic();
                var oLevelResultDetailLogic = new OLevelResultDetailLogic();
                var applicantJambDetailLogic = new ApplicantJambDetailLogic();
                var applicantSponsorLogic = new SponsorLogic();
                var personLogic = new PersonLogic();

                viewModel.ApplicationForm = applicationFormLogic.GetBy(Fid);
                if (viewModel.ApplicationForm != null && viewModel.ApplicationForm.Id > 0)
                {
                    viewModel.ApplicationFormSetting =
                        applicationFormSettingLogic.GetModelBy(
                            a => a.Application_Form_Setting_Id == viewModel.ApplicationForm.Setting.Id);
                    viewModel.Person = personLogic.GetModelBy(a => a.Person_Id == viewModel.ApplicationForm.Person.Id);
                    viewModel.ApplicantSponsor =
                        applicantSponsorLogic.GetModelBy(a => a.Person_Id == viewModel.ApplicationForm.Person.Id);
                    viewModel.SupplementaryCourse = supplementaryCourseLogic.GetBy(viewModel.Person);
                    viewModel.AppliedCourse = appliedCourseLogic.GetBy(viewModel.Person);
                    viewModel.ApplicantJambDetail = applicantJambDetailLogic.GetBy(viewModel.ApplicationForm);
                    viewModel.FirstSittingOLevelResult =
                        oLevelResultLogic.GetModelBy(
                            a => a.Application_Form_Id == viewModel.ApplicationForm.Id && a.O_Level_Exam_Sitting_Id == 1);
                    viewModel.SecondSittingOLevelResult =
                        oLevelResultLogic.GetModelBy(
                            a => a.Application_Form_Id == viewModel.ApplicationForm.Id && a.O_Level_Exam_Sitting_Id == 2);
                    if (viewModel.FirstSittingOLevelResult != null && viewModel.FirstSittingOLevelResult.Id > 0)
                    {
                        viewModel.FirstSittingOLevelResultDetails =
                            oLevelResultDetailLogic.GetModelsBy(
                                a => a.Applicant_O_Level_Result_Id == viewModel.FirstSittingOLevelResult.Id);
                    }
                    if (viewModel.SecondSittingOLevelResult != null && viewModel.SecondSittingOLevelResult.Id > 0)
                    {
                        viewModel.SecondSittingOLevelResultDetails =
                            oLevelResultDetailLogic.GetModelsBy(
                                a => a.Applicant_O_Level_Result_Id == viewModel.SecondSittingOLevelResult.Id);
                    }
                    viewModel.QRVerification = "http://portal.abiastateuniversity.edu.ng//Applicant/Form/AcknowledgmentSlip?FormId=" + FormId;
                    TempData.Clear();
                }
                return View(viewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private void SendSms(ApplicationForm applicationForm, Programme programme)
        //{
        //    try
        //    {
        //        send sms to applicant
        //        Sms textMessage = new Sms();
        //        textMessage.Sender = "";

        //        string message = "";
        //        string number = "234" + applicationForm.Person.MobilePhone;
        //        if (applicationForm.Rejected == false)
        //        {
        //            message = "Hello " + applicationForm.Person.LastName + ", Your application for Admission into the " +
        //                      programme.ShortName + " programme has been received. Your application no is " +
        //                      applicationForm.Number + " Thanks";
        //        }
        //        else
        //        {
        //            message = "Hello " + applicationForm.Person.LastName + ", Your application for Admission into the " +
        //                      programme.ShortName +
        //                      " programme has been rejected. You failed to meet the entry criteria. Thanks";
        //        }

        //        textMessage.SendSMS(number, message);

        //        var smsClient = new InfoBipSMS();
        //        var smsMessage = new InfoSMS();
        //        smsMessage.from = "ABSU";
        //        smsMessage.to = number;
        //        smsMessage.text = message;
        //        smsClient.SendSMS(smsMessage);
        //    }
        //    catch (Exception)
        //    {
        //        do nothing
        //    }
        //}
        public void SendSmartSms(ApplicationForm applicationForm, Programme programme)
        {
            try
            {
                if (applicationForm != null && programme != null)
                {
                    string message = "";
                    string number = "234" + applicationForm.Person.MobilePhone;
                    if (applicationForm.Rejected == false)
                    {
                        message = "Hello " + applicationForm.Person.LastName + ", Your application for Admission into the " + programme.ShortName + " programme has been received. Your application no is " + applicationForm.Number + " Thanks";
                    }
                    else
                    {
                        message = "Hello " + applicationForm.Person.LastName + ", Your application for Admission into the " + programme.ShortName + " programme has been rejected. You failed to meet the entry criteria. Thanks";
                    }

                    SmartSMS smartSMS = new SmartSMS();
                    smartSMS.sender = "ABSU";
                    smartSMS.to = number;
                    smartSMS.message = message;
                    smartSMS.type = 0;
                    smartSMS.routing = 3;
                    SmartSmsLogic smartSmsLogic = new SmartSmsLogic();
                    smartSmsLogic.SendSingleSmsMessage(smartSMS);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SavePersonPassport(string junkFilePath, string pathForSaving, Person person)
        {
            try
            {
                if (System.IO.File.Exists(junkFilePath))
                {
                    string folderPath = Path.GetDirectoryName(pathForSaving);
                    string mainFileName = person.Id + "__";

                    DeleteFileIfExist(folderPath, mainFileName);

                    System.IO.File.Move(junkFilePath, pathForSaving);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetPersonPassportDestination(PostJambViewModel existingViewModel, out string junkFilePath,
            out string destinationFilePath)
        {
            const string TILDA = "~";

            try
            {
                string passportUrl = existingViewModel.Person.ImageFileUrl;
                junkFilePath = Server.MapPath(TILDA + existingViewModel.Person.ImageFileUrl);
                destinationFilePath = junkFilePath.Replace("Junk", "Student");
                existingViewModel.Person.ImageFileUrl = passportUrl.Replace("Junk", "Student");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetPersonCredentialDestination(PostJambViewModel existingViewModel, out string junkFilePath,
            out string destinationFilePath)
        {
            const string TILDA = "~";

            try
            {
                string passportUrl = existingViewModel.FirstSittingOLevelResult.ScannedCopyUrl;
                junkFilePath = Server.MapPath(TILDA + existingViewModel.FirstSittingOLevelResult.ScannedCopyUrl);
                destinationFilePath = junkFilePath.Replace("Junk", "Credential");
                if (passportUrl != null)
                {
                    existingViewModel.FirstSittingOLevelResult.ScannedCopyUrl = passportUrl.Replace("Junk", "Credential");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        //[HttpPost]
        //public ActionResult PostJAMBPreview(PostJAMBViewModel viewModel)
        //{
        //    //const string CHANNEL = "INTERSWITCH";
        //    PostJAMBViewModel existingViewModel = (PostJAMBViewModel)TempData["viewModel"];

        //    try
        //    {
        //        //Role role = new Role() { Id = 6 };
        //        //PersonType personType = new PersonType() { Id = existingViewModel.ApplicationFormSetting.PersonType.Id };

        //        //Nationality nationality = new Nationality() { Id = 1 };
        //        //StudentType studentType = new StudentType() { Id = 3 };
        //        //StudentCategory studentCategory = new StudentCategory() { Id = 1 };

        //        //existingViewModel.Student.Role = role;
        //        //existingViewModel.Student.Type = studentType;
        //        //existingViewModel.Student.PersonType = personType;
        //        //existingViewModel.Student.Nationality = nationality;
        //        //existingViewModel.Student.Category = studentCategory;
        //        //existingViewModel.Student.DateEntered = DateTime.Now;

        //        //StudentLogic studentLogic = new StudentLogic();
        //        //Abundance_Nk.Model.Model.Student student = studentLogic.Add(existingViewModel.Student);
        //        //if (student != null && student.Id > 0)
        //        //{
        //        //    existingViewModel.Student.FullName = student.FullName;
        //        //}

        //        SponsorLogic sponsorLogic = new SponsorLogic();
        //        existingViewModel.Sponsor.Student = viewModel.Student;
        //        sponsorLogic.Create(existingViewModel.Sponsor);

        //        //    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
        //        //    existingViewModel.AppliedCourse.Student = student;
        //        //    appliedCourseLogic.Create(existingViewModel.AppliedCourse);


        //        if (existingViewModel.IsAwaitingResult == false)
        //        {
        //            OLevelResultLogic oLevelResultLogic = new OLevelResultLogic();
        //            OLevelResultDetailLogic oLevelResultDetailLogic = new OLevelResultDetailLogic();
        //            if (existingViewModel.FirstSittingOLevelResult != null && existingViewModel.FirstSittingOLevelResult.ExamNumber != null && existingViewModel.FirstSittingOLevelResult.Type != null && existingViewModel.FirstSittingOLevelResult.ExamYear > 0)
        //            {
        //                existingViewModel.FirstSittingOLevelResult.Student = viewModel.Student;
        //                existingViewModel.FirstSittingOLevelResult.Sitting = new OLevelExamSitting() { Id = 1 };
        //                OLevelResult firstSittingOLevelResult = oLevelResultLogic.Create(existingViewModel.FirstSittingOLevelResult);

        //                if (existingViewModel.FirstSittingOLevelResultDetails != null && existingViewModel.FirstSittingOLevelResultDetails.Count > 0 && firstSittingOLevelResult != null)
        //                {
        //                    List<OLevelResultDetail> olevelResultDetails = existingViewModel.FirstSittingOLevelResultDetails.Where(m => m.Grade != null && m.Subject != null).ToList();
        //                    foreach (OLevelResultDetail oLevelResultDetail in olevelResultDetails)
        //                    {
        //                        oLevelResultDetail.Header = firstSittingOLevelResult;
        //                    }

        //                    oLevelResultDetailLogic.Create(olevelResultDetails);
        //                }
        //            }

        //            if (existingViewModel.SecondSittingOLevelResult != null && existingViewModel.SecondSittingOLevelResult.ExamNumber != null && existingViewModel.SecondSittingOLevelResult.Type != null && existingViewModel.SecondSittingOLevelResult.ExamYear > 0)
        //            {
        //                List<OLevelResultDetail> olevelResultDetails = existingViewModel.SecondSittingOLevelResultDetails.Where(m => m.Grade != null && m.Subject != null).ToList();
        //                if (olevelResultDetails != null && olevelResultDetails.Count > 0)
        //                {
        //                    existingViewModel.SecondSittingOLevelResult.Student = viewModel.Student;
        //                    existingViewModel.SecondSittingOLevelResult.Sitting = new OLevelExamSitting() { Id = 2 };
        //                    OLevelResult secondSittingOLevelResult = oLevelResultLogic.Create(existingViewModel.SecondSittingOLevelResult);

        //                    if (existingViewModel.SecondSittingOLevelResultDetails != null && existingViewModel.SecondSittingOLevelResultDetails.Count > 0 && secondSittingOLevelResult != null)
        //                    {
        //                        foreach (OLevelResultDetail oLevelResultDetail in olevelResultDetails)
        //                        {
        //                            oLevelResultDetail.Header = secondSittingOLevelResult;
        //                        }

        //                        oLevelResultDetailLogic.Create(olevelResultDetails);
        //                    }
        //                }
        //            }
        //        }

        //        if (existingViewModel.Programme.Id == 3)
        //        {
        //            if (existingViewModel.PreviousEducation != null)
        //            {
        //                PreviousEducationLogic previousEducationLogic = new PreviousEducationLogic();
        //                existingViewModel.PreviousEducation.Student = viewModel.Student;
        //                previousEducationLogic.Create(existingViewModel.PreviousEducation);
        //            }
        //        }
        //        else
        //        {
        //            if (existingViewModel.StudentJambDetail != null)
        //            {
        //                StudentJambDetailLogic studentJambDetailLogic = new StudentJambDetailLogic();
        //                existingViewModel.StudentJambDetail.Student = viewModel.Student;
        //                studentJambDetailLogic.Create(existingViewModel.StudentJambDetail);
        //            }
        //        }

        //        //Payment payment = new Payment();
        //        //PaymentLogic paymentLogic = new PaymentLogic();
        //        //payment.PaymentMode = new PaymentMode() { Id = existingViewModel.ApplicationFormSetting.PaymentMode.Id };
        //        //payment.PaymentType = new PaymentType() { Id = existingViewModel.ApplicationFormSetting.PaymentType.Id };
        //        //payment.PersonType = new PersonType() { Id = existingViewModel.ApplicationFormSetting.PersonType.Id };
        //        //payment.Fee = new Fee() { Id = existingViewModel.ApplicationProgrammeFee.Fee.Id };
        //        //payment.DatePaid = DateTime.Now;

        //        //OnlinePayment newOnlinePayment = null;
        //        //Payment newPayment = paymentLogic.Create(payment);
        //        //if (newPayment != null)
        //        //{
        //        //    OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();
        //        //    OnlinePayment onlinePayment = new OnlinePayment();
        //        //    onlinePayment.Channel = CHANNEL;
        //        //    onlinePayment.Payment = newPayment;
        //        //    onlinePayment.TransactionNumber = newPayment.Id.ToString();
        //        //    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
        //        //}

        //        ApplicationForm applicationForm = new ApplicationForm();
        //        ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
        //        applicationForm.ProgrammeFee = new ApplicationProgrammeFee() { Id = existingViewModel.ApplicationProgrammeFee.Id };
        //        applicationForm.Setting = new ApplicationFormSetting() { Id = existingViewModel.ApplicationFormSetting.Id };
        //        applicationForm.DateSubmitted = DateTime.Now;
        //        applicationForm.Person = (Person)viewModel.Student;
        //        applicationForm.Payment = viewModel.Payment;
        //        applicationForm.ProgrammeFee.Programme = existingViewModel.AppliedCourse.FirstChoiceProgramme;
        //        applicationForm.IsAwaitingResult = existingViewModel.IsAwaitingResult;

        //        ApplicationForm newApplicationForm = applicationFormLogic.Create(applicationForm);
        //        existingViewModel.ApplicationFormNumber = newApplicationForm.Number;


        //        TempData["viewModel"] = existingViewModel;
        //        return RedirectToAction("PostJAMBSlip", "Form");

        //    }
        //    catch (Exception ex)
        //    {
        //        SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
        //    }

        //    TempData["viewModel"] = existingViewModel;
        //    return View(existingViewModel);
        //}

        //private void GenerateInvoice(PostJAMBFormPaymentViewModel postJAMBFormPaymentViewModel)
        //{
        //    try
        //    {
        //        Abundance_Nk.Model.Model.Student student = CreatePerson(postJAMBFormPaymentViewModel);
        //        Payment payment = CreatePayment(postJAMBFormPaymentViewModel);


        //    }
        //    catch (Exception ex)
        //    {
        //        SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
        //    }
        //}

        private Person CreatePerson(PostJAMBFormPaymentViewModel existingViewModel)
        {
            try
            {
                var role = new Role { Id = 6 };
                var personType = new PersonType { Id = existingViewModel.ApplicationFormSetting.PersonType.Id };
                var nationality = new Nationality { Id = 1 };

                existingViewModel.Person.Role = role;
                existingViewModel.Person.Type = personType;
                existingViewModel.Person.Nationality = nationality;
                existingViewModel.Person.DateEntered = DateTime.Now;

                var personLogic = new PersonLogic();
                Person person = personLogic.Create(existingViewModel.Person);
                if (person != null && person.Id > 0)
                {
                    existingViewModel.Person = person;
                }

                return person;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Payment CreatePayment(PostJAMBFormPaymentViewModel existingViewModel)
        {
            try
            {
                var payment = new Payment();
                var paymentLogic = new PaymentLogic();
                payment.PaymentMode = new PaymentMode { Id = existingViewModel.ApplicationFormSetting.PaymentMode.Id };
                payment.PaymentType = new PaymentType { Id = existingViewModel.ApplicationFormSetting.PaymentType.Id };
                payment.PersonType = new PersonType { Id = existingViewModel.ApplicationFormSetting.PersonType.Id };
                if (existingViewModel.ApplicationFormSetting.FeeType.Id > 0)
                {
                    payment.FeeType = new FeeType { Id = existingViewModel.ApplicationFormSetting.FeeType.Id };
                }
                else
                {
                    payment.FeeType = new FeeType { Id = existingViewModel.ApplicationProgrammeFee.FeeType.Id };
                }
                payment.DatePaid = DateTime.Now;
                payment.Person = existingViewModel.Person;
                payment.Session = existingViewModel.ApplicationFormSetting.Session;

                OnlinePayment newOnlinePayment = null;
                Payment newPayment = paymentLogic.Create(payment);
                if (newPayment != null)
                {
                    var channel = new PaymentChannel { Id = (int)PaymentChannel.Channels.Etranzact };
                    var onlinePaymentLogic = new OnlinePaymentLogic();
                    var onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = channel;
                    onlinePayment.Payment = newPayment;
                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                }

                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private AppliedCourse CreateAppliedCourse(PostJAMBFormPaymentViewModel viewModel)
        {
            try
            {
                var appliedCourse = new AppliedCourse();
                appliedCourse.Programme = viewModel.Programme;
                appliedCourse.Department = viewModel.AppliedCourse.Department;
                appliedCourse.Person = viewModel.Person;
                appliedCourse.Setting = viewModel.ApplicationFormSetting;

                var appliedCourseLogic = new AppliedCourseLogic();
                appliedCourse = appliedCourseLogic.Create(appliedCourse);
                return appliedCourse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void UpdateOLevelResultDetail(PostJambViewModel viewModel)
        {
            try
            {
                if (viewModel != null && viewModel.FirstSittingOLevelResultDetails != null &&
                    viewModel.FirstSittingOLevelResultDetails.Count > 0)
                {
                    foreach (
                        OLevelResultDetail firstSittingOLevelResultDetail in viewModel.FirstSittingOLevelResultDetails)
                    {
                        if (firstSittingOLevelResultDetail.Subject != null)
                        {
                            firstSittingOLevelResultDetail.Subject =
                                viewModel.OLevelSubjects.Where(m => m.Id == firstSittingOLevelResultDetail.Subject.Id)
                                    .SingleOrDefault();
                        }
                        if (firstSittingOLevelResultDetail.Grade != null)
                        {
                            firstSittingOLevelResultDetail.Grade =
                                viewModel.OLevelGrades.Where(m => m.Id == firstSittingOLevelResultDetail.Grade.Id)
                                    .SingleOrDefault();
                        }
                    }
                }

                if (viewModel != null && viewModel.SecondSittingOLevelResultDetails != null &&
                    viewModel.SecondSittingOLevelResultDetails.Count > 0)
                {
                    foreach (
                        OLevelResultDetail secondSittingOLevelResultDetail in viewModel.SecondSittingOLevelResultDetails
                        )
                    {
                        if (secondSittingOLevelResultDetail.Subject != null)
                        {
                            secondSittingOLevelResultDetail.Subject =
                                viewModel.OLevelSubjects.Where(m => m.Id == secondSittingOLevelResultDetail.Subject.Id)
                                    .SingleOrDefault();
                        }
                        if (secondSittingOLevelResultDetail.Grade != null)
                        {
                            secondSittingOLevelResultDetail.Grade =
                                viewModel.OLevelGrades.Where(m => m.Id == secondSittingOLevelResultDetail.Grade.Id)
                                    .SingleOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }
        }

        private void PopulateAllDropDowns()
        {
            var existingViewModel = (PostJambViewModel)TempData["viewModel"];
            var postJAMBFormPaymentViewModel = (PostJAMBFormPaymentViewModel)TempData["PostJAMBFormPaymentViewModel"];

            try
            {
                if (existingViewModel == null)
                {
                    viewModel = new PostJambViewModel();

                    ViewBag.StateId = viewModel.StateSelectList;
                    ViewBag.SexId = viewModel.SexSelectList;
                    ViewBag.FirstChoiceFacultyId = viewModel.FacultySelectList;
                    ViewBag.SecondChoiceFacultyId = viewModel.FacultySelectList;
                    ViewBag.LgaId = new SelectList(new List<LocalGovernment>(), ID, NAME);
                    ViewBag.RelationshipId = viewModel.RelationshipSelectList;
                    ViewBag.FirstSittingOLevelTypeId = viewModel.OLevelTypeSelectList;
                    ViewBag.SecondSittingOLevelTypeId = viewModel.OLevelTypeSelectList;
                    ViewBag.FirstSittingExamYearId = viewModel.ExamYearSelectList;
                    ViewBag.SecondSittingExamYearId = viewModel.ExamYearSelectList;
                    ViewBag.ReligionId = viewModel.ReligionSelectList;
                    ViewBag.AbilityId = viewModel.AbilitySelectList;
                    ViewBag.SupplementaryDepartmentId =
                        Utility.PopulateDepartmentSelectListItemBy(postJAMBFormPaymentViewModel.Programme);
                    ViewBag.DayOfBirthId = new SelectList(new List<Value>(), ID, NAME);
                    ViewBag.MonthOfBirthId = viewModel.MonthOfBirthSelectList;
                    ViewBag.YearOfBirthId = viewModel.YearOfBirthSelectList;

                    if (postJAMBFormPaymentViewModel.Programme.Id == 2 || postJAMBFormPaymentViewModel.Programme.Id == 5 ||
                        postJAMBFormPaymentViewModel.Programme.Id == 6)
                    {
                        ViewBag.PreviousEducationStartDayId = new SelectList(new List<Value>(), ID, NAME);
                        ViewBag.PreviousEducationStartMonthId = viewModel.PreviousEducationStartMonthSelectList;
                        ViewBag.PreviousEducationStartYearId = viewModel.PreviousEducationStartYearSelectList;

                        ViewBag.PreviousEducationEndDayId = new SelectList(new List<Value>(), ID, NAME);
                        ViewBag.PreviousEducationEndMonthId = viewModel.PreviousEducationEndMonthSelectList;
                        ViewBag.PreviousEducationEndYearId = viewModel.PreviousEducationEndYearSelectList;

                        ViewBag.ResultGradeId = viewModel.ResultGradeSelectList;
                        ViewBag.QualificationId = viewModel.EducationalQualificationSelectList;
                        ViewBag.ITDurationId = viewModel.ITDurationSelectList;
                    }
                    else if (postJAMBFormPaymentViewModel.Programme.Id == 1 &&
                             postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 6 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 8 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 10 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 12 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 14 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 18 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 20 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 22)
                    {

                        ViewBag.PreviousEducationStartDayId = new SelectList(new List<Value>(), ID, NAME);
                        ViewBag.PreviousEducationStartMonthId = viewModel.PreviousEducationStartMonthSelectList;
                        ViewBag.PreviousEducationStartYearId = viewModel.PreviousEducationStartYearSelectList;

                        ViewBag.PreviousEducationEndDayId = new SelectList(new List<Value>(), ID, NAME);
                        ViewBag.PreviousEducationEndMonthId = viewModel.PreviousEducationEndMonthSelectList;
                        ViewBag.PreviousEducationEndYearId = viewModel.PreviousEducationEndYearSelectList;

                        ViewBag.ResultGradeId = viewModel.ResultGradeSelectList;
                        ViewBag.QualificationId = viewModel.EducationalQualificationSelectList;
                        ViewBag.ITDurationId = viewModel.ITDurationSelectList;

                        if (postJAMBFormPaymentViewModel.ApplicantJambDetail != null)
                        {
                            ViewBag.JambScoreId = new SelectList(viewModel.JambScoreSelectList, "Value", "Text",
                                postJAMBFormPaymentViewModel.ApplicantJambDetail.JambScore);
                            if (postJAMBFormPaymentViewModel.ApplicantJambDetail.InstitutionChoice != null)
                            {
                                ViewBag.InstitutionChoiceId = new SelectList(viewModel.InstitutionChoiceSelectList,
                                    "Value", "Text",
                                    postJAMBFormPaymentViewModel.ApplicantJambDetail.InstitutionChoice.Id);
                            }
                            else
                            {
                                ViewBag.InstitutionChoiceId = viewModel.InstitutionChoiceSelectList;
                            }


                        }
                        else
                        {
                            ViewBag.JambScoreId = viewModel.JambScoreSelectList;
                            ViewBag.InstitutionChoiceId = viewModel.InstitutionChoiceSelectList;
                            ViewBag.Subject1Id = viewModel.OLevelSubjectSelectList;
                            ViewBag.Subject2Id = viewModel.OLevelSubjectSelectList;
                            ViewBag.Subject3Id = viewModel.OLevelSubjectSelectList;
                            ViewBag.Subject4Id = viewModel.OLevelSubjectSelectList;
                        }

                    }
                    else
                    {
                        if (postJAMBFormPaymentViewModel.ApplicantJambDetail != null)
                        {
                            ViewBag.JambScoreId = new SelectList(viewModel.JambScoreSelectList, "Value", "Text",
                                postJAMBFormPaymentViewModel.ApplicantJambDetail.JambScore);
                            if (postJAMBFormPaymentViewModel.ApplicantJambDetail.InstitutionChoice != null)
                            {
                                ViewBag.InstitutionChoiceId = new SelectList(viewModel.InstitutionChoiceSelectList,
                                    "Value", "Text",
                                    postJAMBFormPaymentViewModel.ApplicantJambDetail.InstitutionChoice.Id);
                            }
                            else
                            {
                                ViewBag.InstitutionChoiceId = viewModel.InstitutionChoiceSelectList;
                            }

                            if (postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject1 != null)
                            {
                                ViewBag.Subject1Id = new SelectList(viewModel.OLevelSubjectSelectList, VALUE, TEXT,
                                    postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject1.Id);
                            }
                            else
                            {
                                ViewBag.Subject1Id = viewModel.OLevelSubjectSelectList;
                            }

                            if (postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject2 != null)
                            {
                                ViewBag.Subject2Id = new SelectList(viewModel.OLevelSubjectSelectList, VALUE, TEXT,
                                    postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject2.Id);
                            }
                            else
                            {
                                ViewBag.Subject2Id = viewModel.OLevelSubjectSelectList;
                            }

                            if (postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject3 != null)
                            {
                                ViewBag.Subject3Id = new SelectList(viewModel.OLevelSubjectSelectList, VALUE, TEXT,
                                    postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject3.Id);
                            }
                            else
                            {
                                ViewBag.Subject3Id = viewModel.OLevelSubjectSelectList;
                            }

                            if (postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject4 != null)
                            {
                                ViewBag.Subject4Id = new SelectList(viewModel.OLevelSubjectSelectList, VALUE, TEXT,
                                    postJAMBFormPaymentViewModel.ApplicantJambDetail.Subject4.Id);
                            }
                            else
                            {
                                ViewBag.Subject4Id = viewModel.OLevelSubjectSelectList;
                            }
                        }
                        else
                        {
                            ViewBag.JambScoreId = viewModel.JambScoreSelectList;
                            ViewBag.InstitutionChoiceId = viewModel.InstitutionChoiceSelectList;
                            ViewBag.Subject1Id = viewModel.OLevelSubjectSelectList;
                            ViewBag.Subject2Id = viewModel.OLevelSubjectSelectList;
                            ViewBag.Subject3Id = viewModel.OLevelSubjectSelectList;
                            ViewBag.Subject4Id = viewModel.OLevelSubjectSelectList;
                        }
                    }

                    SetDefaultSelectedSittingSubjectAndGrade(viewModel);
                }
                else
                {
                    if (existingViewModel.Person.Religion == null)
                    {
                        existingViewModel.Person.Religion = new Religion();
                    }
                    if (existingViewModel.Person.State == null)
                    {
                        existingViewModel.Person.State = new State();
                    }
                    if (existingViewModel.Person.Sex == null)
                    {
                        existingViewModel.Person.Sex = new Sex();
                    }
                    if (existingViewModel.AppliedCourse.Programme == null)
                    {
                        existingViewModel.AppliedCourse.Programme = new Programme();
                    }
                    if (existingViewModel.Sponsor.Relationship == null)
                    {
                        existingViewModel.Sponsor.Relationship = new Relationship();
                    }
                    if (existingViewModel.FirstSittingOLevelResult.Type == null)
                    {
                        existingViewModel.FirstSittingOLevelResult.Type = new OLevelType();
                    }
                    if (existingViewModel.Applicant.Ability == null)
                    {
                        existingViewModel.Applicant.Ability = new Ability();
                    }
                    if (existingViewModel.Person.YearOfBirth == null)
                    {
                        existingViewModel.Person.YearOfBirth = new Value();
                    }
                    if (existingViewModel.Person.MonthOfBirth == null)
                    {
                        existingViewModel.Person.MonthOfBirth = new Value();
                    }
                    if (existingViewModel.Person.DayOfBirth == null)
                    {
                        existingViewModel.Person.DayOfBirth = new Value();
                    }

                    ViewBag.ReligionId = new SelectList(existingViewModel.ReligionSelectList, VALUE, TEXT,
                        existingViewModel.Person.Religion.Id);
                    ViewBag.StateId = new SelectList(existingViewModel.StateSelectList, VALUE, TEXT,
                        existingViewModel.Person.State.Id);
                    ViewBag.SexId = new SelectList(existingViewModel.SexSelectList, VALUE, TEXT,
                        existingViewModel.Person.Sex.Id);
                    ViewBag.ProgrammeId = new SelectList(existingViewModel.FacultySelectList, VALUE, TEXT,
                        existingViewModel.AppliedCourse.Programme.Id);
                    ViewBag.RelationshipId = new SelectList(existingViewModel.RelationshipSelectList, VALUE, TEXT,
                        existingViewModel.Sponsor.Relationship.Id);
                    ViewBag.FirstSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList, VALUE,
                        TEXT, existingViewModel.FirstSittingOLevelResult.Type.Id);
                    ViewBag.FirstSittingExamYearId = new SelectList(existingViewModel.ExamYearSelectList, VALUE, TEXT,
                        existingViewModel.FirstSittingOLevelResult.ExamYear);
                    ViewBag.SecondSittingExamYearId = new SelectList(existingViewModel.ExamYearSelectList, VALUE, TEXT,
                        existingViewModel.SecondSittingOLevelResult.ExamYear);
                    ViewBag.AbilityId = new SelectList(existingViewModel.AbilitySelectList, VALUE, TEXT,
                        existingViewModel.Applicant.Ability.Id);
                    if (existingViewModel.SupplementaryCourse != null && existingViewModel.SupplementaryCourse.Department != null)
                    {

                        ViewBag.SupplementaryDepartmentId =
                            new SelectList(existingViewModel.SupplementaryDepartmentSelectList, VALUE, TEXT,
                                existingViewModel.SupplementaryCourse.Department.Id);

                    }
                    else
                    {
                        ViewBag.SupplementaryDepartmentId =
                            Utility.PopulateDepartmentSelectListItemBy(postJAMBFormPaymentViewModel.Programme);
                    }


                    SetDateOfBirthDropDown(existingViewModel);

                    if (postJAMBFormPaymentViewModel.Programme.Id == 2 || postJAMBFormPaymentViewModel.Programme.Id == 5 ||
                        postJAMBFormPaymentViewModel.Programme.Id == 6)
                    {
                        SetPreviousEducationEndDateDropDowns(existingViewModel);
                        SetPreviousEducationStartDateDropDowns(existingViewModel);
                        ViewBag.ResultGradeId = new SelectList(existingViewModel.ResultGradeSelectList, VALUE, TEXT,
                            existingViewModel.PreviousEducation.ResultGrade.Id);
                        ViewBag.QualificationId = new SelectList(existingViewModel.EducationalQualificationSelectList,
                            VALUE, TEXT, existingViewModel.PreviousEducation.Qualification.Id);
                        ViewBag.ITDurationId = new SelectList(existingViewModel.ITDurationSelectList, VALUE, TEXT,
                            existingViewModel.PreviousEducation.ITDuration.Id);
                    }
                    else if (postJAMBFormPaymentViewModel.Programme.Id == 1 &&
                             postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 6 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 8 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 10 || postJAMBFormPaymentViewModel.ApplicationFormSetting.Id == 14)
                    {
                        SetPreviousEducationEndDateDropDowns(existingViewModel);
                        SetPreviousEducationStartDateDropDowns(existingViewModel);
                        ViewBag.ResultGradeId = new SelectList(existingViewModel.ResultGradeSelectList, VALUE, TEXT,
                            existingViewModel.PreviousEducation.ResultGrade.Id);
                        ViewBag.QualificationId = new SelectList(existingViewModel.EducationalQualificationSelectList,
                            VALUE, TEXT, existingViewModel.PreviousEducation.Qualification.Id);
                        ViewBag.ITDurationId = new SelectList(existingViewModel.ITDurationSelectList, VALUE, TEXT,
                            existingViewModel.PreviousEducation.ITDuration.Id);

                        ViewBag.InstitutionChoiceId = new SelectList(existingViewModel.InstitutionChoiceSelectList,
                           VALUE, TEXT, existingViewModel.ApplicantJambDetail.InstitutionChoice.Id);
                        ViewBag.JambScoreId = new SelectList(existingViewModel.JambScoreSelectList, VALUE, TEXT,
                            existingViewModel.ApplicantJambDetail.JambScore);
                    }
                    else
                    {
                        ViewBag.InstitutionChoiceId = new SelectList(existingViewModel.InstitutionChoiceSelectList,
                            VALUE, TEXT, existingViewModel.ApplicantJambDetail.InstitutionChoice.Id);
                        ViewBag.JambScoreId = new SelectList(existingViewModel.JambScoreSelectList, VALUE, TEXT,
                            existingViewModel.ApplicantJambDetail.JambScore);
                        if (existingViewModel.ApplicantJambDetail.Subject1 != null)
                        {
                            ViewBag.Subject1Id = new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT,
                                existingViewModel.ApplicantJambDetail.Subject1.Id);
                        }
                        else
                        {
                            ViewBag.Subject1Id = existingViewModel.OLevelSubjectSelectList;
                        }

                        if (existingViewModel.ApplicantJambDetail.Subject2 != null)
                        {
                            ViewBag.Subject2Id = new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT,
                                existingViewModel.ApplicantJambDetail.Subject2.Id);
                        }
                        else
                        {
                            ViewBag.Subject2Id = existingViewModel.OLevelSubjectSelectList;
                        }

                        if (existingViewModel.ApplicantJambDetail.Subject3 != null)
                        {
                            ViewBag.Subject3Id = new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT,
                                existingViewModel.ApplicantJambDetail.Subject3.Id);
                        }
                        else
                        {
                            ViewBag.Subject3Id = existingViewModel.OLevelSubjectSelectList;
                        }

                        if (existingViewModel.ApplicantJambDetail.Subject4 != null)
                        {
                            ViewBag.Subject4Id = new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT,
                                existingViewModel.ApplicantJambDetail.Subject4.Id);
                        }
                        else
                        {
                            ViewBag.Subject4Id = existingViewModel.OLevelSubjectSelectList;
                        }
                    }

                    if (existingViewModel.Person.LocalGovernment != null &&
                        existingViewModel.Person.LocalGovernment.Id > 0)
                    {
                        ViewBag.LgaId = new SelectList(existingViewModel.LocalGovernmentSelectList, VALUE, TEXT,
                            existingViewModel.Person.LocalGovernment.Id);
                    }
                    else
                    {
                        ViewBag.LgaId = new SelectList(new List<LocalGovernment>(), VALUE, TEXT);
                    }

                    if (existingViewModel.SecondSittingOLevelResult.Type != null)
                    {
                        ViewBag.SecondSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList, VALUE,
                            TEXT, existingViewModel.SecondSittingOLevelResult.Type.Id);
                    }
                    else
                    {
                        ViewBag.SecondSittingOLevelTypeId = new SelectList(existingViewModel.OLevelTypeSelectList, VALUE,
                            TEXT, 0);
                    }

                    SetSelectedSittingSubjectAndGrade(existingViewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }
        }

        private void SetDateOfBirthDropDown(PostJambViewModel existingViewModel)
        {
            try
            {
                ViewBag.MonthOfBirthId = new SelectList(existingViewModel.MonthOfBirthSelectList, VALUE, TEXT,
                    existingViewModel.Person.MonthOfBirth.Id);
                ViewBag.YearOfBirthId = new SelectList(existingViewModel.YearOfBirthSelectList, VALUE, TEXT,
                    existingViewModel.Person.YearOfBirth.Id);
                if ((existingViewModel.DayOfBirthSelectList == null || existingViewModel.DayOfBirthSelectList.Count == 0) &&
                    (existingViewModel.Person.MonthOfBirth.Id > 0 && existingViewModel.Person.YearOfBirth.Id > 0))
                {
                    existingViewModel.DayOfBirthSelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.Person.MonthOfBirth,
                            existingViewModel.Person.YearOfBirth);
                }
                else
                {
                    if (existingViewModel.DayOfBirthSelectList != null && existingViewModel.Person.DayOfBirth.Id > 0)
                    {
                        ViewBag.DayOfBirthId = new SelectList(existingViewModel.DayOfBirthSelectList, VALUE, TEXT,
                            existingViewModel.Person.DayOfBirth.Id);
                    }
                    else if (existingViewModel.DayOfBirthSelectList != null &&
                             existingViewModel.Person.DayOfBirth.Id <= 0)
                    {
                        ViewBag.DayOfBirthId = existingViewModel.DayOfBirthSelectList;
                    }
                    else if (existingViewModel.DayOfBirthSelectList == null)
                    {
                        existingViewModel.DayOfBirthSelectList = new List<SelectListItem>();
                        ViewBag.DayOfBirthId = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.Person.DayOfBirth != null && existingViewModel.Person.DayOfBirth.Id > 0)
                {
                    ViewBag.DayOfBirthId = new SelectList(existingViewModel.DayOfBirthSelectList, VALUE, TEXT,
                        existingViewModel.Person.DayOfBirth.Id);
                }
                else
                {
                    ViewBag.DayOfBirthId = existingViewModel.DayOfBirthSelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetPreviousEducationStartDateDropDowns(PostJambViewModel existingViewModel)
        {
            try
            {
                ViewBag.PreviousEducationStartMonthId =
                    new SelectList(existingViewModel.PreviousEducationStartMonthSelectList, VALUE, TEXT,
                        existingViewModel.PreviousEducation.StartMonth.Id);
                ViewBag.PreviousEducationStartYearId =
                    new SelectList(existingViewModel.PreviousEducationStartYearSelectList, VALUE, TEXT,
                        existingViewModel.PreviousEducation.StartYear.Id);
                if ((existingViewModel.PreviousEducationStartDaySelectList == null ||
                     existingViewModel.PreviousEducationStartDaySelectList.Count == 0) &&
                    (existingViewModel.PreviousEducation.StartMonth.Id > 0 &&
                     existingViewModel.PreviousEducation.StartYear.Id > 0))
                {
                    existingViewModel.PreviousEducationStartDaySelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.PreviousEducation.StartMonth,
                            existingViewModel.PreviousEducation.StartYear);
                }
                else
                {
                    if (existingViewModel.PreviousEducationStartDaySelectList != null &&
                        existingViewModel.PreviousEducation.StartDay.Id > 0)
                    {
                        ViewBag.PreviousEducationStartDayId =
                            new SelectList(existingViewModel.PreviousEducationStartDaySelectList, VALUE, TEXT,
                                existingViewModel.PreviousEducation.StartDay.Id);
                    }
                    else if (existingViewModel.PreviousEducationStartDaySelectList != null &&
                             existingViewModel.PreviousEducation.StartDay.Id <= 0)
                    {
                        ViewBag.PreviousEducationStartDayId = existingViewModel.PreviousEducationStartDaySelectList;
                    }
                    else if (existingViewModel.PreviousEducationStartDaySelectList == null)
                    {
                        existingViewModel.PreviousEducationStartDaySelectList = new List<SelectListItem>();
                        ViewBag.PreviousEducationStartDayId = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.PreviousEducation.StartDay != null &&
                    existingViewModel.PreviousEducation.StartDay.Id > 0)
                {
                    ViewBag.PreviousEducationStartDayId =
                        new SelectList(existingViewModel.PreviousEducationStartDaySelectList, VALUE, TEXT,
                            existingViewModel.PreviousEducation.StartDay.Id);
                }
                else
                {
                    ViewBag.PreviousEducationStartDayId = existingViewModel.PreviousEducationStartDaySelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetPreviousEducationEndDateDropDowns(PostJambViewModel existingViewModel)
        {
            try
            {
                ViewBag.PreviousEducationEndMonthId =
                    new SelectList(existingViewModel.PreviousEducationEndMonthSelectList, VALUE, TEXT,
                        existingViewModel.PreviousEducation.EndMonth.Id);
                ViewBag.PreviousEducationEndYearId = new SelectList(
                    existingViewModel.PreviousEducationEndYearSelectList, VALUE, TEXT,
                    existingViewModel.PreviousEducation.EndYear.Id);
                if ((existingViewModel.PreviousEducationEndDaySelectList == null ||
                     existingViewModel.PreviousEducationEndDaySelectList.Count == 0) &&
                    (existingViewModel.PreviousEducation.EndMonth.Id > 0 &&
                     existingViewModel.PreviousEducation.EndYear.Id > 0))
                {
                    existingViewModel.PreviousEducationEndDaySelectList =
                        Utility.PopulateDaySelectListItem(existingViewModel.PreviousEducation.EndMonth,
                            existingViewModel.PreviousEducation.EndYear);
                }
                else
                {
                    if (existingViewModel.PreviousEducationEndDaySelectList != null &&
                        existingViewModel.PreviousEducation.EndDay.Id > 0)
                    {
                        ViewBag.PreviousEducationEndDayId =
                            new SelectList(existingViewModel.PreviousEducationEndDaySelectList, VALUE, TEXT,
                                existingViewModel.PreviousEducation.EndDay.Id);
                    }
                    else if (existingViewModel.PreviousEducationEndDaySelectList != null &&
                             existingViewModel.PreviousEducation.EndDay.Id <= 0)
                    {
                        ViewBag.PreviousEducationEndDayId = existingViewModel.PreviousEducationEndDaySelectList;
                    }
                    else if (existingViewModel.PreviousEducationEndDaySelectList == null)
                    {
                        existingViewModel.PreviousEducationEndDaySelectList = new List<SelectListItem>();
                        ViewBag.PreviousEducationEndDayId = new List<SelectListItem>();
                    }
                }

                if (existingViewModel.PreviousEducation.EndDay != null &&
                    existingViewModel.PreviousEducation.EndDay.Id > 0)
                {
                    ViewBag.PreviousEducationEndDayId =
                        new SelectList(existingViewModel.PreviousEducationEndDaySelectList, VALUE, TEXT,
                            existingViewModel.PreviousEducation.EndDay.Id);
                }
                else
                {
                    ViewBag.PreviousEducationEndDayId = existingViewModel.PreviousEducationEndDaySelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetDefaultSelectedSittingSubjectAndGrade(PostJambViewModel viewModel)
        {
            try
            {
                if (viewModel != null && viewModel.FirstSittingOLevelResultDetails != null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        ViewData["FirstSittingOLevelSubjectId" + i] = viewModel.OLevelSubjectSelectList;
                        ViewData["FirstSittingOLevelGradeId" + i] = viewModel.OLevelGradeSelectList;
                    }
                }

                if (viewModel != null && viewModel.SecondSittingOLevelResultDetails != null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        ViewData["SecondSittingOLevelSubjectId" + i] = viewModel.OLevelSubjectSelectList;
                        ViewData["SecondSittingOLevelGradeId" + i] = viewModel.OLevelGradeSelectList;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }
        }

        private void SetSelectedSittingSubjectAndGrade(PostJambViewModel existingViewModel)
        {
            try
            {
                if (existingViewModel != null && existingViewModel.FirstSittingOLevelResultDetails != null &&
                    existingViewModel.FirstSittingOLevelResultDetails.Count > 0)
                {
                    int i = 0;
                    foreach (
                        OLevelResultDetail firstSittingOLevelResultDetail in
                            existingViewModel.FirstSittingOLevelResultDetails)
                    {
                        if (firstSittingOLevelResultDetail.Subject != null &&
                            firstSittingOLevelResultDetail.Grade != null)
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT,
                                    firstSittingOLevelResultDetail.Subject.Id);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, VALUE, TEXT,
                                    firstSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            ViewData["FirstSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT, 0);
                            ViewData["FirstSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, VALUE, TEXT, 0);
                        }

                        i++;
                    }
                }

                if (existingViewModel != null && existingViewModel.SecondSittingOLevelResultDetails != null &&
                    existingViewModel.SecondSittingOLevelResultDetails.Count > 0)
                {
                    int i = 0;
                    foreach (
                        OLevelResultDetail secondSittingOLevelResultDetail in
                            existingViewModel.SecondSittingOLevelResultDetails)
                    {
                        if (secondSittingOLevelResultDetail.Subject != null &&
                            secondSittingOLevelResultDetail.Grade != null)
                        {
                            ViewData["SecondSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT,
                                    secondSittingOLevelResultDetail.Subject.Id);
                            ViewData["SecondSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, VALUE, TEXT,
                                    secondSittingOLevelResultDetail.Grade.Id);
                        }
                        else
                        {
                            ViewData["SecondSittingOLevelSubjectId" + i] =
                                new SelectList(existingViewModel.OLevelSubjectSelectList, VALUE, TEXT, 0);
                            ViewData["SecondSittingOLevelGradeId" + i] =
                                new SelectList(existingViewModel.OLevelGradeSelectList, VALUE, TEXT, 0);
                        }

                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }
        }

        public JsonResult GetLocalGovernmentsByState(string id)
        {
            try
            {
                var lgaLogic = new LocalGovernmentLogic();

                Expression<Func<LOCAL_GOVERNMENT, bool>> selector = l => l.State_Id == id;
                List<LocalGovernment> lgas = lgaLogic.GetModelsBy(selector);

                return Json(new SelectList(lgas, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public JsonResult GetDayOfBirthBy(string monthId, string yearId)
        {
            try
            {
                if (string.IsNullOrEmpty(monthId) || string.IsNullOrEmpty(yearId))
                {
                    return null;
                }

                var month = new Value { Id = Convert.ToInt32(monthId) };
                var year = new Value { Id = Convert.ToInt32(yearId) };
                List<Value> days = Utility.GetNumberOfDaysInMonth(month, year);

                return Json(new SelectList(days, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public JsonResult GetDepartmentByProgrammeId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var programme = new Programme { Id = Convert.ToInt32(id) };

                var departmentLogic = new DepartmentLogic();
                List<Department> departments = departmentLogic.GetDepartmentForApplicationBy(programme);

                return Json(new SelectList(departments, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetDepartmentOptionByDepartment(string id, string programmeid)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var department = new Department { Id = Convert.ToInt32(id) };
                var programme = new Programme { Id = Convert.ToInt32(programmeid) };
                var departmentLogic = new DepartmentOptionLogic();
                List<DepartmentOption> departmentOptions = departmentLogic.GetBy(department, programme);

                return Json(new SelectList(departmentOptions, ID, NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public virtual ActionResult UploadFile(FormCollection form)
        {
            HttpPostedFileBase file = Request.Files["MyFile"];

            bool isUploaded = false;
            string personId = form["Person.Id"];
            string passportUrl = form["Person.ImageFileUrl"];
            string message = "File upload failed";

            string path = null;
            string imageUrl = null;
            string imageUrlDisplay = null;

            try
            {
                if (file != null && file.ContentLength != 0)
                {
                    var fileInfo = new FileInfo(file.FileName);
                    string fileExtension = fileInfo.Extension;
                    string newFile = personId + "__";
                    string newFileName = newFile +
                                         DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
                                         fileExtension;

                    decimal sizeAllowed = 50; //50kb
                    string invalidFileMessage = InvalidFile(file.ContentLength, fileExtension, sizeAllowed);
                    if (!string.IsNullOrEmpty(invalidFileMessage))
                    {
                        isUploaded = false;
                        TempData["imageUrl"] = null;
                        return Json(new { isUploaded, message = invalidFileMessage, imageUrl = passportUrl }, "text/html",
                            JsonRequestBehavior.AllowGet);
                    }

                    string pathForSaving = Server.MapPath("~/Content/Junk");
                    if (CreateFolderIfNeeded(pathForSaving))
                    {
                        DeleteFileIfExist(pathForSaving, personId);

                        file.SaveAs(Path.Combine(pathForSaving, newFileName));

                        isUploaded = true;
                        message = "File uploaded successfully!";

                        path = Path.Combine(pathForSaving, newFileName);
                        if (path != null)
                        {
                            imageUrl = "/Content/Junk/" + newFileName;
                            imageUrlDisplay = appRoot + imageUrl + "?t=" + DateTime.Now;

                            //imageUrlDisplay = "/ilaropoly" + imageUrl + "?t=" + DateTime.Now;
                            TempData["imageUrl"] = imageUrl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = string.Format("File upload failed: {0}", ex.Message);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }

            return Json(new { isUploaded, message, imageUrl = imageUrlDisplay }, "text/html", JsonRequestBehavior.AllowGet);
        }

        private string InvalidFile(decimal uploadedFileSize, string fileExtension, decimal sizeAllowed)
        {
            try
            {
                string message = null;
                decimal oneKiloByte = 1024;
                decimal maximumFileSize = sizeAllowed * oneKiloByte;

                decimal actualFileSizeToUpload = Math.Round(uploadedFileSize / oneKiloByte, 1);
                if (InvalidFileType(fileExtension))
                {
                    message = "File type '" + fileExtension +
                              "' is invalid! File type must be any of the following: .jpg, .jpeg, .png or .jif ";
                }
                else if (actualFileSizeToUpload > (maximumFileSize / oneKiloByte))
                {
                    message = "Your file size of " + actualFileSizeToUpload.ToString("0.#") +
                              " Kb is too large, maximum allowed size is " + (maximumFileSize / oneKiloByte) + " Kb";
                }

                return message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidFileType(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return false;
                case ".png":
                    return false;
                case ".gif":
                    return false;
                case ".jpeg":
                    return false;
                default:
                    return true;
            }
        }

        private void DeleteFileIfExist(string folderPath, string fileName)
        {
            try
            {
                string wildCard = fileName + "*.*";
                IEnumerable<string> files = Directory.EnumerateFiles(folderPath, wildCard, SearchOption.TopDirectoryOnly);

                if (files != null && files.Count() > 0)
                {
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CreateFolderIfNeeded(string path)
        {
            try
            {
                bool result = true;
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception)
                    {
                        /*TODO: You must process this exception.*/
                        result = false;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public virtual ActionResult UploadCredentialFile(FormCollection form)
        {
            HttpPostedFileBase file = Request.Files["MyCredentialFile"];

            bool isUploaded = false;
            string personId = form["Person.Id"];
            string passportUrl = form["Person.ImageFileUrl"];
            string message = "File upload failed";

            string path = null;
            string imageUrl = null;
            string imageUrlDisplay = null;

            try
            {
                if (file != null && file.ContentLength != 0)
                {
                    var fileInfo = new FileInfo(file.FileName);
                    string fileExtension = fileInfo.Extension;
                    string newFile = personId + "_credential_";
                    string newFileName = newFile +
                                         DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
                                         fileExtension;

                    decimal sizeAllowed = 500;
                    string invalidFileMessage = InvalidFile(file.ContentLength, fileExtension, sizeAllowed);
                    if (!string.IsNullOrEmpty(invalidFileMessage))
                    {
                        isUploaded = false;
                        TempData["CredentialimageUrl"] = null;
                        return Json(new { isUploaded, message = invalidFileMessage, imageUrl = passportUrl }, "text/html",
                            JsonRequestBehavior.AllowGet);
                    }

                    string pathForSaving = Server.MapPath("~/Content/Junk/Credential");
                    if (CreateFolderIfNeeded(pathForSaving))
                    {
                        DeleteFileIfExist(pathForSaving, personId);

                        file.SaveAs(Path.Combine(pathForSaving, newFileName));

                        isUploaded = true;
                        message = "File uploaded successfully!";

                        path = Path.Combine(pathForSaving, newFileName);
                        if (path != null)
                        {
                            imageUrl = "/Content/Junk/Credential/" + newFileName;
                            imageUrlDisplay = appRoot + imageUrl + "?t=" + DateTime.Now;

                            //imageUrlDisplay = "/ilaropoly" + imageUrl + "?t=" + DateTime.Now;
                            TempData["CredentialimageUrl"] = imageUrl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = string.Format("File upload failed: {0}", ex.Message);
            }

            return Json(new { isUploaded, message, imageUrl = imageUrlDisplay }, "text/html", JsonRequestBehavior.AllowGet);
        }

        private void ModifyOlevelResult(OLevelResult oLevelResult, List<OLevelResultDetail> oLevelResultDetails)
        {
            try
            {
                var oLevelResultDetailLogic = new OLevelResultDetailLogic();
                if (oLevelResult != null && oLevelResult.ExamNumber != null && oLevelResult.Type != null &&
                    oLevelResult.ExamYear > 0)
                {
                    if (oLevelResult != null && oLevelResult.Id > 0)
                    {
                        oLevelResultDetailLogic.DeleteBy(oLevelResult);
                        var oLevelResultLogic = new OLevelResultLogic();
                        oLevelResultLogic.Modify(oLevelResult);
                    }
                    else
                    {
                        var oLevelResultLogic = new OLevelResultLogic();
                        OLevelResult newOLevelResult = oLevelResultLogic.Create(oLevelResult);
                        oLevelResult.Id = newOLevelResult.Id;
                    }

                    if (oLevelResultDetails != null && oLevelResultDetails.Count > 0)
                    {
                        List<OLevelResultDetail> olevelResultDetails =
                            oLevelResultDetails.Where(
                                m => m.Grade != null && m.Grade.Id > 0 && m.Subject != null && m.Subject.Id > 0)
                                .ToList();
                        foreach (OLevelResultDetail oLevelResultDetail in olevelResultDetails)
                        {
                            oLevelResultDetail.Header = oLevelResult;
                            oLevelResultDetailLogic.Create(oLevelResultDetail);
                        }

                        //oLevelResultDetailLogic.Create(olevelResultDetails);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult UploadImage(FormCollection formData)
        {
            ApplicationFormJsonModel jsonModel = new ApplicationFormJsonModel();
            try
            {
                HttpPostedFileBase file = Request.Files["image"];

                string personId = formData["personId"].ToString();

                if (file != null && file.ContentLength != 0)
                {
                    FileInfo fileInfo = new FileInfo(file.FileName);
                    string fileExtension = fileInfo.Extension;
                    string newFile = personId + "__";
                    string newFileName = newFile + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + fileExtension;

                    decimal sizeAllowed = 50; //50kb
                    string invalidFileMessage = InvalidFile(file.ContentLength, fileExtension, sizeAllowed);
                    if (!string.IsNullOrEmpty(invalidFileMessage))
                    {
                        jsonModel.IsError = true;
                        jsonModel.Message = invalidFileMessage;
                        return Json(jsonModel, JsonRequestBehavior.AllowGet);
                    }

                    string pathForSaving = Server.MapPath("~/Content/Junk");
                    if (CreateFolderIfNeeded(pathForSaving))
                    {
                        DeleteFileIfExist(pathForSaving, personId);

                        file.SaveAs(Path.Combine(pathForSaving, newFileName));

                        PersonLogic personLogic = new PersonLogic();
                        long myPersonId = Convert.ToInt64(personId);
                        Person person = personLogic.GetModelBy(p => p.Person_Id == myPersonId);

                        person.ImageFileUrl = "/Content/Junk/" + newFileName;

                        personLogic.Modify(person);

                        jsonModel.IsError = false;
                        jsonModel.Message = "File uploaded successfully!";

                        string path = Path.Combine(pathForSaving, newFileName);
                        if (path != null)
                        {
                            jsonModel.ImageFileUrl = "/Content/Junk/" + newFileName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                jsonModel.IsError = true;
                jsonModel.Message = "Error! " + ex.Message;
            }

            return Json(jsonModel, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveImage(FormCollection formData)
        {
            ApplicationFormJsonModel jsonModel = new ApplicationFormJsonModel();
            try
            {
                HttpPostedFileBase file = Request.Files["image"];

                string personId = formData["personId"].ToString();

                if (file != null && file.ContentLength != 0)
                {
                    jsonModel.IsError = false;
                    jsonModel.Message = "File removed successfully!";
                }
            }
            catch (Exception ex)
            {
                jsonModel.IsError = true;
                jsonModel.Message = "Error! " + ex.Message;
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }

            return Json(jsonModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InitializeBankPayment(string InvoiceNumber)
        {
            PostJAMBFormPaymentViewModel bankItViewModel = new PostJAMBFormPaymentViewModel();
            try
            {

                if (InvoiceNumber == null)
                {
                    SetMessage("Kindly regenerate the invoice to continue! ", Message.Category.Error);
                    return RedirectToAction("PostJambFormInvoiceGeneration");
                }
                PaymentLogic paymentLogic = new PaymentLogic();
                Payment payment = paymentLogic.GetBy(InvoiceNumber);
                var appliedCourse = new AppliedCourse();
                var appliedCourseLogic = new AppliedCourseLogic();
                appliedCourse = appliedCourseLogic.GetBy(payment.Person);

                var level = new Level { Id = 1 };
                payment.FeeDetails = paymentLogic.SetFeeDetails(payment, appliedCourse.Programme.Id,
                    level.Id, payment.PaymentMode.Id, appliedCourse.Department.Id, payment.Session.Id);

                decimal amount = payment.FeeDetails.Sum(p => p.Fee.Amount);

                System.Security.Cryptography.SHA256Managed sha256 = new System.Security.Cryptography.SHA256Managed();
                Byte[] EncryptedSHA512 = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(amount + ConfigurationManager.AppSettings["EtranzactTerminalID"].ToString() + payment.InvoiceNumber +
                                                                                               ConfigurationManager.AppSettings["EtranzactResponseUrl"].ToString() + ConfigurationManager.AppSettings["EtranzactSecretKey"].ToString()));
                sha256.Clear();
                string cksum = BitConverter.ToString(EncryptedSHA512).Replace("-", "").ToLower();

                bankItViewModel.BankItRequest = new BankItRequest();
                bankItViewModel.BankItRequest.TerminalId = ConfigurationManager.AppSettings["EtranzactTerminalID"].ToString();
                bankItViewModel.BankItRequest.TransactionId = payment.InvoiceNumber;
                bankItViewModel.BankItRequest.Amount = amount.ToString();
                bankItViewModel.BankItRequest.Description = payment.FeeType.Name;
                bankItViewModel.BankItRequest.NotificationUrl = ConfigurationManager.AppSettings["EtranzactResponseUrl"].ToString();
                bankItViewModel.BankItRequest.ResponseUrl = ConfigurationManager.AppSettings["EtranzactResponseUrl"].ToString();
                bankItViewModel.BankItRequest.Checksum = cksum;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
                Bugsnag.AspNet.Client.Current.Notify(ex);
            }

            TempData["PostJAMBFormPaymentViewModel"] = bankItViewModel;
            return View(bankItViewModel);
        }
        private void GetAmountCommission(out decimal addOn, out decimal commission, Programme programme, FeeType feeType, Session session)
        {
            PaymentPaystackCommission paymentPaystackCommission = new PaymentPaystackCommission();
            try
            {

                if (feeType != null && session != null && programme != null)
                {
                    PaymentPayStackCommissionLogic paymentPayStackCommissionLogic = new PaymentPayStackCommissionLogic();
                    paymentPaystackCommission = paymentPayStackCommissionLogic.GetModelBy(s => s.FeeType_Id == feeType.Id && s.Session_Id == session.Id && s.Programme_Id == programme.Id && s.Activated == true);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            commission = paymentPaystackCommission != null ? paymentPaystackCommission.Amount : 0;
            addOn = paymentPaystackCommission != null ? (decimal)paymentPaystackCommission.AddOnFee : 0;
        }

        public Payment ReturnMonnfyPayment(string confirmationOrderNumber)
        {
            Payment payment = null;
            try
            {
                if (confirmationOrderNumber.StartsWith("AB"))
                {
                    string MonnifyURL = ConfigurationManager.AppSettings["MonnifyUrl"].ToString();
                    string MonnifyUser = ConfigurationManager.AppSettings["MonnifyApiKey"].ToString();
                    string MonnifySecrect = ConfigurationManager.AppSettings["MonnifyContractCode"].ToString();
                    string MonnifyCode = ConfigurationManager.AppSettings["MonnifyCode"].ToString();
                    PaymentMonnifyLogic paymentMonnifyLogic = new PaymentMonnifyLogic(MonnifyURL, MonnifyUser, MonnifySecrect, MonnifyCode);
                    var PaymentMonnify = paymentMonnifyLogic.GetInvoiceStatus(confirmationOrderNumber);
                    if (PaymentMonnify != null && PaymentMonnify.Completed && PaymentMonnify.Payment != null && PaymentMonnify.Amount > 2000 && (PaymentMonnify.Payment.FeeType.Id == 1 || PaymentMonnify.Payment.FeeType.Id == 6 || PaymentMonnify.Payment.FeeType.Id == 18))
                    {
                        payment = PaymentMonnify.Payment;
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return payment;
        }
        public JsonResult FetchJambRecord(string jambNumber)
        {
            JambRecordJsonModel result = new JambRecordJsonModel();
            try
            {
                if (!String.IsNullOrEmpty(jambNumber))
                {

                    var jambRecord = JambRecordLogic.GetModelsBy(f => f.Jamb_Registration_Number == jambNumber).FirstOrDefault();
                    if (jambRecord?.Id > 0)
                    {
                        if (jambRecord.CandidateName != null)
                        {
                            List<string> fullNameSplit = new List<string>();
                            //remove multiple spaces if any
                            result.IsError = false;
                            var nameSplitArray = jambRecord.CandidateName.Split(' ');
                            if (nameSplitArray.Length > 0)
                            {
                                foreach (var item in nameSplitArray)
                                {
                                    if (!String.IsNullOrEmpty(item))
                                        fullNameSplit.Add(item);
                                }
                                result.FirstName = !String.IsNullOrEmpty(fullNameSplit[1]) ? fullNameSplit[1].ToUpper() : "";
                                result.LastName = !String.IsNullOrEmpty(fullNameSplit[0]) ? fullNameSplit[0].ToUpper() : "";
                                result.OtherName = !String.IsNullOrEmpty(fullNameSplit[2]) ? fullNameSplit[2].ToUpper() : "";
                            }

                            result.JambNo = jambRecord.JambRegistrationNumber;

                            if (jambRecord.DepartmentId > 0)
                            {
                                DepartmentLogic departmentLogic = new DepartmentLogic();
                                var department = departmentLogic.GetModelBy(f => f.Department_Id == jambRecord.DepartmentId);
                                if (department?.Id > 0)
                                {
                                    result.CourseId = department.Id;
                                }
                            }
                            if (jambRecord.StateId != null)
                            {
                                StateLogic stateLogic = new StateLogic();
                                var state = stateLogic.GetModelBy(f => f.State_Id == jambRecord.StateId);
                                if (state?.Id != null)
                                {
                                    result.StateId = state.Id;
                                }
                            }

                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public bool IsJambNumberExist(string JambNo, ApplicationFormSetting applicationFormSetting)
        {
            ApplicantJambDetailLogic applicantJambDetailLogic = new ApplicantJambDetailLogic();
            var applicantJambDetail = applicantJambDetailLogic.GetModelsBy(f => f.Applicant_Jamb_Registration_Number == JambNo).FirstOrDefault();
            if (applicantJambDetail?.Person?.Id > 0 && applicantJambDetail?.ApplicationForm?.Id > 0)
            {
                if (applicantJambDetail.ApplicationForm.Setting.Id == applicationFormSetting.Id)
                    return true;
            }
            return false;
        }
        private void SetJambPersonPassportDestination(Person person, JambRecord jambRecord, out string jambPassportFilePath,
        out string destinationFilePath)
        {
            const string TILDA = "~";

            try
            {
                var fileExtension = jambRecord.ImageFileUrl.Split('.').Last();
                //change the fileName of the jamb passport
                string newFile = person.Id + "__";
                string newFileName = newFile +
                                     DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + "." +
                                     fileExtension;

                string passportUrl = jambRecord.ImageFileUrl;
                jambPassportFilePath = Server.MapPath(TILDA + jambRecord.ImageFileUrl);

                string directoryPath = Server.MapPath("~/Content/Junk");
                destinationFilePath = Path.Combine(directoryPath, newFileName);
                person.ImageFileUrl = "/Content/Junk/" + newFileName;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void SaveJambPersonPassport(string junkFilePath, string pathForSaving, Person person)
        {
            try
            {
                if (System.IO.File.Exists(junkFilePath))
                {
                    string folderPath = Path.GetDirectoryName(pathForSaving);
                    string mainFileName = person.Id + "__";

                    DeleteFileIfExist(folderPath, mainFileName);
                    //used move in other to still keep the jamb original passport for future reference
                    System.IO.File.Copy(junkFilePath, pathForSaving);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        private void DeleteFileIfExist(string folderPath)
        {
            try
            {

                var filePath = Server.MapPath(folderPath);
                FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                    file.Delete();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void SetCorrectJambPassport()
        {

        }



    }
}