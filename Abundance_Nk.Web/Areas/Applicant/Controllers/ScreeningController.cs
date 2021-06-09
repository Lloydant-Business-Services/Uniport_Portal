using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Applicant.Controllers
{
    [AllowAnonymous]
    public class ScreeningController :BaseController
    {
        // GET: Applicant/Screening
        public ActionResult Index()
        {
            var viewModel = new PostUtmeResultViewModel();
            try
            {
                ViewBag.ProgrammeId = new SelectList(Utility.PopulateUndergraduateProgrammeSelectListItem(), "Value", "Text");
                ViewBag.ExamId = new SelectList(viewModel.ExamSelectListItem, "Value", "Text");
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Index(PostUtmeResultViewModel viewModel)
        {
            try
            {

                if (viewModel.Programme != null && viewModel.Programme.Id > 0 && viewModel.Name != null)
                {
                    PutmeResultLogic putmeResultLogic = new PutmeResultLogic();
                    viewModel.Results = putmeResultLogic.GetModelsBy(a => a.FULLNAME.ToLower().Contains(viewModel.Name.ToLower()) && a.NODEPARTMENT == viewModel.ApplicationFormSetting.Id.ToString()).OrderBy(a => a.FullName).ToList();
                  
                    viewModel.AdmissionListDropDownModels = new List<AdmissionListDropDownModel>();

                    for (int i = 0; i < viewModel.Results.Count; i++)
                    {
                        viewModel.AdmissionListDropDownModels.Add(new AdmissionListDropDownModel() { ResultId = viewModel.Results[i].Id, Name = viewModel.Results[i].FullName.ToUpper() });
                    }
                    viewModel.Result = new PutmeResult();
                    ViewBag.ResultListId = new SelectList(viewModel.AdmissionListDropDownModels, "ResultId", "Name");

                    if (viewModel.Results == null || viewModel.Results.Count <= 0)
                    {
                        SetMessage("Result not found for this applicant for the selected exam. Kindly check the exam and try again.", Message.Category.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            if (viewModel.Programme != null)
            {
                ViewBag.ProgrammeId = new SelectList(Utility.PopulateUndergraduateProgrammeSelectListItem(), "Value", "Text", viewModel.Programme.Id);
                ViewBag.ExamId = new SelectList(viewModel.ExamSelectListItem, "Value", "Text", viewModel.ApplicationFormSetting.Id);
            }
            else
            {
                ViewBag.ProgrammeId = new SelectList(Utility.PopulateUndergraduateProgrammeSelectListItem(), "Value", "Text");
                ViewBag.ExamId = new SelectList(viewModel.ExamSelectListItem, "Value", "Text");
            }

            return View(viewModel);
        }
        [HttpPost]
        public async Task<ActionResult> CheckByName(PostUtmeResultViewModel viewModel)
        {
            try
            {
                if (viewModel.Result != null && viewModel.Result.Id > 0)
                {

                    PutmeResultLogic putmeResultLogic = new PutmeResultLogic();
                    viewModel.ResultSlip = putmeResultLogic.GetResult(viewModel.Result, viewModel.ApplicationFormSetting.Id);
                    if (viewModel.ResultSlip == null)
                    {
                        SetMessage("Your result is still being processed! Please check back later", Message.Category.Error);
                        return RedirectToAction("Index");
                    }

                    var payment = new ScratchCard();
                    var paymentScratchCardLogic = new ScratchCardLogic();

                    string MonnifySUBAccount = ConfigurationManager.AppSettings["MonnifySUBAccount"].ToString();
                    string MonnifyURL = ConfigurationManager.AppSettings["MonnifyUrl"].ToString();
                    string MonnifyUser = ConfigurationManager.AppSettings["MonnifyApiKey"].ToString();
                    string MonnifySecrect = ConfigurationManager.AppSettings["MonnifyContractCode"].ToString();
                    string MonnifyCode = ConfigurationManager.AppSettings["MonnifyCode"].ToString();


                    PaystackLogic paystackLogic = new PaystackLogic();
                    PaymentMonnifyLogic paymentMonnifyLogic = new PaymentMonnifyLogic(MonnifyURL, MonnifyUser, MonnifySecrect, MonnifyCode);


                    string PaystackSecrect = ConfigurationManager.AppSettings["PaystackSecrect"].ToString();
                    paystackLogic.VerifyPayment(new Payment() { InvoiceNumber = viewModel.PinNumber }, PaystackSecrect);

                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    var xp = await paymentEtranzactLogic.GetModelsByFODAsync(a => a.Payment_Id == 501102);
                    var paymentEtranzact = paymentEtranzactLogic.GetModelBy(p => p.Confirmation_No == viewModel.PinNumber);

                    if (paymentEtranzact != null && paymentEtranzact.Payment.Payment.Person.Id == viewModel.ResultSlip.PersonId)
                    {
                        TempData["PostUtmeResultViewModel"] = viewModel;
                        return RedirectToAction("ResultSlip");
                    }
                    else
                    {
                        var paystackPayment = paystackLogic.GetBy(viewModel.PinNumber);
                        if (paystackPayment != null && paystackPayment.Payment.Person.Id == viewModel.ResultSlip.PersonId)
                        {
                            TempData["PostUtmeResultViewModel"] = viewModel;
                            return RedirectToAction("ResultSlip");
                        }

                        var monnifyPayment = paymentMonnifyLogic.GetBy(viewModel.PinNumber);
                        if (monnifyPayment != null && monnifyPayment.Completed && monnifyPayment.Payment.Person.Id == viewModel.ResultSlip.PersonId)
                        {
                            TempData["PostUtmeResultViewModel"] = viewModel;
                            return RedirectToAction("ResultSlip");
                        }

                        SetMessage("Pin is invalid!", Message.Category.Error);
                        return RedirectToAction("Index");
                    }

                    //payment = paymentScratchCardLogic.GetModelsBy(p => p.Pin == viewModel.PinNumber).FirstOrDefault();
                    //if (payment == null)
                    //{
                    //    SetMessage("Pin is invalid! Please check that you have typed in the correct detail",Message.Category.Error);
                    //    return RedirectToAction("Index");
                    //}

                    //FeeType feetype;
                    //if (viewModel.Programme.Id == 7)
                    //{
                    //    feetype = new FeeType { Id = 7 };
                    //}
                    //else
                    //{
                    //    feetype = new FeeType { Id = 7 };
                    //}
                    //if (paymentScratchCardLogic.ValidatePin(viewModel.PinNumber, feetype))
                    //{
                    //    bool pinUseStatus = paymentScratchCardLogic.IsPinUsed(viewModel.PinNumber, (long)viewModel.ResultSlip.PersonId);
                    //    if (!pinUseStatus)
                    //    {
                    //        paymentScratchCardLogic.UpdatePin(viewModel.PinNumber, new Person { Id = (long) viewModel.ResultSlip.PersonId });
                    //        TempData["PostUtmeResultViewModel"] = viewModel;
                    //        return RedirectToAction("ResultSlip");

                    //    }
                    //    else
                    //    {
                    //        var  pins = paymentScratchCardLogic.GetBy((long)viewModel.ResultSlip.PersonId,1);
                    //        foreach(var pin in pins)
                    //        {
                    //            if (pin.Pin == viewModel.PinNumber)
                    //            {
                    //                paymentScratchCardLogic.Modify(viewModel.PinNumber, new Person { Id = (long)viewModel.ResultSlip.PersonId });
                    //            }
                    //        }
                    //        SetMessage("Pin has been used by another applicant! Please cross check and Try again.", Message.Category.Error);
                    //        return RedirectToAction("Index");
                    //    }

                    //}
                    //else
                    //{
                    //    SetMessage("Pin is not for this purpose!.", Message.Category.Error);
                    //    return RedirectToAction("Index");
                    //}



                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("Index");
        }

        public ActionResult ResultSlip()
        {
            try
            {
                var viewModel = new PostUtmeResultViewModel();
                viewModel = (PostUtmeResultViewModel)TempData["PostUtmeResultViewModel"];
                if (viewModel.ResultSlip != null)
                {
                    return View(viewModel);
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                
                throw;
            }

            return View();
        }
   
        public ActionResult PurchaseCard()
        {
            try
            {
                var viewModel = new PostUtmeResultViewModel();
                viewModel.ApplicantJambDetail = new ApplicantJambDetail();


                return View(viewModel);
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }
    
        [HttpPost]
        public ActionResult PurchaseCard(PostUtmeResultViewModel viewModel)
        {
            try
            {
                return View(viewModel);
                ApplicantJambDetailLogic applicantJambDetialLogic = new ApplicantJambDetailLogic();
                viewModel.ApplicantJambDetail = applicantJambDetialLogic.GetBy(viewModel.ApplicantJambDetail.JambRegistrationNumber);
                if (viewModel.ApplicantJambDetail != null && viewModel.ApplicantJambDetail.Person != null)
                {
                    Payment payment = new Payment();
                    PaymentLogic paymentLogic = new PaymentLogic();

                    payment.PaymentMode = new PaymentMode() { Id = 1 };
                    payment.PaymentType = new PaymentType() { Id = 2 };
                    payment.Session = new Session() { Id = 13 };
                    payment.DatePaid = DateTime.Now;
                    payment.PersonType = new PersonType() { Id = 4 };
                    payment.FeeType = new FeeType() { Id = (int)FeeTypes.ResultChecking };
                    payment.Person = viewModel.ApplicantJambDetail.Person;
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

                    newPayment.PaymentMode = new PaymentMode() { Id = 1 };
                    newPayment.PaymentType = new PaymentType() { Id = 2 };
                    newPayment.Session = new Session() { Id = 18 ,Name ="2018/2019"};
                    newPayment.DatePaid = DateTime.Now;
                    newPayment.PersonType = new PersonType() { Id = 4 };
                    newPayment.FeeType = new FeeType() { Id = (int)FeeTypes.ResultChecking, Name ="Result Checking Access Pin" };
                    newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, 1, 1, 1, 1, 18);

                    string PaystackSecrect = ConfigurationManager.AppSettings["PaystackSecrect"].ToString();
                    string PaystackSubAccount = ConfigurationManager.AppSettings["PaystackSubAccount"].ToString();
                    if (!String.IsNullOrEmpty(PaystackSecrect))
                    {
                        newPayment.Person = viewModel.ApplicantJambDetail.Person;
                        PaystackLogic paystackLogic = new PaystackLogic();
                        var Paystack = paystackLogic.GetBy(newPayment);
                        if (Paystack == null)
                        {
                            decimal amount = 0;
                            amount = newPayment.FeeDetails.Sum(a => a.Fee.Amount);

                            PaystackRepsonse paystackRepsonse = paystackLogic.MakePayment(newPayment, PaystackSecrect, PaystackSubAccount,amount,"N/A","N/A", viewModel.ApplicantJambDetail.JambRegistrationNumber,1600);
                            if (paystackRepsonse != null && paystackRepsonse.status && !String.IsNullOrEmpty(paystackRepsonse.data.authorization_url))
                            {
                                paystackRepsonse.Paystack = new Paystack();
                                paystackRepsonse.Paystack.Payment = newPayment;
                                paystackRepsonse.Paystack.authorization_url = paystackRepsonse.data.authorization_url;
                                paystackRepsonse.Paystack.access_code = paystackRepsonse.data.access_code;
                                Paystack = paystackLogic.Create(paystackRepsonse.Paystack);
                            }
                        }

                    }


                    string MonnifySUBAccount = ConfigurationManager.AppSettings["MonnifySUBAccount"].ToString();
                    string MonnifyURL = ConfigurationManager.AppSettings["MonnifyUrl"].ToString();
                    string MonnifyUser = ConfigurationManager.AppSettings["MonnifyApiKey"].ToString();
                    string MonnifySecrect = ConfigurationManager.AppSettings["MonnifyContractCode"].ToString();
                    string MonnifyCode = ConfigurationManager.AppSettings["MonnifyCode"].ToString();
                    //Split used here is school account. So substract commision from total fees thats what is split amount
                    decimal Total = payment.FeeDetails.Sum(x => x.Fee.Amount);
                    decimal SchoolCut = Total - 1600;
                    Total += 250;
                    MonnifySplit split = new MonnifySplit { splitAmount = SchoolCut.ToString(), subAccountCode = MonnifySUBAccount };

                    PaymentMonnifyLogic paymentMonnifyLogic = new PaymentMonnifyLogic(MonnifyURL, MonnifyUser, MonnifySecrect, MonnifyCode);
                    paymentMonnifyLogic.GenerateInvoice(payment, Total.ToString(), DateTime.Now.AddDays(7), new List<MonnifySplit> { split });


                    return RedirectToAction("Invoice", "Form",
                           new { Area = "Applicant", ivn = newPayment.InvoiceNumber });
                }

                SetMessage("Jamb Number not found! Please check that you have typed in the correct detail", Message.Category.Error);
                return View(viewModel);
            }
            catch (Exception)
            {
                
                throw;
            }

            return View();
        }
        public ActionResult PUTMEScreenVenueSchedule()
        {
            try
            {
                return File(Server.MapPath("~/Content/Manual/") + "Hall Allocation.pdf", "application/pdf", "Hall Allocation.pdf");
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured. " + ex.Message, Message.Category.Error);
                return RedirectToAction("Index", "Home", new { Area = "" });
            }
        }

    }


    public class AdmissionListDropDownModel
    {
        public long ResultId { get; set; }
        public string Name { get; set; }
    }
}